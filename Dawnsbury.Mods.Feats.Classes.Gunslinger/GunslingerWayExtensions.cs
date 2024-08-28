using Dawnsbury.Audio;
using Dawnsbury.Auxiliary;
using Dawnsbury.Core;
using Dawnsbury.Core.Animations.Movement;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Coroutines.Options;
using Dawnsbury.Core.Coroutines.Requests;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Rules;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Mods.Feats.Classes.Gunslinger.Enums;
using Dawnsbury.Mods.Items.Firearms;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Dawnsbury.Mods.Feats.Classes.Gunslinger.Extensions
{
    /// <summary>
    /// A Gunslinger sub-class called a way
    /// </summary>
    public static class GunslingerWayExtensions
    {
        public static readonly FeatName GunslingerSniperStealthInitiative = ModManager.RegisterFeatName("Gunslinger Sniper Stealth Init", "Stealth for Initiative");

        public static readonly FeatName GunslingerSniperPerceptionInitiative = ModManager.RegisterFeatName("Gunslinger Sniper Perception Init", "Perception for Initiative");

        public static readonly QEffectId OneShotOneKillQEID = ModManager.RegisterEnumMember<QEffectId>("One Shot, One Kill QEID");

        public static void WithWaySkill(this Feat wayFeat, FeatName waySkillFeat)
        {
            wayFeat.WithOnSheet((CalculatedCharacterSheetValues character) =>
            {
                if (!character.HasFeat(waySkillFeat))
                {
                    character.GrantFeat(waySkillFeat);
                }
            });
        }

        public static void WithWaySkillOptions(this Feat wayFeat, List<FeatName> waySkillFeats)
        {
            wayFeat.WithOnSheet((CalculatedCharacterSheetValues character) =>
            {
                character.AddSelectionOption(new SingleFeatSelectionOption(wayFeat.Name + " Way Skill Selection", "Way Skill", 1, (feat => waySkillFeats.Contains(feat.FeatName))));
            });
        }

        public static void WithDrifersReloadingStrikeLogic(this Feat drifterFeat)
        {
            drifterFeat.WithPermanentQEffect("Drifter's Reloading Strike", delegate (QEffect self)
            {
                self.ProvideMainAction = (QEffect reloadingStrikeShotEffect) =>
                {
                    Creature owner = reloadingStrikeShotEffect.Owner;
                    Item? ranged = owner.HeldItems.FirstOrDefault(item => Firearms.IsItemFirearmOrCrossbow(item) && (!Firearms.IsItemLoaded(item) || Firearms.IsMultiAmmoWeaponReloadable(item)) && item.HasTrait(Trait.Ranged) && !item.HasTrait(Trait.TwoHanded));
                    Item? melee = owner.HeldItems.FirstOrDefault(item => item.HasTrait(Trait.Melee) && !item.HasTrait(Trait.TwoHanded));
                    if (ranged == null || (melee == null && !owner.HasFreeHand))
                    {
                        return null;
                    }

                    return new ActionPossibility(new CombatAction(reloadingStrikeShotEffect.Owner, new SideBySideIllustration(ranged.Illustration, (melee != null) ? melee.Illustration : IllustrationName.Fist), "Reloading Strike", [Gunslinger.GunslingerTrait, Trait.Basic], "{b}Requirements{/b} You're wielding a firearm or crossbow in one hand, and your other hand either wields a one-handed melee weapon or is empty.\n\nStrike an opponent within reach with your one-handed melee weapon (or, if your other hand is empty, with an unarmed attack), and then Interact to reload.", Target.Self()
                    .WithAdditionalRestriction((Func<Creature, string>)(self => (self.Battle != null && self.Battle.AllCreatures.Count(creature => self.DistanceTo(creature) <= 1 && creature != self && !self.FriendOf(creature)) > 0) ? null : "No valid melee targets.")))
                    .WithActionCost(1).WithItem(ranged)
                    .WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (CombatAction action, Creature attacker, Creature defender, CheckResult result) =>
                    {
                        await owner.Battle.GameLoop.StateCheck();
                        List<Option> options = new List<Option>();
                        if (melee != null)
                        {
                            GameLoop.AddDirectUsageOnCreatureOptions(attacker.CreateStrike(melee).WithActionCost(0), options, true);
                        }
                        else
                        {
                            foreach (Item unarmedItem in owner.MeleeWeapons.Where(possibleUnarmedItem => possibleUnarmedItem.HasTrait(Trait.Unarmed)))
                            {
                                GameLoop.AddDirectUsageOnCreatureOptions(owner.CreateStrike(unarmedItem).WithActionCost(0), options, true);
                            }
                        }

                        if (options.Count != 0)
                        {
                            Option chosenStrike = options[0];
                            if (options.Count >= 2)
                            {
                                chosenStrike = (await owner.Battle.SendRequest(new AdvancedRequest(owner, "Choose a creature to Strike.", options)
                                {
                                    TopBarText = "Choose a creature to Strike.",
                                    TopBarIcon = (melee != null) ? melee.Illustration : IllustrationName.Fist
                                })).ChosenOption;
                            }

                            if (chosenStrike != null && chosenStrike is not CancelOption)
                            {
                                await chosenStrike.Action();
                            }
                        }

                        Gunslinger.AwaitReloadItem(attacker, ranged);
                    })));
                };
            });
        }

        public static void WithDrifersIntoTheFrayLogic(this Feat drifterFeat)
        {
            drifterFeat.WithPermanentQEffect("Drifter's Into the Fray", delegate (QEffect self)
            {
                self.StartOfCombat = async (QEffect startOfCombat) =>
                {
                    if (await startOfCombat.Owner.Battle.AskForConfirmation(startOfCombat.Owner, IllustrationName.FreeAction, "Stride as a free action towards a creature?", "Yes"))
                    {
                        Tile? tileToStrideTo = await GetTileCloserToEnemy(startOfCombat.Owner, "Stride towards the selected enemy.");
                        if (tileToStrideTo != null)
                        {
                            await startOfCombat.Owner.MoveTo(tileToStrideTo, null, new MovementStyle()
                            {
                                MaximumSquares = startOfCombat.Owner.Speed,
                            });
                        }
                    }
                };
            });
        }

        // TODO: Add Create a Diversion
        public static void WithPistolerosRaconteursReloadLogic(this Feat pistoleroFeat)
        {
            pistoleroFeat.WithOnCreature(creature =>
            {
                creature.AddQEffect(new QEffect()
                {
                    StateCheck = (QEffect permanentState) =>
                    {
                        foreach (Item heldItem in permanentState.Owner.HeldItems)
                        {
                            permanentState.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                            {
                                ProvideMainAction = (QEffect raconteursReloadEffect) =>
                                {
                                    if (Firearms.IsItemFirearmOrCrossbow(heldItem) && (!Firearms.IsItemLoaded(heldItem) || Firearms.IsMultiAmmoWeaponReloadable(heldItem)) && heldItem.WeaponProperties != null)
                                    {
                                        CombatAction raconteursReloadAction = CommonCombatActions.Demoralize(permanentState.Owner);
                                        raconteursReloadAction.Name = "Raconteur's Reload";
                                        raconteursReloadAction.Item = heldItem;
                                        raconteursReloadAction.ActionCost = 1;
                                        raconteursReloadAction.Illustration = new SideBySideIllustration(heldItem.Illustration, IllustrationName.Demoralize);
                                        raconteursReloadAction.Description = "Interact to reload and then attempt a Deception check to Create a Diversion or an Intimidation check to Demoralize.";
                                        raconteursReloadAction.Target = Target.Ranged(heldItem.WeaponProperties.MaximumRange);
                                        raconteursReloadAction.WithEffectOnSelf((Creature self) =>
                                        {
                                            Gunslinger.AwaitReloadItem(self, heldItem);
                                        });

                                        return new ActionPossibility(raconteursReloadAction);
                                    }

                                    return null;
                                }
                            });
                        }
                    }
                });
            });
        }

        public static void WithPistolerosTenPacesLogic(this Feat pistoleroFeat)
        {
            pistoleroFeat.WithPermanentQEffect("Pistolero's Ten Paces", delegate (QEffect self)
            {
                self.StartOfCombat = async (QEffect startOfCombat) =>
                {
                    if (await startOfCombat.Owner.Battle.AskForConfirmation(startOfCombat.Owner, IllustrationName.FreeAction, "Step up to 10 ft as a free action?", "Yes"))
                    {
                        await self.Owner.StrideAsync("Choose where to 5 ft. Step. (1/2)", allowStep: true, maximumFiveFeet: true, allowPass: true);
                        await self.Owner.StrideAsync("Choose where to 5 ft. Step. (2/2)", allowStep: true, maximumFiveFeet: true, allowPass: true);
                    }
                };
                self.BonusToInitiative = (QEffect bonusToInitiative) =>
                {
                    return new Bonus(2, BonusType.Untyped, "Ten Paces", true);
                };
            });
        }

        public static void WithSnipersCoveredReloadLogic(this Feat sniperFeat)
        {
            sniperFeat.WithOnCreature(creature =>
            {
                creature.AddQEffect(new QEffect()
                {
                    StateCheck = (QEffect permanentState) =>
                    {
                        foreach (Item heldItem in permanentState.Owner.HeldItems)
                        {
                            permanentState.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                            {
                                ProvideMainAction = (QEffect coveredReloadEffect) =>
                                {
                                    if (Firearms.IsItemFirearmOrCrossbow(heldItem) && (!Firearms.IsItemLoaded(heldItem) || Firearms.IsMultiAmmoWeaponReloadable(heldItem)) && heldItem.WeaponProperties != null)
                                    {
                                        Creature self = coveredReloadEffect.Owner;
                                        bool canHide = self.Battle.AllCreatures.Any<Creature>((Func<Creature, bool>)(cr => cr.EnemyOf(self) && HiddenRules.HasCoverOrConcealment(self, cr)));
                                        bool canTakeCover = !self.HasEffect(QEffectId.TakingCover);
                                        CombatAction coveredReloadAciton = new CombatAction(self, heldItem.Illustration, "Covered Reload", [Trait.Basic], "Reload then, either Take Cover or attempt to Hide.", (Target)Target.Self().WithAdditionalRestriction(targetingSelf => (!canHide && !canTakeCover) ? "Can't Take Cover or Hide" : null));
                                        coveredReloadAciton.Item = heldItem;
                                        coveredReloadAciton.ActionCost = 1;
                                        coveredReloadAciton.WithEffectOnSelf(async (Creature self) =>
                                        {
                                            Gunslinger.AwaitReloadItem(self, heldItem);

                                            CombatAction takeCoverAction = new CombatAction(self, (Illustration)IllustrationName.TakeCover, "Take cover", new Trait[1]
                                            {
                                                Trait.Basic
                                            }, "{i}You hunker down.{/i}\n\nUntil you move, attack or become Unconscious, any standard cover you have instead counts as greater cover (you don't gain this benefit against creatures against whom you don't have standard cover).\n\nIn addition, if you're prone, you count as having greater cover from all ranged attacks.", (Target)Target.Self().WithAdditionalRestriction((Func<Creature, string>)(innerSelf => !innerSelf.HasEffect(QEffectId.TakingCover) ? (string)null : "You're already taking cover.")))
                                            .WithActionCost(0)
                                            .WithEffectOnSelf((Func<Creature, Task>)(async innerSelf =>
                                            {
                                                innerSelf.AddQEffect(new QEffect("Taking cover", "Until you move, attack or become Unconscious, any standard cover you have instead counts as greater cover (you don't gain this benefit against creatures against whom you don't have standard cover).\n\nIn addition, if you're prone, you count as having greater cover from all ranged attacks.", ExpirationCondition.Never, innerSelf, (Illustration)IllustrationName.TakeCover)
                                                {
                                                    Id = QEffectId.TakingCover,
                                                    CountsAsABuff = true,
                                                    StateCheck = (Action<QEffect>)(sc =>
                                                    {
                                                        if (!sc.Owner.HasEffect(QEffectId.Unconscious))
                                                            return;
                                                        sc.ExpiresAt = ExpirationCondition.Immediately;
                                                    }),
                                                    AfterYouTakeAction = (Func<QEffect, CombatAction, Task>)(async (qfSelf, action) =>
                                                    {
                                                        if (action.Name == "Covered Reload" || (!action.HasTrait(Trait.Attack) && !action.HasTrait(Trait.Move)))
                                                            return;
                                                        qfSelf.ExpiresAt = ExpirationCondition.Immediately;
                                                    }),
                                                    IncreaseCover = (Func<QEffect, CombatAction, CoverKind, CoverKind>)((qfSelf, attack, existingCover) => attack != null && attack.HasTrait(Trait.Ranged) && qfSelf.Owner.HasEffect(QEffectId.Prone) && existingCover < CoverKind.Greater || existingCover == CoverKind.Standard ? CoverKind.Greater : existingCover)
                                                });
                                            }));

                                            CombatAction hideAction = new CombatAction(self, (Illustration)IllustrationName.Hide, "Hide", new Trait[2]
                                            {
                                                Trait.Basic,
                                                Trait.AttackDoesNotTargetAC
                                            }, "Make one Stealth check against the Perception DCs of each enemy creature that can see you but that you have cover or concealment from. On a success, you become Hidden to that creature.", (Target)Target.Self((Func<Creature, AI, float>)((cr, ai) => ai.HideSelf())).WithAdditionalRestriction((Func<Creature, string>)(innerSelf =>
                                            {
                                                if (HiddenRules.IsHiddenFromAllEnemies(innerSelf, innerSelf.Occupies))
                                                    return "You're already hidden from all enemies.";
                                                return !innerSelf.Battle.AllCreatures.Any<Creature>((Func<Creature, bool>)(cr => cr.EnemyOf(innerSelf) && HiddenRules.HasCoverOrConcealment(innerSelf, cr))) ? "You don't have cover or concealment from any enemy." : (string)null;
                                            }))).WithActionCost(0).WithActionId(ActionId.Hide).WithSoundEffect(SfxName.Hide).WithEffectOnSelf((Func<Creature, Task>)(async innerSelf =>
                                            {
                                                int roll = R.NextD20();
                                                foreach (Creature creature in innerSelf.Battle.AllCreatures.Where<Creature>((Func<Creature, bool>)(cr => cr.EnemyOf(innerSelf))))
                                                {
                                                    if (!innerSelf.DetectionStatus.HiddenTo.Contains(creature) && HiddenRules.HasCoverOrConcealment(innerSelf, creature))
                                                    {
                                                        CheckBreakdown breakdown = CombatActionExecution.BreakdownAttack(new CombatAction(innerSelf, (Illustration)IllustrationName.Hide, "Hide", new Trait[1]
                                                        {
                                                            Trait.Basic
                                                        }, "[this condition has no description]", (Target)Target.Self()).WithActiveRollSpecification(new ActiveRollSpecification(Checks.SkillCheck(Skill.Stealth), Checks.DefenseDC(Defense.Perception))), creature);
                                                        CheckBreakdownResult breakdownResult = new CheckBreakdownResult(breakdown, roll);
                                                        string str8 = breakdown.DescribeWithFinalRollTotal(breakdownResult);
                                                        StringBuilder stringHandler;
                                                        if (breakdownResult.CheckResult >= CheckResult.Success)
                                                        {
                                                            innerSelf.DetectionStatus.HiddenTo.Add(creature);
                                                            Tile occupies = creature.Occupies;
                                                            Color lightBlue = Color.LightBlue;
                                                            string str9 = innerSelf?.ToString();
                                                            string str10 = creature?.ToString();
                                                            stringHandler = new StringBuilder(string.Empty, 10);
                                                            stringHandler.Append(" (");
                                                            stringHandler.Append(breakdownResult.D20Roll.ToString() + breakdown.TotalCheckBonus.WithPlus());
                                                            stringHandler.Append("=");
                                                            stringHandler.Append(breakdownResult.D20Roll + breakdown.TotalCheckBonus);
                                                            stringHandler.Append(" vs. ");
                                                            stringHandler.Append(breakdown.TotalDC);
                                                            stringHandler.Append(").");
                                                            string stringAndClear = stringHandler.ToString();
                                                            string log = str9 + " successfully hid from " + str10 + stringAndClear;
                                                            string logDetails = str8;
                                                            occupies.Overhead("hidden from", lightBlue, log, "Hide", logDetails);
                                                        }
                                                        else
                                                        {
                                                            Tile occupies = creature.Occupies;
                                                            Color red = Color.Red;
                                                            string str11 = innerSelf?.ToString();
                                                            string str12 = creature?.ToString();
                                                            stringHandler = new StringBuilder(string.Empty, 10);
                                                            stringHandler.Append(" (");
                                                            stringHandler.Append(breakdownResult.D20Roll.ToString() + breakdown.TotalCheckBonus.WithPlus());
                                                            stringHandler.Append("=");
                                                            stringHandler.Append(breakdownResult.D20Roll + breakdown.TotalCheckBonus);
                                                            stringHandler.Append(" vs. ");
                                                            stringHandler.Append(breakdown.TotalDC);
                                                            stringHandler.Append(").");
                                                            string stringAndClear = stringHandler.ToString();
                                                            string log = str11 + " failed to hide from " + str12 + stringAndClear;
                                                            string logDetails = str8;
                                                            occupies.Overhead("hide failed", red, log, "Hide", logDetails);
                                                        }
                                                    }
                                                }
                                            }));

                                            if (canHide && canTakeCover)
                                            {
                                                if (canHide && canTakeCover && await self.Battle.AskForConfirmation(self, coveredReloadAciton.Illustration, "Choose to Take Cover or Hide.", "Take Cover", "Hide"))
                                                {
                                                    await takeCoverAction.AllExecute();
                                                    bool x = self.HasEffect(QEffectId.TakingCover);
                                                }
                                                else
                                                {
                                                    await hideAction.AllExecute();
                                                }
                                            }

                                            if (!canHide && await self.Battle.AskForConfirmation(self, coveredReloadAciton.Illustration, "Would you like to Take Cover as a free action?", "Yes"))
                                            {
                                                await takeCoverAction.AllExecute();
                                                bool x = self.HasEffect(QEffectId.TakingCover);
                                            }
                                            else if (!canTakeCover && await self.Battle.AskForConfirmation(self, coveredReloadAciton.Illustration, "Would you like to Hide as a free action?", "Yes"))
                                            {
                                                await hideAction.AllExecute();
                                            }
                                        });

                                        return new ActionPossibility(coveredReloadAciton);
                                    }

                                    return null;
                                }
                            });
                        }
                    }
                });
            });
        }

        public static void WithSnipersOneShotOneKillLogic(this Feat sniperFeat)
        {
            sniperFeat.WithOnSheet((CalculatedCharacterSheetValues character) =>
            {
                character.AddSelectionOption(new SingleFeatSelectionOption("Sniper Initiative Choice", "Sniper Initiative", 1, (feat => feat.FeatName == GunslingerSniperStealthInitiative || feat.FeatName == GunslingerSniperPerceptionInitiative)));
            });

            sniperFeat.WithPermanentQEffect("Sniper's One Shot, One Kill", delegate (QEffect self)
            {
                self.StartOfCombat = async (QEffect startOfCombat) =>
                {
                    if (self.Owner.HasFeat(GunslingerSniperStealthInitiative))
                    {   
                        int stealthDC = self.Owner.Initiative;
                        foreach (Creature enemy in self.Owner.Battle.AllCreatures.Where(creature => !self.Owner.FriendOf(creature) && HiddenRules.HasCoverOrConcealment(self.Owner, creature) && creature.Initiative < stealthDC))
                        {
                            self.Owner.DetectionStatus.HiddenTo.Add(enemy);
                        }

                        self.Owner.Battle.Log(self.Owner.Name + " has rolled Stealth for initiative" + ((self.Owner.DetectionStatus.EnemiesYouAreHiddenFrom.Count() > 0) ? "and is hidden to:\n" + string.Join(",", self.Owner.DetectionStatus.EnemiesYouAreHiddenFrom) : "."));

                        List<Item> heldItems = startOfCombat.Owner.HeldItems.Where(item => Firearms.IsItemFirearmOrCrossbow(item)).ToList();
                        if (heldItems.Count > 0)
                        {
                            startOfCombat.Owner.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtEndOfYourTurn)
                            {
                                Id = OneShotOneKillQEID,
                                AddExtraWeaponDamage = (Item item) =>
                                {
                                    if (Firearms.IsItemFirearmOrCrossbow(item) && item.WeaponProperties != null)
                                    {
                                        QEffect? oneShotOneKillEffect = startOfCombat.Owner.QEffects.FirstOrDefault(qe => qe.Id == OneShotOneKillQEID);
                                        if (oneShotOneKillEffect != null)
                                        {
                                            oneShotOneKillEffect.ExpiresAt = ExpirationCondition.Immediately;
                                        }

                                        return (DiceFormula.FromText("1d6", "One Shot, One Kill"), item.WeaponProperties.DamageKind);
                                    }

                                    return null;
                                },
                                AfterYouTakeAction = async (QEffect afterAction, CombatAction action) =>
                                {
                                    QEffect? oneShotOneKillEffect = startOfCombat.Owner.QEffects.FirstOrDefault(qe => qe.Id == OneShotOneKillQEID);
                                    if (oneShotOneKillEffect != null && oneShotOneKillEffect.ExpiresAt == ExpirationCondition.Immediately)
                                    {
                                        afterAction.Owner.RemoveAllQEffects(qe => qe.Id == OneShotOneKillQEID);
                                    }
                                }
                            });
                        }
                    }
                };
                self.BonusToInitiative = (QEffect bonusToInitiative) =>
                {
                    if (self.Owner.HasFeat(GunslingerSniperStealthInitiative))
                    {
                        int stealthAndPerceptionDifference = self.Owner.Skills.Get(Skill.Stealth) - self.Owner.Perception;
                        if (stealthAndPerceptionDifference != 0)
                        {
                            return new Bonus(stealthAndPerceptionDifference, BonusType.Untyped, "One Shot, One Kill (Stealth)", stealthAndPerceptionDifference > 0);
                        }
                    }

                    return null;
                };
            });
        }

        public static void WithVanguardClearAPathLogic(this Feat vanguardFeat)
        {

        }

        public static void WithVanguardLivingFortificationLogic(this Feat vanguardFeat)
        {

        }

        public static async Task<Tile?> GetAnEnemiesTileAsync(Creature self, string messageString)
        {
            List<Option> options = new List<Option>();
            List<Tile> tiles = self.Battle.AllCreatures.Where(creature => self != creature && !self.FriendOf(creature)).Select(creature => creature.Occupies).ToList();
            foreach (Tile tile in tiles)
            {
                options.Add(new TileOption(tile, tile.PrimaryOccupant.Name ?? "Tile (" + tile.X + "," + tile.Y + ")", null, (AIUsefulness) (float) int.MinValue, true));
            }

            Option selectedOption = (await self.Battle.SendRequest(new AdvancedRequest(self, messageString, options)
            {
                IsMainTurn = false,
                IsStandardMovementRequest = false,
                TopBarIcon = IllustrationName.WarpStep,
                TopBarText = messageString

            })).ChosenOption;

            if (selectedOption != null)
            {
                if (selectedOption is CancelOption cancel)
                {
                    return null;
                }

                return ((TileOption)selectedOption).Tile;
            }

            return null;
        }

        public static async Task<Tile?> GetTileCloserToEnemy(Creature self, string messageString)
        {
            Tile startingTile = self.Occupies;
            List<Tile> enemyTiles = self.Battle.AllCreatures.Where(creature => self != creature && !self.FriendOf(creature)).Select(creature => creature.Occupies).ToList();
            List<Option> options = new List<Option>();
            foreach (Tile tile in self.Battle.Map.AllTiles)
            {
                if (tile.IsFree && IsTileCloserToAnyOfTheseTiles(startingTile, tile, enemyTiles) && (new LongMovement(self, tile, new MovementStyle() { MaximumSquares = self.Speed }, null, false).Path != null))
                {
                    options.Add(new TileOption(tile, "Tile (" + tile.X + "," + tile.Y + ")", null, (AIUsefulness)(float)int.MinValue, true));
                }
            }

            Option selectedOption = (await self.Battle.SendRequest(new AdvancedRequest(self, messageString, options)
            {
                IsMainTurn = false,
                IsStandardMovementRequest = false,
                TopBarIcon = IllustrationName.WarpStep,
                TopBarText = messageString

            })).ChosenOption;

            if (selectedOption != null)
            {
                if (selectedOption is CancelOption cancel)
                {
                    return null;
                }

                return ((TileOption)selectedOption).Tile;
            }

            return null;
        }

        private static bool IsTileCloserToAnyOfTheseTiles(Tile originalTile, Tile potentialCloserTile, List<Tile> tilesToCheck)
        {
            
            foreach (Tile tileToCheck in tilesToCheck)
            {
                if (potentialCloserTile.DistanceTo(tileToCheck) < originalTile.DistanceTo(tileToCheck))
                {
                    return true;
                }
            }
            return false;
        }
    }
}