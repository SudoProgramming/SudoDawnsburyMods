using Dawnsbury.Audio;
using Dawnsbury.Auxiliary;
using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
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
using Dawnsbury.Core.Mechanics.Damage;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Rules;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Mods.Feats.Classes.Gunslinger.Enums;
using Dawnsbury.Mods.Feats.Classes.Gunslinger.RegisteredComponents;
using Dawnsbury.Mods.Feats.Classes.Gunslinger.Ways;
using Dawnsbury.Mods.Items.Firearms.RegisteredComponents;
using Dawnsbury.Mods.Items.Firearms.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Dawnsbury.Core.Mechanics.Core.CalculatedNumber;

namespace Dawnsbury.Mods.Feats.Classes.Gunslinger
{
    /// <summary>
    /// The Gunslinger class
    /// </summary>
    public static class Gunslinger
    {
        /// <summary>
        /// The description text for what miss fire does
        /// </summary>
        private static string misfireDescriptionText = "{i}(A misfire will cause your firearm to jam requiring an interact action before use again, and any attack during the misfire will be a Critical Failure.){/i}";

        /// <summary>
        /// Creates the Gunslinger Feats
        /// </summary>
        /// <returns>The Enumerable of Gunslinger Feats</returns>
        public static IEnumerable<Feat> CreateGunslingerFeats()
        {
            // Creates each of the different Gunslinger Ways
            GunslingerWay wayOfTheDrifter = new GunslingerWay(GunslingerWayID.Drifter);
            GunslingerWay wayOfThePistolero = new GunslingerWay(GunslingerWayID.Pistolero);
            GunslingerWay wayOfTheSniper = new GunslingerWay(GunslingerWayID.Sniper);
            GunslingerWay wayOfTheVanguard = new GunslingerWay(GunslingerWayID.Vanguard);

            //// TODO
            ////Feat wayOfTheTriggerbrandFeat = new Feat(WayOfTheTriggerbrandFeatName, "You prefer firearms that work well as weapons in both melee and ranged combat, particularly those that allow you to exercise a bit of style and flair. You might be a survivor who cobbled your weapon together from the City of Smog's street scrap or a noble wielder of a master smith's bespoke commission for duels among Alkenstar's elite.",
            ////"You gain the {i}Slinger's Reload, Initial Deed, and Way Skill{/i} below:\n\n" +
            ////"{b}Slinger's Reload{/b} Touch and Go {icon:Action}\n{b}Requirements{/b} You're wielding a combination weapon.\n\nYou can Step toward an enemy, you can Interact to change your weapon between melee or ranged modes, and you then Interact to reload.\n\n" +
            ////"{b}Initial Deed{/b} Spring the Trap {icon:FreeAction}\n{b}Trigger{/b} You roll initiative\nYou choose which mode your combination weapon is set to. On your first turn, your movement and ranged attacks don't trigger reactions.\n\n" +
            ////"{b}Way Skill{/b} Thievery\nYou become trained in Thievery.", new List<Trait>(), null);

            // An additional character sheet selection for Stealth being rolled for initative
            yield return new Feat(GunslingerFeatNames.GunslingerSniperStealthInitiative, "You keep hidden or at a distance, staying out of the fray and bringing unseen death to your foes.", "You roll Stealth as initiative, you deal 1d6 percision damage with your first strike from a firearm or crossbow on your first turn.\n\nYou can begin hidden to creatures who rolled lower than you in initiative if you have standard cover or greater to them.", [], null);

            // An additional character sheet selection for Perception being rolled for initative
            yield return new Feat(GunslingerFeatNames.GunslingerSniperPerceptionInitiative, "You stay alert and ready for a fight.", "You will roll perception as initiative as normal, and will gain no other benefits from One Shot, One Kill.", [], null);

            // Creates and adds the logic for the Singular Expertise class feature
            Feat singularExpertiseFeat = new Feat(GunslingerFeatNames.SingularExpertise, "You have particular expertise with guns and crossbows that grants you greater proficiency with them and the ability to deal more damage.", "You gain a +1 circumstance bonus to damage rolls with firearms and crossbows.", [], null);
            AddSingularExpertiseLogic(singularExpertiseFeat);
            yield return singularExpertiseFeat;

            // Creates the class selection feat for the Gunslinger
            yield return new ClassSelectionFeat(GunslingerFeatNames.GunslingerClass, "While some fear projectile weapons, you savor the searing flash, wild kick, and cloying smoke that accompanies a gunshot, or snap of the cable and telltale thunk of your crossbow just before your bolt finds purchase. Ready to draw a bead on an enemy at every turn, you rely on your reflexes, steady hand, and knowledge of your weapons to riddle your foes with holes.",
                GunslingerTraits.Gunslinger, new EnforcedAbilityBoost(Ability.Dexterity), 8,
                [Trait.Will, Trait.Unarmed, Trait.Simple, Trait.Martial, FirearmTraits.AdvancedCrossbow, FirearmTraits.AdvancedFirearm, Trait.UnarmoredDefense, Trait.LightArmor, Trait.MediumArmor],
                [Trait.Perception, Trait.Fortitude, Trait.Reflex, FirearmTraits.SimpleCrossbow, FirearmTraits.MartialCrossbow, FirearmTraits.SimpleFirearm, FirearmTraits.MartialFirearm],
                3,
                "{b}1. Gunslinger's Way{/b} All gunslingers have a particular way they follow, a combination of philosophy and combat style that defines both how they fight and the weapons they excel with. At 1st level, your way grants you an initial deed, a unique reload action called a slinger's reload, and proficiency with a particular skill. You also gain advanced and greater deeds at later levels, as well as access to way-specific feats.\n\n" +
                "{b}2. Singular Expertise{/b} You have particular expertise with guns and crossbows that grants you greater proficiency with them and the ability to deal more damage. You gain a +1 circumstance bonus to damage rolls with firearms and crossbows.\r\n\r\nThis intense focus on firearms and crossbows prevents you from reaching the same heights with other weapons. Your proficiency with unarmed attacks and with weapons other than firearms and crossbows can't be higher than trained, even if you gain an ability that would increase your proficiency in one or more other weapons to match your highest weapon proficiency (such as the weapon expertise feats many ancestries have). If you have gunslinger weapon mastery, the limit is expert, and if you have gunslinging legend, the limit is master.\n\n" +
                "{b}3. Gunslinger Feat{/b}", new List<Feat>() { wayOfTheDrifter.Feat, wayOfThePistolero.Feat, wayOfTheSniper.Feat, wayOfTheVanguard.Feat })
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    // Adds the Singular Expertise base class feature, adds a Level 1 Gunslinger feat selection, and adds the Will Expert profeciency at level 3
                    sheet.AddFeat(singularExpertiseFeat, null);
                    sheet.AddSelectionOption(new SingleFeatSelectionOption("GunslingerFeat1", "Gunslinger feat", 1, (Feat ft) => ft.HasTrait(GunslingerTraits.Gunslinger)));
                    sheet.AddAtLevel(3, delegate (CalculatedCharacterSheetValues values)
                    {
                        values.SetProficiency(Trait.Will, Proficiency.Expert);
                    });
                });

            // Level 1 Class Feats
            TrueFeat coatedMunitionsFeat = new TrueFeat(GunslingerFeatNames.CoatedMunitions, 1, "You coat your munitions with mysterious alchemical mixed liquids you keep in small vials.", "{b}Requirements{/b} You're wielding a loaded firearm or crossbow.\n\nUntil the end of your turn, your next attack deals an addtional 1 persistent damage and 1 spalsh damage of your choice between acid, cold, electricity, fire or poison.", [GunslingerTraits.Gunslinger, Trait.Homebrew], null);
            AddCoatedMunitionsLogic(coatedMunitionsFeat);
            yield return coatedMunitionsFeat;

            // Creates and adds the logic for the Cover Fire class feat
            TrueFeat coverFireFeat = new TrueFeat(GunslingerFeatNames.CoverFire, 1, "You lay down suppressive fire to protect allies by forcing foes to take cover from your wild attacks.", "{b}Frequency{/b} once per round\n\n{b}Requirements{/b} You're wielding a loaded firearm or crossbow.\n\nMake a firearm or crossbow Strike; the target must decide before you roll your attack whether it will duck out of the way.\n\nIf the target ducks, it gains a +2 circumstance bonus to AC against your attack, or a +4 circumstance bonus to AC if it has cover. It also takes a –2 circumstance penalty to ranged attack rolls until the end of its next turn.\n\nIf the target chooses not to duck, you gain a +1 circumstance bonus to your attack roll for that Strike.", [GunslingerTraits.Gunslinger]).WithActionCost(1);
            AddCoverFireLogic(coverFireFeat);
            yield return coverFireFeat;

            // Creates and adds the logic for the Crossbow Crackshot class feat
            TrueFeat crossbowCrackShotFeat = new TrueFeat(GunslingerFeatNames.CrossbowCrackShot, 1, "You're exceptionally skilled with the crossbow.", "The first time each round that you Interact to reload a crossbow you are wielding, including Interact actions as part of your slinger's reload and similar effects, you increase the range increment for your next Strike with that weapon by 10 feet and deal 1 additional precision damage per weapon damage die with that Strike.\n\nIf your crossbow has the backstabber trait and you are attacking an off-guard target, backstabber deals 2 additional precision damage per weapon damage die instead of its normal effects.", [GunslingerTraits.Gunslinger]);
            AddCrossbowCrackShotLogic(crossbowCrackShotFeat);
            yield return crossbowCrackShotFeat;

            // Creates and adds the logic for the Hit the Dirt class feat
            TrueFeat hitTheDirtFeat = new TrueFeat(GunslingerFeatNames.HitTheDirt, 1, "You fling yourself out of harm's way.", "You Leap. Your movement gives you a +2 circumstance bonus to AC against the triggering attack. Regardless of whether or not the triggering attack hits, you land prone after completing your Leap.", [GunslingerTraits.Gunslinger]).WithActionCost(-2);
            AddHitTheDirtLogic(hitTheDirtFeat);
            yield return hitTheDirtFeat;

