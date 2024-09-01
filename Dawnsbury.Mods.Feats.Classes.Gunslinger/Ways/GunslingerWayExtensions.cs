using Dawnsbury.Audio;
using Dawnsbury.Auxiliary;
using Dawnsbury.Core;
using Dawnsbury.Core.Animations.Movement;
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
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
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
            drifterFeat.WithPermanentQEffect("Drifter's Reloading Strike", delegate (QEffect self)
            {
                self.ProvideMainAction = (reloadingStrikeShotEffect) =>
                {
                    // Collects the Strikes owner and if they have any ranged and melee options that meet the action requirements. If they don't return null to hide the action.
                    Creature owner = reloadingStrikeShotEffect.Owner;
                    Item? ranged = owner.HeldItems.FirstOrDefault(item => FirearmUtilities.IsItemFirearmOrCrossbow(item) && (!FirearmUtilities.IsItemLoaded(item) || FirearmUtilities.IsMultiAmmoWeaponReloadable(item)) && item.HasTrait(Trait.Ranged) && !item.HasTrait(Trait.TwoHanded));
                    Item? melee = owner.HeldItems.FirstOrDefault(item => item.HasTrait(Trait.Melee) && !item.HasTrait(Trait.TwoHanded));
                    if (ranged == null || melee == null && !owner.HasFreeHand)
                    {
                        return null;
                    }

                    // Creates and returns the action with all desired restrictions
                    return new ActionPossibility(new CombatAction(reloadingStrikeShotEffect.Owner, new SideBySideIllustration(ranged.Illustration, melee != null ? melee.Illustration : IllustrationName.Fist), "Reloading Strike", [Trait.Basic], driftersWay.SlingersReloadRulesText, Target.Self()
                    .WithAdditionalRestriction(self => self.Battle != null && self.Battle.AllCreatures.Count(creature => self.DistanceTo(creature) <= 1 && creature != self && !self.FriendOf(creature)) > 0 ? null : "No valid melee targets."))
                    .WithActionCost(1).WithItem(ranged)
                    .WithEffectOnEachTarget(async (CombatAction action, Creature attacker, Creature defender, CheckResult result) =>
                    {
                        // Collects action possibilities for the Strike subaction.
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

                        // If there are possible melee actions, prompt for choice then strike, otherwise strike with the only option.
                        if (options.Count != 0)
                        {
                            Option chosenStrike = options[0];
                            if (options.Count >= 2)
                            {
                                chosenStrike = (await owner.Battle.SendRequest(new AdvancedRequest(owner, "Choose a creature to Strike.", options)
                                {
                                    TopBarText = "Choose a creature to Strike.",
                                    TopBarIcon = melee != null ? melee.Illustration : IllustrationName.Fist
                                })).ChosenOption;
                            }

                            if (chosenStrike != null && chosenStrike is not CancelOption)
                            {
                                await chosenStrike.Action();
                            }
                        }

                        // Reload item
                        Gunslinger.AwaitReloadItem(attacker, ranged);
                    }));
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
            // Adds a permanent start of combat effect where you can stride to a tile closer to an enemy
            drifterFeat.WithPermanentQEffect("Drifter's Into the Fray", delegate (QEffect self)
            {
                self.StartOfCombat = async (startOfCombat) =>
                {
                    // Prompts for reaction, then has the user select a tile closer to the enemy then strides towards it.
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

        /// <summary>
        /// Adds Reconteurs Reload logic
        /// </summary>
        /// <param name="pistoleroWay">The Pistolero way</param>
        public static void WithPistolerosRaconteursReloadLogic(this GunslingerWay pistoleroWay)
        {
            Feat pistoleroFeat = pistoleroWay.Feat;
            // Adds to the creature a state check that provides either an action to Demoralize or Create a Diversion for each item.
            pistoleroFeat.WithOnCreature(creature =>
            {
                creature.AddQEffect(new QEffect()
                {
                    StateCheck = (permanentState) =>
                    {
                        foreach (Item heldItem in permanentState.Owner.HeldItems)
                        {
                            permanentState.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                            {
                                ProvideMainAction = (raconteursReloadDemoralizeEffect) =>
                                {
                                    // If the item is acceptable, the Demoarlize action is made as an option, with a Reload attached.
                                    if (FirearmUtilities.IsItemFirearmOrCrossbow(heldItem) && (!FirearmUtilities.IsItemLoaded(heldItem) || FirearmUtilities.IsMultiAmmoWeaponReloadable(heldItem)) && heldItem.WeaponProperties != null)
                                    {
                                        CombatAction demoralizeAction = CommonCombatActions.Demoralize(permanentState.Owner);
                                        demoralizeAction.ActionCost = 1;
                                        demoralizeAction.Name = "Raconteur's Reload (Demoralize)";
                                        demoralizeAction.Illustration = new SideBySideIllustration(heldItem.Illustration, demoralizeAction.Illustration);
                                        demoralizeAction.Description = "Interact to reload and then attempt an Intimidation check to Demoralize.";
                                        demoralizeAction.WithEffectOnSelf(async innerSelf =>
                                        {
                                            Gunslinger.AwaitReloadItem(raconteursReloadDemoralizeEffect.Owner, heldItem);
                                        });

                                        return new ActionPossibility(demoralizeAction);
                                    }

                                    return null;
                                }
                            });
                            permanentState.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                            {
                                ProvideMainAction = (raconteursReloadDiversionEffect) =>
                                {
                                    // If the item is acceptable, the Create a Diversion action is made as an option, with a Reload attached.
                                    if (FirearmUtilities.IsItemFirearmOrCrossbow(heldItem) && (!FirearmUtilities.IsItemLoaded(heldItem) || FirearmUtilities.IsMultiAmmoWeaponReloadable(heldItem)) && heldItem.WeaponProperties != null)
                                    {
                                        Creature self = raconteursReloadDiversionEffect.Owner;

                                        // HACK: Currently there is no CommonCombatActions for 'Create a Diversion' this should be replaced with that if it is ever added.
                                        CombatAction createADiversion = new CombatAction(self, (Illustration)IllustrationName.CreateADiversion, "Create a Diversion", new Trait[6]
                                        {
                                            Trait.Basic,
                                            Trait.Mental,
                                            Trait.AttackDoesNotTargetAC,
                                            Trait.AlwaysHits,
                                            Trait.Auditory,
                                            Trait.Linguistic
                                        }, "Choose any number of enemy creatures you can see.\n\nMake one Deception check against the Perception DC of all those creatures. On a success, you become Hidden to those creatures.\n\nWhether or not you succeed,  creatures you attempt to divert gain a +4 circumstance bonus to their Perception DCs against your attempts to Create a Diversion for the rest of the encounter.", Target.MultipleCreatureTargets(Target.Ranged(100), Target.Ranged(100), Target.Ranged(100), Target.Ranged(100), Target.Ranged(100), Target.Ranged(100), Target.Ranged(100), Target.Ranged(100), Target.Ranged(100), Target.Ranged(100), Target.Ranged(100), Target.Ranged(100), Target.Ranged(100), Target.Ranged(100), Target.Ranged(100), Target.Ranged(100), Target.Ranged(100), Target.Ranged(100), Target.Ranged(100), Target.Ranged(100)).WithMinimumTargets(1).WithMustBeDistinct().WithSimultaneousAnimation()).WithSoundEffect(SfxName.Feint).WithActionId(ActionId.CreateADiversion);
                                        createADiversion.WithEffectOnChosenTargets(async (creature, targets) =>
                                        {
                                            Gunslinger.AwaitReloadItem(raconteursReloadDiversionEffect.Owner, heldItem);
                                            int roll = R.NextD20();
                                            foreach (Creature chosenCreature in targets.ChosenCreatures)
                                            {
                                                CheckBreakdown breakdown = CombatActionExecution.BreakdownAttack(new CombatAction(self, null, "Create a Diversion", new Trait[1]
                                                {
                                                    Trait.Basic
                                                }, "[this condition has no description]", Target.Self()).WithActionId(ActionId.CreateADiversion).WithActiveRollSpecification(new ActiveRollSpecification(Checks.SkillCheck(Skill.Deception), Checks.DefenseDC(Defense.Perception))), chosenCreature);
                                                CheckBreakdownResult breakdownResult = new CheckBreakdownResult(breakdown, roll);
                                                string str1 = breakdown.DescribeWithFinalRollTotal(breakdownResult);
                                                StringBuilder stringHandler;
                                                int d20Roll;
                                                if (breakdownResult.CheckResult >= CheckResult.Success)
                                                {
                                                    self.DetectionStatus.HiddenTo.Add(chosenCreature);
                                                    Tile occupies = chosenCreature.Occupies;
                                                    Color lightBlue = Color.LightBlue;
                                                    string str2 = self?.ToString();
                                                    string str3 = chosenCreature?.ToString();
                                                    stringHandler = new StringBuilder(10);
                                                    stringHandler.Append(" (");
                                                    d20Roll = breakdownResult.D20Roll;
                                                    string str4 = d20Roll.ToString() + breakdown.TotalCheckBonus.WithPlus();
                                                    stringHandler.Append(str4);
                                                    stringHandler.Append("=");
                                                    stringHandler.Append(breakdownResult.D20Roll + breakdown.TotalCheckBonus);
                                                    stringHandler.Append(" vs. ");
                                                    stringHandler.Append(breakdown.TotalDC);
                                                    stringHandler.Append(").");
                                                    string stringAndClear = stringHandler.ToString();
                                                    string log = str2 + " successfully hid from " + str3 + stringAndClear;
                                                    string logDetails = str1;
                                                    occupies.Overhead("hidden from", lightBlue, log, "Create a Diversion", logDetails);
                                                }
                                                else
                                                {
                                                    Tile occupies = chosenCreature.Occupies;
                                                    Color red = Color.Red;
                                                    string str5 = self?.ToString();
                                                    string str6 = chosenCreature?.ToString();
                                                    stringHandler = new StringBuilder(10);
                                                    stringHandler.Append(" (");
                                                    d20Roll = breakdownResult.D20Roll;
                                                    string str7 = d20Roll.ToString() + breakdown.TotalCheckBonus.WithPlus();
                                                    stringHandler.Append(str7);
                                                    stringHandler.Append("=");
                                                    stringHandler.Append(breakdownResult.D20Roll + breakdown.TotalCheckBonus);
                                                    stringHandler.Append(" vs. ");
                                                    stringHandler.Append(breakdown.TotalDC);
                                                    stringHandler.Append(").");
                                                    string stringAndClear = stringHandler.ToString();
                                                    string log = str5 + " failed to hide from " + str6 + stringAndClear;
                                                    string logDetails = str1;
                                                    occupies.Overhead("diversion failed", red, log, "Create a Diversion", logDetails);
                                                }
                                                chosenCreature.AddQEffect(new QEffect()
                                                {
                                                    BonusToDefenses = (effect, action, defense) =>
                                                    {
                                                        if (defense != Defense.Perception || action == null || action.ActionId != ActionId.CreateADiversion || action.Owner != creature)
                                                            return null;
                                                        if (creature.HasEffect(QEffectId.ConfabulatorLegendary))
                                                            return null;
                                                        if (creature.HasEffect(QEffectId.ConfabulatorMaster))
                                                            return new Bonus(1, BonusType.Circumstance, "Fool me twice... (Confabulator master)");
                                                        return creature.HasEffect(QEffectId.ConfabulatorExpert) ? new Bonus(2, BonusType.Circumstance, "Fool me twice... (Confabulator)") : new Bonus(4, BonusType.Circumstance, "Fool me twice...");
                                                    }
                                                });
                                            }
                                        });
                                        // HACK: This is the end of the 'Create a Diversion'

                                        createADiversion.ActionCost = 1;
                                        createADiversion.Name = "Raconteur's Reload (Diversion)";
                                        createADiversion.Illustration = new SideBySideIllustration(heldItem.Illustration, createADiversion.Illustration);
                                        createADiversion.Description = "Interact to reload and then attempt a Deception check to Create a Diversion.";

                                        return new ActionPossibility(createADiversion);
                                    }

                                    return null;
                                }
                            });
                        }
                    }
                });
            });
        }

        /// <summary>
        /// Adds Ten Paces logic
        /// </summary>
        /// <param name="pistoleroWay">The Pistolero way</param>
        public static void WithPistolerosTenPacesLogic(this GunslingerWay pistoleroWay)
        {
            Feat pistoleroFeat = pistoleroWay.Feat;
            // Adds a permanent bonus to initiative and a start of combat stide with no reaction of 10 ft.
            // TODO: UPDATE
            pistoleroFeat.WithPermanentQEffect("Pistolero's Ten Paces", delegate (QEffect self)
            {
                self.StartOfCombat = async (startOfCombat) =>
                {
                    if (await startOfCombat.Owner.Battle.AskForConfirmation(startOfCombat.Owner, IllustrationName.FreeAction, "Step up to 10 ft as a free action?", "Yes"))
                    {
                        Tile? tileToStepTo = await GetStepableTileWithinRange(startOfCombat.Owner, "Choose which tile to step to.", 2);
                        if (tileToStepTo != null)
                        {
                            await startOfCombat.Owner.MoveTo(tileToStepTo, null, new MovementStyle()
                            {
                                MaximumSquares = 2,
                                PermitsStep = true
                            });
                        }
                    }
                };
                self.BonusToInitiative = (bonusToInitiative) =>
                {
                    return new Bonus(2, BonusType.Untyped, "Ten Paces", true);
                };
            });
        }

        /// <summary>
        /// Adds Covered Reload Logic
        /// </summary>
        /// <param name="sniperWay">The Sniper way</param>
        public static void WithSnipersCoveredReloadLogic(this GunslingerWay sniperWay)
        {
            Feat sniperFeat = sniperWay.Feat;
            // Adds to the creature a state check to add the covered reload action to appropiate held weapons.
            sniperFeat.WithOnCreature(creature =>
            {
                creature.AddQEffect(new QEffect()
                {
                    StateCheck = (permanentState) =>
                    {
                        foreach (Item heldItem in permanentState.Owner.HeldItems)
                        {
                            permanentState.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                            {
                                ProvideMainAction = (coveredReloadEffect) =>
                                {
                                    if (FirearmUtilities.IsItemFirearmOrCrossbow(heldItem) && (!FirearmUtilities.IsItemLoaded(heldItem) || FirearmUtilities.IsMultiAmmoWeaponReloadable(heldItem)) && heldItem.WeaponProperties != null)
                                    {
                                        // Gets the self creature and checks if it can either hide, take cover, or both
                                        Creature self = coveredReloadEffect.Owner;
                                        bool canHide = self.Battle.AllCreatures.Any(cr => cr.EnemyOf(self) && HiddenRules.HasCoverOrConcealment(self, cr));
                                        bool canTakeCover = !self.HasEffect(QEffectId.TakingCover);

                                        // Creates the Cover Reload action that will prompt the user for which effect they would like on use.
                                        CombatAction coveredReloadAciton = new CombatAction(self, heldItem.Illustration, "Covered Reload", [Trait.Basic], sniperWay.SlingersReloadRulesText, Target.Self().WithAdditionalRestriction(targetingSelf => !canHide && !canTakeCover ? "Can't Take Cover or Hide" : null));
                                        coveredReloadAciton.Item = heldItem;
                                        coveredReloadAciton.ActionCost = 1;
                                        coveredReloadAciton.WithEffectOnSelf(async (self) =>
                                        {
                                            Gunslinger.AwaitReloadItem(self, heldItem);

                                            // HACK: Currently there is no CommonCombatActions for 'Take Cover' this should be replaced with that if it is ever added.
                                            CombatAction takeCoverAction = new CombatAction(self, (Illustration)IllustrationName.TakeCover, "Take cover", new Trait[1]
                                            {
                                                Trait.Basic
                                            }, "{i}You hunker down.{/i}\n\nUntil you move, attack or become Unconscious, any standard cover you have instead counts as greater cover (you don't gain this benefit against creatures against whom you don't have standard cover).\n\nIn addition, if you're prone, you count as having greater cover from all ranged attacks.", Target.Self().WithAdditionalRestriction(innerSelf => !innerSelf.HasEffect(QEffectId.TakingCover) ? null : "You're already taking cover."))
                                            .WithActionCost(0)
                                            .WithEffectOnSelf(async innerSelf =>
                                            {
                                                innerSelf.AddQEffect(new QEffect("Taking cover", "Until you move, attack or become Unconscious, any standard cover you have instead counts as greater cover (you don't gain this benefit against creatures against whom you don't have standard cover).\n\nIn addition, if you're prone, you count as having greater cover from all ranged attacks.", ExpirationCondition.Never, innerSelf, (Illustration)IllustrationName.TakeCover)
                                                {
                                                    Id = QEffectId.TakingCover,
                                                    CountsAsABuff = true,
                                                    StateCheck = sc =>
                                                    {
                                                        if (!sc.Owner.HasEffect(QEffectId.Unconscious))
                                                            return;
                                                        sc.ExpiresAt = ExpirationCondition.Immediately;
                                                    },
                                                    AfterYouTakeAction = async (qfSelf, action) =>
                                                    {
                                                        if (action.Name == "Covered Reload" || !action.HasTrait(Trait.Attack) && !action.HasTrait(Trait.Move))
                                                            return;
                                                        qfSelf.ExpiresAt = ExpirationCondition.Immediately;
                                                    },
                                                    IncreaseCover = (qfSelf, attack, existingCover) => attack != null && attack.HasTrait(Trait.Ranged) && qfSelf.Owner.HasEffect(QEffectId.Prone) && existingCover < CoverKind.Greater || existingCover == CoverKind.Standard ? CoverKind.Greater : existingCover
                                                });
                                            });
                                            // HACK: This is the end of the 'Take Cover'

                                            // HACK: Currently there is no CommonCombatActions for 'Hide' this should be replaced with that if it is ever added.
                                            CombatAction hideAction = new CombatAction(self, (Illustration)IllustrationName.Hide, "Hide", new Trait[2]
                                            {
                                                Trait.Basic,
                                                Trait.AttackDoesNotTargetAC
                                            }, "Make one Stealth check against the Perception DCs of each enemy creature that can see you but that you have cover or concealment from. On a success, you become Hidden to that creature.", Target.Self((cr, ai) => ai.HideSelf()).WithAdditionalRestriction(innerSelf =>
                                            {
                                                if (HiddenRules.IsHiddenFromAllEnemies(innerSelf, innerSelf.Occupies))
                                                    return "You're already hidden from all enemies.";
                                                return !innerSelf.Battle.AllCreatures.Any(cr => cr.EnemyOf(innerSelf) && HiddenRules.HasCoverOrConcealment(innerSelf, cr)) ? "You don't have cover or concealment from any enemy." : null;
                                            })).WithActionCost(0).WithActionId(ActionId.Hide).WithSoundEffect(SfxName.Hide).WithEffectOnSelf(async innerSelf =>
                                            {
                                                int roll = R.NextD20();
                                                foreach (Creature creature in innerSelf.Battle.AllCreatures.Where<Creature>(cr => cr.EnemyOf(innerSelf)))
                                                {
                                                    if (!innerSelf.DetectionStatus.HiddenTo.Contains(creature) && HiddenRules.HasCoverOrConcealment(innerSelf, creature))
                                                    {
                                                        CheckBreakdown breakdown = CombatActionExecution.BreakdownAttack(new CombatAction(innerSelf, (Illustration)IllustrationName.Hide, "Hide", new Trait[1]
                                                        {
                                                            Trait.Basic
                                                        }, "[this condition has no description]", Target.Self()).WithActiveRollSpecification(new ActiveRollSpecification(Checks.SkillCheck(Skill.Stealth), Checks.DefenseDC(Defense.Perception))), creature);
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
                                                    // HACK: This is the end of the 'Hide'
                                                }
                                            });

                                            // If you can Hide and Take Cover prompt the user to pick
                                            if (canHide && canTakeCover)
                                            {
                                                if (canHide && canTakeCover && await self.Battle.AskForConfirmation(self, coveredReloadAciton.Illustration, "Choose to Take Cover or Hide.", "Take Cover", "Hide"))
                                                {
                                                    await takeCoverAction.AllExecute();
                                                }
                                                else
                                                {
                                                    await hideAction.AllExecute();
                                                }
                                            }

                                            // Prompts the user to use their only option between Take Cover and Hide
                                            if (!canHide && await self.Battle.AskForConfirmation(self, coveredReloadAciton.Illustration, "Would you like to Take Cover as a free action?", "Yes"))
                                            {
                                                await takeCoverAction.AllExecute();
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

        /// <summary>
        /// Adds One Shot, One Kill logic
        /// </summary>
        /// <param name="sniperWay">The Sniper way</param>
        public static void WithSnipersOneShotOneKillLogic(this GunslingerWay sniperWay)
        {
            Feat sniperFeat = sniperWay.Feat;
            // Adds a choice between rolling Stealth or Perception for initiative
            sniperFeat.WithOnSheet((character) =>
            {
                character.AddSelectionOption(new SingleFeatSelectionOption("Sniper Initiative Choice", "Sniper Initiative", 1, feat => feat.FeatName == GunslingerFeatNames.GunslingerSniperStealthInitiative || feat.FeatName == GunslingerFeatNames.GunslingerSniperPerceptionInitiative));
            });

            // Adds a permanent bonus to initiaitve bonus/penalty depending on your stealth. And the start of combat effect for hiding and dealing more damage.
            sniperFeat.WithPermanentQEffect("Sniper's One Shot, One Kill", delegate (QEffect self)
            {
                self.StartOfCombat = async (startOfCombat) =>
                {
                    // If you are rolling stealth for initiative effects have to be applied
                    if (self.Owner.HasFeat(GunslingerFeatNames.GunslingerSniperStealthInitiative))
                    {
                        // Handles the hiding on stealth initiative rolls
                        int stealthDC = self.Owner.Initiative;
                        foreach (Creature enemy in self.Owner.Battle.AllCreatures.Where(creature => !self.Owner.FriendOf(creature) && HiddenRules.HasCoverOrConcealment(self.Owner, creature) && creature.Initiative < stealthDC))
                        {
                            self.Owner.DetectionStatus.HiddenTo.Add(enemy);
                        }

                        self.Owner.Battle.Log(self.Owner.Name + " has rolled Stealth for initiative" + (self.Owner.DetectionStatus.EnemiesYouAreHiddenFrom.Count() > 0 ? "and is hidden to:\n" + string.Join(",", self.Owner.DetectionStatus.EnemiesYouAreHiddenFrom) : "."));

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
                };

                // Adds a bonus/penalty depending on the difference between Stealth and Perception
                self.BonusToInitiative = (bonusToInitiative) =>
                {
                    if (self.Owner.HasFeat(GunslingerFeatNames.GunslingerSniperStealthInitiative))
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

        /// <summary>
        /// Adds Clear a Path logic
        /// </summary>
        /// <param name="vanguardWay">The vanguard way</param>
        public static void WithVanguardClearAPathLogic(this GunslingerWay vanguardWay)
        {
            Feat vanguardFeat = vanguardWay.Feat;
            // Adds to the creature a state check to add the clear a path action to appropiate held weapons.
            vanguardFeat.WithOnCreature(creature =>
            {
                creature.AddQEffect(new QEffect()
                {
                    StateCheck = (permanentState) =>
                    {
                        Creature owner = permanentState.Owner;
                        if (owner.HeldItems.Count == 1 && owner.HeldItems.Any(item => item.HasTrait(Trait.TwoHanded) && FirearmUtilities.IsItemFirearmOrCrossbow(item)))
                        {
                            Item item = owner.HeldItems[0];
                            owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                            {
                                ProvideMainAction = (clearAPathEffect) =>
                                {
                                    if ((!FirearmUtilities.IsItemLoaded(item) || FirearmUtilities.IsMultiAmmoWeaponReloadable(item)) && item.WeaponProperties != null)
                                    {
                                        // Creates a shove action and updates it's properties to match Clear a Path
                                        CombatAction clearAPathAction = Possibilities.CreateShove(owner);
                                        clearAPathAction.Name = "Clear a Path";
                                        clearAPathAction.Item = item;
                                        clearAPathAction.ActionCost = 1;
                                        clearAPathAction.Illustration = new SideBySideIllustration(item.Illustration, clearAPathAction.Illustration);
                                        clearAPathAction.Description = vanguardWay.SlingersReloadRulesText;
                                        StrikeModifiers strikeModifiers = clearAPathAction.StrikeModifiers;
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
                                        clearAPathAction.WithEffectOnSelf((self) =>
                                        {
                                            Gunslinger.AwaitReloadItem(self, item);
                                        });

                                        return new ActionPossibility(clearAPathAction);
                                    }

                                    return null;
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
            vanguardFeat.WithPermanentQEffect("Living Fortification", delegate (QEffect self)
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

        /// <summary>
        /// Asyncronisly gets a user selected tile that is closer to an enemy
        /// </summary>
        /// <param name="self">The creature being used to compare distance</param>
        /// <param name="messageString">The user prompt message</param>
        /// <returns>The selected tile or null</returns>
        public static async Task<Tile?> GetTileCloserToEnemy(Creature self, string messageString)
        {
            // Determines the starting tile, all enemy tiles and initatlizes the options list
            Tile startingTile = self.Occupies;
            List<Tile> enemyTiles = self.Battle.AllCreatures.Where(creature => self != creature && !self.FriendOf(creature)).Select(creature => creature.Occupies).ToList();
            List<Option> options = new List<Option>();

            // Cycles through all map tiles and determines if the tile is closer to an enemy and if the user can actually reach the tile
            foreach (Tile tile in self.Battle.Map.AllTiles)
            {
                // HACK: Pathfinding is internal, So the reflection should be removed when and if it is made public
                if (tile.IsFree && IsTileCloserToAnyOfTheseTiles(startingTile, tile, enemyTiles))
                {
                    MovementStyle movementStyle = new MovementStyle()
                    {
                        MaximumSquares = self.Speed
                    };
                    PathfindingDescription pathfindingDescription = new PathfindingDescription()
                    {
                        Squares = movementStyle.MaximumSquares,
                        Style = movementStyle
                    };
                    IList<Tile>? pathToTiles = (IList<Tile>?)(typeof(ModManager).Assembly.GetType("Dawnsbury.Core.Intelligence.Pathfinding").GetMethod("GetPath", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Invoke(null, [self, tile, tile.Battle, pathfindingDescription]));
                    if (pathToTiles != null)
                    {
                        options.Add(new TileOption(tile, "Tile (" + tile.X + "," + tile.Y + ")", null, (AIUsefulness)int.MinValue, true));
                    }
                }
            }

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
                    // HACK: Pathfinding is internal, So the reflection should be removed when and if it is made public
                    MovementStyle movementStyle = new MovementStyle()
                    {
                        MaximumSquares = range
                    };
                    PathfindingDescription pathfindingDescription = new PathfindingDescription()
                    {
                        Squares = movementStyle.MaximumSquares,
                        Style = movementStyle
                    };
                    IList<Tile>? pathToTiles = (IList<Tile>?)(typeof(ModManager).Assembly.GetType("Dawnsbury.Core.Intelligence.Pathfinding").GetMethod("GetPath", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Invoke(null, [self, tile, tile.Battle, pathfindingDescription]));
                    if (pathToTiles != null)
                    {
                        options.Add(new TileOption(tile, "Tile (" + tile.X + "," + tile.Y + ")", null, (AIUsefulness)int.MinValue, true));
                    }
                }
            }

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