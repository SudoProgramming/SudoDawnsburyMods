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
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Mods.Feats.Classes.Gunslinger.Enums;
using Dawnsbury.Mods.Items.Firearms;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
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
                character.AddSelectionOption((SelectionOption) new SingleFeatSelectionOption(wayFeat.Name + " Way Skill Selection", "Way Skill", 1, (feat => waySkillFeats.Contains(feat.FeatName))));
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
                    return new Bonus(2, BonusType.Circumstance, "Ten Paces", true);
                };
            });
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