            // Creates and adds the logic for the Sword and Pistol class feat
            TrueFeat swordAndPistolFeat = new TrueFeat(GunslingerFeatNames.SwordAndPistol, 1, "You're comfortable wielding a firearm or crossbow in one hand and a melee weapon in the other, combining melee attacks with shots from the firearm.", "When you make a successful ranged Strike against an enemy within your reach with your one-handed firearm or one-handed crossbow, that enemy is flat-footed against your next melee attack with a one-handed melee weapon.\n\nWhen you make a successful melee Strike against an enemy with your one-handed melee weapon, the next ranged Strike you make against that enemy with a one-handed firearm or one-handed crossbow doesn't trigger reactions that would trigger on a ranged attack, such as Attack of Opportunity. Either of these benefits is lost if not used by the end of your next turn.", [GunslingerTraits.Gunslinger]);
            AddSwordAndPistolLogic(swordAndPistolFeat);
            yield return swordAndPistolFeat;

            // Level 2 Class Feats
            // Creates and adds the logic for the Defensive Armaments class feat
            TrueFeat defensiveAramentsFeat = new TrueFeat(GunslingerFeatNames.DefensiveArmaments, 2, "You use bulky firearms or crossbows to shield your body from your foes' attacks.", "Any two-handed firearms and two-handed crossbows you wield gain the parry trait. If an appropriate weapon already has the parry trait, increase the circumstance bonus to AC it grants when used to parry from +1 to +2.", [GunslingerTraits.Gunslinger]);
            AddDefensiveAramentsLogic(defensiveAramentsFeat);
            yield return defensiveAramentsFeat;

            // Creates and adds the logic for the Fake Out class feat
            TrueFeat fakeOutFeat = new TrueFeat(GunslingerFeatNames.FakeOut, 2, "With a skilled flourish of your weapon, you force an enemy to acknowledge you as a threat.", "{b}Trigger{/b} An ally is about to use an action that requires an attack roll, targeting a creature within your weapon's first range increment.\n\n{b}Requirements{/b} You're wielding a loaded firearm or crossbow.\n\nMake an attack roll to Aid the triggering attack. If you dealt damage to that enemy since the start of your last turn, you gain a +1 circumstance bonus to this roll.\n\n{i}Aid{/i}\n\n{b}Critical Success{/b} Your ally a +2 circumstance bonus\n{b}Success{/b} Your ally a +1 circumstance bonus\n{b}Critical Failure{/b} Your ally a -1 circumstance penalty\n", [GunslingerTraits.Gunslinger, Trait.Visual]).WithActionCost(-2);
            AddFakeOutLogic(fakeOutFeat);
            yield return fakeOutFeat;

            // Creates and adds the logic for the Pistol Twirl class feat
            TrueFeat pistolTwirlFeat = new TrueFeat(GunslingerFeatNames.PistolTwirl, 2, "Your quick gestures and flair for performance distract your opponent, leaving it vulnerable to your follow-up attacks.", "{b}Requirements{/b} You're wielding a loaded one-handed ranged weapon.\n\nYou Feint against an opponent within the required weapon's first range increment, rather than an opponent within melee reach. If you succeed, the foe is flat-footed against your melee and ranged attacks, rather than only your melee attacks. On a critical failure, you're flat-footed against the target's melee and ranged attacks, rather than only its melee attacks.", [GunslingerTraits.Gunslinger]).WithActionCost(1);
            pistolTwirlFeat.WithPrerequisite((CalculatedCharacterSheetValues sheet) => (sheet.Proficiencies.AllProficiencies.ContainsKey(Trait.Deception) && sheet.Proficiencies.AllProficiencies[Trait.Deception] >= Proficiency.Trained), "trained in Deception");
            AddPistolTwirlLogic(pistolTwirlFeat);
            yield return pistolTwirlFeat;

            // Creates and adds the logic for the Risky Reload class feat
            TrueFeat riskyReloadFeat = new TrueFeat(GunslingerFeatNames.RiskyReload, 2, "You've practiced a technique for rapidly reloading your firearm, but attempting to use this technique is a dangerous gamble with your firearm's functionality.", "{b}Requirements{/b} You're wielding a firearm.\n\nInteract to reload a firearm, then make a Strike with that firearm. If the Strike fails, the firearm misfires. " + misfireDescriptionText, [GunslingerTraits.Gunslinger, Trait.Flourish]).WithActionCost(1);
            AddRiskyReloadLogic(riskyReloadFeat);
            yield return riskyReloadFeat;

            // Creates and adds the logic for the Warning Shot class feat
            TrueFeat warningShotFeat = new TrueFeat(GunslingerFeatNames.WarningShot, 2, "Who needs words when the roar of a gun is so much more succinct?", "{b}Requirements{/b} You're wielding a loaded firearm.\n\nYou attempt to Demoralize a foe by firing your weapon into the air, using the firearm's maximum range rather than the usual range of 30 feet. This check doesn't take the –4 circumstance penalty if the target doesn't share a language with you.", [GunslingerTraits.Gunslinger]);
            warningShotFeat.WithActionCost(1).WithPrerequisite((CalculatedCharacterSheetValues sheet) => (sheet.Proficiencies.AllProficiencies.ContainsKey(Trait.Intimidation) && sheet.Proficiencies.AllProficiencies[Trait.Intimidation] >= Proficiency.Trained), "trained in Intimidation");
            AddWarningShotLogic(warningShotFeat);
            yield return warningShotFeat;

            // Creates and adds the logic for the Alchemical Shot class feat
            TrueFeat alchemicalShotFeat = new TrueFeat(GunslingerFeatNames.AlchemicalShot, 4, "You've practiced a technique for mixing alchemical bombs with your loaded shot.", "{b}Requirements{/b} You have an alchemical bomb worn or in one hand, and are wielding a firearm or crossbow.\n\nYou Interact to retrieve the bomb (if it's not already in your hand) and pour its contents onto your ammunition, consuming the bomb, then resume your grip on the required weapon. Next, Strike with your firearm. The Strike deals damage of the same type as the bomb (for instance, fire damage for alchemist's fire), and it deals an additional 1d6 persistent damage of the same type as the bomb. If the Strike is a failure, you take 1d6 damage of the same type as the bomb you used, and the firearm misfires. " + misfireDescriptionText, [GunslingerTraits.Gunslinger]);
            alchemicalShotFeat.WithActionCost(2);
            AddAlchemicalShotLogic(alchemicalShotFeat);
            yield return alchemicalShotFeat;

            // Creates and adds the logic for the Black Powder Boost class feat
            TrueFeat blackPowderBoostFeat = new TrueFeat(GunslingerFeatNames.BlackPowderBoost, 4, "You fire your weapon as you jump, using the kickback to go farther.", "{b}Requirements{/b} You're wielding a loaded firearm.\n\nYou Leap and discharge your firearm to add a +10-foot status bonus to the distance traveled. If you spend 2 actions for Black Powder Boost, you Long Jump instead.", [GunslingerTraits.Gunslinger]);
            AddBlackPowderBoostLogic(blackPowderBoostFeat);
            yield return blackPowderBoostFeat;

            // Creates and adds the logic for the Paired Shots class feat
            TrueFeat pairedShotsFeat = new TrueFeat(GunslingerFeatNames.PairedShots, 4, "Your shots hit simultaneously.", "{b}Requirements{/b} You're wielding two weapons, each of which can be either a loaded one-handed firearm or loaded one-handed crossbow.\n\nMake two Strikes, one with each of your two ranged weapons, each using your current multiple attack penalty. Both Strikes must have the same target.\n\nIf both attacks hit, combine their damage and then add any applicable effects from both weapons. Combine the damage from both Strikes and apply resistances and weaknesses only once. This counts as two attacks when calculating your multiple attack penalty.", [GunslingerTraits.Gunslinger]).WithActionCost(2);
            AddPairedShotsLogic(pairedShotsFeat);
            yield return pairedShotsFeat;

