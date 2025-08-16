using Dawnsbury.Audio;
using Dawnsbury.Auxiliary;
using Dawnsbury.Campaign.Encounters;
using Dawnsbury.Core;
using Dawnsbury.Core.Animations;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.Archetypes;
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
using Dawnsbury.Display;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Display.Text;
using Dawnsbury.IO;
using Dawnsbury.Modding;
using Dawnsbury.Mods.Feats.Classes.Gunslinger.Enums;
using Dawnsbury.Mods.Feats.Classes.Gunslinger.RegisteredComponents;
using Dawnsbury.Mods.Feats.Classes.Gunslinger.Ways;
using Dawnsbury.Mods.Items.Firearms;
using Dawnsbury.Mods.Items.Firearms.RegisteredComponents;
using Dawnsbury.Mods.Items.Firearms.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using static Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.BarbarianFeatsDb.AnimalInstinctFeat;
using static Dawnsbury.Core.Mechanics.Core.CalculatedNumber;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dawnsbury.Mods.Feats.Classes.Gunslinger
{
    /// <summary>
    /// The Gunslinger class
    /// TODO WHERE PR MERGED:
    /// - Remove Reflection
    /// - Use StrikeModifers4
    /// - Use Trait.Firearm
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

            yield return new Feat(GunslingerFeatNames.AdvancedShooterFirearm, "You've dedicated your training to the most complex and weird firearms.", "You are a Master in advanced firearms", [], null)
                .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                {
                    sheet.Proficiencies.Set([Trait.Firearm, Trait.Advanced], Proficiency.Master);
                });

            yield return new Feat(GunslingerFeatNames.AdvancedShooterCrossbow, "You've dedicated your training to the most complex and weird crossbow.", "You are a Master in advanced crossbow", [], null)
                .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                {
                    sheet.Proficiencies.Set([Trait.Crossbow, Trait.Advanced], Proficiency.Master);
                });

            yield return new Feat(GunslingerFeatNames.PistoleroDedicationDeception, "The way of the Pistolero.", "You become trained in Deception.", [GunslingerTraits.PistoleroSkillChoice], null)
                .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                {
                    sheet.TrainInThisOrSubstitute(Skill.Deception);
                });

            yield return new Feat(GunslingerFeatNames.PistoleroDedicationIntimidation, "The way of the Pistolero.", "You become trained in Intimidation.", [GunslingerTraits.PistoleroSkillChoice], null)
                .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                {
                    sheet.TrainInThisOrSubstitute(Skill.Intimidation);
                });

            // Creates and adds the logic for the Singular Expertise class feature
            Feat singularExpertiseFeat = new Feat(GunslingerFeatNames.SingularExpertise, "You have particular expertise with guns and crossbows that grants you greater proficiency with them and the ability to deal more damage.", "You gain a +1 circumstance bonus to damage rolls with firearms and crossbows.", [], null);
            AddSingularExpertiseLogic(singularExpertiseFeat);
            yield return singularExpertiseFeat;

            // Creates the class selection feat for the Gunslinger
            yield return new ClassSelectionFeat(GunslingerFeatNames.GunslingerClass, "While some fear projectile weapons, you savor the searing flash, wild kick, and cloying smoke that accompanies a gunshot, or snap of the cable and telltale thunk of your crossbow just before your bolt finds purchase. Ready to draw a bead on an enemy at every turn, you rely on your reflexes, steady hand, and knowledge of your weapons to riddle your foes with holes.",
                GunslingerTraits.Gunslinger, new EnforcedAbilityBoost(Ability.Dexterity), 8,
                [Trait.Will, Trait.Unarmed, Trait.Simple, Trait.Martial,Trait.UnarmoredDefense, Trait.LightArmor, Trait.MediumArmor, GunslingerTraits.DummyAdvancedProf],
                [Trait.Perception, Trait.Fortitude, Trait.Reflex, GunslingerTraits.DummySimpleAndMartialProf],
                3,
                "{b}1. Gunslinger's Way{/b} All gunslingers have a particular way they follow, a combination of philosophy and combat style that defines both how they fight and the weapons they excel with. At 1st level, your way grants you an initial deed, a unique reload action called a slinger's reload, and proficiency with a particular skill. You also gain advanced and greater deeds at later levels, as well as access to way-specific feats.\n\n" +
                "{b}2. Singular Expertise{/b} You have particular expertise with guns and crossbows that grants you greater proficiency with them and the ability to deal more damage. You gain a +1 circumstance bonus to damage rolls with firearms and crossbows.\r\n\r\nThis intense focus on firearms and crossbows prevents you from reaching the same heights with other weapons. Your proficiency with unarmed attacks and with weapons other than firearms and crossbows can't be higher than trained, even if you gain an ability that would increase your proficiency in one or more other weapons to match your highest weapon proficiency (such as the weapon expertise feats many ancestries have). If you have gunslinger weapon mastery, the limit is expert, and if you have gunslinging legend, the limit is master.\n\n" +
                "{b}3. Gunslinger Feat{/b}\n\n" +
                "{b}At higher levels:{/b}\n" +
                "{b}Level 2:{/b} Gunslinger feat\n" +
                "{b}Level 3:{/b} General feat, skill increase, expert in Will,\n" +
                "{b}Level 4:{/b} Gunslinger feat\n" +
                "{b}Level 5:{/b}Ability boosts, ancestry feat, skill increase, gunslinger weapon master {i}(Your proficiency rank increases to master with simple and martial firearms and crossbows. Your proficiency rank for advanced firearms and crossbows, simple weapons, martial weapons, and unarmed attacks increases to expert. You gain access to the critical specialization effects for firearms and crossbows.){/i}\n" +
                "{b}Level 6:{/b}Gunslinger feat\n" +
                "{b}Level 7:{/b}General feat, skill increase, weapon specialization {i}(you deal 2 additional damage with weapons and unarmed attacks in which you are an expert; this damage increases to 3 if you're a master, and to 4 if you're legendary){/i}, master in Perception\n" +
                "{b}Level 8:{/b}Gunslinger feat\n", new List<Feat>() { wayOfTheDrifter.Feat, wayOfThePistolero.Feat, wayOfTheSniper.Feat, wayOfTheVanguard.Feat })
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    // Base Prof
                    sheet.Proficiencies.Set([Trait.Firearm, Trait.Simple], Proficiency.Expert);
                    sheet.Proficiencies.Set([Trait.Firearm, Trait.Material], Proficiency.Expert);
                    sheet.Proficiencies.Set([Trait.Firearm, Trait.Advanced], Proficiency.Trained);
                    sheet.Proficiencies.Set([Trait.Crossbow, Trait.Simple], Proficiency.Expert);
                    sheet.Proficiencies.Set([Trait.Crossbow, Trait.Material], Proficiency.Expert);
                    sheet.Proficiencies.Set([Trait.Crossbow, Trait.Advanced], Proficiency.Trained);

                    // Adds the Singular Expertise base class feature, adds a Level 1 Gunslinger feat selection, and adds the Will Expert profeciency at level 3
                    sheet.AddFeat(singularExpertiseFeat, null);
                    sheet.AddSelectionOption(new SingleFeatSelectionOption("1stGunslingerFeat", "Gunslinger feat", 1, (Feat ft) => ft.HasTrait(GunslingerTraits.Gunslinger)));
                    sheet.AddAtLevel(3, delegate (CalculatedCharacterSheetValues values)
                    {
                        values.SetProficiency(Trait.Will, Proficiency.Expert);
                    });
                    sheet.AddAtLevel(5, delegate (CalculatedCharacterSheetValues values)
                    {
                        // Upgraded Prof
                        sheet.Proficiencies.Set([Trait.Firearm, Trait.Simple], Proficiency.Master);
                        sheet.Proficiencies.Set([Trait.Firearm, Trait.Material], Proficiency.Master);
                        sheet.Proficiencies.Set([Trait.Firearm, Trait.Advanced], Proficiency.Expert);
                        sheet.Proficiencies.Set([Trait.Crossbow, Trait.Simple], Proficiency.Master);
                        sheet.Proficiencies.Set([Trait.Crossbow, Trait.Material], Proficiency.Master);
                        sheet.Proficiencies.Set([Trait.Crossbow, Trait.Advanced], Proficiency.Expert);

                        values.SetProficiency(Trait.Unarmed, Proficiency.Expert);
                        values.SetProficiency(Trait.Simple, Proficiency.Expert);
                        values.SetProficiency(Trait.Martial, Proficiency.Expert);
                        // TODO: Crit Specilization needed to be added for fire arms
                    });
                    sheet.AddAtLevel(7, delegate (CalculatedCharacterSheetValues values)
                    {
                        values.SetProficiency(Trait.Perception, Proficiency.Master);
                    });
                })
                .WithOnCreature((creature) =>
                {
                    if (creature.Level >= 5)
                    {
                        creature.AddQEffect(new QEffect("Weapon Mastery", "Your firearm and crossbow attacks trigger {tooltip:criteffect}critical specialization effects{/}.")
                        {
                            YouHaveCriticalSpecialization = (QEffect specialization, Item item, CombatAction action, Creature defender) =>
                            {
                                return item.HasTrait(Trait.Firearm) || item.HasTrait(Trait.Crossbow);
                            }
                        });
                    }
                    if (creature.Level >= 7)
                    {
                        creature.AddQEffect(QEffect.WeaponSpecialization());
                    }
                });

            // Level 1 Class Feats
            TrueFeat coatedMunitionsFeat = new TrueFeat(GunslingerFeatNames.CoatedMunitions, 1, "You coat your munitions with mysterious alchemical mixed liquids you keep in small vials.", "{b}Requirements{/b} You're wielding a loaded firearm or crossbow.\n\nUntil the end of your turn, your next attack deals an addtional 1 persistent damage and 1 spalsh damage of your choice between acid, cold, electricity, fire or poison.", [GunslingerTraits.Gunslinger, Trait.Homebrew], null);
            coatedMunitionsFeat.WithActionCost(1);
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
            TrueFeat hitTheDirtFeat = new TrueFeat(GunslingerFeatNames.HitTheDirt, 1, "You fling yourself out of harm's way.", "{b}Trigger{/b} A creature you can see attempts a ranged Strike against you.\n\nYou Leap. Your movement gives you a +2 circumstance bonus to AC against the triggering attack. Regardless of whether or not the triggering attack hits, you land prone after completing your Leap.", [GunslingerTraits.Gunslinger]).WithActionCost(-2);
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
            TrueFeat fakeOutFeat = new TrueFeat(GunslingerFeatNames.FakeOut, 2, "With a skilled flourish of your weapon, you force an enemy to acknowledge you as a threat.", "{b}Trigger{/b} An ally is about to use an attack action, targeting a creature within your weapon's first range increment.\n\n{b}Requirements{/b} You're wielding a loaded firearm or crossbow.\n\nMake an attack roll to Aid the triggering attack. If you dealt damage to that enemy since the start of your last turn, you gain a +1 circumstance bonus to this roll.\n\n{i}Aid{/i}\n\n{b}Critical Success{/b} Your ally a +2 circumstance bonus\n{b}Success{/b} Your ally a +1 circumstance bonus\n{b}Critical Failure{/b} Your ally a -1 circumstance penalty\n", [GunslingerTraits.Gunslinger, Trait.Visual]).WithActionCost(-2);
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

            // Level 4 Class Feats
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

            // Level 6 Class Feats
            // Creates and adds the logic for the Drifter's Juke class feat
            TrueFeat driftersJuke = new TrueFeat(GunslingerFeatNames.DriftersJuke, 6, "You move in and out of range to complement your attacks.", "{b}Requirements{/b} You're wielding a firearm or crossbow in one hand, and your other hand is either wielding a melee weapon or is empty.\n\nYou may Step, make a Strike, Step, and make another Strike (Each is opptional). One Strike must be a ranged Strike using your firearm or crossbow, and the other must be a melee Strike using your melee weapon or unarmed attack.", [GunslingerTraits.Gunslinger]);
            driftersJuke.WithActionCost(2);
            driftersJuke.WithPrerequisite(GunslingerFeatNames.WayOfTheDrifter, "Way of the Drifter");
            AddDriftersJukeLogic(driftersJuke);
            yield return driftersJuke;

            // Creates and adds the logic for the Pistolero's Challenge class feat
            TrueFeat pistolerosChallenge = new TrueFeat(GunslingerFeatNames.PistolerosChallenge, 6, "With a stern call, carefully chosen barb, or some other challenging declaration, you demand your foe's attention in a duel.", "Make a Deception or Intimidation check against a creature within 30 feet against their Will DC. The target will be immune to this for the rest of the encounter. If you succeed, they are your challenged foe, and you can only have 1 challenged foe at a time.\n\n{b}Success{/b} You and the target gain a +2 status bonus to damage rolls with Strikes against each other. If you are a master in the skill you used for the check, the bonus increases to +3, and if you're legendary it is instead +4.\b{b}Critical Failure{/b} You become frightened 1 and can't use this again for 1 minute.", [Trait.Auditory, Trait.Flourish, Trait.Linguistic, Trait.Mental, GunslingerTraits.Gunslinger]);
            pistolerosChallenge.WithActionCost(1);
            pistolerosChallenge.WithPrerequisite(GunslingerFeatNames.WayOfThePistolero, "Way of the Pistolero");
            AddPistolerosChallengeLogic(pistolerosChallenge);
            yield return pistolerosChallenge;

            // Creates and adds the logic for the Sniper's Aim class feat
            TrueFeat snipersAim = new TrueFeat(GunslingerFeatNames.SnipersAim, 6, "You take an extra moment to carefully sync your aim and breathing, then fire a shot with great accuracy.", "Make a ranged weapon Strike. You gain a +2 circumstance bonus to this Strike's attack roll and ignore the target's concealment. If you're using a kickback firearm, you don't take the normal circumstance penalty on this Strike for not having the required Strength score or firing without using a stabilizer.", [Trait.Concentrate, GunslingerTraits.Gunslinger]);
            snipersAim.WithActionCost(2);
            snipersAim.WithPrerequisite(GunslingerFeatNames.WayOfTheSniper, "Way of the Sniper");
            AddSnipersAimLogic(snipersAim);
            yield return snipersAim;

            // Creates and adds the logic for the Phalanx Breaker class feat
            TrueFeat phalanxBreaker = new TrueFeat(GunslingerFeatNames.PhalanxBreaker, 6, "You know that to take out an enemy formation, you must punch a hole through its center.", "{b}Requirements{/b} You're wielding a two-handed firearm or a two-handed crossbow.\n\nMake a ranged Strike with the required weapon against a target within the weapon's first range increment. The target is pushed directly back 10 feet (20 feet on a critical hit), and if this pushes the target into an obstacle, the target takes bludgeoning damage equal to half your level.", [GunslingerTraits.Gunslinger]);
            phalanxBreaker.WithActionCost(2);
            phalanxBreaker.WithPrerequisite(GunslingerFeatNames.WayOfTheVanguard, "Way of the Vanguard");
            AddPhalanxBreakerLogic(phalanxBreaker);
            yield return phalanxBreaker;

            // Creates and adds the logic for the Advanced Shooter class feat
            TrueFeat advancedShooter = new TrueFeat(GunslingerFeatNames.AdvancedShooter, 6, "You've dedicated your training to the most complex and weird weapons of your favorite group.", "Choose firearms or crossbows. You become a Master with all advanced weapons of your choice.", [GunslingerTraits.Gunslinger]);
            AddAdvancedShooterLogic(advancedShooter);
            yield return advancedShooter;

            // Creates and adds the logic for the Cauterize class feat
            TrueFeat cauterize = new TrueFeat(GunslingerFeatNames.Cauterize, 6, "You use the smoking barrel of your firearm to sear shut a bleeding wound.", "Make a Strike with your firearm. You then press the heated barrel to the wounds of you or an ally within reach that is taking persistent bleed damage, giving an immediate flat check to end the bleed with the lower DC for particularly effective assistance.", [Trait.Flourish, GunslingerTraits.Gunslinger]);
            AddCauterizeLogic(cauterize);
            yield return cauterize;

            // Creates and adds the logic for the Scatter Blast class feat
            // TODO: Figure out temp range increment increase - range increment increases by 20 feet and the 
            TrueFeat scatterBlast = new TrueFeat(GunslingerFeatNames.ScatterBlast, 6, "You pack your weapon with additional shot and powder, creating a risky but devastating wave of destruction.", "{b}Requirements{/b} You're wielding a loaded firearm that has the scatter trait.\n\nMake a ranged Strike with the firearm. The firearm's range increment increases by 20 feet and the radius of its scatter increases by 20 feet. The Strike gains the following failure conditions.\n\n{b}Failure{/b} The firearm misfires, but it doesn't cause the other critical failure effects listed below.\n{b}Critical Failure{/b} The firearm misfires and also explodes. It becomes broken, and it deals its normal weapon damage to all creatures in a 20-foot burst centered on the firearm, with a basic Reflex save against your class DC. This damage includes any from the weapon's fundamental and property runes.", [GunslingerTraits.Gunslinger]);
            scatterBlast.WithActionCost(2);
            AddScatterBlastLogic(scatterBlast);
            yield return scatterBlast;

            // Level 8 Class Feats
            // Creates and adds the logic for the Stab And Blast class feat
            TrueFeat stabAndBlast = new TrueFeat(GunslingerFeatNames.StabAndBlast, 8, "You slice or smash your opponent with the melee portion of your weapon before pulling the trigger at point-blank range.", "{b}Requirements{/b} You're wielding a loaded firearm that has a bayonet or stock attached.\n\nMake a melee Strike with the required weapon. If the Strike is successful, you can immediately make a ranged Strike against the same target with a +2 circumstance bonus to the attack roll. This counts as two attacks toward your multiple attack penalty, but you don't apply the multiple attack penalty until after making both attacks.", [Trait.Flourish, GunslingerTraits.Gunslinger]);
            stabAndBlast.WithActionCost(1);
            AddStabAndBlastLogic(stabAndBlast);
            yield return stabAndBlast;

            // Creates and adds the logic for the Smoke Curtain class feat
            TrueFeat smokeCurtain = new TrueFeat(GunslingerFeatNames.SmokeCurtain, 8, "You load an extra dose of powder into your shot, causing it to belch a cloud of smoke.", "{b}Requirements{/b} You're wielding a loaded firearm.\n\nYou make a Strike with your firearm and create a cloud of smoke in a 20-foot emanation centered on your location. Creatures are concealed while within the smoke. The smoke dissipates at the start of your next turn. If your Strike is a critical failure, your firearm misfires.", [GunslingerTraits.Gunslinger]);
            smokeCurtain.WithActionCost(2);
            AddSmokeCurtainLogic(smokeCurtain);
            yield return smokeCurtain;

            // Creates and adds the logic for the Leap And Fire class feat
            TrueFeat leapAndFire = new TrueFeat(GunslingerFeatNames.LeapAndFire, 8, "You're quick enough to line up a shot even while diving to the ground.", "When you use your Hit the Dirt! reaction, you may make a ranged Strike with a loaded firearm or crossbow, targeting the creature whose attack triggered the reaction. You can choose to make this strike before or after leaping. (Before you fall prone)", [GunslingerTraits.Gunslinger]);
            leapAndFire.WithPrerequisite(GunslingerFeatNames.HitTheDirt, "Hit the Dirt");
            AddLeapAndFireLogic(leapAndFire);
            yield return leapAndFire;

            // Creates and adds the logic for the Grit And Tenacity class feat
            TrueFeat gritAndTenacity = new TrueFeat(GunslingerFeatNames.GritAndTenacity, 8, "You call upon deep reserves of toughness and mental fortitude to power through an otherwise debilitating effect.", "{b}Trigger{/b} You fail a Fortitude or Will save.\n{b}Frequency{/b} once per combat\n\nReroll the triggering save with a +2 circumstance bonus; you must use the second result.", [Trait.Fortune, GunslingerTraits.Gunslinger]);
            gritAndTenacity.WithActionCost(-2);
            AddGritAndTenacityLogic(gritAndTenacity);
            yield return gritAndTenacity;

            // Creates and adds the logic for the Bullet Split class feat
            TrueFeat bulletSplit = new TrueFeat(GunslingerFeatNames.BulletSplit, 8, "You carefully align your weapon with the edge of your blade, splitting the projectile in two as you fire to attack two different targets.", "{b}Requirements{/b} You're wielding a firearm or crossbow in one hand and a slashing, versatile S, or bayonet melee weapon in the other.\n\nMake two Strikes, one each against two separate targets. The targets must be adjacent to each other and within your weapon's maximum range. Each of these attacks takes a –2 penalty to the attack roll, but the two count as only one attack when calculating your multiple attack penalty.", [Trait.Flourish, GunslingerTraits.Gunslinger]);
            bulletSplit.WithActionCost(1);
            AddBulletSplitLogic(bulletSplit);
            yield return bulletSplit;

            // Dedication and Archetype
            Feat drifterDedication = new Feat(GunslingerFeatNames.WayOfTheDrifterDedication, "The way of the Drifter.", "You become trained in Acrobatics.", [], null)
                .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                {
                    sheet.TrainInThisOrSubstitute(Skill.Acrobatics);
                });
            Feat pistoleroDedication = new Feat(GunslingerFeatNames.WayOfThePistoleroDedication, "The way of the Pistolero.", "You become trained in Deception.", [], null)
                .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                {
                    sheet.TrainInThisOrSubstitute(Skill.Deception);
                    sheet.AddSelectionOptionRightNow(new SingleFeatSelectionOption("PistoleroDedicationSkill", "Pistolero Skill Choice", -1, (Feat ft) => ft.HasTrait(GunslingerTraits.PistoleroSkillChoice)));
                });
            Feat sniperDedication = new Feat(GunslingerFeatNames.WayOfTheSniperDedication, "The way of the Sniper.", "You become trained in Stealth.", [], null)
                .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                {
                    sheet.TrainInThisOrSubstitute(Skill.Stealth);
                });
            Feat vanguardDedication = new Feat(GunslingerFeatNames.WayOfTheVanguardDedication, "The way of the Vanguard.", "You become trained in Athletics.", [], null)
                .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                {
                    sheet.TrainInThisOrSubstitute(Skill.Athletics);
                });

            yield return ArchetypeFeats.CreateMulticlassDedication(GunslingerTraits.Gunslinger, "You excel in using specific types of ranged weapons.", "You have familiarity with martial crossbows and firearms, treating them as simple weapons for the purposes of proficiency. You become trained in gunslinger class DC.\n\nChoose a gunslinger way. You become trained in your way's associated skill; if you were already trained in this skill, you become trained in a skill of your choice. You don't gain any other abilities from your choice of way.",[drifterDedication, pistoleroDedication, sniperDedication, vanguardDedication])
                .WithDemandsAbility14(Ability.Dexterity)
                .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                {
                    sheet.Proficiencies.AddProficiencyAdjustment((List<Trait> traits) => traits.Contains(Trait.Martial) && (traits.Contains(Trait.Firearm) || traits.Contains(Trait.Crossbow)), Trait.Simple);
                    sheet.SetProficiency(GunslingerTraits.Gunslinger, Proficiency.Trained);
                });

            Feat slingersReadiness = new TrueFeat(GunslingerFeatNames.SlingersReadiness, 6, "You've learned a gunslinger's tricks for staking out your territory in a fight.", "You gain the initial deed for the way you selected with Gunslinger's Dedication.", [])
                .WithAvailableAsArchetypeFeat(GunslingerTraits.Gunslinger);
            slingersReadiness.WithRulesTextCreator((CharacterSheet sheet) =>
            {
                CalculatedCharacterSheetValues values = sheet.Calculated;

                if (values.HasFeat(GunslingerFeatNames.WayOfTheDrifterDedication))
                {
                    return wayOfTheDrifter.InitialDeedRulesText;
                }
                else if (values.HasFeat(GunslingerFeatNames.WayOfThePistoleroDedication))
                {
                    return wayOfThePistolero.InitialDeedRulesText;
                }
                else if (values.HasFeat(GunslingerFeatNames.WayOfTheSniperDedication))
                {
                    return wayOfTheSniper.InitialDeedRulesText;
                }
                else if (values.HasFeat(GunslingerFeatNames.WayOfTheVanguardDedication))
                {
                    return wayOfTheVanguard.InitialDeedRulesText;
                }

                return null;
            });
            GunslingerWayExtensions.AddDrifersIntoTheFrayLogic(slingersReadiness, true);
            GunslingerWayExtensions.AddPistolerosTenPacesLogic(slingersReadiness, true);
            GunslingerWayExtensions.AddSnipersOneShotOneKillLogic(slingersReadiness, true);
            GunslingerWayExtensions.AddVanguardLivingFortificationLogic(slingersReadiness, true);

            yield return slingersReadiness;

            foreach (Feat feat in ArchetypeFeats.CreateBasicAndAdvancedMulticlassFeatGrantingArchetypeFeats(GunslingerTraits.Gunslinger, "Shooting"))
            {
                yield return feat;
            }
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
                    if (action.HasTrait(Trait.Firearm) || action.HasTrait(Trait.Crossbow) || (action.Item != null && action.Item.WeaponProperties != null && FirearmUtilities.IsItemFirearmOrCrossbow(action.Item)))
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
                        // HACK: Hotfix for null ref StrikeRules error
                        coverFireAction.StrikeModifiers = basicStrike.StrikeModifiers;
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
                                if (!target.OwningFaction.IsPlayer)
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
                    }
                    ;

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
                    if (item.HasTrait(Trait.Firearm) && FirearmUtilities.IsItemLoaded(item) && item.WeaponProperties != null)
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
                            //// A crash happens if the sound effect of the second weapon is too long and is still playing, so a swap is needed 
                            //SfxName? replacementSfx = null;
                            //if (firstHeldItem.WeaponProperties != null && secondHeldItem.WeaponProperties != null && firstHeldItem.WeaponProperties.Sfx == secondHeldItem.WeaponProperties.Sfx)
                            //{
                            //    if (secondHeldItem.HasTrait(Trait.Crossbow))
                            //    {
                            //        replacementSfx = (firstHeldItem.WeaponProperties.Sfx == SfxName.Bow) ? SfxName.Fist : SfxName.Bow;
                            //    }
                            //    else if (secondHeldItem.HasTrait(Trait.Firearm))
                            //    {
                            //        replacementSfx = (firstHeldItem.WeaponProperties.Sfx == FirearmSFXNames.SmallFirearm1) ? FirearmSFXNames.SmallFirearm2 : FirearmSFXNames.SmallFirearm1;
                            //    }

                            //    if (replacementSfx != null)
                            //    {
                            //        secondHeldItem.WeaponProperties.Sfx = (SfxName)replacementSfx;
                            //    }
                            //}

                            await pairedShotEffect.Owner.MakeStrike(targets.ChosenCreature, firstHeldItem, currentMap);
                            await pairedShotEffect.Owner.MakeStrike(targets.ChosenCreature, secondHeldItem, currentMap);

                            //if (replacementSfx != null && firstHeldItem.WeaponProperties != null && secondHeldItem.WeaponProperties != null)
                            //{
                            //    secondHeldItem.WeaponProperties.Sfx = firstHeldItem.WeaponProperties.Sfx;
                            //}
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
                            ActionPossibility damageAction = new ActionPossibility(new CombatAction(coatedMunitionsEffect.Owner, illustraions[damageKind], damageString, [Trait.Basic], "You deal an additional {Blue}1{/} persistent {Blue}" + damageString + "{/} damage and {Blue}1{/} {Blue}" + damageString + "{/} splash damage.", Target.Self()
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
                                                    CommonSpellEffects.DealDirectDamage(null, DiceFormula.FromText("1"), creature, CheckResult.Success, damageKind);
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

                        foreach (Item firearm in owner.HeldItems.Where(item => item.HasTrait(Trait.Firearm)))
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
                    Creature owner = cleanupEffects.Owner;
                    if (owner.HasEffect(GunslingerQEIDs.HitTheDirt) && owner.HP > 0)
                    {
                        async Task<Item?> GetStrikeWeapon(List<Item> weapons)
                        {
                            if (weapons.Count == 2)
                            {
                                ChoiceButtonOption choice = await owner.AskForChoiceAmongButtons(new SideBySideIllustration(weapons[0].Illustration, weapons[1].Illustration), $"Strike with which weapon?", weapons[0].Name, weapons[1].Name, "Pass");
                                if (choice.Index == 2)
                                {
                                    return null;
                                }

                                return weapons[choice.Index];
                            }

                            return weapons.FirstOrDefault();
                        }

                        List<Item> validWeapons = owner.HeldItems.Where(item => FirearmUtilities.IsItemFirearmOrCrossbow(item) && FirearmUtilities.IsItemLoaded(item)).ToList();
                        bool hasLeapAndFire = owner.HasFeat(GunslingerFeatNames.LeapAndFire);
                        bool strikeNeedsToBeMadeAfterLeap = false;
                        if (hasLeapAndFire && action.Owner != null)
                        {
                            if (validWeapons.Count > 0)
                            {
                                Illustration illustration = (validWeapons.Count == 2) ? new SideBySideIllustration(validWeapons[0].Illustration, validWeapons[1].Illustration) : validWeapons[0].Illustration;
                                ChoiceButtonOption choice = await owner.AskForChoiceAmongButtons(illustration, $"Make a Strike against {action.Owner} as a free action?", "Before Leaping", "After Leaping", "Pass");
                                if (choice.Index == 1)
                                {
                                    strikeNeedsToBeMadeAfterLeap = true;
                                }
                                else if (choice.Index == 0)
                                {
                                    Item? weapon = await GetStrikeWeapon(validWeapons);
                                    if (weapon != null)
                                    {
                                        await owner.MakeStrike(action.Owner, weapon);
                                    }
                                }
                            }
                        }
                        owner.RemoveAllQEffects(qe => qe.Id == GunslingerQEIDs.HitTheDirt);
                        int leapDistance = ((owner.Speed >= 6) ? 3 : 2) + (owner.HasEffect(QEffectId.PowerfulLeap) ? 1 : 0);
                        CombatAction leapAction = CommonCombatActions.Leap(owner);
                        leapAction.EffectOnChosenTargets = null;
                        Tile? tileToLeapTo = await GetLeapTileWithinDistance(cleanupEffects.Owner, "Choose the tile to leap to.", leapDistance);
                        if (tileToLeapTo != null)
                        {
                            await owner.SingleTileMove(tileToLeapTo, leapAction);
                            if (strikeNeedsToBeMadeAfterLeap && action.Owner != null)
                            {
                                Item? weapon = await GetStrikeWeapon(validWeapons);
                                if (weapon != null)
                                {
                                    await owner.MakeStrike(action.Owner, weapon);
                                }
                            }
                        }

                        await owner.FallProne();
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
                                        await FirearmUtilities.AwaitReloadItem(self, heldItem);
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
            swordAndPistolFeat.WithPermanentQEffect("After melee Strikes the target is debuffed against ranged Strikes, and after ranged Strikes the enemy is debuffed against melee Strikes.", delegate (QEffect self)
            {
                self.BeforeYourActiveRoll = async (QEffect addingEffects, CombatAction action, Creature defender) =>
                {
                    // If you attack within your melee range with a ranged Firearm or Crossbow, you gain a Melee buff 
                    if (action.HasTrait(Trait.Ranged) && !action.HasTrait(Trait.TwoHanded) && (action.HasTrait(Trait.Firearm) || action.HasTrait(Trait.Crossbow)) && addingEffects.Owner.DistanceTo(defender) == 1 && !addingEffects.Owner.QEffects.Any(qe => qe.Id == GunslingerQEIDs.SwordAndPistolMeleeBuff && qe.Tag != null && qe.Tag == defender))
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
                                        if (!item.HasTrait(Trait.DoesNotProvoke) && item.HasTrait(Trait.Ranged) && !item.HasTrait(Trait.TwoHanded) && (item.HasTrait(Trait.Firearm) || item.HasTrait(Trait.Crossbow)))
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
                                if (action.HasTrait(Trait.Strike) && action.HasTrait(Trait.Ranged) && !action.HasTrait(Trait.TwoHanded) && (action.HasTrait(Trait.Firearm) || action.HasTrait(Trait.Crossbow)) && defender == attackedCreature)
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
                        CombatAction pistolTwirlAction = new CombatAction(self.Owner, new SideBySideIllustration(item.Illustration, IllustrationName.Feint), "Pistol Twirl", [Trait.Basic], pistolTwirlFeat.RulesText, Target.Ranged(item.WeaponProperties.RangeIncrement)).WithActionCost(1).WithItem(item)
                        .WithActiveRollSpecification(new ActiveRollSpecification(TaggedChecks.SkillCheck(Skill.Deception), Checks.DefenseDC(Defense.Perception)))
                        .WithEffectOnEachTarget(async delegate (CombatAction pistolTwirl, Creature attacker, Creature defender, CheckResult result)
                        {
                            switch (result)
                            {
                                case CheckResult.CriticalSuccess:
                                    defender.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtEndOfSourcesTurn)
                                    {
                                        Name = "Pistol Twirl",
                                        Source = attacker,
                                        CannotExpireThisTurn = true,
                                        Illustration = item.Illustration,
                                        Description = "Falt-footed against " + attacker.Name + "'s next melee or ranged attack",
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
                                    defender.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtEndOfSourcesTurn)
                                    {
                                        Name = "Pistol Twirl",
                                        Source = attacker,
                                        Illustration = item.Illustration,
                                        Description = "Falt-footed against " + attacker.Name + "'s next melee or ranged attack",
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
                                // HACK: Hotfix for null ref StrikeRules error
                                riskyReloadAction.StrikeModifiers = basicStrike.StrikeModifiers;
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
                fakeOutEffect.StartOfYourPrimaryTurn = async (QEffect startOfTurn, Creature self) =>
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
                if (creature.OwningFaction == null || creature.OwningFaction.IsPlayer)
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
                                    if (action.HasTrait(Trait.Attack) && !action.Name.StartsWith("Aid Strike") && ally.HasLineOfEffectTo(defender.Occupies) < CoverKind.Blocked && ally.DistanceTo(defender) <= mainWeapon.WeaponProperties?.RangeIncrement && await creature.Battle.AskToUseReaction(ally, "Make an attack roll to Aid the triggering attack." + fakeOutTargetTextAddition))
                                    {
                                        CombatAction aidStrike = ally.CreateStrike(mainWeapon);
                                        aidStrike.ActionCost = 0;
                                        aidStrike.WithExtraTrait(FirearmTraits.IgnoreDoubleBarrel);
                                        aidStrike.ProjectileKind = ProjectileKind.None;
                                        aidStrike.WithSoundEffect(ally.HasTrait(Trait.Female) ? SfxName.Intimidate : SfxName.MaleIntimidate);
                                        // HACK: Since Aid is not in base DD, this name should not be changed for mod support
                                        aidStrike.Name = "Aid Strike (" + mainWeapon.Name + ")";
                                        aidStrike.Traits.Add(Trait.ReactiveAttack);
                                        aidStrike.Item = mainWeapon;
                                        aidStrike.ChosenTargets = action.ChosenTargets;
                                        aidStrike.StrikeModifiers.AdditionalBonusesToAttackRoll = [(creaturesAttacked.Contains(defender)) ? new Bonus(1, BonusType.Circumstance, "Attacked last round") : null];
                                        aidStrike.WithActiveRollSpecification(new ActiveRollSpecification(Checks.Attack(mainWeapon, ally.Actions.AttackedThisManyTimesThisTurn), Checks.FlatDC((PlayerProfile.Instance.IsBooleanOptionEnabled("MoreBasicActions.AidDCIs15") ? 15 : 20))));
                                        aidStrike.EffectOnOneTarget = null;
                                        aidStrike.EffectOnChosenTargets = null;
                                        aidStrike.WithEffectOnEachTarget(async delegate (CombatAction aidAction, Creature attacker, Creature defender, CheckResult result)
                                        {
                                            QEffect createAidQEffect(CheckResult checkResult, Proficiency proficiency)
                                            {
                                                int bonusAmount = 0;
                                                switch (result)
                                                {
                                                    case CheckResult.CriticalSuccess:
                                                        if (proficiency == Proficiency.Legendary)
                                                        {
                                                            bonusAmount = 4;
                                                        }
                                                        else if (proficiency == Proficiency.Master)
                                                        {
                                                            bonusAmount = 3;
                                                        }
                                                        else
                                                        {
                                                            bonusAmount = 2;
                                                        }
                                                        break;
                                                    case CheckResult.Success:
                                                        bonusAmount = 1;
                                                        break;
                                                    case CheckResult.CriticalFailure:
                                                        bonusAmount = -1;
                                                        break;
                                                }


                                                return new QEffect("Aid", $"You gain a {(bonusAmount < 1 ? "-" : "+")}{bonusAmount} Circumstance bonus to the next attack roll.")
                                                {
                                                    BonusToAttackRolls = (QEffect bonusToAttackRoll, CombatAction action, Creature? creature) =>
                                                    {
                                                        return new Bonus(bonusAmount, BonusType.Circumstance, "Aid", bonusAmount >= 0);
                                                    },
                                                    AfterYouMakeAttackRoll = (QEffect afterAttackRoll, CheckBreakdownResult result) =>
                                                    {
                                                        afterAttackRoll.ExpiresAt = ExpirationCondition.Immediately;
                                                    },
                                                    Illustration = IllustrationName.GenericCombatManeuver
                                                };
                                            }

                                            // Depending on the attacks result the original attacker gains a bonus
                                            beforeAttackRoll.Owner.AddQEffect(createAidQEffect(result, beforeAttackRoll.Owner.Proficiencies.Get(mainWeapon.Traits)));
                                        });


                                        //// Builds the strike for the aid strike
                                        //CombatAction aidStrike = new CombatAction(ally, new SimpleIllustration(IllustrationName.None), "Aid Strike (" + mainWeapon.Name + ")", [Trait.Ranged], "{b}Critical Success{/b} Your ally gains a +2 circumstance bonus to the triggering action.\n\n\"{b}Success{/b} Your ally gains a +1 circumstance bonus to the triggering action.\n\n\"{b}Critical Failure{/b} Your ally gains a -1 circumstance penalty to the triggering action.\n\n", Target.Ranged(mainWeapon.WeaponProperties.MaximumRange));
                                        //aidStrike.ActionCost = 0;
                                        //aidStrike.Item = mainWeapon;
                                        //aidStrike.Traits.CombatAction = aidStrike;
                                        //aidStrike.ChosenTargets = action.ChosenTargets;
                                        //CalculatedNumberProducer attackCheck = Checks.Attack(mainWeapon);
                                        //attackCheck.WithExtraBonus((Func<CombatAction, Creature, Creature?, Bonus?>)((combatAction, demoralizer, target) => ((creaturesAttacked.Contains(defender)) ? new Bonus(1, BonusType.Circumstance, "Attacked last round") : (Bonus)null)));
                                        //aidStrike.WithActiveRollSpecification(new ActiveRollSpecification(attackCheck, Checks.FlatDC(15)));
                                        //aidStrike.WithEffectOnEachTarget(async delegate (CombatAction aidAction, Creature attacker, Creature defender, CheckResult result)
                                        //{
                                        //    // Depending on the attacks result the original attacker gains a bonus
                                        //    switch (result)
                                        //    {
                                        //        case CheckResult.CriticalSuccess:
                                        //            beforeAttackRoll.Owner.AddQEffect(new QEffect(ExpirationCondition.Never)
                                        //            {
                                        //                BonusToAttackRolls = (QEffect bonusToAttackRoll, CombatAction action, Creature? creature) =>
                                        //                {
                                        //                    return new Bonus(2, BonusType.Circumstance, "Aid", true);
                                        //                }
                                        //            });
                                        //            break;
                                        //        case CheckResult.Success:
                                        //            beforeAttackRoll.Owner.AddQEffect(new QEffect(ExpirationCondition.Never)
                                        //            {
                                        //                BonusToAttackRolls = (QEffect bonusToAttackRoll, CombatAction action, Creature? creature) =>
                                        //                {
                                        //                    return new Bonus(1, BonusType.Circumstance, "Aid", true);
                                        //                }
                                        //            });
                                        //            break;
                                        //        case CheckResult.CriticalFailure:
                                        //            beforeAttackRoll.Owner.AddQEffect(new QEffect(ExpirationCondition.Never)
                                        //            {
                                        //                BonusToAttackRolls = (QEffect bonusToAttackRoll, CombatAction action, Creature? creature) =>
                                        //                {
                                        //                    return new Bonus(-1, BonusType.Circumstance, "Aid", false);
                                        //                }
                                        //            });
                                        //            break;
                                        //    }
                                        //});

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
        /// Adds the logic for the Drifter's Juke feat
        /// </summary>
        /// <param name="driftersJukeFeat">The Drifter's Juke true feat object</param>
        private static void AddDriftersJukeLogic(TrueFeat driftersJukeFeat)
        {
            // Provides the effect that adds the Drifter's Juke as a main action
            driftersJukeFeat.WithPermanentQEffect("Step, Strike, Step, Strike (One Melee Strike, One Ranged Strike)", delegate (QEffect self)
            {
                self.ProvideMainAction = (QEffect driftersJukeEffect) =>
                {
                    Creature owner = driftersJukeEffect.Owner;

                    // Hides the action if the requirements of holding a loaded firearm and a melee weapon or empty
                    if (owner.HeldItems.Count(item => FirearmUtilities.IsItemFirearmOrCrossbow(item) && FirearmUtilities.IsItemLoaded(item) && !item.HasTrait(FirearmTraits.Misfired) && item.WeaponProperties != null) != 1)
                    {
                        return null;
                    }

                    Item firearm = FirearmUtilities.IsItemFirearmOrCrossbow(owner.HeldItems[0]) ? owner.HeldItems[0] : owner.HeldItems[1];
                    Item otherWeapon = (owner.HeldItems.Count > 1) ? (owner.HeldItems[0] == firearm) ? owner.HeldItems[1] : owner.HeldItems[0] : owner.UnarmedStrike;

                    if (firearm.HasTrait(FirearmTraits.Bayonet) && firearm.Tag != null && firearm.Tag is Item bayonet)
                    {
                        otherWeapon = bayonet;
                    }

                    if (!otherWeapon.HasTrait(Trait.Melee))
                    {
                        return null;
                    }

                    // Returns the main action
                    return new ActionPossibility(new CombatAction(driftersJukeEffect.Owner, new SideBySideIllustration(firearm.Illustration, otherWeapon.Illustration), "Drifter's Juke", [Trait.Flourish], "{b}Requirements{/b} You're wielding a firearm or crossbow in one hand, and your other hand is either wielding a melee weapon or is empty.\n\nYou may Step, make a Strike, Step, and make another Strike (Each is opptional). One Strike must be a ranged Strike using your firearm or crossbow, and the other must be a melee Strike using your melee weapon or unarmed attack.", Target.Self())
                        .WithActionCost(2)
                        .WithEffectOnSelf(async (CombatAction action, Creature innerSelf) =>
                        {
                            // A helper method to handle the strikes and steps
                            async Task HandleStrikes(Item weapon)
                            {
                                // Adds the weapon to useage
                                CombatAction strike = innerSelf.CreateStrike(weapon).WithActionCost(0);
                                List<Option> possibilities = new List<Option>();
                                GameLoop.AddDirectUsageOnCreatureOptions(strike, possibilities, true);

                                // Creates the possibilites and prompts for user selection
                                if (possibilities.Count > 0)
                                {
                                    Option chosenOption;
                                    if (possibilities.Count >= 2)
                                    {
                                        var result = await innerSelf.Battle.SendRequest(new AdvancedRequest(innerSelf, "Choose a creature to Strike.", possibilities)
                                        {
                                            TopBarText = "Choose a creature to Strike.",
                                            TopBarIcon = weapon.Illustration
                                        });
                                        chosenOption = result.ChosenOption;
                                    }
                                    else
                                    {
                                        chosenOption = possibilities[0];
                                    }
                                    await chosenOption.Action();
                                }
                            }

                            // Steps then, Strikes with a choice, then Step then strike with the remianing choice
                            bool didMove = await innerSelf.StrideAsync("Choose where to step.", allowStep: true, maximumFiveFeet: true, allowPass: true, allowCancel: true);
                            if (didMove)
                            {
                                ChoiceButtonOption choice = await innerSelf.AskForChoiceAmongButtons(IllustrationName.QuestionMark, "Choose which Strike to use first.", firearm.BaseItemName.HumanizeTitleCase2(), otherWeapon.BaseItemName.HumanizeTitleCase2(), "Pass");
                                Item remainingWeapon = firearm;
                                if (choice.Index != 2)
                                {
                                    Item weapon = (choice.Index == 0) ? firearm : otherWeapon;
                                    remainingWeapon = (choice.Index == 0) ? otherWeapon : firearm;
                                    await HandleStrikes(weapon);
                                }
                                await innerSelf.StrideAsync("Choose where to step.", allowStep: true, maximumFiveFeet: true, allowPass: true, allowCancel: false);
                                if (await innerSelf.AskForConfirmation(remainingWeapon.Illustration, "Make a strike?", "Yes"))
                                {
                                    await HandleStrikes(remainingWeapon);
                                }
                            }
                            else
                            {
                                action.RevertRequested = true;
                            }
                        }));
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Pistolero's Challenge feat
        /// </summary>
        /// <param name="pistolerosChallengeFeat">The Pistolero's Challenge true feat object</param>
        private static void AddPistolerosChallengeLogic(TrueFeat pistolerosChallengeFeat)
        {
            // Provides the effect that adds the Pistolero's Challenge as a main action
            pistolerosChallengeFeat.WithPermanentQEffect("Callenge an emeny to gain increased damage against them.", delegate (QEffect self)
            {
                self.ProvideMainAction = (QEffect pistolerosChallengeEffect) =>
                {
                    PossibilitySection challengeSection = new PossibilitySection("Challenge");

                    CombatAction CreateChallengeAction(string actionName, Trait skill)
                    {
                        return new CombatAction(pistolerosChallengeEffect.Owner, IllustrationName.GenericCombatManeuver, actionName, [Trait.Auditory, Trait.Flourish, Trait.Linguistic, Trait.Mental], "Make a " + actionName + " check against a creature within 30 feet against their Will DC. The target will be immune to this for the rest of the encounter. If you succeed, they are your challenged foe, and you can only have 1 challenged foe at a time.\n\n{b}Success{/b} You and the target gain a +2 status bonus to damage rolls with Strikes against each other. If you are a master " + actionName + ", the bonus increases to +3, and if you're legendary it is instead +4.\n{b}Critical Failure{/b} You become frightened 1 and can't use this again for 1 minute.", Target.Ranged(30))
                        .WithActionCost(1)
                        .WithActionId(GunslingerActionIDs.PistolerosChallenge)
                        .WithEffectOnEachTarget(async delegate (CombatAction challenge, Creature attacker, Creature defender, CheckResult result)
                        {
                            if (result >= CheckResult.Success)
                            {
                                QEffect createPistolerosChallenge(Creature foe, int additionalBonus = 0)
                                {
                                    return new QEffect()
                                    {
                                        Name = $"Pistolero's Challenge ({foe.Name})",
                                        Description = $"You have a +{2 + additionalBonus} status bonus to damage with Strikes against {foe.Name}",
                                        Illustration = IllustrationName.GenericCombatManeuver,
                                        Id = GunslingerQEIDs.PistolerosChallenge,
                                        Tag = foe,
                                        BonusToDamage = (QEffect _, CombatAction action, Creature _) =>
                                        {
                                            if (action.HasTrait(Trait.Strike) && action.ChosenTargets.ChosenCreature == foe)
                                            {
                                                return new Bonus(2 + additionalBonus, BonusType.Circumstance, "Pistolero's Challenge", true);
                                            }

                                            return null;
                                        },
                                        YouBeginAction = async (QEffect qfYouTakeAction, CombatAction action) =>
                                        {
                                            if (action.ActionId == GunslingerActionIDs.PistolerosChallenge)
                                            {
                                                qfYouTakeAction.ExpiresAt = ExpirationCondition.Immediately;
                                            }
                                        }
                                    };
                                }

                                Proficiency proficiency = attacker.Proficiencies.Get(skill);
                                int additionalAttackerBonus = 0;
                                if (proficiency >= Proficiency.Master)
                                {
                                    additionalAttackerBonus = (proficiency == Proficiency.Legendary) ? 2 : 1;
                                }

                                QEffect attackersChallenge = createPistolerosChallenge(defender, additionalAttackerBonus);
                                QEffect defendersChallenge = createPistolerosChallenge(attacker);

                                attackersChallenge.WhenExpires = (QEffect qfExpires) =>
                                {
                                    if (qfExpires.Tag is Creature foe)
                                    {
                                        QEffect? foesChallenge = foe.FindQEffect(GunslingerQEIDs.PistolerosChallenge);
                                        if (foesChallenge != null)
                                        {
                                            foesChallenge.ExpiresAt = ExpirationCondition.Immediately;
                                        }
                                    }
                                };
                                defendersChallenge.WhenExpires = (QEffect qfExpires) =>
                                {
                                    if (qfExpires.Tag is Creature foe)
                                    {
                                        QEffect? foesChallenge = foe.FindQEffect(GunslingerQEIDs.PistolerosChallenge);
                                        if (foesChallenge != null)
                                        {
                                            foesChallenge.ExpiresAt = ExpirationCondition.Immediately;
                                        }
                                    }
                                };

                                attacker.AddQEffect(attackersChallenge);
                                defender.AddQEffect(defendersChallenge);
                            }
                            else if (result == CheckResult.CriticalFailure)
                            {
                                attacker.AddQEffect(QEffect.Frightened(1));
                                attacker.AddQEffect(new QEffect(ExpirationCondition.CountsDownAtEndOfYourTurn)
                                {
                                    Value = 10,
                                    PreventTakingAction = (CombatAction action) =>
                                    {
                                        if (action.ActionId == GunslingerActionIDs.PistolerosChallenge)
                                        {
                                            return $"You can not use {action.Name} for 1 minute after a Critical Failure";
                                        }

                                        return null;
                                    }
                                });
                            }
                        });
                    }

                    challengeSection.AddPossibility(new ActionPossibility(CreateChallengeAction("Deception", Trait.Deception)));
                    challengeSection.AddPossibility(new ActionPossibility(CreateChallengeAction("Intimidation", Trait.Intimidation)));
                    SubmenuPossibility pistolerosChallengeMenu = new SubmenuPossibility(IllustrationName.GenericCombatManeuver, "Pistolero's Challenge");
                    pistolerosChallengeMenu.Subsections.Add(challengeSection);
                    return pistolerosChallengeMenu;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Sniper's Aim feat
        /// </summary>
        /// <param name="snipersAimFeat">The Sniper's Aim true feat object</param>
        private static void AddSnipersAimLogic(TrueFeat snipersAimFeat)
        {
            // Provides the effect that adds the Sniper's Aim as a strike modifier
            snipersAimFeat.WithPermanentQEffect("Make a ranged Strike with a +2 circumstance bonus and ignoring concealment.", delegate (QEffect self)
            {
                self.ProvideStrikeModifier = (Item item) =>
                {
                    if (item.HasTrait(Trait.Ranged))
                    {
                        Creature owner = self.Owner;
                        CombatAction snipersAimAction = owner.CreateStrike(item);
                        snipersAimAction.Name = $"Sniper's Aim ({item.Name})";
                        snipersAimAction.Traits.AddRange([Trait.Concentrate, Trait.UnaffectedByConcealment, FirearmTraits.IgnoreKickbackPenalty]);
                        snipersAimAction.WithActionCost(2);
                        snipersAimAction.Illustration = new SideBySideIllustration(item.Illustration, IllustrationName.GenericCombatManeuver);
                        snipersAimAction.Item = item;
                        snipersAimAction.Description = StrikeRules.CreateBasicStrikeDescription2(snipersAimAction.StrikeModifiers, additionalAttackRollText: "You gain a +2 circumstance bonus to this Strike's attack roll and ignore the target's concealment. If you're using a kickback firearm, you will take no circumstance penalty if you do not meet the Strength requirement.");
                        snipersAimAction.StrikeModifiers.AdditionalBonusesToAttackRoll = [new Bonus(2, BonusType.Circumstance, "Sniper's Aim")];

                        // Checks if the item needs to be reloaded
                        ((CreatureTarget)snipersAimAction.Target).WithAdditionalConditionOnTargetCreature((Creature attacker, Creature defender) =>
                        {
                            if (FirearmUtilities.IsItemFirearmOrCrossbow(item) && !FirearmUtilities.IsItemLoaded(item))
                            {
                                return Usability.NotUsable("Needs to be reloaded.");
                            }

                            return Usability.Usable;
                        });

                        return snipersAimAction;
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Phalanx Breaker feat
        /// </summary>
        /// <param name="phalanxBreakerFeat">The Phalanx Breaker true feat object</param>
        private static void AddPhalanxBreakerLogic(TrueFeat phalanxBreakerFeat)
        {
            // Provides the effect that adds the Phalanx Breaker as a main action
            phalanxBreakerFeat.WithPermanentQEffect("Make a ranged Strike that pushes the target back and deals damage if the target hits a obsticle.", delegate (QEffect self)
            {
                self.ProvideStrikeModifier = (Item item) =>
                {
                    if (FirearmUtilities.IsItemFirearmOrCrossbow(item) && item.HasTrait(Trait.TwoHanded) && item.WeaponProperties != null)
                    {
                        Creature owner = self.Owner;
                        CombatAction phalanxBreakerAction = owner.CreateStrike(item);
                        phalanxBreakerAction.Name = $"Phalanx Breaker ({item.Name})";
                        phalanxBreakerAction.WithActionCost(2);
                        phalanxBreakerAction.Illustration = new SideBySideIllustration(item.Illustration, IllustrationName.GenericCombatManeuver);
                        phalanxBreakerAction.Item = item;
                        phalanxBreakerAction.Description = StrikeRules.CreateBasicStrikeDescription2(phalanxBreakerAction.StrikeModifiers, additionalAttackRollText: "This strike must be within the weapon's first range increment.", additionalSuccessText: "The target is pushed directly back 10 feet.", additionalCriticalSuccessText: "The target is pushed directly back 20 feet.", additionalAftertext: "If the target is pushed into a obsticle, the target takes bludgeoning damage equal to half your level.");
                        phalanxBreakerAction.Target = Target.Ranged(item.WeaponProperties.RangeIncrement); // TODO: Check
                        phalanxBreakerAction.StrikeModifiers.OnEachTarget = async (Creature attacker, Creature defender, CheckResult result) =>
                        {
                            if (!defender.WeaknessAndResistance.ImmunityToForcedMovement)
                            {
                                if (result >= CheckResult.Success)
                                {
                                    int distanceToMove = (result == CheckResult.CriticalSuccess) ? 4 : 2;
                                    Tile startingTile = defender.Occupies;
                                    await attacker.PushCreature(defender, distanceToMove);
                                    if (startingTile.DistanceTo(defender.Occupies) < distanceToMove)
                                    {
                                        await CommonSpellEffects.DealDirectDamage(null, DiceFormula.FromText($"{Math.Floor(attacker.Level / 2.0)}"), defender, result, DamageKind.Bludgeoning);
                                    }
                                }
                            }
                        };

                        // Checks if the item needs to be reloaded
                        ((CreatureTarget)phalanxBreakerAction.Target).WithAdditionalConditionOnTargetCreature((Creature attacker, Creature defender) =>
                        {
                            if (!FirearmUtilities.IsItemLoaded(item))
                            {
                                return Usability.NotUsable("Needs to be reloaded.");
                            }

                            return Usability.Usable;
                        });

                        return phalanxBreakerAction;
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Advanced Shooter feat
        /// </summary>
        /// <param name="advancedShooterFeat">The Advanced Shooter true feat object</param>
        private static void AddAdvancedShooterLogic(TrueFeat advancedShooterFeat)
        {
            // Provides the effect that adds the Advanced Shooter as a main action
            advancedShooterFeat.WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                sheet.AddSelectionOption(new SingleFeatSelectionOption("Advanced Shooter Choice", "Advanced Shooter", 6, feat => feat.FeatName == GunslingerFeatNames.AdvancedShooterFirearm || feat.FeatName == GunslingerFeatNames.AdvancedShooterCrossbow));
            });
        }

        /// <summary>
        /// Adds the logic for the Cauterize feat
        /// </summary>
        /// <param name="phalanxBreakerFeat">The Cauterize true feat object</param>
        private static void AddCauterizeLogic(TrueFeat cauterizeFeat)
        {
            // Provides the effect that adds the Cauterize as a strike modifier
            cauterizeFeat.WithPermanentQEffect("Make a ranged Strike then give an ally with persistent bleed an aided recovery check.", delegate (QEffect self)
            {
                self.ProvideStrikeModifier = (Item item) =>
                {
                    if (item.HasTrait(Trait.Firearm))
                    {
                        bool IsAnAllyAdjacentAndBleeding(Creature self, Creature ally)
                        {
                            return self.FriendOf(ally) && (self == ally || self.IsAdjacentTo(ally)) && ally.QEffects.Any(qf => qf.Id == QEffectId.PersistentDamage && qf.Key != null && qf.Key.ContainsIgnoreCase("bleed"));
                        }

                        Creature owner = self.Owner;
                        CombatAction cauterizeAction = owner.CreateStrike(item);
                        cauterizeAction.Name = $"Cauterize ({item.Name})";
                        cauterizeAction.Traits.AddRange([Trait.Flourish, FirearmTraits.IgnoreKickbackPenalty]);
                        cauterizeAction.WithActionCost(1);
                        cauterizeAction.Illustration = new SideBySideIllustration(item.Illustration, IllustrationName.PersistentBleed);
                        cauterizeAction.Item = item;
                        cauterizeAction.Description = StrikeRules.CreateBasicStrikeDescription2(cauterizeAction.StrikeModifiers, additionalAftertext: "You then press the heated barrel to the wounds of you or an ally within reach that is taking persistent bleed damage, giving an immediate flat check to end the bleed with the lower DC for particularly effective assistance.");
                        cauterizeAction.StrikeModifiers.OnEachTarget = async (Creature attacker, Creature defender, CheckResult result) =>
                        {
                            List<Option> possibilities = new List<Option>();
                            foreach (Creature ally in attacker.Battle.AllCreatures.Where(creature => IsAnAllyAdjacentAndBleeding(attacker, creature)))
                            {
                                possibilities.Add(Option.ChooseCreature("Assisted Recover Check", ally, async () =>
                                {
                                    CombatAction recoveryCheck = new CombatAction(attacker, IllustrationName.PersistentBleed, "Recovery Check", [Trait.Manipulate, Trait.AttackDoesNotTargetAC, Trait.Basic], "", Target.AdjacentFriendOrSelf())
                                        .WithActionCost(0)
                                        .WithEffectOnEachTarget(async delegate (CombatAction assistedAid, Creature attacker, Creature defender, CheckResult result)
                                        {
                                            defender.QEffects.Where(qf => qf.Id == QEffectId.PersistentDamage && qf.Key!.ContainsIgnoreCase("bleed")).ToList().ForEach(qf => qf.RollPersistentDamageRecoveryCheck(true));
                                        });
                                    await ally.Battle.GameLoop.FullCast(recoveryCheck, ChosenTargets.CreateSingleTarget(ally));
                                }));
                            }

                            // Creates the possibilites and prompts for user selection
                            if (possibilities.Count > 0)
                            {
                                Option chosenOption;
                                if (possibilities.Count >= 2)
                                {
                                    var requestResult = await attacker.Battle.SendRequest(new AdvancedRequest(attacker, "Choose an ally with persistent bleed.", possibilities)
                                    {
                                        TopBarText = "Choose an ally with persistent bleed.",
                                        TopBarIcon = IllustrationName.PersistentBleed
                                    });
                                    chosenOption = requestResult.ChosenOption;
                                }
                                else
                                {
                                    chosenOption = possibilities[0];
                                }
                                await chosenOption.Action();
                            }
                        };

                        // Checks if the item needs to be reloaded
                        ((CreatureTarget)cauterizeAction.Target).WithAdditionalConditionOnTargetCreature((Creature attacker, Creature defender) =>
                        {
                            if (!FirearmUtilities.IsItemLoaded(item))
                            {
                                return Usability.NotUsable("Needs to be reloaded.");
                            }
                            else if (!attacker.Battle.AllCreatures.Any(creature => IsAnAllyAdjacentAndBleeding(attacker, creature)))
                            {
                                return Usability.NotUsable("No adjacent ally with persistent bleed damge.");
                            }

                            return Usability.Usable;
                        });

                        return cauterizeAction;
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Scatter Blast feat
        /// </summary>
        /// <param name="scatterBlastFeat">The Scatter Blast true feat object</param>
        private static void AddScatterBlastLogic(TrueFeat scatterBlastFeat)
        {
            // Provides the effect that adds the Cauterize as a strike modifier
            scatterBlastFeat.WithPermanentQEffect("Make a ranged Strike with a scatter firearm that has an increased scatter range but can misfire or break on failure.", delegate (QEffect self)
            {
                self.ProvideStrikeModifier = (Item item) =>
                {
                    if (item.HasTrait(Trait.Firearm) && item.WeaponProperties != null && (item.HasTrait(FirearmTraits.Scatter5) || item.HasTrait(FirearmTraits.Scatter10)))
                    {
                        Creature owner = self.Owner;
                        Item dupItem = item.Duplicate();
                        dupItem.WeaponProperties!.WithRangeIncrement(dupItem.WeaponProperties!.RangeIncrement + 4);
                        CombatAction scatterBlastAction = owner.CreateStrike(dupItem);
                        scatterBlastAction.Name = $"Scatter Blast ({item.Name})";
                        scatterBlastAction.Traits.Add(FirearmTraits.IgnoreScatter);
                        scatterBlastAction.WithActionCost(2);
                        scatterBlastAction.Illustration = new SideBySideIllustration(item.Illustration, IllustrationName.GenericCombatManeuver);
                        scatterBlastAction.Item = item;
                        scatterBlastAction.Description = StrikeRules.CreateBasicStrikeDescription4(scatterBlastAction.StrikeModifiers, prologueText: "The firearm's range increment increases by 20 feet and the radius of its scatter increases by 20 feet.", additionalFailureText: "The firearm misfires", additionalCriticalFailureText: "The item breaks and is not useable for the remainder of this combat. It also deals its normal weapon damage to all creatures in a 20-foot burst centered on the firearm, with a basic Reflex save against your class DC. This damage includes any from the weapon's fundamental and property runes.");
                        scatterBlastAction.StrikeModifiers.OnEachTarget = async (Creature attacker, Creature defender, CheckResult result) =>
                        {
                            CombatAction CreateScatterExplosion(Creature source, int addedScatterRange)
                            {
                                return new CombatAction(source, IllustrationName.GenericCombatManeuver, "Scatter Damage", [Trait.DoNotShowInCombatLog], "The scatter explosion from Scatter Blast", Target.Emanation(4 + addedScatterRange))
                                    .WithActionCost(0);
                            }
                            if (result >= CheckResult.Success)
                            {
                                int addedScatterRange = (item.HasTrait(FirearmTraits.Scatter5)) ? 1 : 2;
                                CombatAction explosion = CreateScatterExplosion(defender, addedScatterRange);
                                explosion.WithEffectOnEachTarget(async delegate (CombatAction scatterExplosion, Creature attacker, Creature defender, CheckResult result)
                                {
                                    await CommonSpellEffects.DealDirectDamage(scatterExplosion, DiceFormula.FromText($"{item.WeaponProperties.DamageDieCount}"), defender, result, DamageKind.Untyped);
                                });

                                await attacker.Battle.GameLoop.FullCast(explosion);
                            }
                            else if (result == CheckResult.Failure)
                            {
                                item.Traits.Add(FirearmTraits.Misfired);
                            }
                            else if (result == CheckResult.CriticalFailure)
                            {
                                attacker.AddQEffect(new QEffect()
                                {
                                    PreventTakingAction = (CombatAction action) =>
                                    {
                                        if (action.Item != null && action.Item == item)
                                        {
                                            return $"{item.Name} is broken";
                                        }

                                        return null;
                                    }
                                });
                                CombatAction explosion = CreateScatterExplosion(attacker, 0);
                                explosion.WithSavingThrow(new SavingThrow(Defense.Reflex, 2 + attacker.Level + attacker.Abilities.Dexterity));
                                explosion.WithEffectOnEachTarget(async delegate (CombatAction scatterExplosion, Creature attacker, Creature defender, CheckResult result)
                                {
                                    await CommonSpellEffects.DealBasicDamage(scatterExplosion, attacker, defender, result, DiceFormula.FromText(item.WeaponProperties.Damage), item.DetermineDamageKinds()[0]);
                                });
                                EmanationTarget emanationTarget = (EmanationTarget)explosion.Target;
                                AreaSelection areaSelection = Areas.DetermineTiles(emanationTarget);
                                explosion.ChosenTargets.SetFromArea(emanationTarget, areaSelection?.TargetedTiles ?? new HashSet<Tile>());
                                await explosion.AllExecute();
                            }

                            FirearmUtilities.DischargeItem(item);
                        };

                        // Checks if the item needs to be reloaded
                        ((CreatureTarget)scatterBlastAction.Target).WithAdditionalConditionOnTargetCreature((Creature attacker, Creature defender) =>
                        {
                            if (!FirearmUtilities.IsItemLoaded(item))
                            {
                                return Usability.NotUsable("Needs to be reloaded.");
                            }

                            return Usability.Usable;
                        });

                        return scatterBlastAction;
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Stab And Blast feat
        /// </summary>
        /// <param name="scatterBlastFeat">The Stab And Blast true feat object</param>
        private static void AddStabAndBlastLogic(TrueFeat stabAndBlastFeat)
        {
            // Provides the effect that adds the Stab And Blast as a strike modifier
            stabAndBlastFeat.WithPermanentQEffect("Make a melee strike with your Bayonet or Stock then a ranged stike.", delegate (QEffect self)
            {
                self.ProvideStrikeModifier = (Item item) =>
                {
                    if (item is AttachedWeapon attachedWeapon && (item.HasTrait(FirearmTraits.Bayonet) || item.HasTrait(FirearmTraits.ReinforcedStock)))
                    {
                        Item rangedWeapon = attachedWeapon.ItemAttachedTo;

                        Creature owner = self.Owner;
                        CombatAction stabAndBlastAction = owner.CreateStrike(item);
                        stabAndBlastAction.Name = $"Stab and Blast";
                        stabAndBlastAction.Traits.Add(Trait.Flourish);
                        stabAndBlastAction.WithActionCost(1);
                        stabAndBlastAction.Illustration = new SideBySideIllustration(item.Illustration, rangedWeapon.Illustration);
                        stabAndBlastAction.Item = rangedWeapon;
                        stabAndBlastAction.Description = StrikeRules.CreateBasicStrikeDescription3(stabAndBlastAction.StrikeModifiers, additionalSuccessText: "Make a ranged Strike against the same target with a +2 circumstance bonus to the attack roll. This counts as two attacks toward your multiple attack penalty, but you don't apply the multiple attack penalty until after making both attacks.");
                        stabAndBlastAction.StrikeModifiers.OnEachTarget = async (Creature attacker, Creature defender, CheckResult result) =>
                        {
                            if (result >= CheckResult.Success)
                            {
                                CombatAction rangedStrike = attacker.CreateStrike(rangedWeapon);
                                rangedStrike.WithActionCost(0);
                                rangedStrike.StrikeModifiers.QEffectForStrike = new QEffect(ExpirationCondition.EphemeralAtEndOfImmediateAction)
                                {
                                    BonusToAttackRolls = (QEffect qfAttackRoll, CombatAction action, Creature? target) =>
                                    {
                                        return new Bonus(2, BonusType.Circumstance, "Stab and Blast", true);
                                    }
                                };
                                await attacker.MakeStrike(rangedStrike, defender);
                            }
                        };

                        // Checks if the item needs to be reloaded
                        ((CreatureTarget)stabAndBlastAction.Target).WithAdditionalConditionOnTargetCreature((Creature attacker, Creature defender) =>
                        {
                            if (!FirearmUtilities.IsItemLoaded(rangedWeapon))
                            {
                                return Usability.NotUsable("Needs to be reloaded.");
                            }

                            return Usability.Usable;
                        });

                        return stabAndBlastAction;
                    }
                    //if ((item.HasTrait(FirearmTraits.Bayonet) || item.HasTrait(FirearmTraits.ReinforcedStock)) && item.Tag != null && item.Tag is Item rangedWeapon && FirearmUtilities.IsItemFirearmOrCrossbow(rangedWeapon))
                    //{
                    //    Creature owner = self.Owner;
                    //    CombatAction stabAndBlastAction = owner.CreateStrike(item);
                    //    stabAndBlastAction.Name = $"Stab and Blast";
                    //    stabAndBlastAction.Traits.Add(Trait.Flourish);
                    //    stabAndBlastAction.WithActionCost(1);
                    //    stabAndBlastAction.Illustration = new SideBySideIllustration(item.Illustration, rangedWeapon.Illustration);
                    //    stabAndBlastAction.Item = rangedWeapon;
                    //    stabAndBlastAction.Description = StrikeRules.CreateBasicStrikeDescription3(stabAndBlastAction.StrikeModifiers, additionalSuccessText: "Make a ranged Strike against the same target with a +2 circumstance bonus to the attack roll. This counts as two attacks toward your multiple attack penalty, but you don't apply the multiple attack penalty until after making both attacks.");
                    //    stabAndBlastAction.StrikeModifiers.OnEachTarget = async (Creature attacker, Creature defender, CheckResult result) =>
                    //    {
                    //        if (result >= CheckResult.Success)
                    //        {
                    //            CombatAction meleeStrike = attacker.CreateStrike(rangedWeapon);
                    //            meleeStrike.WithActionCost(0);
                    //            meleeStrike.StrikeModifiers.QEffectForStrike = new QEffect(ExpirationCondition.EphemeralAtEndOfImmediateAction)
                    //            {
                    //                BonusToAttackRolls = (QEffect qfAttackRoll, CombatAction action, Creature? target) =>
                    //                {
                    //                    return new Bonus(2, BonusType.Circumstance, "Stab and Blast", true);
                    //                }
                    //            };
                    //            attacker.Actions.AttackedThisManyTimesThisTurn--;
                    //            await attacker.MakeStrike(meleeStrike, defender);
                    //            attacker.Actions.AttackedThisManyTimesThisTurn++;
                    //        }
                    //    };

                    //    // Checks if the item needs to be reloaded
                    //    ((CreatureTarget)stabAndBlastAction.Target).WithAdditionalConditionOnTargetCreature((Creature attacker, Creature defender) =>
                    //    {
                    //        if (!FirearmUtilities.IsItemLoaded(rangedWeapon))
                    //        {
                    //            return Usability.NotUsable("Needs to be reloaded.");
                    //        }

                    //        return Usability.Usable;
                    //    });

                    //    return stabAndBlastAction;
                    //}

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Scatter Blast feat
        /// </summary>
        /// <param name="scatterBlastFeat">The Scatter Blast true feat object</param>
        private static void AddSmokeCurtainLogic(TrueFeat scatterBlastFeat)
        {
            // Provides the effect that adds the Scatter Blast as a strike modifier
            scatterBlastFeat.WithPermanentQEffect("Make a ranged Strike and create a cloud of smoke around yourself.", delegate (QEffect self)
            {
                self.ProvideStrikeModifier = (Item item) =>
                {
                    if (item.HasTrait(Trait.Firearm))
                    {
                        Creature owner = self.Owner;
                        CombatAction scatterBlastAction = owner.CreateStrike(item);
                        scatterBlastAction.Name = $"Smoke Curtain ({item.Name})";
                        scatterBlastAction.WithActionCost(2);
                        scatterBlastAction.Illustration = new SideBySideIllustration(item.Illustration, IllustrationName.GenericCombatManeuver);
                        scatterBlastAction.Item = item;
                        scatterBlastAction.Description = StrikeRules.CreateBasicStrikeDescription4(scatterBlastAction.StrikeModifiers, prologueText: "Create a cloud of smoke in a 20-foot emanation centered on your location. Creatures are concealed while within the smoke. The smoke dissipates at the start of your next turn.", additionalCriticalFailureText: "The firearm misfires");
                        scatterBlastAction.StrikeModifiers.OnEachTarget = async (Creature attacker, Creature defender, CheckResult result) =>
                        {
                            Tile origin = attacker.Occupies;
                            List<TileQEffect> effectsToRemoveAtStartOfTurn = new List<TileQEffect>();
                            foreach (Tile tile in attacker.Battle.Map.AllTiles.Where(tile => tile.HasLineOfEffectToIgnoreLesser(origin) < CoverKind.Blocked && tile.DistanceTo(origin) <= 4))
                            {
                                TileQEffect tileQEffect = new TileQEffect()
                                {
                                    StateCheck = source =>
                                    {
                                        source.Owner.FoggyTerrain = true;
                                    }
                                };
                                effectsToRemoveAtStartOfTurn.Add(tileQEffect);
                                tile.AddQEffect(tileQEffect);
                            }
                            attacker.AddQEffect(new QEffect()
                            {
                                StartOfYourPrimaryTurn = async (QEffect startOfTurn, Creature me) =>
                                {
                                    foreach (TileQEffect tileEffect in effectsToRemoveAtStartOfTurn)
                                    {
                                        tileEffect.ExpiresAt = ExpirationCondition.Immediately;
                                    }
                                    startOfTurn.ExpiresAt = ExpirationCondition.Immediately;
                                }
                            });
                            if (result == CheckResult.CriticalFailure)
                            {
                                item.Traits.Add(FirearmTraits.Misfired);
                            }
                        };

                        // Checks if the item needs to be reloaded
                        ((CreatureTarget)scatterBlastAction.Target).WithAdditionalConditionOnTargetCreature((Creature attacker, Creature defender) =>
                        {
                            if (!FirearmUtilities.IsItemLoaded(item))
                            {
                                return Usability.NotUsable("Needs to be reloaded.");
                            }

                            return Usability.Usable;
                        });

                        return scatterBlastAction;
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Leap and Fire feat
        /// </summary>
        /// <param name="leapAndFireFeat">The Leap and Fire true feat object</param>
        private static void AddLeapAndFireLogic(TrueFeat leapAndFireFeat)
        {
            // Provides the effect that adds the Leap and Fire as a strike modifier
            leapAndFireFeat.WithPermanentQEffect("Make a ranged Strike when you Hit the Dirt!", delegate (QEffect self)
            {
            });
        }

        /// <summary>
        /// Adds the logic for the Grit and Tenacity feat
        /// </summary>
        /// <param name="gritAndTenacityFeat">The Grit and Tenacity true feat object</param>
        private static void AddGritAndTenacityLogic(TrueFeat gritAndTenacityFeat)
        {
            // Provides the effect that adds the Grit and Tenacity as a strike modifier
            gritAndTenacityFeat.WithPermanentQEffect("When you fail a Fortitude or Will save you can reroll with a +2 circumstance bonus.", delegate (QEffect self)
            {
                self.Tag = true;
                self.RerollSavingThrow = async (QEffect reroll, CheckBreakdownResult result, CombatAction action) =>
                {
                    if ((action.SavingThrow?.Defense == Defense.Fortitude || action.SavingThrow?.Defense == Defense.Will) && result.CheckResult <= CheckResult.Failure && reroll.Tag is bool canUse && canUse)
                    {
                        Creature owner = reroll.Owner;
                        if (await owner.AskToUseReaction("You rolled a " + result.CheckResult.HumanizeLowerCase2() + "against " + action + ". Use Grit and Tenacity to reroll the save with a +2 circumstance bonus?"))
                        {
                            owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                            {
                                BonusToDefenses = (QEffect bonusToSave, CombatAction? action, Defense defense) =>
                                {
                                    return new Bonus(2, BonusType.Circumstance, "Grit and Tenacity", true);
                                }
                            });

                            return RerollDirection.RerollAndKeepSecond;
                        }
                    }

                    return RerollDirection.DoNothing;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Bullet Split feat
        /// </summary>
        /// <param name="bulletSplitFeat">The Bullet Split true feat object</param>
        private static void AddBulletSplitLogic(TrueFeat bulletSplitFeat)
        {
            // Provides the effect that adds the Bullet Split as a main action
            bulletSplitFeat.WithPermanentQEffect("Make a ranged strike against two adjacent enemies with a -2 circumstance penality.", delegate (QEffect self)
            {
                self.ProvideStrikeModifier = (Item item) =>
                {
                    if (FirearmUtilities.IsItemFirearmOrCrossbow(item))
                    {
                        Creature owner = self.Owner;
                        CombatAction bulletSplitAction = owner.CreateStrike(item);
                        bulletSplitAction.Name = $"Bullet Split ({item.Name})";
                        bulletSplitAction.WithActionCost(1);
                        bulletSplitAction.Illustration = new SideBySideIllustration(item.Illustration, IllustrationName.GenericCombatManeuver);
                        bulletSplitAction.Item = item;
                        bulletSplitAction.Description = StrikeRules.CreateBasicStrikeDescription3(bulletSplitAction.StrikeModifiers, prologueText: "Make two Strikes, one each against two separate targets. The targets must be adjacent to each other and within your weapon's maximum range. Each of these attacks takes a –2 penalty to the attack roll, but the two count as only one attack when calculating your multiple attack penalty.");
                        bulletSplitAction.StrikeModifiers.QEffectForStrike = new QEffect(ExpirationCondition.Ephemeral)
                        {
                            BonusToAttackRolls = (QEffect attackRoll, CombatAction action, Creature? defender) =>
                            {
                                return new Bonus(-2, BonusType.Circumstance, "Bullet Split", false);
                            }
                        };
                        bulletSplitAction.StrikeModifiers.OnEachTarget = async (Creature attacker, Creature defender, CheckResult result) =>
                        {
                            Item duplicatedItem = item.Duplicate();
                            // Adds the weapon to useage
                            CombatAction strike = attacker.CreateStrike(duplicatedItem).WithActionCost(0);
                            ((CreatureTarget)strike.Target).WithAdditionalConditionOnTargetCreature((Creature a, Creature d) =>
                            {
                                if (d == defender || !defender.IsAdjacentTo(d))
                                {
                                    return Usability.NotUsableOnThisCreature("not adjacent");
                                }

                                return Usability.Usable;
                            });
                            strike.StrikeModifiers.QEffectForStrike = new QEffect(ExpirationCondition.Ephemeral)
                            {
                                BonusToAttackRolls = (QEffect attackRoll, CombatAction action, Creature? defender) =>
                                {
                                    return new Bonus(-2, BonusType.Circumstance, "Bullet Split", false);
                                }
                            };
                            List<Option> possibilities = new List<Option>();
                            GameLoop.AddDirectUsageOnCreatureOptions(strike, possibilities, true);

                            // Creates the possibilites and prompts for user selection
                            if (possibilities.Count > 0)
                            {
                                Option chosenOption;
                                if (possibilities.Count >= 2)
                                {
                                    var requestResult = await attacker.Battle.SendRequest(new AdvancedRequest(attacker, "Choose a creature to Strike.", possibilities)
                                    {
                                        TopBarText = "Choose a creature to Strike.",
                                        TopBarIcon = duplicatedItem.Illustration
                                    });
                                    chosenOption = requestResult.ChosenOption;
                                }
                                else
                                {
                                    chosenOption = possibilities[0];
                                }

                                attacker.Actions.AttackedThisManyTimesThisTurn--;
                                await chosenOption.Action();
                                attacker.Actions.AttackedThisManyTimesThisTurn++;
                            }
                        };

                        // Checks if the item needs to be reloaded
                        ((CreatureTarget)bulletSplitAction.Target).WithAdditionalConditionOnTargetCreature((Creature attacker, Creature defender) =>
                        {
                            if (!FirearmUtilities.IsItemLoaded(item))
                            {
                                return Usability.NotUsable("Needs to be reloaded.");
                            }
                            else if (!owner.HeldItems.Any(heldItem => heldItem != item && (heldItem.WeaponProperties != null && (heldItem.WeaponProperties.DamageKind == DamageKind.Slashing || heldItem.HasTrait(Trait.VersatileS)) || heldItem.HasTrait(FirearmTraits.Bayonet))))
                            {
                                return Usability.NotUsable("You must be holding a Slashing, Versatile S or Bayonet weapon in the other hand.");
                            }
                            else if (!attacker.Battle.AllCreatures.Any(creature => defender != creature && !attacker.FriendOf(creature) && creature.IsAdjacentTo(defender)))
                            {
                                return Usability.NotUsableOnThisCreature("not adjacent");
                            }

                            return Usability.Usable;
                        });

                        return bulletSplitAction;
                    }

                    return null;
                };
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
