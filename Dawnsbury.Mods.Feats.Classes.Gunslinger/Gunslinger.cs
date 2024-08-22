using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using System.Collections.Generic;
using Dawnsbury.Mods.Items.Firearms;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CharacterBuilder;
using System.Reflection.Metadata.Ecma335;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Tiles;
using System.ComponentModel.Design;
using Dawnsbury.Core.Mechanics.Core;
using System.Linq;
using System;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;

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
        private static string missfireDescriptionText = "{i}(A misfire will cause your firearm to jam requiring an interact action before use again, and any attack during the misfire will be a Critical Failure.){/i}";

        /// <summary>
        /// The Gunslinger Class Selection Feat
        /// </summary>
        public static readonly FeatName GunslingerClassFeatName = ModManager.RegisterFeatName("GunslingerClassFeat", "Gunslinger");

        /// <summary>
        /// The Way of the Drifter Subclass Feat Name
        /// </summary>
        public static readonly FeatName WayOfTheDrifterFeatName = ModManager.RegisterFeatName("Way of the Drifter", "Way of the Drifter");

        /// <summary>
        /// The Way of the Pistolero Subclass Feat Name
        /// </summary>
        public static readonly FeatName WayOfThePistoleroFeatName = ModManager.RegisterFeatName("Way of the Pistolero", "Way of the Pistolero");

        /// <summary>
        /// The Way of the Sniper Subclass Feat Name
        /// </summary>
        public static readonly FeatName WayOfTheSniperFeatName = ModManager.RegisterFeatName("Way of the Sniper", "Way of the Sniper");

        ///// <summary>
        ///// The Way of the Triggerbrand Subclass Feat Name
        ///// TODO: Add Combination Weapons and this Subclass
        ///// </summary>
        //public static readonly FeatName WayOfTheTriggerbrandFeatName = ModManager.RegisterFeatName("Way of the Triggerbrand", "Way of the Triggerbrand");

        /// <summary>
        /// The Way of the Vanguard Subclass Feat Name
        /// </summary>
        public static readonly FeatName WayOfTheVanguardFeatName = ModManager.RegisterFeatName("Way of the Vanguard", "Way of the Vanguard");

        /// <summary>
        /// The Hit the Dirt class feat name
        /// </summary>
        public static readonly FeatName HitTheDirtFeatName = ModManager.RegisterFeatName("Hit the Dirt", "Hit the Dirt");

        /// <summary>
        /// The Cover Fire class feat name
        /// </summary>
        public static readonly FeatName CoverFireFeatName = ModManager.RegisterFeatName("Cover Fire", "Cover Fire");

        /// <summary>
        /// The Crossbow Crack Shot class feat name
        /// </summary>
        public static readonly FeatName CrossbowCrackShotFeatName = ModManager.RegisterFeatName("Crossbow Crack Shot", "Crossbow Crack Shot");

        /// <summary>
        /// The Sword and Pistol class feat name
        /// </summary>
        public static readonly FeatName SwordAndPistolFeatName = ModManager.RegisterFeatName("Sword and Pistol", "Sword and Pistol");

        ///// <summary>
        ///// The Defensive Armaments class feat name
        ///// TODO: Add Parry Trait
        ///// </summary>
        //public static readonly FeatName DefensiveArmamentsFeatName = ModManager.RegisterFeatName("Defensive Armaments", "Defensive Armaments");

        /// <summary>
        /// The Fake Out class feat name
        /// </summary>
        public static readonly FeatName FakeOutFeatName = ModManager.RegisterFeatName("Fake Out", "Fake Out");

        /// <summary>
        /// The Pistol Twirl class feat name
        /// </summary>
        public static readonly FeatName PistolTwirlFeatName = ModManager.RegisterFeatName("Pistol Twirl", "Pistol Twirl");

        /// <summary>
        /// The Risky Reload class feat name
        /// </summary>
        public static readonly FeatName RiskyReloadFeatName = ModManager.RegisterFeatName("Risky Reload", "Risky Reload");

        /// <summary>
        /// The Warning Shot class feat name
        /// </summary>
        public static readonly FeatName WarningShotFeatName = ModManager.RegisterFeatName("Warning Shot", "Warning Shot");

        /// <summary>
        /// The Alchemical Shot class feat name
        /// </summary>
        public static readonly FeatName AlchemicalShotFeatName = ModManager.RegisterFeatName("Alchemical Shot", "Alchemical Shot");

        /// <summary>
        /// The Instant Backup class feat name
        /// </summary>
        public static readonly FeatName InstantBackupFeatName = ModManager.RegisterFeatName("Instant Backup", "Instant Backup");

        /// <summary>
        /// The Paired Shots class feat name
        /// HACK: Currently the percision damage is added from both attacks. Dawnsbury doesn't break out precision damage to check which is higher
        /// </summary>
        public static readonly FeatName PairedShotsFeatName = ModManager.RegisterFeatName("Paired Shots", "Paired Shots");

        /// <summary>
        /// The Running Reload class feat name
        /// </summary>
        public static readonly FeatName RunningReloadFeatName = ModManager.RegisterFeatName("Running Reload", "Running Reload");

        /// <summary>
        /// The Gunslinger class trait 
        /// </summary>
        public static readonly Trait GunslingerTrait = ModManager.RegisterTrait("Gunslinger", new TraitProperties("Gunslinger", relevant: true) { IsClassTrait = true });

        /// <summary>
        /// Creates the Gunslinger Feats
        /// </summary>
        /// <returns>The Enumerable of Gunslinger Feats</returns>
        public static IEnumerable<Feat> CreateGunslingerFeats()
        {

            yield return new ClassSelectionFeat(GunslingerClassFeatName, "While some fear projectile weapons, you savor the searing flash, wild kick, and cloying smoke that accompanies a gunshot, or snap of the cable and telltale thunk of your crossbow just before your bolt finds purchase. Ready to draw a bead on an enemy at every turn, you rely on your reflexes, steady hand, and knowledge of your weapons to riddle your foes with holes.",
                GunslingerTrait, new EnforcedAbilityBoost(Ability.Dexterity), 8,
                [Trait.Will, Trait.Unarmed, Trait.Simple, Trait.Martial, Firearms.AdvancedCrossbowTrait, Firearms.AdvancedFirearmTrait, Trait.UnarmoredDefense, Trait.LightArmor, Trait.MediumArmor],
                [Trait.Perception, Trait.Fortitude, Trait.Reflex, Firearms.SimpleCrossbowTrait, Firearms.MartialCrossbowTrait, Firearms.SimpleFirearmTrait, Firearms.MartialFirearmTrait],
                3,
                "{b}1. Gunslinger's Way{/b} All gunslingers have a particular way they follow, a combination of philosophy and combat style that defines both how they fight and the weapons they excel with. At 1st level, your way grants you an initial deed, a unique reload action called a slinger's reload, and proficiency with a particular skill. You also gain advanced and greater deeds at later levels, as well as access to way-specific feats.\n\n" +
                "{b}2. Singular Expertise{/b} You have particular expertise with guns and crossbows that grants you greater proficiency with them and the ability to deal more damage. You gain a +1 circumstance bonus to damage rolls with firearms and crossbows.\r\n\r\nThis intense focus on firearms and crossbows prevents you from reaching the same heights with other weapons. Your proficiency with unarmed attacks and with weapons other than firearms and crossbows can't be higher than trained, even if you gain an ability that would increase your proficiency in one or more other weapons to match your highest weapon proficiency (such as the weapon expertise feats many ancestries have). If you have gunslinger weapon mastery, the limit is expert, and if you have gunslinging legend, the limit is master.\n\n" +
                "{b}3. Gunslinger Feat{/b}", new List<Feat>()
                {
                    // TODO
                    new Feat(WayOfTheDrifterFeatName, "You're a wanderer traveling from land to land with your gun and a melee weapon as company. Maybe you learned to fight with blade and pistol as a Shackles pirate, mastered the hand cannon and katana in Minkai, or practiced with a hatchet and clan pistol in Dongun Hold. You win battles by relying on mobility and flexible use of your weapons.",
                    "You gain the {i}Slinger's Reload, Initial Deed, and Way Skill{/i} below:\n\n" +
                    "{b}Slinger's Reload{/b} Reloading Strike {icon:Action}\n{b}Requirements{/b} You're wielding a firearm or crossbow in one hand, and your other hand either wields a one-handed melee weapon or is empty.\n\nStrike an opponent within reach with your one-handed melee weapon (or, if your other hand is empty, with an unarmed attack), and then Interact to reload.\n\n" +
                    "{b}Initial Deed{/b} Into the Fray {icon:FreeAction}\n{b}Trigger{/b} You roll initiative\nYou can stride as a free action toward an enemy.\n\n" +
                    "{b}Way Skill{/b} Acrobatics\nYou become trained in Acrobatics.", new List<Trait>(), null),

                    // TODO
                    new Feat(WayOfThePistoleroFeatName, "Whether you're a professional duelist or a pistol-twirling entertainer, you have quick feet and quicker hands that never seem to let you down, and an equally sharp wit and tongue that jab your foes. You might leave a hand free or cultivate the ambidexterity for twin weapons. Either way, you stay close enough to your enemies to leverage your superior reflexes while leaving enough space to safely fire.",
                    "You gain the {i}Slinger's Reload, Initial Deed, and Way Skill{/i} below:\n\n" +
                    "{b}Slinger's Reload{/b} Raconteur's Reload {icon:Action}\nInteract to reload and then attempt a Deception check to Create a Diversion or an Intimidation check to Demoralize.\n\n" +
                    "{b}Initial Deed{/b} Ten Paces {icon:FreeAction}\n{b}Trigger{/b} You roll initiative\nYou gain a +2 circumstance bonus to your initiative roll, and you can Step up to 10 feet as a free action.\n\n" +
                    "{b}Way Skill{/b} Deception or Intimidation\nYou become trained in your choice between Deception or Intimidation.", new List<Trait>(), null),

                    // TODO
                    new Feat(WayOfTheSniperFeatName, "You practice a style of shooting that relies on unerring accuracy and perfect placement of your first shot. You keep hidden or at a distance, staying out of the fray and bringing unseen death to your foes.",
                    "You gain the {i}Slinger's Reload, Initial Deed, and Way Skill{/i} below:\n\n" +
                    "{b}Slinger's Reload{/b} Covered Reload {icon:Action}\nEither Take Cover or attempt to Hide, then Interact to reload.\n\n" +
                    "{b}Initial Deed{/b} One Shot, One Kill {icon:FreeAction}\n{b}Trigger{/b} You roll initiative\nYou instead roll Stealth for initiative. On your first turn, your first Strike with that weapon deals an additional 1d6 precision damage.\n\n" +
                    "{b}Way Skill{/b} Stealth\nYou become trained in Stealth.", new List<Trait>(), null),

                    // TODO
                    //new Feat(WayOfTheTriggerbrandFeatName, "You prefer firearms that work well as weapons in both melee and ranged combat, particularly those that allow you to exercise a bit of style and flair. You might be a survivor who cobbled your weapon together from the City of Smog's street scrap or a noble wielder of a master smith's bespoke commission for duels among Alkenstar's elite.",
                    //"You gain the {i}Slinger's Reload, Initial Deed, and Way Skill{/i} below:\n\n" +
                    //"{b}Slinger's Reload{/b} Touch and Go {icon:Action}\n{b}Requirements{/b} You're wielding a combination weapon.\n\nYou can Step toward an enemy, you can Interact to change your weapon between melee or ranged modes, and you then Interact to reload.\n\n" +
                    //"{b}Initial Deed{/b} Spring the Trap {icon:FreeAction}\n{b}Trigger{/b} You roll initiative\nYou choose which mode your combination weapon is set to. On your first turn, your movement and ranged attacks don't trigger reactions.\n\n" +
                    //"{b}Way Skill{/b} Thievery\nYou become trained in Thievery.", new List<Trait>(), null),

                    // TODO
                    new Feat(WayOfTheVanguardFeatName, "You practice a unique combat style originated by dwarven siege engineers, using heavy weapons with wide attack areas to blast holes through enemy lines, clear an opening for your allies, and defend the conquered territory.",
                    "You gain the {i}Slinger's Reload, Initial Deed, and Way Skill{/i} below:\n\n" +
                    "{b}Slinger's Reload{/b} Clear a Path {icon:Action}\n{b}Requirements{/b} You're wielding a two-handed firearm or two-handed crossbow.\n\nYou make an Athletics check to Shove an opponent within your reach using your weapon, then Interact to reload. For this Shove, you don't need a free hand, and you add the weapon's item bonus on attack rolls (if any) to the Athletics check. If your last action was a ranged Strike with the weapon, use the same multiple attack penalty as that Strike for the Shove; the Shove still counts toward your multiple attack penalty on further attacks as normal.\n\n" +
                    "{b}Initial Deed{/b} Into the Fray {icon:FreeAction}\n{b}Trigger{/b} You roll initiative\nGain a +1 circumstance bonus to AC until the start of your first turn, or a +2 circumstance bonus if the chosen weapon has the parry trait.\n\n" +
                    "{b}Way Skill{/b} Athletics\nYou become trained in Athletics.", new List<Trait>(), null)
                }).WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddSelectionOption(new SingleFeatSelectionOption("GunslingerFeat1", "Gunslinger feat", 1, (Feat ft) => ft.HasTrait(GunslingerTrait)));
                    sheet.AddAtLevel(3, delegate (CalculatedCharacterSheetValues values)
                    {
                        values.SetProficiency(Trait.Will, Proficiency.Expert); // Add Stubborn Check also
                    });
                });

            // TODO
            TrueFeat coverFireFeat = new TrueFeat(CoverFireFeatName, 1, "You lay down suppressive fire to protect allies by forcing foes to take cover from your wild attacks.", "{b}Frequency{/b} once per round\n\n{b}Requirements{/b} You're wielding a loaded firearm or crossbow.\n\nMake a firearm or crossbow Strike; the target must decide before you roll your attack whether it will duck out of the way.\n\nIf the target ducks, it gains a +2 circumstance bonus to AC against your attack, or a +4 circumstance bonus to AC if it has cover. It also takes a –2 circumstance penalty to ranged attack rolls until the end of its next turn.\n\nIf the target chooses not to duck, you gain a +1 circumstance bonus to your attack roll for that Strike.", [GunslingerTrait]).WithActionCost(1);
            AddCoverFireLogic(coverFireFeat);
            yield return coverFireFeat;

            // TODO
            yield return new TrueFeat(CrossbowCrackShotFeatName, 1, "You're exceptionally skilled with the crossbow.", "The first time each round that you Interact to reload a crossbow you are wielding, including Interact actions as part of your slinger's reload and similar effects, you increase the range increment for your next Strike with that weapon by 10 feet and deal 1 additional precision damage per weapon damage die with that Strike.\n\nIf your crossbow has the backstabber trait and you are attacking an off-guard target, backstabber deals 2 additional precision damage per weapon damage die instead of its normal effects.", [GunslingerTrait]);

            // TODO
            yield return new TrueFeat(HitTheDirtFeatName, 1, "You fling yourself out of harm's way.", "You Leap. Your movement gives you a +2 circumstance bonus to AC against the triggering attack. Regardless of whether or not the triggering attack hits, you land prone after completing your Leap.", [GunslingerTrait]).WithActionCost(-2);

            // TODO
            yield return new TrueFeat(SwordAndPistolFeatName, 1, "You're comfortable wielding a firearm or crossbow in one hand and a melee weapon in the other, combining melee attacks with shots from the firearm.", "When you make a successful ranged Strike against an enemy within your reach with your one-handed firearm or one-handed crossbow, that enemy is flat-footed against your next melee attack with a one-handed melee weapon.\n\nWhen you make a successful melee Strike against an enemy with your one-handed melee weapon, the next ranged Strike you make against that enemy with a one-handed firearm or one-handed crossbow doesn't trigger reactions that would trigger on a ranged attack, such as Attack of Opportunity. Either of these benefits is lost if not used by the end of your next turn.", [GunslingerTrait]);

            //// TODO
            //yield return new TrueFeat(DefensiveArmamentsFeatName, 2, "You use bulky firearms or crossbows to shield your body from your foes' attacks.", "Any two-handed firearms and two-handed crossbows you wield gain the parry trait. If an appropriate weapon already has the parry trait, increase the circumstance bonus to AC it grants when used to parry from +1 to +2.", [GunslingerTrait]);

            // TODO
            yield return new TrueFeat(FakeOutFeatName, 2, "With a skilled flourish of your weapon, you force an enemy to acknowledge you as a threat.", "{b}Trigger{/b} An ally is about to use an action that requires an attack roll, targeting a creature within your weapon's first range increment.\n\n{b}Requirements{/b} You're wielding a loaded firearm or crossbow.\n\nMake an attack roll to Aid the triggering attack. If you dealt damage to that enemy since the start of your last turn, you gain a +1 circumstance bonus to this roll.\n\n{i}Aid{/i}\n\n{b}Critical Success{/b} Your ally a +2 circumstance bonus\n{b}Success{/b} Your ally a +1 circumstance bonus\n{b}Critical Failure{/b} Your ally a -1 circumstance penalty\n", [GunslingerTrait, Trait.Visual]).WithActionCost(-2);

            // TODO
            yield return new TrueFeat(PistolTwirlFeatName, 2, "Your quick gestures and flair for performance distract your opponent, leaving it vulnerable to your follow-up attacks.", "{b}Requirements{/b} You're wielding a loaded one-handed ranged weapon.\n\nYou Feint against an opponent within the required weapon's first range increment, rather than an opponent within melee reach. If you succeed, the foe is flat-footed against your melee and ranged attacks, rather than only your melee attacks. On a critical failure, you're flat-footed against the target's melee and ranged attacks, rather than only its melee attacks.", [GunslingerTrait]).WithActionCost(1).WithPrerequisite((CalculatedCharacterSheetValues sheet) => (sheet.Proficiencies.AllProficiencies[Trait.Deception] >= Proficiency.Trained), "trained in Deception");

            // TODO
            yield return new TrueFeat(RiskyReloadFeatName, 2, "You've practiced a technique for rapidly reloading your firearm, but attempting to use this technique is a dangerous gamble with your firearm's functionality.", "{b}Requirements{/b} You're wielding a firearm.\n\nInteract to reload a firearm, then make a Strike with that firearm. If the Strike fails, the firearm misfires. " + missfireDescriptionText, [GunslingerTrait, Trait.Flourish]).WithActionCost(1);

            // TODO
            yield return new TrueFeat(WarningShotFeatName, 2, "Who needs words when the roar of a gun is so much more succinct?", "{b}Requirements{/b} You're wielding a loaded firearm.\n\nYou attempt to Demoralize a foe by firing your weapon into the air, using the firearm's maximum range rather than the usual range of 30 feet.", [GunslingerTrait]).WithActionCost(1).WithPrerequisite((CalculatedCharacterSheetValues sheet) => (sheet.Proficiencies.AllProficiencies[Trait.Intimidation] >= Proficiency.Trained), "trained in Intimidation");

            // TODO
            yield return new TrueFeat(AlchemicalShotFeatName, 4, "You've practiced a technique for mixing alchemical bombs with your loaded shot.", "{b}Requirements{/b} You have an alchemical bomb worn or in one hand, and are wielding a firearm or crossbow.\n\nYou Interact to retrieve the bomb (if it's not already in your hand) and pour its contents onto your ammunition, consuming the bomb, then resume your grip on the required weapon. Next, Strike with your firearm. The Strike deals damage of the same type as the bomb (for instance, fire damage for alchemist's fire), and it deals an additional 1d6 persistent damage of the same type as the bomb. If the Strike is a failure, you take 1d6 damage of the same type as the bomb you used, and the firearm misfires. " + missfireDescriptionText, [GunslingerTrait]);

            // TODO
            yield return new TrueFeat(InstantBackupFeatName, 4, "Even as your firearm misfires, you quickly draw a backup weapon.", "Release the misfired weapon if you so choose, and Interact to draw a one-handed weapon.\n\n" + missfireDescriptionText, [GunslingerTrait]).WithActionCost(-2);

            // TODO
            TrueFeat pairedShotsFeat = new TrueFeat(PairedShotsFeatName, 4, "Your shots hit simultaneously.", "{b}Requirements{/b} You're wielding two weapons, each of which can be either a loaded one-handed firearm or loaded one-handed crossbow.\n\nMake two Strikes, one with each of your two ranged weapons, each using your current multiple attack penalty. Both Strikes must have the same target.\n\nIf both attacks hit, combine their damage and then add any applicable effects from both weapons. You add any precision damage, only once, to the attack of your choice. Combine the damage from both Strikes and apply resistances and weaknesses only once. This counts as two attacks when calculating your multiple attack penalty.", [GunslingerTrait]).WithActionCost(2);
            AddPairedShotsLogic(pairedShotsFeat);
            yield return pairedShotsFeat;

            // TODO
            yield return new TrueFeat(RunningReloadFeatName, 4, "You can reload your weapon on the move.", "You Stride, Step, or Sneak, then Interact to reload.", [GunslingerTrait]).WithActionCost(1);
        }

        /// <summary>
        /// Patches all feats for the Gunslinger
        /// </summary>
        /// <param name="feat">The feat to patch</param>
        public static void PatchFeats(Feat feat)
        {
            // Patches Intimidating Stike to be selectable by Gunslinger
            if (feat.FeatName == FeatName.QuickDraw)
            {
                PatchQuickDraw(feat);
            }
        }

        /// <summary>
        /// Adds the logic for the Cover Fire feat
        /// </summary>
        /// <param name="coverFireFeat">The Cover Fire true feat object</param>
        private static void AddCoverFireLogic(TrueFeat coverFireFeat)
        {
            coverFireFeat.WithPermanentQEffect(coverFireFeat.FlavorText, delegate (QEffect self)
            {
                self.ProvideStrikeModifier = (Item item) =>
                {
                    QEffect technicalEffectForOncePerRound = new QEffect("Technical Cover Fire", "[this condition has no description]")
                    {
                        ExpiresAt = ExpirationCondition.ExpiresAtStartOfYourTurn
                    };
                    CombatAction basicStrike = self.Owner.CreateStrike(item);

                    CombatAction coverFireAction = new CombatAction(self.Owner, new SideBySideIllustration(item.Illustration, IllustrationName.TakeCover), "Cover Fire", [GunslingerTrait, Trait.Basic, Trait.IsHostile, Trait.Attack], "{b}Frequency{/b} once per round\n\n{b}Requirements{/b} You're wielding a loaded firearm or crossbow.\n\nMake a firearm or crossbow Strike; the target must decide before you roll your attack whether it will duck out of the way.\n\nIf the target ducks, it gains a +2 circumstance bonus to AC against your attack, or a +4 circumstance bonus to AC if it has cover. It also takes a –2 circumstance penalty to ranged attack rolls until the end of its next turn.\n\nIf the target chooses not to duck, you gain a +1 circumstance bonus to your attack roll for that Strike.", basicStrike.Target);
                    coverFireAction.WithActionCost(1);
                    coverFireAction.WithEffectOnChosenTargets(async delegate (Creature attacker, ChosenTargets targets)
                    {
                        if (attacker.QEffects.Count(qe => qe == technicalEffectForOncePerRound) == 0)
                        {
                            attacker.AddQEffect(technicalEffectForOncePerRound);
                        }

                        Creature? target = targets.ChosenCreature;
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
                            BonusToDefenses = (QEffect bonus, CombatAction action, Defense defense) =>
                            {
                                return new Bonus(cover > 0 ? 4 : 2, BonusType.Circumstance, "Cover Fire", true);
                            }
                        };

                        if (target != null)
                        {
                            bool shouldDodge = true;
                            if (target.OwningFaction != target.Battle.You)
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
                            else
                            {
                                shouldDodge = await target.Battle.AskForConfirmation(self.Owner, IllustrationName.QuestionMark, "Duck to gain +" + (cover > 0 ? "4" : "2") + " circumstance bonus to AC against the attack, along with a -2 circumstance penalty to ranged attack rolls until the end of your next turn?", "Duck");
                            }

                            if (shouldDodge)
                            {
                                target.AddQEffect(acBonus);
                                target.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtEndOfSourcesTurn)
                                {
                                    BonusToAttackRolls = (QEffect penalty, CombatAction action, Creature? defender) =>
                                    {
                                        if(basicStrike.HasTrait(Trait.Ranged))
                                        {
                                            return new Bonus(-2, BonusType.Circumstance, "Cover Fire", false);
                                        }

                                        return null;
                                    }
                                });
                            }
                            else
                            {
                                attacker.AddQEffect(attackRollBonus);
                            }

                            await attacker.MakeStrike(target, item);
                            attacker.RemoveAllQEffects(qe => qe == attackRollBonus);
                            target.RemoveAllQEffects(qe => qe == acBonus);
                        }
                    });

                    // Checks if the item needs to be reloaded
                    ((CreatureTarget)coverFireAction.Target).WithAdditionalConditionOnTargetCreature((Creature attacker, Creature defender) =>
                    {
                        if (!IsItemLoaded(item))
                        {
                            return Usability.NotUsable("Needs to be reloaded.");
                        }
                        else if (attacker.QEffects.Count(qe => qe.Name == technicalEffectForOncePerRound.Name) > 0)
                        {
                            return Usability.NotUsable("Already used this round.");
                        }

                        return Usability.Usable;
                    });

                    return coverFireAction;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Paired Shots feat
        /// </summary>
        /// <param name="pairedShotsFeat">The Paired Shots true feat object</param>
        private static void AddPairedShotsLogic(TrueFeat pairedShotsFeat)
        {
            pairedShotsFeat.WithPermanentQEffect(pairedShotsFeat.FlavorText, delegate (QEffect self)
            {
                self.ProvideMainAction = (QEffect pairedShotEffect) =>
                {
                    if (pairedShotEffect.Owner.HeldItems.Count(item => (item.HasTrait(Trait.Crossbow) || item.HasTrait(Firearms.FirearmTrait)) && IsItemLoaded(item) && item.WeaponProperties != null) != 2)
                    {
                        return null;
                    }
                    int currentMap = self.Owner.Actions.AttackedThisManyTimesThisTurn;
                    Item firstHeldItem = self.Owner.HeldItems[0];
                    Item secondHeldItem = self.Owner.HeldItems[1];
                    int maxRange = Math.Min(firstHeldItem.WeaponProperties.MaximumRange, secondHeldItem.WeaponProperties.MaximumRange);

                    return new ActionPossibility(new CombatAction(pairedShotEffect.Owner, new SideBySideIllustration(firstHeldItem.Illustration, secondHeldItem.Illustration), "Paired Shots", [GunslingerTrait, Trait.Basic, Trait.IsHostile], pairedShotsFeat.RulesText, Target.Ranged(maxRange)).WithActionCost(2).WithEffectOnChosenTargets(async delegate (Creature attacker, ChosenTargets targets)
                    {
                        if (targets.ChosenCreature != null)
                        {
                            await pairedShotEffect.Owner.MakeStrike(targets.ChosenCreature, firstHeldItem, currentMap);
                            await pairedShotEffect.Owner.MakeStrike(targets.ChosenCreature, secondHeldItem, currentMap);
                        }
                    }));
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
            quickDrawFeat.Traits.Add(GunslingerTrait);
            for (int i = 0; i < quickDrawFeat.Prerequisites.Count; i++)
            {
                Prerequisite prereq = quickDrawFeat.Prerequisites[i];
                if (prereq is ClassPrerequisite classPrerequisite)
                {
                    if (!classPrerequisite.AllowedClasses.Contains(GunslingerTrait))
                    {
                        List<Trait> updatedAllowedClasses = classPrerequisite.AllowedClasses;
                        updatedAllowedClasses.Add(GunslingerTrait);
                        quickDrawFeat.Prerequisites[i] = new ClassPrerequisite(updatedAllowedClasses);
                    }
                }


            }
        }

        /// <summary>
        /// Determines if the item is a firearm or a crossbow
        /// </summary>
        /// <param name="item">The item being checked</param>
        /// <returns>True if the item is a firearm or crossbow and false otherwise</returns>
        private static bool IsItemFirearmOrCrossbow(Item item, bool checkIfItsLoaded = false)
        {
            if (item.HasTrait(Firearms.FirearmTrait) || item.HasTrait(Trait.Crossbow))
            {
                if (checkIfItsLoaded)
                {
                    return IsItemLoaded(item);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if the item is loaded
        /// </summary>
        /// <param name="item">The item being checked</param>
        /// <returns>True if the item is loaded and false otherwise</returns>
        private static bool IsItemLoaded(Item item)
        {
            return item.EphemeralItemProperties != null && !item.EphemeralItemProperties.NeedsReload;
        }

        /// <summary>
        /// Adds any of the given traits if it is missing from the traits
        /// </summary>
        /// <param name="traits">Traits being added to</param>
        /// <param name="traitsToAdd">An array of traits to add</param>
        private static void AddTraitIfNeeded(Traits traits, Trait[] traitsToAdd)
        {
            foreach (Trait trait in traitsToAdd)
            {
                if (!traits.Contains(trait))
                {
                    traits.Add(trait);
                }
            }
        }
    }
}