            // Creates and adds the logic for the Running Reload class feat
            TrueFeat runningReloadFeat = new TrueFeat(GunslingerFeatNames.RunningReload, 4, "You can reload your weapon on the move.", "You Stride, Step, or Sneak, then Interact to reload.", [GunslingerTraits.Gunslinger]).WithActionCost(1);
            AddRunningReloadLogic(runningReloadFeat);
            yield return runningReloadFeat;
        }

        /// <summary>
        /// Patches all feats for the Gunslinger
        /// </summary>
        /// <param name="feat">The feat to patch</param>
        public static void PatchFeat(Feat feat)
        {
            // Patches Quick Draw to be selectable by Gunslinger
            if (feat.FeatName == FeatName.QuickDraw)
            {
                PatchQuickDraw(feat);
            }
        }

        /// <summary>
        /// Adds the logic for the Singular Expertise base class feature
        /// </summary>
        /// <param name="singularExpertiseFeat">The Sinular Expertise feat object</param>
        private static void AddSingularExpertiseLogic(Feat singularExpertiseFeat)
        {
            // Adds a permanent Bonus to Damage effect if the criteria matches
            singularExpertiseFeat.WithPermanentQEffect("+1 Circumstance to Firearm/Crossbow damage", delegate (QEffect self)
            {
                self.BonusToDamage = (QEffect self, CombatAction action, Creature defender) =>
                {
                    if (action.HasTrait(FirearmTraits.Firearm) || action.HasTrait(Trait.Crossbow) || (action.Item != null && action.Item.WeaponProperties != null && FirearmUtilities.IsItemFirearmOrCrossbow(action.Item)))
                    {
                        return new Bonus(1, BonusType.Circumstance, "Singular Expertise");
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Cover Fire feat
        /// </summary>
        /// <param name="coverFireFeat">The Cover Fire true feat object</param>
        private static void AddCoverFireLogic(TrueFeat coverFireFeat)
        {
            // Adds a permanent Cover Fire action for items that match the criteria
            coverFireFeat.WithPermanentQEffect("+1 Circumstance to attack roll or target gets +2/+4 bonus to AC and range penalty", delegate (QEffect self)
            {
                self.ProvideStrikeModifier = (Item item) =>
                {
                    if (FirearmUtilities.IsItemFirearmOrCrossbow(item) && FirearmUtilities.IsItemLoaded(item) && !item.HasTrait(Trait.TwoHanded) && item.WeaponProperties != null)
                    {
                        // Creates a technical effect to track using this only once per round and creates a basic strike for the item
                        QEffect technicalEffectForOncePerRound = new QEffect("Technical Cover Fire", "[this condition has no description]")
                        {
                            ExpiresAt = ExpirationCondition.ExpiresAtStartOfYourTurn
                        };
                        CombatAction basicStrike = self.Owner.CreateStrike(item);

                        // Creatres the Cover Fire action for the item with the logic for each chosen target
                        CombatAction coverFireAction = new CombatAction(self.Owner, new SideBySideIllustration(item.Illustration, IllustrationName.TakeCover), "Cover Fire", [Trait.Basic, Trait.IsHostile, Trait.Attack], coverFireFeat.RulesText, basicStrike.Target);
                        coverFireAction.WithActionCost(1);
                        coverFireAction.Item = item;
                        coverFireAction.WithEffectOnChosenTargets(async delegate (Creature attacker, ChosenTargets targets)
                        {
                            // Adds the once per round restriction to the attacker
                            if (!attacker.QEffects.Any(qe => qe == technicalEffectForOncePerRound))
                            {
                                attacker.AddQEffect(technicalEffectForOncePerRound);
                            }

                            // Determines the target creature and the cover kind to that creature then creates the two possible effects for Cover Fire
                            Creature? target = targets.ChosenCreature;
                            if (target != null)
                            {
                                CoverKind cover = attacker.HasLineOfEffectTo(target.Occupies);

                                QEffect attackRollBonus = new QEffect(ExpirationCondition.ExpiresAtStartOfYourTurn)
                                {
                                    BonusToAttackRolls = (QEffect penalty, CombatAction action, Creature? defender) =>
                                    {
                                        return new Bonus(1, BonusType.Circumstance, "Cover Fire", true);
                                    }
                                };
                                QEffect acBonus = new QEffect(ExpirationCondition.ExpiresAtStartOfYourTurn)
                                {
                                    BonusToDefenses = (QEffect bonus, CombatAction? action, Defense defense) =>
                                    {
                                        return new Bonus(cover > 0 ? 4 : 2, BonusType.Circumstance, "Cover Fire", true);
                                    }
                                };

                                // Determines the logic for all non-human controlled creatures when Cover Fire targets them
                                bool shouldDodge = true;
                                if (!target.OwningFaction.IsHumanControlled)
                                {
                                    if (cover <= 0 && target.WieldsItem(Trait.Ranged))
                                    {
                                        shouldDodge = false;

                                    }
                                    else
                                    {
                                        shouldDodge = true;
                                    }

                                }

                                // Prompts the user for which effect they would like if the creature is human controlled
                                else
                                {
                                    shouldDodge = await target.Battle.AskForConfirmation(self.Owner, IllustrationName.QuestionMark, "Duck to gain +" + (cover > 0 ? "4" : "2") + " circumstance bonus to AC against the attack, along with a -2 circumstance penalty to ranged attack rolls until the end of your next turn?", "Duck");
                                }

                                // If Dodging is selected AC bonus and the ranged penalty is applied
                                if (shouldDodge)
                                {
                                    target.AddQEffect(acBonus);
                                    target.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtEndOfSourcesTurn)
                                    {
                                        BonusToAttackRolls = (QEffect penalty, CombatAction action, Creature? defender) =>
                                        {
                                            if (basicStrike.HasTrait(Trait.Ranged))
                                            {
                                                return new Bonus(-2, BonusType.Circumstance, "Cover Fire", false);
                                            }

                                            return null;
                                        }
                                    });
                                }

                                // Is dodging is not chosen the attack bonus is applied
                                else
                                {
                                    attacker.AddQEffect(attackRollBonus);
                                }

                                // Makes the strike and removes all needed effects
                                await attacker.MakeStrike(target, item);
                                attacker.RemoveAllQEffects(qe => qe == attackRollBonus);
                                target.RemoveAllQEffects(qe => qe == acBonus);
                            }
                        });

                        // Checks if the item needs to be reloaded
                        ((CreatureTarget)coverFireAction.Target).WithAdditionalConditionOnTargetCreature((Creature attacker, Creature defender) =>
                        {
                            if (!FirearmUtilities.IsItemLoaded(item))
                            {
                                return Usability.NotUsable("Needs to be reloaded.");
                            }
                            else if (attacker.QEffects.Any(qe => qe.Name == technicalEffectForOncePerRound.Name))
                            {
                                return Usability.NotUsable("Already used this round.");
                            }

                            return Usability.Usable;
                        });

                        coverFireAction.WithTargetingTooltip((action, defender, index) => action.Description);

                        return coverFireAction;
                    };

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Warning Shot feat
        /// </summary>
        /// <param name="pairedShotsFeat">The Warning Shot true feat object</param>
        private static void AddWarningShotLogic(TrueFeat warningShotFeat)
        {
            // Adds a permanent Warning Shot action for items that match the criteria
            warningShotFeat.WithPermanentQEffect("Ranged Demoralize with no language penalty", delegate (QEffect self)
            {
                self.ProvideStrikeModifier = (Item item) =>
                {
                    if (item.HasTrait(FirearmTraits.Firearm) && FirearmUtilities.IsItemLoaded(item) && item.WeaponProperties != null)
                    {
                        // Creates a demoarlize action that has the effect for Intimidating glare
                        CombatAction warningShotAction = CommonCombatActions.Demoralize(self.Owner);
                        warningShotAction.Name = "Warning Shot";
                        warningShotAction.Item = item;
                        warningShotAction.ActionCost = 1;
                        warningShotAction.ActionId = GunslingerActionIDs.WarningShot;
                        warningShotAction.Illustration = new SideBySideIllustration(item.Illustration, IllustrationName.Demoralize);
                        warningShotAction.Description = warningShotFeat.RulesText;
                        warningShotAction.Target = Target.Ranged(item.WeaponProperties.MaximumRange);
                        warningShotAction.StrikeModifiers.QEffectForStrike = new QEffect(ExpirationCondition.EphemeralAtEndOfImmediateAction)
                        {
                            Id = QEffectId.IntimidatingGlare,
                        };

                        return warningShotAction;
                    }

                    return null;
                };

                // Discharges the firearm
                self.YouBeginAction = async (QEffect dischargeEffect, CombatAction action) =>
                {
                    if (action.ActionId == GunslingerActionIDs.WarningShot && action.Item != null)
                    {
                        FirearmUtilities.DischargeItem(action.Item);
                    }
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Paired Shots feat
        /// </summary>
        /// <param name="pairedShotsFeat">The Paired Shots true feat object</param>
        private static void AddPairedShotsLogic(TrueFeat pairedShotsFeat)
        {
            // Adds a permanent Paired Shots action if both held items are Firearms or Crossbows
            pairedShotsFeat.WithPermanentQEffect("Stike with both weapons at same MAP", delegate (QEffect self)
            {
                self.ProvideMainAction = (QEffect pairedShotEffect) =>
                {
                    if (pairedShotEffect.Owner.HeldItems.Count(item => FirearmUtilities.IsItemFirearmOrCrossbow(item) && FirearmUtilities.IsItemLoaded(item) && !item.HasTrait(FirearmTraits.Misfired) && item.WeaponProperties != null) != 2)
                    {
                        return null;
                    }

                    // Sets up the action effect by grabbing both items, and determining the minimum max range between them
                    int currentMap = self.Owner.Actions.AttackedThisManyTimesThisTurn;
                    Item firstHeldItem = self.Owner.HeldItems[0];
                    Item secondHeldItem = self.Owner.HeldItems[1];
                    int maxRange = Math.Min(firstHeldItem.WeaponProperties.MaximumRange, secondHeldItem.WeaponProperties.MaximumRange);

                    // Returns the action which will make two strikes, one with each weapon
                    return new ActionPossibility(new CombatAction(pairedShotEffect.Owner, new SideBySideIllustration(firstHeldItem.Illustration, secondHeldItem.Illustration), "Paired Shots", [Trait.Basic, Trait.IsHostile], pairedShotsFeat.RulesText, Target.Ranged(maxRange)).WithActionCost(2).WithEffectOnChosenTargets(async delegate (Creature attacker, ChosenTargets targets)
                    {
                        if (targets.ChosenCreature != null)
                        {
                            // A crash happens if the sound effect of the second weapon is too long and is still playing, so a swap is needed 
                            SfxName? replacementSfx = null;
                            if (firstHeldItem.WeaponProperties != null && secondHeldItem.WeaponProperties != null && firstHeldItem.WeaponProperties.Sfx == secondHeldItem.WeaponProperties.Sfx)
                            {
                                if (secondHeldItem.HasTrait(Trait.Crossbow))
                                {
                                    replacementSfx = (firstHeldItem.WeaponProperties.Sfx == SfxName.Bow) ? SfxName.Fist : SfxName.Bow;
                                }
                                else if (secondHeldItem.HasTrait(FirearmTraits.Firearm))
                                {
                                    replacementSfx = (firstHeldItem.WeaponProperties.Sfx == FirearmSFXNames.SmallFirearm1) ? FirearmSFXNames.SmallFirearm2 : FirearmSFXNames.SmallFirearm1;
                                }

                                if (replacementSfx != null)
                                {
                                    secondHeldItem.WeaponProperties.Sfx = (SfxName)replacementSfx;
                                }
                            }

                            await pairedShotEffect.Owner.MakeStrike(targets.ChosenCreature, firstHeldItem, currentMap);
                            await pairedShotEffect.Owner.MakeStrike(targets.ChosenCreature, secondHeldItem, currentMap);

                            if (replacementSfx != null && firstHeldItem.WeaponProperties != null && secondHeldItem.WeaponProperties != null)
                            {
                                secondHeldItem.WeaponProperties.Sfx = firstHeldItem.WeaponProperties.Sfx;
                            }
                        }
                    }));
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Coated Munitions feat
        /// </summary>
        /// <param name="alchemicalShotFeat">The Coated Munitions true feat object</param>
        private static void AddCoatedMunitionsLogic(TrueFeat coatedMunitionsFeat)
        {
            // Adds a permanent Coated Munitions action if the appropiate weapon is held
            coatedMunitionsFeat.WithPermanentQEffect("Add 1 persistent and 1 splash damage of chosen type to next Strike", delegate (QEffect self)
            {
                self.ProvideActionIntoPossibilitySection = (QEffect coatedMunitionsEffect, PossibilitySection possibilitySection) =>
                {
                    if (possibilitySection.PossibilitySectionId == PossibilitySectionId.MainActions)
                    {
                        DamageKind[] elementalDamageKinds = [DamageKind.Acid, DamageKind.Cold, DamageKind.Electricity, DamageKind.Fire, DamageKind.Poison];
                        Dictionary<DamageKind, IllustrationName> illustraions = new Dictionary<DamageKind, IllustrationName>()
                        {
                            {DamageKind.Acid, IllustrationName.ResistAcid}, {DamageKind.Cold, IllustrationName.ResistCold}, {DamageKind.Electricity, IllustrationName.ResistElectricity}, {DamageKind.Fire, IllustrationName.ResistFire}, {DamageKind.Poison, IllustrationName.ResistEnergy}
                        };
                        PossibilitySection elementalDamageSection = new PossibilitySection("Elemental Damage");
                        foreach (DamageKind damageKind in elementalDamageKinds)
                        {
                            string damageString = damageKind.ToString();
                            ActionPossibility damageAction = new ActionPossibility(new CombatAction(coatedMunitionsEffect.Owner, illustraions[damageKind], damageString, [], "You deal an additional {Blue}1{/} persistent {Blue}" + damageString + "{/} damage and {Blue}1{/} {Blue}" + damageString + "{/} splash damage.", Target.Self()
                                .WithAdditionalRestriction((Creature user) =>
                                {
                                    if (user.QEffects.Any(qe => qe.Name == "Coated Munitions is Applied"))
                                    {
                                        return "Munitions are already coated.";
                                    }

                                    return null;
                                }))
                                .WithActionCost(1)
                                .WithEffectOnSelf(async (CombatAction damageEffect, Creature owner) =>
                                {
                                    owner.AddQEffect(new QEffect("Coated Munitions is Applied", "[This is a technical effect with no description]")
                                    {
                                        AddExtraKindedDamageOnStrike = (CombatAction action, Creature defender) =>
                                        {
                                            if (action.Item != null && FirearmUtilities.IsItemFirearmOrCrossbow(action.Item))
                                            {
                                                Map map = defender.Battle.Map;
                                                Tile? tile = map.AllTiles.FirstOrDefault(tile => tile.PrimaryOccupant == defender);
                                                foreach (Creature creature in tile.Neighbours.Creatures)
                                                {
                                                    creature.DealDirectDamage(null, DiceFormula.FromText("1"), creature, CheckResult.Success, damageKind);
                                                }

                                                return new KindedDamage(DiceFormula.FromText("1", "Coated Munitions (" + damageString + ")"), damageKind);
                                            }

                                            return null;
                                        },
                                        ExpiresAt = ExpirationCondition.ExpiresAtEndOfYourTurn
                                    });
                                }));
                            elementalDamageSection.AddPossibility(damageAction);
                        }

                        SubmenuPossibility coatedMunitionsMenu = new SubmenuPossibility(IllustrationName.Bomb, "Coated Munitions");
                        coatedMunitionsMenu.Subsections.Add(elementalDamageSection);
                        return coatedMunitionsMenu;
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Black Powder Boost feat
        /// </summary>
        /// <param name="blackPowderBoostFeat">The Black Powder Boost true feat object</param>
        private static void AddBlackPowderBoostLogic(TrueFeat blackPowderBoostFeat)
        {
            // Adds a permanent Black Powder Boost action if the appropiate weapon is held
            blackPowderBoostFeat.WithPermanentQEffect("+10 ft status bonus to Leap and Long Jump", delegate (QEffect self)
            {
                self.ProvideActionIntoPossibilitySection = (QEffect blackPowderBoostEffect, PossibilitySection possibilitySection) =>
                {
                    if (possibilitySection.PossibilitySectionId == PossibilitySectionId.MainActions)
                    {
                        Creature owner = blackPowderBoostEffect.Owner;
                        SubmenuPossibility blackPowderBoostMenu = new SubmenuPossibility(IllustrationName.Jump, "Black Powder Boost");

                        foreach (Item firearm in owner.HeldItems.Where(item => item.HasTrait(FirearmTraits.Firearm)))
                        {
                            // Creates a Black Powder Boost button and calculates the standard leap distance
                            PossibilitySection firearmBlackPowderBoostSection = new PossibilitySection(firearm.Name);
                            int leapDistance = (((owner.Speed >= 6) ? 3 : 2) + (owner.HasEffect(QEffectId.PowerfulLeap) ? 1 : 0) + 2);

                            // Adds the 1 action boost that acts as an extended leap
                            CombatAction blackPowderBoostOneAction = CommonCombatActions.Leap(owner, leapDistance);
                            blackPowderBoostOneAction.ActionId = GunslingerActionIDs.BlackPowderBoost;
                            blackPowderBoostOneAction.Item = firearm;
                            blackPowderBoostOneAction.Illustration = new SideBySideIllustration(firearm.Illustration, IllustrationName.Action);
                            blackPowderBoostOneAction.Name = "Boosted Leap";
                            blackPowderBoostOneAction.Description = "You Leap and discharge your firearm to add a +10-foot status bonus to the distance traveled.";
                            blackPowderBoostOneAction.WithActionCost(1);

                            // Checks if the item needs to be reloaded
                            ((TileTarget)blackPowderBoostOneAction.Target).AdditionalTargetingRequirement = ((Creature reloader, Tile tile) =>
                            {
                                if (!FirearmUtilities.IsItemLoaded(firearm))
                                {
                                    return Usability.NotUsable("Needs to be reloaded.");
                                }

                                return Usability.Usable;
                            });

                            firearmBlackPowderBoostSection.AddPossibility(new ActionPossibility(blackPowderBoostOneAction));

                            // Adds the 2 action boost that acts as an extended long jump
                            CombatAction blackPowderBoostTwoAction = new CombatAction(owner, new SideBySideIllustration(firearm.Illustration, IllustrationName.TwoActions), "Boosted Long Jump", [Trait.Basic, Trait.Move], "You Stride, then attempt a DC 15 Athletics check to make a long jump in the direction you were Striding.\n\nIf you didn't Stride at least 10 feet, you automatically fail your check.\n\n{b}Success{/b} You Leap up to a distance equal to your check result rounded down to the nearest 5 feet. You can't jump farther than your land Speed.\n{b}Failure{/b} You Leap.\n{b}Critical Failure{/b} You Leap, then fall and land prone.\n\nYou discharge your firearm to add a +10-foot status bonus to the distance traveled.", Target.Self());
                            blackPowderBoostTwoAction.WithActionCost(2);
                            blackPowderBoostTwoAction.ActionId = GunslingerActionIDs.BlackPowderBoost;
                            blackPowderBoostTwoAction.Item = firearm;
                            blackPowderBoostTwoAction.WithEffectOnSelf(async (Creature leaper) =>
                            {
                                // Collects the starting tile and handles the first stride
                                Tile startingTile = leaper.Occupies;
                                await leaper.StrideAsync("Choose a tile to Stride to. (1/2)");

                                // Gets the tile after striding and determines how far was moved
                                Tile currentTile = leaper.Occupies;
                                int distanceMoved = startingTile.DistanceTo(currentTile);
                                bool autoFailure = distanceMoved < 2 && (distanceMoved == 0 || !currentTile.DifficultTerrain);
                                CheckResult result;
                                int totalResult = -1;

                                // If the long jump isn't an auto failure an Ath
                                if (!autoFailure)
                                {
                                    int diceResult = R.NextD20();
                                    int athleticsMod = leaper.Skills.Get(Skill.Athletics);
                                    totalResult = diceResult + athleticsMod;
                                    result = (totalResult >= 15) ? CheckResult.Success : (totalResult <= 5) ? CheckResult.CriticalFailure : CheckResult.Failure;
                                    result = (diceResult == 1 && result != CheckResult.CriticalFailure) ? result - 1 : result;
                                    leaper.Battle.Log(leaper.ToString() + " " + ((result >= CheckResult.Success) ? "{Green}succeeds{/}" : "{Red}fails{/}") + " a long jump:");
                                    leaper.Battle.Log(athleticsMod.ToString() + "+" + diceResult + "=" + totalResult + " vs. 15");
                                }

                                // If an auto failure happened no need to do the check
                                else
                                {
                                    result = CheckResult.Failure;
                                    leaper.Battle.Log(leaper.ToString() + " {Red}fails{/} to long jump, since they did not stride at least 10 ft.");
                                }

                                // Sets the Failure distances to the leap distance but also updates the successful long jumps
                                int longJumpDistance = leapDistance;
                                if (result >= CheckResult.Success)
                                {
                                    int leapBasedOnSpeed = ((owner.Speed >= 6) ? 3 : 2);
                                    int longDistance = totalResult % 5;
                                    int longDistanceGained = (longDistance < leapBasedOnSpeed) ? leapBasedOnSpeed : (longDistance > owner.Speed) ? owner.Speed : longDistance;
                                    longJumpDistance = (longDistanceGained + (owner.HasEffect(QEffectId.PowerfulLeap) ? 1 : 0) + 2);
                                }

                                // Handles the Long Jump and lands prone on a critical failure
                                Tile? tileToLeapTo = await GetLongJumpTileWithinDistance(leaper, startingTile, "Choose the tile to leap to. (2/2)", longJumpDistance);
                                if (tileToLeapTo != null)
                                {
                                    await leaper.SingleTileMove(tileToLeapTo, null);
                                }
                                if (result == CheckResult.CriticalFailure)
                                {
                                    await leaper.FallProne();
                                }
                            });

                            // Checks if the item needs to be reloaded
                            ((SelfTarget)blackPowderBoostTwoAction.Target).WithAdditionalRestriction((Creature reloader) =>
                            {
                                if (!FirearmUtilities.IsItemLoaded(firearm))
                                {
                                    return "Needs to be reloaded.";
                                }

                                return null;
                            });

                            // Adds all the posibilites for each weapon and finalizes the button
                            firearmBlackPowderBoostSection.AddPossibility(new ActionPossibility(blackPowderBoostTwoAction));

                            blackPowderBoostMenu.Subsections.Add(firearmBlackPowderBoostSection);
                        }

                        return blackPowderBoostMenu;
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
        /// Adds the logic for the Alchemical Shot feat
        /// </summary>
        /// <param name="alchemicalShotFeat">The Alchemical Shot true feat object</param>
        private static void AddAlchemicalShotLogic(TrueFeat alchemicalShotFeat)
        {
            // Adds to the creature a state check to add the Alchemical Shot action to appropiate held weapons with each alchemical bomb
            alchemicalShotFeat.WithOnCreature(creature =>
            {
                creature.AddQEffect(new QEffect("Alchemical Shot {icon:TwoActions}", "Changes damage to match selected bomb and deals an addition 1d6 persistent damage")
                {
                    StateCheck = (QEffect permanentState) =>
                    {
                        // Collects the unique bombs carried or held
                        List<Item> heldBombs = permanentState.Owner.HeldItems.Concat(permanentState.Owner.CarriedItems).Where(item => item.HasTrait(Trait.Alchemical) && item.HasTrait(Trait.Bomb)).ToList();
                        HashSet<string> uniqueBombNames = new HashSet<string>(heldBombs.Select(bomb => bomb.Name).ToList());
                        List<Item> uniqueBombsHeld = new List<Item>();
                        foreach (string bombName in uniqueBombNames)
                        {
                            Item? matchingBomb = heldBombs.FirstOrDefault(bomb => bomb.Name == bombName);
                            if (matchingBomb != null)
                            {
                                uniqueBombsHeld.Add(matchingBomb);
                            }
                        }

                        permanentState.ProvideActionIntoPossibilitySection = (QEffect alchemicalShotEffect, PossibilitySection possibilitySection) =>
                        {
                            if (possibilitySection.PossibilitySectionId == PossibilitySectionId.MainActions)
                            {
                                Creature owner = alchemicalShotEffect.Owner;
                                SubmenuPossibility alchemicalShotMenu = new SubmenuPossibility(IllustrationName.Bomb, "Alchemical Shot");

                                foreach (Item item in owner.HeldItems.Where(item => FirearmUtilities.IsItemFirearmOrCrossbow(item) && FirearmUtilities.IsItemLoaded(item) && item.WeaponProperties != null))
                                {
                                    // Creates a Alchemical Shot button
                                    PossibilitySection alchemicalShotSection = new PossibilitySection(item.Name);

                                    // For each bomb the bomb will be added as a strike modifier for weapon
                                    foreach (Item bomb in uniqueBombsHeld)
                                    {
                                        // Adjusts the damage type and creates a tempory item that will be used instead of the normal weapon.
                                        DamageKind alchemicalDamageType = (bomb != null && bomb.WeaponProperties != null) ? bomb.WeaponProperties.DamageKind : item.WeaponProperties.DamageKind;
                                        Item alchemicalBombLoadedWeapon = new Item(item.Illustration, item.Name, item.Traits.ToArray())
                                        {
                                            WeaponProperties = new WeaponProperties(item.WeaponProperties.Damage, alchemicalDamageType) { Sfx = item.WeaponProperties.Sfx }.WithRangeIncrement(item.WeaponProperties.RangeIncrement)
                                        };

                                        string alchemicalDamageString = alchemicalDamageType.ToString();
                                        CombatAction alchemicalShotAction = new CombatAction(permanentState.Owner, new SideBySideIllustration(item.Illustration, bomb.Illustration), "Alchemical Shot (" + bomb.Name + ")", [Trait.Basic, Trait.Strike],
                                            "Make a Strike that deals {Blue}" + alchemicalDamageString + "{/} instead of its normal damage type, and deals an additional {Blue}1d6{/} persistent {Blue}" + alchemicalDamageString + "{/} damage.", Target.Ranged(item.WeaponProperties.MaximumRange));
                                        alchemicalShotAction.Item = item;
                                        alchemicalShotAction.ActionCost = 2;

                                        // The shot will be fired and remove the selected bomb
                                        alchemicalShotAction.WithEffectOnEachTarget(async delegate (CombatAction pistolTwirl, Creature attacker, Creature defender, CheckResult result)
                                        {
                                            if (defender != null)
                                            {
                                                result = await permanentState.Owner.MakeStrike(defender, alchemicalBombLoadedWeapon);
                                                pistolTwirl.CheckResult = result;
                                                FirearmUtilities.DischargeItem(item);
                                                for (int i = 0; i < permanentState.Owner.HeldItems.Count; i++)
                                                {
                                                    if (permanentState.Owner.HeldItems.Contains(bomb))
                                                    {
                                                        permanentState.Owner.HeldItems.Remove(bomb);
                                                        break;
                                                    }
                                                    else if (permanentState.Owner.CarriedItems.Contains(bomb))
                                                    {
                                                        permanentState.Owner.CarriedItems.Remove(bomb);
                                                    }
                                                }
                                                if (result >= CheckResult.Success)
                                                {
                                                    defender.AddQEffect(QEffect.PersistentDamage("1d6", alchemicalDamageType));
                                                }
                                                else if (result == CheckResult.CriticalFailure)
                                                {
                                                    attacker.AddQEffect(QEffect.PersistentDamage("1d6", alchemicalDamageType));
                                                    item.Traits.Add(FirearmTraits.Misfired);
                                                }
                                            }

                                        });

                                        // Checks if the item needs to be reloaded
                                        ((CreatureTarget)alchemicalShotAction.Target).WithAdditionalConditionOnTargetCreature((Creature attacker, Creature defender) =>
                                        {
                                            if (!FirearmUtilities.IsItemLoaded(item))
                                            {
                                                return Usability.NotUsable("Needs to be reloaded.");
                                            }
                                            else if (heldBombs.Count == 0)
                                            {
                                                return Usability.NotUsable("You have no more alchemical bombs.");
                                            }

                                            return Usability.Usable;
                                        });

                                        alchemicalShotSection.AddPossibility(new ActionPossibility(alchemicalShotAction));
                                    }

                                    alchemicalShotMenu.Subsections.Add(alchemicalShotSection);
                                }

                                return alchemicalShotMenu;
                            }

                            return null;
                        };
                    }
                });
            });
        }

        /// <summary>
        /// Adds the logic for the Hit the Dirt feat
        /// </summary>
        /// <param name="hitTheDirtFeat">The Hit the Dirt true feat object</param>
        private static void AddHitTheDirtLogic(TrueFeat hitTheDirtFeat)
        {
            // Adds a permanent Hit the Dirt reaction
            hitTheDirtFeat.WithPermanentQEffect("+2 Circumstance to AC, then Leap and fall prone", delegate (QEffect self)
            {
                self.YouAreTargeted = async (QEffect hitTheDirtEffect, CombatAction action) =>
                {
                    if (hitTheDirtEffect.Owner.HasLineOfEffectTo(action.Owner.Occupies) < CoverKind.Blocked && action.Owner.VisibleToHumanPlayer && action.HasTrait(Trait.Ranged) && await hitTheDirtEffect.Owner.Battle.AskToUseReaction(hitTheDirtEffect.Owner, "Use reaction to gain +2 circumstance bonus to AC for this attack then leap and fall prone?"))
                    {
                        hitTheDirtEffect.Owner.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtEndOfAnyTurn)
                        {
                            Id = GunslingerQEIDs.HitTheDirt,
                            BonusToDefenses = (QEffect q, CombatAction? action, Defense defense) =>
                            {
                                if (action?.HasTrait(Trait.Ranged) ?? false)
                                {
                                    return new Bonus(2, BonusType.Circumstance, "Hit the Dirt", true);
                                }

                                return null;
                            }
                        });
                    }
                };

                // Prompts the user to leap and sets them to prone
                self.AfterYouAreTargeted = async (QEffect cleanupEffects, CombatAction action) =>
                {
                    if (cleanupEffects.Owner.HasEffect(GunslingerQEIDs.HitTheDirt))
                    {
                        cleanupEffects.Owner.RemoveAllQEffects(qe => qe.Id == GunslingerQEIDs.HitTheDirt);
                        int leapDistance = ((cleanupEffects.Owner.Speed >= 6) ? 3 : 2) + (cleanupEffects.Owner.HasEffect(QEffectId.PowerfulLeap) ? 1 : 0);
                        CombatAction leapAction = CommonCombatActions.Leap(cleanupEffects.Owner);
                        leapAction.EffectOnChosenTargets = null;
                        Tile? tileToLeapTo = await GetLeapTileWithinDistance(cleanupEffects.Owner, "Choose the tile to leap to.", leapDistance);
                        if (tileToLeapTo != null)
                        {
                            await cleanupEffects.Owner.SingleTileMove(tileToLeapTo, leapAction);
                        }

                        await cleanupEffects.Owner.FallProne();
                    }
                };
            });
        }

        // <summary>
        /// Adds the logic for the Running Reload feat
        /// </summary>
        /// <param name="runningReloadFeat">The Running Reload true feat object</param>
        private static void AddRunningReloadLogic(TrueFeat runningReloadFeat)
        {
            // Adds a permanent Running Reload action if the appropiate weapon is held
            runningReloadFeat.WithPermanentQEffect("Stride and reload", delegate (QEffect self)
            {
                self.ProvideActionIntoPossibilitySection = (QEffect runningReloadEffect, PossibilitySection possibilitySection) =>
                {
                    if (possibilitySection.PossibilitySectionId == PossibilitySectionId.MainActions)
                    {
                        SubmenuPossibility runningReloadMenu = new SubmenuPossibility(IllustrationName.WarpStep, "Running Reload");

                        foreach (Item heldItem in runningReloadEffect.Owner.HeldItems)
                        {
                            if (FirearmUtilities.IsItemFirearmOrCrossbow(heldItem) && heldItem.WeaponProperties != null)
                            {
                                PossibilitySection runningReloadSection = new PossibilitySection(heldItem.Name);
                                CombatAction itemAction = new CombatAction(runningReloadEffect.Owner, new SideBySideIllustration(heldItem.Illustration, IllustrationName.WarpStep), "Running Reload", [Trait.Basic], runningReloadFeat.RulesText, Target.Self()
                                .WithAdditionalRestriction((Creature user) =>
                                {
                                    if (FirearmUtilities.IsItemLoaded(heldItem) && !FirearmUtilities.IsMultiAmmoWeaponReloadable(heldItem))
                                    {
                                        return "Can not be reloaded.";
                                    }

                                    return null;
                                })).WithActionCost(1).WithItem(heldItem).WithEffectOnSelf(async (action, self) =>
                                {
                                    if (!await self.StrideAsync("Choose where to Stride with Running Reload.", allowCancel: true))
                                    {
                                        action.RevertRequested = true;
                                    }
                                    else
                                    {
                                        FirearmUtilities.AwaitReloadItem(self, heldItem);
                                    }
                                });
                                ActionPossibility itemPossibility = new ActionPossibility(itemAction);

                                runningReloadSection.AddPossibility(itemPossibility);
                                runningReloadMenu.Subsections.Add(runningReloadSection);
                            }
                        }

                        return runningReloadMenu;
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Sword and Pistol feat
        /// </summary>
        /// <param name="swordAndPistolFeat">The Sword and Pistol true feat object</param>
        private static void AddSwordAndPistolLogic(TrueFeat swordAndPistolFeat)
        {
            // Adds a permanent Effect that will adjust depending on if you attacked in melee or ranged with appropiate weapons
            swordAndPistolFeat.WithPermanentQEffect("After melee Strikes and Ranged Strikes alter the opposite", delegate (QEffect self)
            {
                self.BeforeYourActiveRoll = async (QEffect addingEffects, CombatAction action, Creature defender) =>
                {
                    // If you attack within your melee range with a ranged Firearm or Crossbow, you gain a Melee buff 
                    if (action.HasTrait(Trait.Ranged) && !action.HasTrait(Trait.TwoHanded) && (action.HasTrait(FirearmTraits.Firearm) || action.HasTrait(Trait.Crossbow)) && addingEffects.Owner.DistanceTo(defender) == 1 && !addingEffects.Owner.QEffects.Any(qe => qe.Id == GunslingerQEIDs.SwordAndPistolMeleeBuff && qe.Tag != null && qe.Tag == defender))
                    {
                        addingEffects.Owner.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtEndOfYourTurn)
                        {
                            Id = GunslingerQEIDs.SwordAndPistolMeleeBuff,
                            CannotExpireThisTurn = true,
                            Tag = defender,
                            BeforeYourActiveRoll = async (QEffect rollEffect, CombatAction action, Creature attackedCreature) =>
                            {
                                if (action.HasTrait(Trait.Strike) && action.HasTrait(Trait.Melee) && !action.HasTrait(Trait.TwoHanded) && defender == attackedCreature)
                                {
                                    QEffect flatFooted = QEffect.FlatFooted("Sword and Pistol");
                                    flatFooted.ExpiresAt = ExpirationCondition.EphemeralAtEndOfImmediateAction;
                                    attackedCreature.AddQEffect(flatFooted);
                                    rollEffect.Owner.RemoveAllQEffects(qe => qe.Id == GunslingerQEIDs.SwordAndPistolMeleeBuff && qe.Tag != null && qe.Tag == defender);
                                }
                            }
                        });
                    }

                    // If you attack with melee you gain a Ranged buff 
                    else if (action.HasTrait(Trait.Melee) && !action.HasTrait(Trait.TwoHanded) && !addingEffects.Owner.QEffects.Any(qe => qe.Id == GunslingerQEIDs.SwordAndPistolRangedBuff && qe.Tag != null && qe.Tag == defender))
                    {
                        addingEffects.Owner.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtEndOfYourTurn)
                        {
                            // Adds an effect that will prevent reactions to this effect
                            Id = GunslingerQEIDs.SwordAndPistolRangedBuff,
                            CannotExpireThisTurn = true,
                            Tag = defender,
                            StateCheck = (QEffect q) =>
                            {
                                if (addingEffects.Owner.HasEffect(GunslingerQEIDs.SwordAndPistolRangedBuff))
                                {
                                    foreach (Item item in addingEffects.Owner.HeldItems.Concat(addingEffects.Owner.CarriedItems))
                                    {
                                        if (!item.HasTrait(Trait.DoesNotProvoke) && item.HasTrait(Trait.Ranged) && !item.HasTrait(Trait.TwoHanded) && (item.HasTrait(FirearmTraits.Firearm) || item.HasTrait(Trait.Crossbow)))
                                        {
                                            item.Traits.Add(GunslingerTraits.TemporaryDoesNotProvoke);
                                            item.Traits.Add(Trait.DoesNotProvoke);
                                        }
                                    }
                                }
                            },

                            // Checks if the target the same as the effect, if they are not the same a reaction should be prompted
                            YouBeginAction = async (QEffect startAction, CombatAction action) =>
                            {
                                if (action.ChosenTargets.ChosenCreature != null && action.ChosenTargets.ChosenCreature != defender)
                                {
                                    await startAction.Owner.ProvokeOpportunityAttacks(action);
                                }
                            },

                            // After a valid attack is done the effects should be removed
                            BeforeYourActiveRoll = async (QEffect rollEffect, CombatAction action, Creature attackedCreature) =>
                            {
                                if (action.HasTrait(Trait.Strike) && action.HasTrait(Trait.Ranged) && !action.HasTrait(Trait.TwoHanded) && (action.HasTrait(FirearmTraits.Firearm) || action.HasTrait(Trait.Crossbow)) && defender == attackedCreature)
                                {
                                    foreach (Item item in addingEffects.Owner.HeldItems.Concat(addingEffects.Owner.CarriedItems))
                                    {
                                        if (item.HasTrait(GunslingerTraits.TemporaryDoesNotProvoke))
                                        {
                                            item.Traits.Remove(Trait.DoesNotProvoke);
                                            item.Traits.Remove(GunslingerTraits.TemporaryDoesNotProvoke);
                                        }
                                    }
                                    rollEffect.Owner.RemoveAllQEffects(qe => qe.Id == GunslingerQEIDs.SwordAndPistolRangedBuff && qe.Tag != null && qe.Tag == defender);
                                }
                            }
                        }); ;
                    }
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Pistol Twirl feat
        /// </summary>
        /// <param name="pistolTwirlFeat">The Pistol Twirl true feat object</param>
        private static void AddPistolTwirlLogic(TrueFeat pistolTwirlFeat)
        {
            // Adds a permananet Pistol Twirl action for the appropiate weapons
            pistolTwirlFeat.WithPermanentQEffect("Ranged Feint", delegate (QEffect self)
            {
                self.ProvideStrikeModifier = (Item item) =>
                {
                    if (FirearmUtilities.IsItemFirearmOrCrossbow(item) && FirearmUtilities.IsItemLoaded(item) && !item.HasTrait(Trait.TwoHanded) && item.WeaponProperties != null)
                    {
                        // Creates the action and handles the success results of the actions
                        CombatAction pistolTwirlAction = new CombatAction(self.Owner, new SideBySideIllustration(item.Illustration, IllustrationName.Feint), "Pistol Twirl", [], pistolTwirlFeat.RulesText, Target.Ranged(item.WeaponProperties.RangeIncrement)).WithActionCost(1).WithItem(item)
                        .WithActiveRollSpecification(new ActiveRollSpecification(Checks.SkillCheck(Skill.Deception), Checks.DefenseDC(Defense.Perception)))
                        .WithEffectOnEachTarget(async delegate (CombatAction pistolTwirl, Creature attacker, Creature defender, CheckResult result)
                        {
                            switch (result)
                            {
                                case CheckResult.CriticalSuccess:
                                    defender.AddQEffect(new QEffect(ExpirationCondition.CountsDownAtEndOfYourTurn)
                                    {
                                        Value = 2,
                                        YouAreTargeted = async (QEffect targeted, CombatAction action) =>
                                        {
                                            if (action.Owner != null && action.Owner == attacker && (action.HasTrait(Trait.Melee) || action.HasTrait(Trait.Ranged)))
                                            {
                                                QEffect flatFooted = QEffect.FlatFooted("Pistol Twirl");
                                                flatFooted.ExpiresAt = ExpirationCondition.Immediately;
                                                defender.AddQEffect(flatFooted);
                                            }
                                        }
                                    });
                                    break;
                                case CheckResult.Success:
                                    defender.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtEndOfAnyTurn)
                                    {
                                        YouAreTargeted = async (QEffect targeted, CombatAction action) =>
                                        {
                                            if (action.Owner != null && action.Owner == attacker && (action.HasTrait(Trait.Melee) || action.HasTrait(Trait.Ranged)))
                                            {
                                                QEffect flatFooted = QEffect.FlatFooted("Pistol Twirl");
                                                flatFooted.ExpiresAt = ExpirationCondition.Immediately;
                                                defender.AddQEffect(flatFooted);
                                            }
                                        }
                                    });
                                    break;
                                case CheckResult.CriticalFailure:
                                    attacker.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtEndOfYourTurn)
                                    {
                                        CannotExpireThisTurn = true,
                                        YouAreTargeted = async (QEffect targeted, CombatAction action) =>
                                        {
                                            if (action.Owner != null && action.Owner == defender && (action.HasTrait(Trait.Melee) || action.HasTrait(Trait.Ranged)))
                                            {
                                                QEffect flatFooted = QEffect.FlatFooted("Pistol Twirl");
                                                flatFooted.ExpiresAt = ExpirationCondition.Immediately;
                                                attacker.AddQEffect(flatFooted);
                                            }
                                        }
                                    });
                                    break;
                            }
                        });
                        return pistolTwirlAction;
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Risky Reload feat
        /// </summary>
        /// <param name="riskyReloadFeat">The Risky Reload true feat object</param>
        private static void AddRiskyReloadLogic(TrueFeat riskyReloadFeat)
        {
            // Adds a permanent Running Reload action if the appropiate weapon is held
            riskyReloadFeat.WithPermanentQEffect("Reload and Strike", delegate (QEffect self)
            {
                self.ProvideActionIntoPossibilitySection = (QEffect riskyReloadEffect, PossibilitySection possibilitySection) =>
                {
                    if (possibilitySection.PossibilitySectionId == PossibilitySectionId.MainActions)
                    {
                        SubmenuPossibility riskyReloadMenu = new SubmenuPossibility(IllustrationName.GenericCombatManeuver, "Risky Reload");

                        foreach (Item heldItem in riskyReloadEffect.Owner.HeldItems)
                        {
                            if (FirearmUtilities.IsItemFirearmOrCrossbow(heldItem) && heldItem.WeaponProperties != null)
                            {
                                PossibilitySection riskyReloadSection = new PossibilitySection(heldItem.Name);
                                // Creates the strike and reloads and misfires the weapon if the attack misses
                                CombatAction basicStrike = riskyReloadEffect.Owner.CreateStrike(heldItem);
                                CombatAction riskyReloadAction = new CombatAction(riskyReloadEffect.Owner, new SideBySideIllustration(heldItem.Illustration, IllustrationName.TrueStrike), "Risky Reload", [Trait.Flourish, Trait.Basic], riskyReloadFeat.RulesText, basicStrike.Target).WithActionCost(1).WithItem(heldItem);
                                riskyReloadAction.WithEffectOnEachTarget(async delegate (CombatAction riskyReload, Creature attacker, Creature defender, CheckResult result)
                                {
                                    if (heldItem.HasTrait(FirearmTraits.DoubleBarrel))
                                    {
                                        heldItem.EphemeralItemProperties.AmmunitionLeftInMagazine++;
                                        heldItem.EphemeralItemProperties.NeedsReload = false;

                                    }
                                    else
                                    {
                                        await attacker.CreateReload(heldItem).WithActionCost(0).WithItem(heldItem).AllExecute();
                                    }

                                    if (!heldItem.EphemeralItemProperties.NeedsReload)
                                    {
                                        CheckResult strikeResult = await riskyReload.Owner.MakeStrike(defender, heldItem);
                                        if (strikeResult <= CheckResult.Failure && !heldItem.HasTrait(FirearmTraits.Misfired))
                                        {
                                            heldItem.Traits.Add(FirearmTraits.Misfired);
                                        }
                                    }
                                    else
                                    {
                                        riskyReloadEffect.Owner.Battle.Log("A strike with " + heldItem.Name + " could not be made.");
                                    }
                                });

                                // Checks if the item needs to be reloaded
                                ((CreatureTarget)riskyReloadAction.Target).WithAdditionalConditionOnTargetCreature((Creature attacker, Creature defender) =>
                                {
                                    if (FirearmUtilities.IsItemLoaded(heldItem) && !FirearmUtilities.IsMultiAmmoWeaponReloadable(heldItem))
                                    {
                                        return Usability.NotUsable("Can not be reloaded.");
                                    }


                                    return Usability.Usable;
                                });

                                riskyReloadAction.WithTargetingTooltip((action, defender, index) => action.Description);

                                ActionPossibility riskyReloadPossibility = new ActionPossibility(riskyReloadAction);

                                riskyReloadSection.AddPossibility(riskyReloadPossibility);
                                riskyReloadMenu.Subsections.Add(riskyReloadSection);
                            }
                        }

                        return riskyReloadMenu;
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Crossbow Crack Shot feat
        /// </summary>
        /// <param name="crossbowCrackShotFeat">The Crossbow Crack Shot true feat object</param>
        private static void AddCrossbowCrackShotLogic(TrueFeat crossbowCrackShotFeat)
        {
            // Adds a Permanent effect for strikes that are crossbows
            crossbowCrackShotFeat.WithPermanentQEffect("Reloading increase range by +10 and adds +1/+2 precision damage", delegate (QEffect self)
            {
                self.AfterYouTakeAction = async (QEffect crossbowCrackshotEffect, CombatAction action) =>
                {
                    if (GetReloadAIDs().Contains(action.ActionId) && !crossbowCrackshotEffect.Owner.HasEffect(GunslingerQEIDs.CrossbowCrackShot))
                    {
                        if (action.Item == null)
                        {
                            action.Item = crossbowCrackshotEffect.Owner.HeldItems.FirstOrDefault(item => item.HasTrait(Trait.Crossbow) && FirearmUtilities.IsItemLoaded(item));
                        }
                        if (action.Item != null && action.Item.HasTrait(Trait.Crossbow) && action.Item.WeaponProperties != null) // Base Reload has null action.Item
                        {
                            Item crossbow = action.Item;
                            crossbow.WeaponProperties.WithRangeIncrement(crossbow.WeaponProperties.RangeIncrement + 2);
                            crossbowCrackshotEffect.Owner.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtStartOfYourTurn)
                            {
                                Id = GunslingerQEIDs.CrossbowCrackShot,
                                Tag = crossbow,
                                BonusToDamage = (QEffect bonusToDamage, CombatAction action, Creature defender) =>
                                {
                                    if (action.Item != null && action.Item == crossbow)
                                    {
                                        Creature attacker = bonusToDamage.Owner;
                                        QEffect? cbcsEffect = bonusToDamage.Owner.QEffects.FirstOrDefault(qe => qe.Id == GunslingerQEIDs.CrossbowCrackShot);
                                        if (cbcsEffect != null)
                                        {
                                            cbcsEffect.ExpiresAt = ExpirationCondition.Immediately;
                                        }

                                        int backstabberDamage = (crossbow.HasTrait(Trait.Backstabber) && defender.IsFlatFootedTo(attacker, action)) ? 2 : 0;
                                        crossbow.WeaponProperties.WithRangeIncrement(crossbow.WeaponProperties.RangeIncrement - 2);
                                        return new Bonus(crossbow.WeaponProperties.DamageDieCount + backstabberDamage, BonusType.Untyped, "Crossbow Crack Shot" + ((backstabberDamage > 0) ? " (Backstabber)" : string.Empty) + " precision damage", true);
                                    }

                                    return null;
                                },
                            });
                        }
                    }
                };

                // Cleans up the effect if effect was just used
                self.StateCheck = (QEffect state) =>
                {
                    QEffect? cbcsEffect = state.Owner.QEffects.FirstOrDefault(qe => qe.Id == GunslingerQEIDs.CrossbowCrackShot);
                    if (cbcsEffect != null && cbcsEffect.ExpiresAt == ExpirationCondition.Immediately && cbcsEffect.Tag != null && cbcsEffect.Tag is Item crossbow && crossbow.WeaponProperties != null)
                    {

                        state.Owner.RemoveAllQEffects(qe => qe.Id == GunslingerQEIDs.CrossbowCrackShot);
                    }
                };

                // If the effect is not used the adjustments still need to be cleaned up
                self.EndOfAnyTurn = (QEffect endOfTurn) =>
                {
                    if (endOfTurn.Owner.HasEffect(GunslingerQEIDs.CrossbowCrackShot))
                    {
                        QEffect? cbcsEffect = endOfTurn.Owner.QEffects.FirstOrDefault(qe => qe.Id == GunslingerQEIDs.CrossbowCrackShot);
                        if (cbcsEffect != null && cbcsEffect.Tag != null && cbcsEffect.Tag is Item crossbow && crossbow.WeaponProperties != null)
                        {
                            crossbow.WeaponProperties.WithRangeIncrement(crossbow.WeaponProperties.RangeIncrement - 2);
                        }
                        if (cbcsEffect != null)
                        {
                            endOfTurn.Owner.RemoveAllQEffects(qe => qe.Id == GunslingerQEIDs.CrossbowCrackShot);
                        }
                    }
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Defensive Araments feat
        /// </summary>
        /// <param name="defensiveAramentsFeat">The Defensive Araments true feat object</param>
        private static void AddDefensiveAramentsLogic(TrueFeat defensiveAramentsFeat)
        {
            // Adds a permananet effect that adds the Parry trait to items that don't have it when appropiate
            defensiveAramentsFeat.WithPermanentQEffect("Adds or increase Parry trait", delegate (QEffect self)
            {
                self.StateCheck = (QEffect state) =>
                {
                    foreach (Item item in state.Owner.HeldItems)
                    {
                        if (!item.HasTrait(FirearmTraits.Parry) && FirearmUtilities.IsItemFirearmOrCrossbow(item) && item.HasTrait(Trait.TwoHanded))
                        {
                            item.Traits.Add(FirearmTraits.Parry);
                            item.Traits.Add(GunslingerTraits.TemporaryParry);
                        }
                    }
                };

                // Adjusts the bonus for items that already have the parry trait
                self.BonusToDefenses = (QEffect bonusToAC, CombatAction? action, Defense defense) =>
                {
                    QEffect? parryQEffect = bonusToAC.Owner.QEffects.FirstOrDefault(qe => qe.Id == FirearmQEIDs.Parry);
                    if (defense == Defense.AC && bonusToAC.Owner.HasEffect(FirearmQEIDs.Parry) && parryQEffect != null && parryQEffect.Tag != null && parryQEffect.Tag is Item item)
                    {
                        if (item.HasTrait(FirearmTraits.Parry) && !item.HasTrait(GunslingerTraits.TemporaryParry))
                        {
                            return new Bonus(2, BonusType.Circumstance, "Parry (Defensive Armaments)", true);
                        }
                    }

                    return null;
                };

                // Handles cleanup for when you drop or stow
                self.YouBeginAction = async (QEffect actionTakenCleanup, CombatAction action) =>
                {
                    // Checks if the last action was a drop or stow
                    string actionName = action.Name.ToLower();
                    if (actionName != null && (actionName.Contains("drop") || actionName.Contains("stow")))
                    {
                        // Collects all the temporary parry items for cleanup and handles it
                        Item? tempParrytem = self.Owner.HeldItems.FirstOrDefault(item => item.HasTrait(GunslingerTraits.TemporaryParry));
                        if (tempParrytem != null && actionName.Contains(tempParrytem.Name.ToLower()) && tempParrytem.HasTrait(GunslingerTraits.TemporaryParry))
                        {
                            tempParrytem.Traits.Remove(FirearmTraits.Parry);
                            tempParrytem.Traits.Remove(GunslingerTraits.TemporaryParry);
                            self.Owner.HeldItems.Remove(tempParrytem);
                        }
                    }
                };

                // Handles cleanup when you fall unconsious
                self.YouAreDealtLethalDamage = async (QEffect self, Creature attacker, DamageStuff damage, Creature defender) =>
                {
                    // Collects all the temporary parry items for cleanup and handles it
                    Item? tempParrytem = self.Owner.HeldItems.FirstOrDefault(item => item.HasTrait(GunslingerTraits.TemporaryParry));
                    if (tempParrytem != null && tempParrytem.HasTrait(GunslingerTraits.TemporaryParry))
                    {
                        tempParrytem.Traits.Remove(FirearmTraits.Parry);
                        tempParrytem.Traits.Remove(GunslingerTraits.TemporaryParry);
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Fake Out feat
        /// </summary>
        /// <param name="fakeOutFeat">The Fake Out true feat object</param>
        private static void AddFakeOutLogic(TrueFeat fakeOutFeat)
        {
            // Adds a permanent effect with various pieces for each segment of the game state
            fakeOutFeat.WithPermanentQEffect("Aid ally Attack with a Strike", delegate (QEffect fakeOutEffect)
            {
                // Start of combat the tracking Fakeout effect is added
                fakeOutEffect.StartOfCombat = async (QEffect startOfCombat) =>
                {
                    fakeOutEffect.Owner.AddQEffect(new QEffect()
                    {
                        Id = GunslingerQEIDs.FakeOut,
                        Tag = new List<Creature>()
                    });
                };

                // Add the start of the turn the tracking effect has it's list of creatures cleared
                fakeOutEffect.StartOfYourTurn = async (QEffect startOfTurn, Creature self) =>
                {
                    QEffect? fakeOutTrackingEffect = startOfTurn.Owner.QEffects.FirstOrDefault(qe => qe.Id == GunslingerQEIDs.FakeOut);
                    if (fakeOutTrackingEffect != null)
                    {
                        fakeOutTrackingEffect.Tag = new List<Creature>();
                    }
                };

                // After attacking that defender is adding to the tacking effect
                fakeOutEffect.BeforeYourActiveRoll = async (QEffect beforeAttackRoll, CombatAction action, Creature defender) =>
                {
                    QEffect? fakeOutTrackingEffect = beforeAttackRoll.Owner.QEffects.FirstOrDefault(qe => qe.Id == GunslingerQEIDs.FakeOut);
                    if (fakeOutTrackingEffect != null && fakeOutEffect.Tag != null && fakeOutEffect.Tag is List<Creature> creatures)
                    {
                        creatures.Add(defender);
                    }
                };
            });

            // Handles the aid reaction for all allies
            ModManager.RegisterActionOnEachCreature(creature =>
            {
                if (creature.OwningFaction == null || creature.OwningFaction.IsHumanControlled)
                {
                    creature.AddQEffect(new QEffect()
                    {
                        // When an ally contains the Fakeout action and take a reaction a prompt is asked if Fakeout should be used
                        BeforeYourActiveRoll = async (QEffect beforeAttackRoll, CombatAction action, Creature defender) =>
                        {
                            Creature[] alliesWithFakeout = beforeAttackRoll.Owner.Battle.AllCreatures.Where(battleCreature => battleCreature.OwningFaction == beforeAttackRoll.Owner.OwningFaction && battleCreature.HasEffect(GunslingerQEIDs.FakeOut) && battleCreature.Actions.CanTakeReaction()).ToArray();
                            foreach (Creature ally in alliesWithFakeout)
                            {
                                if (ally == beforeAttackRoll.Owner || action.Name == "Aid Strike" || ally.HasLineOfEffectTo(defender.Occupies) == CoverKind.Blocked || !defender.VisibleToHumanPlayer)
                                {
                                    continue;
                                }

                                // Collects the effects and items from the ally using Fakeout and begins building that aid strike subaction
                                QEffect? fakeOutTrackingEffect = ally.QEffects.FirstOrDefault(qe => qe.Id == GunslingerQEIDs.FakeOut);
                                Item? mainWeapon = ally.HeldItems.FirstOrDefault(item => FirearmUtilities.IsItemFirearmOrCrossbow(item));
                                if (mainWeapon != null && FirearmUtilities.IsItemLoaded(mainWeapon) && fakeOutTrackingEffect != null && fakeOutTrackingEffect.Tag != null && fakeOutTrackingEffect.Tag is List<Creature> creaturesAttacked)
                                {
                                    // Prompts the user to use the reaction for this effect
                                    string fakeOutTargetTextAddition = (creaturesAttacked.Contains(defender)) ? " (+1 circumstance bonus to this)" : string.Empty;
                                    if (await creature.Battle.AskToUseReaction(ally, "Make an attack roll to Aid the triggering attack." + fakeOutTargetTextAddition))
                                    {
                                        // Builds the strike for the aid strike
                                        CombatAction aidStrike = new CombatAction(ally, new SimpleIllustration(IllustrationName.None), "Aid Strike (" + mainWeapon.Name + ")", [], "{b}Critical Success{/b} Your ally gains a +2 circumstance bonus to the triggering action.\n\n\"{b}Success{/b} Your ally gains a +1 circumstance bonus to the triggering action.\n\n\"{b}Critical Failure{/b} Your ally gains a -1 circumstance penalty to the triggering action.\n\n", Target.Ranged(mainWeapon.WeaponProperties.MaximumRange));
                                        aidStrike.ActionCost = 0;
                                        aidStrike.Item = mainWeapon;
                                        aidStrike.Traits.CombatAction = aidStrike;
                                        aidStrike.ChosenTargets = action.ChosenTargets;
                                        CalculatedNumberProducer attackCheck = Checks.Attack(mainWeapon);
                                        attackCheck.WithExtraBonus((Func<CombatAction, Creature, Creature?, Bonus?>)((combatAction, demoralizer, target) => ((creaturesAttacked.Contains(defender)) ? new Bonus(1, BonusType.Circumstance, "Attacked last round") : (Bonus)null)));
                                        aidStrike.WithActiveRollSpecification(new ActiveRollSpecification(attackCheck, Checks.FlatDC(15)));
                                        aidStrike.WithEffectOnEachTarget(async delegate (CombatAction aidAction, Creature attacker, Creature defender, CheckResult result)
                                        {
                                            // Depending on the attacks result the original attacker gains a bonus
                                            switch (result)
                                            {
                                                case CheckResult.CriticalSuccess:
                                                    beforeAttackRoll.Owner.AddQEffect(new QEffect(ExpirationCondition.Never)
                                                    {
                                                        BonusToAttackRolls = (QEffect bonusToAttackRoll, CombatAction action, Creature? creature) =>
                                                        {
                                                            return new Bonus(2, BonusType.Circumstance, "Aid", true);
                                                        }
                                                    });
                                                    break;
                                                case CheckResult.Success:
                                                    beforeAttackRoll.Owner.AddQEffect(new QEffect(ExpirationCondition.Never)
                                                    {
                                                        BonusToAttackRolls = (QEffect bonusToAttackRoll, CombatAction action, Creature? creature) =>
                                                        {
                                                            return new Bonus(1, BonusType.Circumstance, "Aid", true);
                                                        }
                                                    });
                                                    break;
                                                case CheckResult.CriticalFailure:
                                                    beforeAttackRoll.Owner.AddQEffect(new QEffect(ExpirationCondition.Never)
                                                    {
                                                        BonusToAttackRolls = (QEffect bonusToAttackRoll, CombatAction action, Creature? creature) =>
                                                        {
                                                            return new Bonus(-1, BonusType.Circumstance, "Aid", false);
                                                        }
                                                    });
                                                    break;
                                            }
                                        });

                                        await aidStrike.AllExecute();
                                    }
                                }
                            }
                        }
                    });
                }
            });
        }

        /// <summary>
        /// Patches Quick Draw to be selectable by Gunslinger
        /// </summary>
        /// <param name="quickDrawFeat">The Quick Draw feat</param>
        private static void PatchQuickDraw(Feat quickDrawFeat)
        {
            // Adds the Gunslinger trait and cycles through the Class Prerequisites that don't have Gunslinger and adds it
            quickDrawFeat.Traits.Add(GunslingerTraits.Gunslinger);
            for (int i = 0; i < quickDrawFeat.Prerequisites.Count; i++)
            {
                Prerequisite prereq = quickDrawFeat.Prerequisites[i];
                if (prereq is ClassPrerequisite classPrerequisite)
                {
                    if (!classPrerequisite.AllowedClasses.Contains(GunslingerTraits.Gunslinger))
                    {
                        List<Trait> updatedAllowedClasses = classPrerequisite.AllowedClasses;
                        updatedAllowedClasses.Add(GunslingerTraits.Gunslinger);
                        quickDrawFeat.Prerequisites[i] = new ClassPrerequisite(updatedAllowedClasses);
                    }
                }


            }
        }

        /// <summary>
        /// Gets the Reload Action IDs
        /// </summary>
        /// <returns>A list of Reload Action IDs</returns>
        private static List<ActionId> GetReloadAIDs()
        {
            return [ActionId.Reload, FirearmActionIDs.DoubleBarrelReload];
        }

        /// <summary>
        /// Asyncronisly gets a tile for leaping witin the distance
        /// </summary>
        /// <param name="self">The creature leaping</param>
        /// <param name="messageString">The message displayed while leaping</param>
        /// <param name="range">The max distance</param>
        /// <returns>The tile selected or null otherwise</returns>
        public static async Task<Tile?> GetLeapTileWithinDistance(Creature self, string messageString, int range)
        {
            // Gets the starting tile, initatlizes the options and collects the possible tiles within range that the user can reach
            Tile startingTile = self.Occupies;
            List<Option> options = new List<Option>();
            foreach (Tile tile in self.Battle.Map.AllTiles)
            {
                if (tile.IsFree && tile.CanIStopMyMovementHere(self) && startingTile.DistanceTo(tile) <= range)
                {
                    options.Add(new TileOption(tile, "Tile (" + tile.X + "," + tile.Y + ")", null, (AIUsefulness)int.MinValue, true));
                }
            }

            // Prompts the user to select a valid tile and returns it or null
            Option selectedOption = (await self.Battle.SendRequest(new AdvancedRequest(self, messageString, options)
            {
                IsMainTurn = false,
                IsStandardMovementRequest = false,
                TopBarIcon = IllustrationName.Jump,
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
        /// Asyncronisly gets a tile for long jumping witin the distance
        /// </summary>
        /// <param name="self">The creature long jumping</param>
        /// <param name="originalTileBeforeStride">The original tile that must be further from</param>
        /// <param name="messageString">The message displayed while long jumping</param>
        /// <param name="range">The max distance</param>
        /// <returns>The tile selected or null otherwise</returns>
        public static async Task<Tile?> GetLongJumpTileWithinDistance(Creature self, Tile originalTileBeforeStride, string messageString, int range)
        {
            // Gets the starting tile, initatlizes the options and collects the possible tiles within range that the user can reach
            Tile startingTile = self.Occupies;
            List<Option> options = new List<Option>();
            foreach (Tile tile in self.Battle.Map.AllTiles)
            {
                if (tile.IsFree && tile.CanIStopMyMovementHere(self) && startingTile.DistanceTo(tile) <= range && originalTileBeforeStride.DistanceTo(tile) > originalTileBeforeStride.DistanceTo(startingTile))
                {
                    options.Add(new TileOption(tile, "Tile (" + tile.X + "," + tile.Y + ")", null, (AIUsefulness)int.MinValue, true));
                }
            }

            // Prompts the user to select a valid tile and returns it or null
            Option selectedOption = (await self.Battle.SendRequest(new AdvancedRequest(self, messageString, options)
            {
                IsMainTurn = false,
                IsStandardMovementRequest = false,
                TopBarIcon = IllustrationName.Jump,
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
    }
}
