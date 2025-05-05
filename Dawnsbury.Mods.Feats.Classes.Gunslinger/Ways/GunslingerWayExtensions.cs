using Dawnsbury.Audio;
using Dawnsbury.Auxiliary;
using Dawnsbury.Core;
using Dawnsbury.Core.Animations.Movement;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
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
using Dawnsbury.Core.Mechanics.Targeting.TargetingRequirements;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Mods.Feats.Classes.Gunslinger.Enums;
using Dawnsbury.Mods.Feats.Classes.Gunslinger.RegisteredComponents;
using Dawnsbury.Mods.Items.Firearms.RegisteredComponents;
using Dawnsbury.Mods.Items.Firearms.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Dawnsbury.Mods.Feats.Classes.Gunslinger.Ways
{
    /// <summary>
    /// Extension methods for the Gunslinger Ways that all take a Feat for a Gunslinger.
    /// </summary>
    public static class GunslingerWayExtensions
    {
        /// <summary>
        /// Grants the feat the Trained feat for the given skill
        /// </summary>
        /// <param name="gunslingerWay">The Gunslinger way</param>
        /// <param name="waySkillFeat">The Skill being trained</param>
        public static void WithWaySkill(this GunslingerWay gunslingerWay, FeatName waySkillFeat)
        {
            Feat wayFeat = gunslingerWay.Feat;
            wayFeat.WithOnSheet((character) =>
            {
                if (!character.HasFeat(waySkillFeat))
                {
                    character.GrantFeat(waySkillFeat);
                }
            });
        }

        /// <summary>
        /// Grants onf of the provided feats the Trained feat for the given skill
        /// </summary>
        /// <param name="gunslingerWay">The Gunslinger way</param>
        /// <param name="waySkillFeats">The Skill options being trained</param>
        public static void WithWaySkillOptions(this GunslingerWay gunslingerWay, List<FeatName> waySkillFeats)
        {
            Feat wayFeat = gunslingerWay.Feat;
            wayFeat.WithOnSheet((character) =>
            {
                character.AddSelectionOption(new SingleFeatSelectionOption(wayFeat.Name + " Way Skill Selection", "Way Skill", 1, feat => waySkillFeats.Contains(feat.FeatName)));
            });
        }

        /// <summary>
        /// Adds the logic for Reloading Strike
        /// </summary>
        /// <param name="driftersWay">The Drifter way</param>
        public static void WithDrifersReloadingStrikeLogic(this GunslingerWay driftersWay)
        {
            Feat drifterFeat = driftersWay.Feat;
            // Adds a permanent effect for the Reloading Strike action
            drifterFeat.WithPermanentQEffect("Reload and melee Strike", delegate (QEffect self)
            {
                self.ProvideStrikeModifier = (Item item) =>
                {
                    if (item.HasTrait(Trait.Melee))
                    {
                        Creature owner = self.Owner;
                        Item? rangedWeapon = owner.HeldItems.FirstOrDefault(heldItem => FirearmUtilities.IsItemFirearmOrCrossbow(heldItem) && !FirearmUtilities.IsItemLoaded(heldItem));
                        CombatAction reloadingStrike = owner.CreateStrike(item);
                        reloadingStrike.Name = $"Reloading Strike";
                        reloadingStrike.WithActionCost(1);
                        reloadingStrike.Illustration = (rangedWeapon != null) ? new SideBySideIllustration(item.Illustration, rangedWeapon.Illustration) : item.Illustration;
                        reloadingStrike.Item = rangedWeapon;
                        reloadingStrike.Description = StrikeRules.CreateBasicStrikeDescription3(reloadingStrike.StrikeModifiers, prologueText: "Reload your weapon, this does not trigger reactions.");
                        reloadingStrike.StrikeModifiers.OnEachTarget = async (Creature attacker, Creature defender, CheckResult result) =>
                        {
                            if (rangedWeapon != null)
                            {
                                await FirearmUtilities.AwaitReloadItem(attacker, rangedWeapon, true);
                            }
                        };

                        // Checks if the item needs to be reloaded
                        ((CreatureTarget)reloadingStrike.Target).WithAdditionalConditionOnTargetCreature((Creature attacker, Creature defender) =>
                        {
                            Item? itemToReload = owner.HeldItems.FirstOrDefault(heldItem => FirearmUtilities.IsItemFirearmOrCrossbow(heldItem) && !FirearmUtilities.IsItemLoaded(heldItem));
                            if (itemToReload == null)
                            {
                                return Usability.NotUsable("No item to reload");
                            }
                            else if (owner.HeldItems.Count(item => FirearmUtilities.IsItemFirearmOrCrossbow(item)) > 1)
                            {
                                return Usability.NotUsable("You can't have two firearms held.");
                            }
                            return Usability.Usable;
                        });

                        return reloadingStrike;
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds Into the Fray logic
        /// </summary>
        /// <param name="driftersWay">The Drifter way</param>
        public static void WithDrifersIntoTheFrayLogic(this GunslingerWay driftersWay)
        {
            Feat drifterFeat = driftersWay.Feat;
            AddDrifersIntoTheFrayLogic(drifterFeat);
        }

        public static void AddDrifersIntoTheFrayLogic(Feat drifterFeat, bool asDedication = false)
        {
            drifterFeat.WithOnCreature((CalculatedCharacterSheetValues sheet, Creature self) =>
            {
                if (!asDedication || sheet.HasFeat(GunslingerFeatNames.WayOfTheDrifterDedication))
                {
                    self.AddQEffect(new QEffect(drifterFeat.BaseName, "Stride as free action on Initiative")
                    {
                        StartOfCombat = async (startOfCombat) =>
                        {
                            startOfCombat.Owner.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtEndOfYourTurn)
                            {
                                StartOfYourPrimaryTurn = async (QEffect startOfTurn, Creature owner) =>
                                {
                                    // Prompts for reaction, then has the user select a tile closer to the enemy then strides towards it.
                                    if (await owner.Battle.AskForConfirmation(owner, IllustrationName.FreeAction, "Stride as a free action towards a creature?", "Yes"))
                                    {
                                        Tile? tileToStrideTo = await GetTileCloserToEnemy(owner, "Stride towards the selected enemy or right-click to cancel.", owner.HasEffect(QEffectId.Prone));
                                        if (tileToStrideTo != null)
                                        {
                                            await owner.MoveTo(tileToStrideTo, null, new MovementStyle()
                                            {
                                                MaximumSquares = owner.Speed,
                                            });
                                        }
                                    }

                                    startOfTurn.ExpiresAt = ExpirationCondition.Immediately;
                                }
                            });
                        }
                    });
                }
            });
        }

        /// <summary>
        /// Adds Reconteurs Reload logic
        /// </summary>
        /// <param name="pistoleroWay">The Pistolero way</param>
        public static void WithPistolerosRaconteursReloadLogic(this GunslingerWay pistoleroWay)
        {
            Feat pistoleroFeat = pistoleroWay.Feat;
            // Adds a permanent Raconteurs Reload action if the appropiate weapon is held
            pistoleroFeat.WithPermanentQEffect("Reload and Create a Diversion or Demoralize", delegate (QEffect self)
            {
                self.ProvideActionIntoPossibilitySection = (QEffect raconteursReloadEffect, PossibilitySection possibilitySection) =>
                {
                    if (possibilitySection.PossibilitySectionId == PossibilitySectionId.MainActions)
                    {
                        Creature owner = raconteursReloadEffect.Owner;
                        SubmenuPossibility raconteursReloadMenu = new SubmenuPossibility(IllustrationName.GenericCombatManeuver, "Raconteurs Reload");

                        foreach (Item item in owner.HeldItems.Where(item => FirearmUtilities.IsItemFirearmOrCrossbow(item) && item.WeaponProperties != null))
                        {
                            // Creates a Raconteurs Reload button
                            PossibilitySection raconteursReloadSection = new PossibilitySection(item.Name);

                            // Adds the 1 action boost that acts as an extended leap
                            CombatAction demoralizeAction = CommonCombatActions.Demoralize(owner);
                            demoralizeAction.ActionCost = 1;
                            demoralizeAction.Item = item;
                            demoralizeAction.Name = "Raconteur's Reload (Demoralize)";
                            demoralizeAction.Illustration = new SideBySideIllustration(item.Illustration, demoralizeAction.Illustration);
                            demoralizeAction.Description = "Interact to reload and then attempt an Intimidation check to Demoralize.\n\n" + demoralizeAction.Description;
                            demoralizeAction.WithEffectOnSelf(async innerSelf =>
                            {
                                await FirearmUtilities.AwaitReloadItem(owner, item);
                            });

                            // Checks if the item needs to be reloaded
                            ((CreatureTarget)demoralizeAction.Target).WithAdditionalConditionOnTargetCreature((Creature attacker, Creature defender) =>
                            {
                                if (FirearmUtilities.IsItemLoaded(item) && !FirearmUtilities.IsMultiAmmoWeaponReloadable(item))
                                {
                                    return Usability.NotUsable("Item is already loaded.");
                                }

                                return Usability.Usable;
                            });

                            ActionPossibility demoralizePossibility = new ActionPossibility(demoralizeAction);
                            raconteursReloadSection.AddPossibility(demoralizePossibility);

                            CombatAction createADiversion = CommonCombatActions.CreateADiversion(owner);

                            createADiversion.ActionCost = 1;
                            createADiversion.Item = item;
                            createADiversion.Name = "Raconteur's Reload (Diversion)";
                            createADiversion.Illustration = new SideBySideIllustration(item.Illustration, createADiversion.Illustration);
                            createADiversion.Description = "Interact to reload and then attempt a Deception check to Create a Diversion.\n\n" + createADiversion.Description;

                            if (createADiversion.Target != null && createADiversion.Target is MultipleCreatureTargetsTarget createADiversionTarget)
                            {
                                createADiversionTarget.WithAdditionalRequirementOnSelf((restrictionOwner, targets) =>
                                {
                                    if (FirearmUtilities.IsItemLoaded(item) && !FirearmUtilities.IsMultiAmmoWeaponReloadable(item))
                                    {
                                        return Usability.NotUsable("Item is already loaded.");
                                    }

                                    return Usability.Usable;
                                });
                            }

                            // Adds all the posibilites for each weapon and finalizes the button
                            raconteursReloadSection.AddPossibility(new ActionPossibility(createADiversion));

                            raconteursReloadMenu.Subsections.Add(raconteursReloadSection);
                        }

                        return raconteursReloadMenu;
                    }

                    return null;
                };
                self.AfterYouTakeAction = async (QEffect dischargeItem, CombatAction action) =>
                {
                    if (action.ActionId == GunslingerActionIDs.BlackPowderBoost && action.Item != null)
                    {
                        FirearmUtilities.DischargeItem(action.Item);
                    }
                };
                self.YouBeginAction = async (QEffect reloadItem, CombatAction action) =>
                {
                    if (action.Name == "Raconteur's Reload (Diversion)" && action.Item != null)
                    {
                        await FirearmUtilities.AwaitReloadItem(reloadItem.Owner, action.Item);
                    }
                };
            });
        }

        /// <summary>
        /// Adds Ten Paces logic
        /// </summary>
        /// <param name="pistoleroWay">The Pistolero way</param>
        public static void WithPistolerosTenPacesLogic(this GunslingerWay pistoleroWay)
        {
            Feat pistoleroFeat = pistoleroWay.Feat;
            AddPistolerosTenPacesLogic(pistoleroFeat);
        }

        public static void AddPistolerosTenPacesLogic(Feat pistoleroFeat, bool asDedication = false)
        {
            // Adds a permanent bonus to initiative and a start of combat stide with no reaction of 10 ft.
            pistoleroFeat.WithOnCreature((CalculatedCharacterSheetValues sheet, Creature self) =>
            {
                if (!asDedication || sheet.HasFeat(GunslingerFeatNames.WayOfThePistoleroDedication))
                {
                    self.AddQEffect(new QEffect(pistoleroFeat.BaseName, "Step 10 ft on Initiative")
                    {
                        StartOfCombat = async (startOfCombat) =>
                        {
                            startOfCombat.Owner.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtEndOfYourTurn)
                            {
                                StartOfYourPrimaryTurn = async (QEffect startOfTurn, Creature owner) =>
                                {
                                    if (await owner.Battle.AskForConfirmation(owner, IllustrationName.FreeAction, "Step up to 10 ft as a free action?", "Yes"))
                                    {
                                        Tile? tileToStepTo = await GetStepableTileWithinRange(owner, "Choose which tile to step to or right-click to cancel.", owner.HasEffect(QEffectId.Prone) ? 1 : 2);
                                        if (tileToStepTo != null)
                                        {
                                            await owner.MoveTo(tileToStepTo, null, new MovementStyle()
                                            {
                                                MaximumSquares = 2,
                                                PermitsStep = true
                                            });
                                        }
                                    }

                                    startOfTurn.ExpiresAt = ExpirationCondition.Immediately;
                                }
                            });
                        },
                        BonusToInitiative = (bonusToInitiative) =>
                        {
                            return new Bonus(2, BonusType.Untyped, "Ten Paces", true);
                        }
                    });
                }
            });
        }

        /// <summary>
        /// Adds Covered Reload Logic
        /// </summary>
        /// <param name="sniperWay">The Sniper way</param>
        public static void WithSnipersCoveredReloadLogic(this GunslingerWay sniperWay)
        {
            Feat sniperFeat = sniperWay.Feat;
            // Adds a permanent Covered Reload action if the appropiate weapon is held
            sniperFeat.WithPermanentQEffect("Reload and Take Cover or Hide", delegate (QEffect self)
            {
                self.ProvideActionIntoPossibilitySection = (QEffect coveredReloadEffect, PossibilitySection possibilitySection) =>
                {
                    if (possibilitySection.PossibilitySectionId == PossibilitySectionId.MainActions)
                    {
                        Creature owner = coveredReloadEffect.Owner;
                        SubmenuPossibility coveredReloadMenu = new SubmenuPossibility(IllustrationName.GenericCombatManeuver, "Covered Reload");

                        foreach (Item item in owner.HeldItems.Where(item => FirearmUtilities.IsItemFirearmOrCrossbow(item) && item.WeaponProperties != null))
                        {
                            // Creates a Raconteurs Reload button
                            PossibilitySection raconteursReloadSection = new PossibilitySection(item.Name);

                            // Gets the self creature and checks if it can either hide, take cover, or both
                            Creature self = coveredReloadEffect.Owner;
                            bool canHide = self.Battle.AllCreatures.Any(cr => cr.EnemyOf(self) && HiddenRules.HasCoverOrConcealment(self, cr));
                            bool canTakeCover = !self.HasEffect(QEffectId.TakingCover);

                            CombatAction takeCoverAction = CommonCombatActions.TakeCover(self);
                            takeCoverAction.StrikeModifiers.OnEachTarget = async (Creature attacker, Creature defender, CheckResult result) =>
                            {
                                await FirearmUtilities.AwaitReloadItem(attacker, item);
                            };

                            takeCoverAction.ActionCost = 1;
                            takeCoverAction.Item = item;
                            takeCoverAction.Name = "Covered Reload (Take Cover)";
                            takeCoverAction.Illustration = new SideBySideIllustration(item.Illustration, takeCoverAction.Illustration);
                            takeCoverAction.Description = "Interact to reload and then Take Cover.\n\n" + takeCoverAction.Description;

                            if (takeCoverAction.Target != null && takeCoverAction.Target is SelfTarget takeCoverTarget)
                            {
                                takeCoverTarget.WithAdditionalRestriction((restrictionOwner) =>
                                {
                                    if (FirearmUtilities.IsItemLoaded(item) && !FirearmUtilities.IsMultiAmmoWeaponReloadable(item))
                                    {
                                        return "Item is already loaded";
                                    }
                                    return null;
                                });
                            }

                            ActionPossibility takeCoverPossibility = new ActionPossibility(takeCoverAction);
                            raconteursReloadSection.AddPossibility(takeCoverPossibility);

                            CombatAction hideAction = CommonCombatActions.CreateHide(self);

                            hideAction.ActionCost = 1;
                            hideAction.Item = item;
                            hideAction.Name = "Covered Reload (Hide)";
                            hideAction.Illustration = new SideBySideIllustration(item.Illustration, hideAction.Illustration);
                            hideAction.Description = "Interact to reload and then attempt a Stealth check to Hide.\n\n" + hideAction.Description;
                            hideAction.EffectOnChosenTargets += async delegate (CombatAction hideAction, Creature user, ChosenTargets target)
                            {
                                await FirearmUtilities.AwaitReloadItem(user, item);
                            };

                            if (hideAction.Target != null && hideAction.Target is SelfTarget hideTarget)
                            {
                                hideTarget.WithAdditionalRestriction((restrictionOwner) =>
                                {
                                    if (FirearmUtilities.IsItemLoaded(item) && !FirearmUtilities.IsMultiAmmoWeaponReloadable(item))
                                    {
                                        return "Item is already loaded";
                                    }
                                    else if (!restrictionOwner.Battle.AllCreatures.Any(cr => cr.EnemyOf(restrictionOwner) && cr.Occupies.FogOfWar != FogOfWar.Blackened && HiddenRules.CountsAsHavingCoverOrConcealment(restrictionOwner, cr)))
                                    {
                                        return "You don't have cover or concealment from any enemy.";
                                    }
                                    return null;
                                });
                            }

                            // Adds all the posibilites for each weapon and finalizes the button
                            raconteursReloadSection.AddPossibility(new ActionPossibility(hideAction));

                            coveredReloadMenu.Subsections.Add(raconteursReloadSection);
                        }

                        return coveredReloadMenu;
                    }

                    return null;
                };
                self.AfterYouTakeAction = async (QEffect dischargeItem, CombatAction action) =>
                {
                    if (action.ActionId == GunslingerActionIDs.BlackPowderBoost && action.Item != null)
                    {
                        FirearmUtilities.DischargeItem(action.Item);
                    }
                };
            });
        }

        /// <summary>
        /// Adds One Shot, One Kill logic
        /// </summary>
        /// <param name="sniperWay">The Sniper way</param>
        public static void WithSnipersOneShotOneKillLogic(this GunslingerWay sniperWay)
        {
            Feat sniperFeat = sniperWay.Feat;
            AddSnipersOneShotOneKillLogic(sniperFeat);
        }

        public static void AddSnipersOneShotOneKillLogic(Feat sniperFeat, bool asDedication = false)
        {
            // Adds a choice between rolling Stealth or Perception for initiative
            sniperFeat.WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                if (!asDedication || sheet.HasFeat(GunslingerFeatNames.WayOfTheSniperDedication))
                {
                    // Adds a choice between rolling Stealth or Perception for initiative
                    sheet.AddSelectionOption(new SingleFeatSelectionOption("Sniper Initiative Choice", "Sniper Initiative", SelectionOption.PRECOMBAT_PREPARATIONS_LEVEL, feat => feat.FeatName == GunslingerFeatNames.GunslingerSniperStealthInitiative || feat.FeatName == GunslingerFeatNames.GunslingerSniperPerceptionInitiative));
                }
            });

            // Adds a permanent bonus to initiaitve bonus/penalty depending on your stealth. And the start of combat effect for hiding and dealing more damage.
            sniperFeat.WithOnCreature((CalculatedCharacterSheetValues sheet, Creature self) =>
            {
                if (!asDedication || sheet.HasFeat(GunslingerFeatNames.WayOfTheSniperDedication))
                {
                    self.AddQEffect(new QEffect(sniperFeat.BaseName, "Can roll Stealth as Initiative and gain 1d6 percision on first Strike")
                    {
                        StartOfCombat = async (startOfCombat) =>
                        {
                            // If you are rolling stealth for initiative effects have to be applied
                            if (startOfCombat.Owner.HasFeat(GunslingerFeatNames.GunslingerSniperStealthInitiative))
                            {
                                string coverText = string.Empty;
                                if (!startOfCombat.Owner.Battle.AllCreatures.Any(cr => cr.EnemyOf(startOfCombat.Owner) && cr.Occupies.FogOfWar != FogOfWar.Blackened && HiddenRules.CountsAsHavingCoverOrConcealment(startOfCombat.Owner, cr)))
                                {
                                    coverText = ". You don't have cover or concealment from any enemy, so no stealth was applied";
                                }

                                // Handles the hiding on stealth initiative rolls
                                int stealthDC = startOfCombat.Owner.Initiative;
                                foreach (Creature enemy in startOfCombat.Owner.Battle.AllCreatures.Where(creature => !startOfCombat.Owner.FriendOf(creature) && HiddenRules.HasCoverOrConcealment(startOfCombat.Owner, creature) && creature.Initiative < stealthDC))
                                {
                                    startOfCombat.Owner.DetectionStatus.HiddenTo.Add(enemy);
                                }

                                startOfCombat.Owner.Battle.Log(startOfCombat.Owner.Name + " has rolled Stealth for initiative" + coverText + (startOfCombat.Owner.DetectionStatus.EnemiesYouAreHiddenFrom.Count() > 0 ? "and is hidden to:\n" + string.Join(",", startOfCombat.Owner.DetectionStatus.EnemiesYouAreHiddenFrom) : "."));

                                // If a Firearm or Crossbow is held the bonus damage is applied
                                if (startOfCombat.Owner.HeldItems.Any(item => FirearmUtilities.IsItemFirearmOrCrossbow(item)))
                                {
                                    startOfCombat.Owner.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtEndOfYourTurn)
                                    {
                                        Id = GunslingerQEIDs.OneShotOneKill,
                                        AddExtraWeaponDamage = (item) =>
                                        {
                                            if (FirearmUtilities.IsItemFirearmOrCrossbow(item) && item.WeaponProperties != null)
                                            {
                                                QEffect? oneShotOneKillEffect = startOfCombat.Owner.QEffects.FirstOrDefault(qe => qe.Id == GunslingerQEIDs.OneShotOneKill);
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
                                            QEffect? oneShotOneKillEffect = startOfCombat.Owner.QEffects.FirstOrDefault(qe => qe.Id == GunslingerQEIDs.OneShotOneKill);
                                            if (oneShotOneKillEffect != null && oneShotOneKillEffect.ExpiresAt == ExpirationCondition.Immediately)
                                            {
                                                afterAction.Owner.RemoveAllQEffects(qe => qe.Id == GunslingerQEIDs.OneShotOneKill);
                                            }
                                        }
                                    });
                                }
                            }
                        },
                        BonusToInitiative = (bonusToInitiative) =>
                        {
                            if (bonusToInitiative.Owner.HasFeat(GunslingerFeatNames.GunslingerSniperStealthInitiative))
                            {
                                int stealthAndPerceptionDifference = bonusToInitiative.Owner.Skills.Get(Skill.Stealth) - bonusToInitiative.Owner.Perception;
                                if (stealthAndPerceptionDifference != 0)
                                {
                                    return new Bonus(stealthAndPerceptionDifference, BonusType.Untyped, "One Shot, One Kill (Stealth)", stealthAndPerceptionDifference > 0);
                                }
                            }

                            return null;
                        }
                    });
                }
            });
        }

        /// <summary>
        /// Adds Clear a Path logic
        /// </summary>
        /// <param name="vanguardWay">The vanguard way</param>
        public static void WithVanguardClearAPathLogic(this GunslingerWay vanguardWay)
        {
            Feat vanguardFeat = vanguardWay.Feat;
            // Adds to the creature a state check to add the clear a path action to appropiate held weapons
            vanguardFeat.WithOnCreature(creature =>
            {
                creature.AddQEffect(new QEffect("Clear a Path {icon:Action}", "Reload and Shove")
                {
                    StateCheck = (permanentState) =>
                    {
                        Creature owner = permanentState.Owner;
                        owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                        {
                            ProvideMainAction = (clearAPathEffect) =>
                            {
                                Item? item = owner.HeldItems.FirstOrDefault(item => FirearmUtilities.IsItemFirearmOrCrossbow(item));
                                if (item == null)
                                {
                                    return null;
                                }

                                // Creates a shove action and updates it's properties to match Clear a Path
                                CombatAction clearAPathAction = CombatManeuverPossibilities.CreateShoveAction(owner, item);
                                clearAPathAction.Name = "Clear a Path";
                                clearAPathAction.Item = item;
                                clearAPathAction.ActionCost = 1;
                                clearAPathAction.Illustration = new SideBySideIllustration(item.Illustration, clearAPathAction.Illustration);
                                clearAPathAction.Description = vanguardWay.SlingersReloadRulesText.Substring(vanguardWay.SlingersReloadRulesText.IndexOf('\n') + 1);
                                clearAPathAction.Target = Target.Touch().WithAdditionalConditionOnTargetCreature((Creature a, Creature d) =>
                                {
                                    if (!item.HasTrait(Trait.TwoHanded))
                                    {
                                        return Usability.NotUsable("" + item.Name + " is NOT two-handed.");
                                    }
                                    else if (FirearmUtilities.IsItemLoaded(item) && !FirearmUtilities.IsMultiAmmoWeaponReloadable(item))
                                    {
                                        return Usability.NotUsable("Item is already loaded.");
                                    }

                                    return Usability.Usable;
                                });
                                StrikeModifiers strikeModifiers = clearAPathAction.StrikeModifiers;
                                strikeModifiers.OnEachTarget = async (Creature attacker, Creature defender, CheckResult result) =>
                                {
                                    await FirearmUtilities.AwaitReloadItem(attacker, item);
                                };
                                strikeModifiers.QEffectForStrike = new QEffect(ExpirationCondition.Immediately)
                                {
                                    BonusToSkillChecks = (Skill skill, CombatAction action, Creature? target) =>
                                    {
                                        if (skill == Skill.Athletics && action.Item != null && action.Item.WeaponProperties != null && action.Item.WeaponProperties.ItemBonus > 0)
                                        {
                                            return new Bonus(action.Item.WeaponProperties.ItemBonus, BonusType.Item, "Clear a Path", true);
                                        }
                                        return null;
                                    }
                                };

                                return new ActionPossibility(clearAPathAction);
                            },

                            // Determines what the last action and sets the MAP to one less if it was a ranged strike with this item
                            BeforeYourActiveRoll = async (QEffect beforeRoll, CombatAction action, Creature target) =>
                            {
                                List<CombatAction> actionsUsed = beforeRoll.Owner.Actions.ActionHistoryThisTurn.ToList();
                                if (actionsUsed.Count > 0)
                                {
                                    CombatAction lastAction = actionsUsed.Last();
                                    if (action.Name == "Clear a Path" && beforeRoll.Owner.Actions.AttackedThisManyTimesThisTurn > 0 && lastAction.HasTrait(Trait.Ranged) && lastAction.Name.ToLower().Contains("strike") && lastAction.Item != null && action.Item != null && action.Item == lastAction.Item)
                                    {
                                        beforeRoll.Owner.Actions.AttackedThisManyTimesThisTurn--;
                                        beforeRoll.Owner.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtEndOfYourTurn)
                                        {
                                            Id = GunslingerQEIDs.ClearAPath,
                                            Tag = lastAction.Item,
                                        });
                                    }
                                }
                            },

                            // Resets MAP and clears the effect
                            AfterYouTakeAction = async (QEffect afterRoll, CombatAction action) =>
                            {
                                if (action.Name == "Clear a Path" && afterRoll.Owner.HasEffect(GunslingerQEIDs.ClearAPath))
                                {
                                    afterRoll.Owner.Actions.AttackedThisManyTimesThisTurn++;
                                    afterRoll.Owner.RemoveAllQEffects(qe => qe.Id == GunslingerQEIDs.ClearAPath);
                                }
                            }
                        });
                    }
                });
            });
        }

        /// <summary>
        /// Adds Living Fortification logic
        /// </summary>
        /// <param name="vanguardWay">The vanguard way</param>
        public static void WithVanguardLivingFortificationLogic(this GunslingerWay vanguardWay)
        {
            Feat vanguardFeat = vanguardWay.Feat;
            // Adds a permanent Start of combat effect for living fortification
            vanguardFeat.WithPermanentQEffect("+1/+2 Circumstance AC on Initiative", delegate (QEffect self)
            {
                self.StartOfCombat = async (startOfCombat) =>
                {
                    int bonus = startOfCombat.Owner.HeldItems.Any(item => item.HasTrait(FirearmTraits.Parry)) ? 2 : 1;
                    startOfCombat.Owner.Battle.Log(startOfCombat.Owner.Name + " raises their weapon defensive. (Living Fortification)");
                    startOfCombat.Owner.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtStartOfYourTurn)
                    {
                        BonusToDefenses = (QEffect bonusToDefenses, CombatAction? action, Defense defense) =>
                        {
                            if (defense == Defense.AC)
                            {
                                return new Bonus(bonus, BonusType.Circumstance, "Living Fortification", true);
                            }

                            return null;
                        }
                    });

                };
                self.ExpiresAt = ExpirationCondition.ExpiresAtStartOfYourTurn;
            });
        }

        public static void AddVanguardLivingFortificationLogic(Feat vanguardFeat, bool asDedication = false)
        {
            // Adds a permanent Start of combat effect for living fortification
            vanguardFeat.WithOnCreature((CalculatedCharacterSheetValues sheet, Creature self) =>
            {
                if (!asDedication || sheet.HasFeat(GunslingerFeatNames.WayOfTheVanguardDedication))
                {
                    self.AddQEffect(new QEffect(vanguardFeat.BaseName, "+1/+2 Circumstance AC on Initiative")
                    {
                        StartOfCombat = async (startOfCombat) =>
                        {
                            if (startOfCombat.Owner.HeldItems.Any(item => FirearmUtilities.IsItemFirearmOrCrossbow(item)))
                            {
                                int bonus = startOfCombat.Owner.HeldItems.Any(item => item.HasTrait(FirearmTraits.Parry)) ? 2 : 1;
                                startOfCombat.Owner.Battle.Log(startOfCombat.Owner.Name + " raises their weapon defensive. (Living Fortification)");
                                startOfCombat.Owner.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtStartOfYourTurn)
                                {
                                    Name = vanguardFeat.BaseName,
                                    Description = $"+{bonus} to AC till the start of your next turn.",
                                    Illustration = IllustrationName.Shield,
                                    BonusToDefenses = (QEffect bonusToDefenses, CombatAction? action, Defense defense) =>
                                    {
                                        if (defense == Defense.AC)
                                        {
                                            return new Bonus(bonus, BonusType.Circumstance, "Living Fortification", true);
                                        }

                                        return null;
                                    }
                                });
                            }

                        },
                        ExpiresAt = ExpirationCondition.ExpiresAtStartOfYourTurn
                    });
                }
            });
        }

        /// <summary>
        /// Asyncronisly gets a user selected tile that is closer to an enemy
        /// </summary>
        /// <param name="self">The creature being used to compare distance</param>
        /// <param name="messageString">The user prompt message</param>
        /// <returns>The selected tile or null</returns>
        public static async Task<Tile?> GetTileCloserToEnemy(Creature self, string messageString, bool isCrawling)
        {
            // Determines the starting tile, all enemy tiles and initatlizes the options list
            Tile startingTile = self.Occupies;
            List<Tile> enemyTiles = self.Battle.AllCreatures.Where(creature => self != creature && !self.FriendOf(creature)).Select(creature => creature.Occupies).ToList();
            List<Option> options = new List<Option>();

            // Cycles through all map tiles and determines if the tile is closer to an enemy and if the user can actually reach the tile
            foreach (Tile tile in self.Battle.Map.AllTiles)
            {
                if (tile.IsFree && IsTileCloserToAnyOfTheseTiles(startingTile, tile, enemyTiles))
                {
                    MovementStyle movementStyle = new MovementStyle()
                    {
                        MaximumSquares = isCrawling ? 1 : self.Speed
                    };
                    PathfindingDescription pathfindingDescription = new PathfindingDescription()
                    {
                        Squares = movementStyle.MaximumSquares,
                        Style = movementStyle
                    };
                    IList<Tile>? pathToTiles = Pathfinding.GetPath(self, tile, tile.Battle, pathfindingDescription);
                    if (pathToTiles != null)
                    {
                        options.Add(new TileOption(tile, "Tile (" + tile.X + "," + tile.Y + ")", null, (AIUsefulness)int.MinValue, true));
                    }
                }
            }

            if (options.Count == 0)
            {
                await self.Battle.AskForConfirmation(self, IllustrationName.WarpStep, "There are no enemies visible, so you can not stride.", "Understood", "Lame, but ok");
                return null;
            }

            // Adds a Cancel Option
            options.Add(new CancelOption(true));

            // Prompts the user for their desired tile and returns it or null
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

        /// <summary>
        /// Asyncronisly gets a stepable tile within in given range
        /// </summary>
        /// <param name="self">The creature used to get the starting tile</param>
        /// <param name="messageString">The user prompt displayed</param>
        /// <param name="range">The range in tiles the user is allowed to step</param>
        /// <returns>The selected tile or null</returns>
        public static async Task<Tile?> GetStepableTileWithinRange(Creature self, string messageString, int range)
        {
            // Gets the starting tile, initatlizes the options and collects the possible tiles within range that the user can reach
            Tile startingTile = self.Occupies;
            List<Option> options = new List<Option>();
            foreach (Tile tile in self.Battle.Map.AllTiles)
            {
                if (tile.IsFree && startingTile.DistanceTo(tile) <= range)
                {
                    MovementStyle movementStyle = new MovementStyle()
                    {
                        MaximumSquares = range
                    };
                    PathfindingDescription pathfindingDescription = new PathfindingDescription()
                    {
                        Squares = movementStyle.MaximumSquares,
                        Style = movementStyle
                    };
                    IList<Tile>? pathToTiles = Pathfinding.GetPath(self, tile, tile.Battle, pathfindingDescription);
                    if (pathToTiles != null)
                    {
                        options.Add(new TileOption(tile, "Tile (" + tile.X + "," + tile.Y + ")", null, (AIUsefulness)int.MinValue, true));
                    }
                }
            }

            // Adds a Cancel Option
            options.Add(new CancelOption(true));

            // Prompts the user to select a valid tile and returns it or null
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


        /// <summary>
        /// Determines if the potentially closer tile is infact closer to the any of the tiles to check than the original tile
        /// </summary>
        /// <param name="originalTile">The original Tile</param>
        /// <param name="potentialCloserTile">The potentially closer tile</param>
        /// <param name="tilesToCheck">The list of possible tiles to check</param>
        /// <returns>True if the potential closer tiles if closer to any of the tiles to check and False otherwise</returns>
        private static bool IsTileCloserToAnyOfTheseTiles(Tile originalTile, Tile potentialCloserTile, List<Tile> tilesToCheck)
        {
            // Determines if the potentially closer tile is infact closer to the any of the tiles to check than the original tile
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