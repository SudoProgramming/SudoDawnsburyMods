using Dawnsbury.Auxiliary;
using Dawnsbury.Core;
using Dawnsbury.Core.Animations.Movement;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Damage;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Modding;
using Dawnsbury.Mods.Feats.Ancestries.Catfolk.RegisteredComponents;
using Dawnsbury.Mods.Feats.Ancestries.Ratfolk.RegisteredComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Schema;
using static Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.BarbarianFeatsDb.AnimalInstinctFeat;
using static Dawnsbury.Delegates;

namespace Dawnsbury.Mods.Feats.Ancestries.Catfolk
{
    /// <summary>
    /// The Ratfolk Ancestry
    /// </summary>
    public static class Catfolk
    {
        /// <summary>
        /// Creates all the Feats for the Catfolk
        /// </summary>
        /// <returns>The Enumerable of Catfolk Feats</returns>
        public static IEnumerable<Feat> CreateCatfolkFeats()
        {
            // Heritage Feats
            // Creates and adds the logic for the Clawed Catfolk heritage
            HeritageSelectionFeat clawedCatfolkFeat = new HeritageSelectionFeat(CatfolkFeatNames.ClawedCatfolk, "Your family has particularly long, sharp claws capable of delivering bleeding wounds with a wicked swipe.", "You gain a claw unarmed attack that deals 1d6 slashing damage. Your claws are in the brawling group and have the agile, finesse, and unarmed traits.");
            AddClawedCatfolkLogic(clawedCatfolkFeat);
            yield return clawedCatfolkFeat;

            // Creates and adds the logic for the Flexible Catfolk heritage
            HeritageSelectionFeat flexibleCatfolkFeat = new HeritageSelectionFeat(CatfolkFeatNames.FlexibleCatfolk, "You've inherited flexibility beyond that of most humanoids.", "You get a +1 circumstance bonus to checks when you attempt to Escape.");
            AddFlexibleCatfolkLogic(flexibleCatfolkFeat);
            yield return flexibleCatfolkFeat;

            // Creates and adds the logic for the Jungle Catfolk heritage
            HeritageSelectionFeat jungleCatfolkFeat = new HeritageSelectionFeat(CatfolkFeatNames.JungleCatfolk, "You’re descended from jungle stalkers and can move swiftly through difficult terrain.", "You ignore difficutl terrain and great difficult terrain.");
            AddJungleCatfolkLogic(jungleCatfolkFeat);
            yield return jungleCatfolkFeat;

            // Creates and adds the logic for the Nine Lives Catfolk heritage
            HeritageSelectionFeat nineLivesCatfolkFeat = new HeritageSelectionFeat(CatfolkFeatNames.NineLivesCatfolk, "Your family has always seemed to bounce back from disaster, not through physical hardiness or specialized skill, but from sheer luck. Other catfolk whisper that you have nine lives.", "If you are reduced to 0 Hit Points by a critical hit on an attack roll, you become dying 1 instead of dying 2.");
            AddNineLivesCatfolkLogic(nineLivesCatfolkFeat);
            yield return nineLivesCatfolkFeat;

            // Creates and adds the logic for the Sharp-Eared Catfolk heritage
            HeritageSelectionFeat sharpEaredCatfolkFeat = new HeritageSelectionFeat(CatfolkFeatNames.SharpEaredCatfolk, "You were born with big, expressive ears that move with your moods and perk up at any unexpected sound.", "You gain a +2 circumstance bonus to Perception checks to Seek a creature or object within 30 feet.");
            AddSharpEaredCatfolkLogic(sharpEaredCatfolkFeat);
            yield return sharpEaredCatfolkFeat;

            // Creates and adds the logic for the Winter Catfolk heritage
            HeritageSelectionFeat winterCatfolkFeat = new HeritageSelectionFeat(CatfolkFeatNames.WinterCatfolk, "You have a thick coat of fur that protects you from the cold.", "You gain you cold resistance equal to half your level (minimum 1).");
            AddWinterCatfolkLogic(winterCatfolkFeat);
            yield return winterCatfolkFeat;

            // Creates the base Ratfolk ancestry selection feat and adds the logic for the Sharp Teeth base feature
            AncestrySelectionFeat catfolkFeat = new AncestrySelectionFeat(CatfolkFeatNames.Catfolk,
                "Catfolk are highly social, feline humanoids prone to curiosity and wandering.\n\nCurious and gregarious wanderers, catfolk combine the features of felines and humanoids in both appearance and temperament. They enjoy learning new things, collecting new tales and trinkets, and ensuring their loved ones are safe and happy. Catfolk view themselves as the chosen guardians of natural places in the world and are often recklessly brave, even in the face of overwhelming opposition. They believe that strong communities, breadth of experience, and continual self-improvement aid them in this fight.\n\n{b}Stable Trip{/b} You don't fall prone when you roll a Critical Failure on a Trip check.",
                [CatfolkTraits.Catfolk, Trait.Humanoid],
                8,
                5,
                [new EnforcedAbilityBoost(Ability.Dexterity), new EnforcedAbilityBoost(Ability.Charisma), new FreeAbilityBoost()],
                [clawedCatfolkFeat, flexibleCatfolkFeat, jungleCatfolkFeat, nineLivesCatfolkFeat, sharpEaredCatfolkFeat, winterCatfolkFeat])
                .WithAbilityFlaw(Ability.Wisdom);
            AddStableTripLogic(catfolkFeat);
            yield return catfolkFeat;

            // Level 1 Ancestry Feats
            // Creates and adds the logic for the Cat Nap Feat
            TrueFeat catNapFeat = new TrueFeat(CatfolkFeatNames.CatNap, 1, "You can briefly sleep to regain your energy quickly.", "You can gain temporary Hit Points equal to your level at the start of each encounter.", [CatfolkTraits.Catfolk]);
            AddCatNapLogic(catNapFeat);
            yield return catNapFeat;

            // Creates and adds the logic for the Cat's Luck Feat
            TrueFeat catsLuckFeat = new TrueFeat(CatfolkFeatNames.CatsLuck, 1, "You instinctively twist away from danger.", "{b}Frequency{/b} once per day\n{b}Trigger{/b} You fail a Reflex saving throw.\n\nYou can reroll the triggering saving throw and use the better result.", [CatfolkTraits.Catfolk, Trait.Fortune]);
            catsLuckFeat.WithActionCost(0);
            AddCatsLuckLogic(catsLuckFeat);
            yield return catsLuckFeat;

            // Creates and adds the logic for the Catfolk Dance Feat
            TrueFeat catfolkDanceFeat = new TrueFeat(CatfolkFeatNames.CatfolkDance, 1, "You have a habit of always being in the way when other creatures attempt to move.", "Attempt an Acrobatics check against an adjacent creature's Reflex DC.\n\n{b}Critical Success{/b} The target creature gains a –2 circumstance penalty to Reflex saves and is flat-footed until the start of your next turn.\n{b}Success{/b} The target creature gains a –2 circumstance penalty to Reflex saves until the start of your next turn.", [CatfolkTraits.Catfolk]);
            catfolkDanceFeat.WithActionCost(1);
            AddCatfolkDanceLogic(catfolkDanceFeat);
            yield return catfolkDanceFeat;

            // Creates and adds the logic for the Catfolk Lore Feat
            TrueFeat catfolkLoreFeat = new TrueFeat(CatfolkFeatNames.CatfolkLore, 1, "Growing up among catfolk has taught you the traditional values of freedom to travel, stewardship of the land, and quick reactions when your curiosity lands you in trouble.", "You gain the trained proficiency rank in Acrobatics and Survival.", [CatfolkTraits.Catfolk]);
            AddCatfolkLoreLogic(catfolkLoreFeat);
            yield return catfolkLoreFeat;

            // Creates and adds the logic for the Catfolk Weapon Familiarity Feat
            TrueFeat catfolkWeaponFamiliarityFeat = new TrueFeat(CatfolkFeatNames.CatfolkWeaponFamiliarity, 1, "You favor weapons that you can use with quick, darting slashes like a cat's claws.", "You are trained with the kukri, scimitar, and sickle. For you, martial catfolk weapons are simple weapons and advanced catfolk weapons are martial weapons.", [CatfolkTraits.Catfolk]);
            AddCatfolkWeaponFamiliarityLogic(catfolkWeaponFamiliarityFeat);
            yield return catfolkWeaponFamiliarityFeat;

            // Creates and adds the logic for the Saberteeth Feat
            TrueFeat saberteethFeat = new TrueFeat(CatfolkFeatNames.Saberteeth, 1, "You have long fangs, natural or augmented.", "You gain a jaws unarmed attack that deals 1d6 piercing damage. Your jaws are in the brawling group and have the unarmed trait.", [CatfolkTraits.Catfolk]);
            AddSaberteethLogic(saberteethFeat);
            yield return saberteethFeat;

            // Creates and adds the logic for the Well-Met Traveler Feat
            TrueFeat wellMetTravelerFeat = new TrueFeat(CatfolkFeatNames.WellMetTraveler, 1, "You have seen people from so many walks of life in your travels that you naturally adopt a pleasant and affable demeanor when meeting others.", "You are trained in Diplomacy. You have a +2 Circumstance bonus to all Diplomacy checks.", [CatfolkTraits.Catfolk]);
            AddWellMetTravelerLogic(wellMetTravelerFeat);
            yield return wellMetTravelerFeat;

            // Level 5 Ancestry Feats
            // Creates and adds the logic for the Light Paws Feat
            TrueFeat lightPawsFeat = new TrueFeat(CatfolkFeatNames.LightPaws, 5, "You can balance on your toes to step carefully over obstructions.", "You Stride and then Step, or Step and then Stride, ignoring difficult terrain during this movement.", [CatfolkTraits.Catfolk]);
            lightPawsFeat.WithActionCost(2);
            AddLightPawsLogic(lightPawsFeat);
            yield return lightPawsFeat;

            // Creates and adds the logic for the Lucky Break Feat
            TrueFeat luckyBreakFeat = new TrueFeat(CatfolkFeatNames.LuckyBreak, 5, "You catch yourself as you make a mistake.", "You can trigger Cat's Luck when you fail or critically fail on an Athletics or Acrobatics skill check, in addition to its normal trigger. When you do, you reroll the triggering skill check and use the better result. This still counts against Cat's Luck's frequency, as normal.", [CatfolkTraits.Catfolk]);
            luckyBreakFeat.WithPrerequisite(CatfolkFeatNames.CatsLuck, "Requires Cat's Luck");
            AddLuckyBreakLogic(luckyBreakFeat);
            yield return luckyBreakFeat;

            // Creates and adds the logic for the Springing Leaper Feat
            TrueFeat springingLeaperFeat = new TrueFeat(CatfolkFeatNames.SpringingLeaper, 5, "Your powerful legs allow you to make sudden and dramatic leaps.", "You gain the Powerful Leap feat. When you leap you will not trigger reactions.", [CatfolkTraits.Catfolk]);
            springingLeaperFeat.WithPrerequisite((CalculatedCharacterSheetValues sheet) => sheet.GetProficiency(Trait.Athletics) >= Proficiency.Expert, "Requires Expert in Athletics");
            AddSpringingLeaperLogic(springingLeaperFeat);
            yield return springingLeaperFeat;

            // Creates and adds the logic for the Well-Groomed Feat
            TrueFeat wellGroomedFeat = new TrueFeat(CatfolkFeatNames.WellGroomed, 5, "You are fastidious about keeping yourself clean, whether licking your fur or carefully using traditional catfolk hygiene products, to salubrious effect.", "You gain a +1 circumstance bonus to saving throws against diseases. If you roll a success on a saving throw against a disease, you get a critical success instead.", [CatfolkTraits.Catfolk]);
            AddWellGroomedLogic(wellGroomedFeat);
            yield return wellGroomedFeat;

            // Creates and adds the logic for the Comfortable Cat Nap Feat
            TrueFeat comfortableCatNapFeat = new TrueFeat(CatfolkFeatNames.ComfortableCatNap, 5, "You get even more out of your naps.", "The first encounter after each long rest, you gain double the amount of temporary HP from Cat Nap.", [CatfolkTraits.Catfolk]);
            comfortableCatNapFeat.WithPrerequisite(CatfolkFeatNames.CatNap, "Comfortable Cat Nap");
            AddComfortableCatNapLogic(comfortableCatNapFeat);
            yield return comfortableCatNapFeat;
        }

        /// <summary>
        /// Adds the Stable Trip Logic
        /// </summary>
        /// <param name="catfolkFeat">The catfolk feat</param>
        public static void AddStableTripLogic(AncestrySelectionFeat catfolkFeat)
        {
            catfolkFeat.WithPermanentQEffect("You don't fall prone from Critical Failures when Tripping.", delegate (QEffect self)
            {
                self.Name = "Stable Trip";
                self.YouBeginAction = async (QEffect youBeginAction, CombatAction action) =>
                {
                    if (action.ActionId == ActionId.Trip)
                    {
                        youBeginAction.Owner.AddQEffect(new QEffect()
                        {
                            Id = CatfolkQEIDs.StableTrip
                        });
                    }
                };
                self.YouAcquireQEffect = (QEffect effectGotten, QEffect acquiredQEffect) =>
                {
                    Creature owner = effectGotten.Owner;
                    if (owner.HasEffect(CatfolkQEIDs.StableTrip) && acquiredQEffect.Id == QEffectId.Prone)
                    {
                        owner.RemoveAllQEffects(qe => qe.Id == CatfolkQEIDs.StableTrip);
                        return null;
                    }

                    return acquiredQEffect;
                };
            });
        }

        /// <summary>
        /// Adds the Clawed Catfolk Logic
        /// </summary>
        /// <param name="clawedCatfolkFeat">The catfolk feat</param>
        public static void AddClawedCatfolkLogic(HeritageSelectionFeat clawedCatfolkFeat)
        {
            // Adds a permanent effect to add a Claw.
            clawedCatfolkFeat.WithPermanentQEffect(null, delegate (QEffect self)
            {
                self.AdditionalUnarmedStrike = new Item(IllustrationName.DragonClaws, "Claw", [Trait.Agile, Trait.Finesse, Trait.Unarmed, Trait.Brawling])
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Slashing));
            });
        }

        /// <summary>
        /// Adds the Flexible Catfolk Logic
        /// </summary>
        /// <param name="flexibleCatfolkFeat">The catfolk feat</param>
        public static void AddFlexibleCatfolkLogic(HeritageSelectionFeat flexibleCatfolkFeat)
        {
            // Adds a permanent effect to have a bonus to escape.
            flexibleCatfolkFeat.WithPermanentQEffect("+1 Circumstance to Escape checks.", delegate (QEffect self)
            {
                self.BonusToAttackRolls = (QEffect bonusToAttack, CombatAction action, Creature? target) =>
                {
                    if (action.ActionId == ActionId.Escape)
                    {
                        return new Bonus(1, BonusType.Circumstance, "Flexible Catfolk", true);
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the Jungle Catfolk Logic
        /// </summary>
        /// <param name="jungleCatfolkFeat">The catfolk feat</param>
        public static void AddJungleCatfolkLogic(HeritageSelectionFeat jungleCatfolkFeat)
        {
            // Adds a permanenet effect to ignore difficult terrain
            jungleCatfolkFeat.WithPermanentQEffect("+2 Circumstance to Seek within 30 ft.", delegate (QEffect self)
            {
                self.Id = QEffectId.IgnoresDifficultTerrain;
            });
        }

        /// <summary>
        /// Adds the Nine Lives Catfolk Logic
        /// </summary>
        /// <param name="nineLivesCatfolkFeat">The catfolk feat</param>
        public static void AddNineLivesCatfolkLogic(HeritageSelectionFeat nineLivesCatfolkFeat)
        {
            // Adds a permanenet effect to go to dying 1 on a crit
            nineLivesCatfolkFeat.WithPermanentQEffect("If you go to 0 on a crit, you go to dying 1 instead of 2.", delegate (QEffect self)
            {
                self.Id = QEffectId.HideNucleus;
            });
        }

        /// <summary>
        /// Adds the Sharp-Eared Catfolk Logic
        /// </summary>
        /// <param name="sharpEaredCatfolkFeat">The catfolk feat</param>
        public static void AddSharpEaredCatfolkLogic(HeritageSelectionFeat sharpEaredCatfolkFeat)
        {
            // Adds a permanenet effect to increase Seek checks within 30 feet
            sharpEaredCatfolkFeat.WithPermanentQEffect("+2 Circumstance to Seek within 30 ft.", delegate (QEffect self)
            {
                self.BonusToAttackRolls = (QEffect bonusToSeek, CombatAction action, Creature? creature) =>
                {
                    if (action.ActionId == ActionId.Seek && creature != null && creature.DetectionStatus.Undetected && self.Owner.DistanceTo(creature) <= 6)
                    {
                        return new Bonus(2, BonusType.Circumstance, "Sharp-Eared Catfolk", true);
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the Winter Catfolk Logic
        /// </summary>
        /// <param name="winterCatfolkFeat">The catfolk feat</param>
        public static void AddWinterCatfolkLogic(HeritageSelectionFeat winterCatfolkFeat)
        {
            // Adds a permanent effect to gain half level cold resistance
            winterCatfolkFeat.WithPermanentQEffect("Cold Resistance", delegate (QEffect self)
            {
                int resistence = Math.Max((self.Owner.Level / 2), 1);
                self.Owner.WeaknessAndResistance.AddResistance(DamageKind.Cold, resistence);
            });
        }

        /// <summary>
        /// Adds the Cat Nap Logic
        /// </summary>
        /// <param name="catNapFeat">The Cat Nap feat</param>
        public static void AddCatNapLogic(TrueFeat catNapFeat)
        {
            // Adds a temporary HP at the start of each encounter
            catNapFeat.WithPermanentQEffect("Gain Temp HP equal to your level at the start of each encounter.", delegate (QEffect self)
            {
                self.StartOfCombat = async (QEffect startOfCombat) =>
                {
                    Creature owner = startOfCombat.Owner;
                    int tempHPAmount = owner.Level;
                    if (owner.HasFeat(CatfolkFeatNames.ComfortableCatNap) && !owner.PersistentUsedUpResources.UsedUpActions.Contains("Comfortable Cat Nap"))
                    {
                        owner.PersistentUsedUpResources.UsedUpActions.Add("Comfortable Cat Nap");
                        tempHPAmount = 2 * tempHPAmount;
                    }

                    owner.GainTemporaryHP(tempHPAmount);
                };
            });
        }

        /// <summary>
        /// Adds the Cat's Luck Logic
        /// </summary>
        /// <param name="catsLuckFeat">The Cat's Luck feat</param>
        public static void AddCatsLuckLogic(TrueFeat catsLuckFeat)
        {
            // Rerolls reflex saving throw
            catsLuckFeat.WithPermanentQEffect("Reroll a failed Reflex save once per day.", delegate (QEffect self)
            {
                self.RerollSavingThrow = async (QEffect saveReroll, CheckBreakdownResult result, CombatAction action) =>
                {
                    Creature owner = saveReroll.Owner;
                    Defense? defense = action.SavingThrow?.Defense;
                    if (result.CheckResult <= CheckResult.Failure && defense != null && defense == Defense.Reflex && result.CheckResult <= CheckResult.Failure && !owner.PersistentUsedUpResources.UsedUpActions.Contains("Cat's Luck") && await owner.AskForConfirmation(IllustrationName.FreeAction, "You failed the saving throw against " + action.Name + ". Use {i}Cat's Luck{/i} to reroll the saving throw and keep the better result?", "Yes"))
                    {
                        owner.PersistentUsedUpResources.UsedUpActions.Add("Cat's Luck");
                        return RerollDirection.RerollAndKeepBest;
                    }

                    return RerollDirection.DoNothing;
                };
            });
        }

        /// <summary>
        /// Adds the Catfolk Dance Logic
        /// </summary>
        /// <param name="catfolkDanceFeat">The Catfolk Dance feat</param>
        public static void AddCatfolkDanceLogic(TrueFeat catfolkDanceFeat)
        {
            // Adds an action that can gives a Relex Save penalty
            catfolkDanceFeat.WithPermanentQEffect("Acrobatics check to give a penalty against Reflex saves.", delegate (QEffect self)
            {
                self.ProvideMainAction = (QEffect mainAction) =>
                {
                    Creature owner = mainAction.Owner;
                    return new ActionPossibility(new CombatAction(owner, IllustrationName.GenericCombatManeuver, "Catfolk Dance", [], catfolkDanceFeat.RulesText, Target.Touch())
                    .WithActionCost(1)
                    .WithActiveRollSpecification(new ActiveRollSpecification(TaggedChecks.SkillCheck(Skill.Acrobatics), Checks.DefenseDC(Defense.Reflex)))
                    .WithEffectOnEachTarget(async (CombatAction action, Creature attacker, Creature defender, CheckResult result) =>
                    {
                        if (result >= CheckResult.Success)
                        {
                            defender.AddQEffect(new QEffect("Catfolk Dance", "You have a -2 Circumstance penalty to Reflex saves until the start of your turn.")
                            {
                                Illustration = IllustrationName.Clumsy,
                                BonusToDefenses = (QEffect bonusToSave, CombatAction? action, Defense defense) =>
                                {
                                    if (action != null && action.SavingThrow != null && defense == Defense.Reflex)
                                    {
                                        return new Bonus(-2, BonusType.Circumstance, "Catfolk Dance", false);
                                    }

                                    return null;
                                }
                            });
                            if (result == CheckResult.CriticalSuccess)
                            {
                                QEffect flatFooted = QEffect.FlatFooted("Catfolk Dance");
                                flatFooted.Source = attacker;
                                flatFooted.ExpiresAt = ExpirationCondition.ExpiresAtStartOfSourcesTurn;
                                defender.AddQEffect(flatFooted);
                            }
                        }
                    })); 
                };
            });
        }

        /// <summary>
        /// Adds the Catfolk Lore Logic
        /// </summary>
        /// <param name="catfolkLore">The Catfolk Lore feat</param>
        public static void AddCatfolkLoreLogic(TrueFeat catfolkLore)
        {
            // Adds both Acrobatics and Survival as trained
            catfolkLore.WithOnSheet((character) =>
            {
                if (!character.HasFeat(FeatName.Acrobatics))
                {
                    character.GrantFeat(FeatName.Acrobatics);
                }
                if (!character.HasFeat(FeatName.Survival))
                {
                    character.GrantFeat(FeatName.Survival);
                }
            });
        }

        /// <summary>
        /// Adds the Catfolk Weapon Familiarity Logic
        /// </summary>
        /// <param name="catfolkWeaponFamiliarityFeat">The Catfolk Weapon Familiarity feat</param>
        public static void AddCatfolkWeaponFamiliarityLogic(TrueFeat catfolkWeaponFamiliarityFeat)
        {
            // Adds the Weapn Familiarity Logic
            catfolkWeaponFamiliarityFeat.WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                Func<List<Trait>, bool> GetProficiencyAdjustment(Trait weaponType)
                {
                    return (List<Trait> traits) => traits.Contains(weaponType) && (traits.Contains(CatfolkTraits.Catfolk) || traits.Contains(Trait.Kukri) || traits.Contains(Trait.Scimitar) || traits.Contains(Trait.Sickle));
                }

                sheet.Proficiencies.AddProficiencyAdjustment(GetProficiencyAdjustment(Trait.Martial), Trait.Simple);
                sheet.Proficiencies.AddProficiencyAdjustment(GetProficiencyAdjustment(Trait.Advanced), Trait.Martial);
            });
        }

        /// <summary>
        /// Adds the Saberteeth Logic
        /// </summary>
        /// <param name="saberteethFeat">The Saberteeth feat</param>
        public static void AddSaberteethLogic(TrueFeat saberteethFeat)
        {
            // Adds a permanent effect to add a Jaws.
            saberteethFeat.WithPermanentQEffect(null, delegate (QEffect self)
            {
                self.AdditionalUnarmedStrike = new Item(IllustrationName.Jaws, "Jaws", [Trait.Unarmed, Trait.Brawling])
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Piercing));
            });
        }

        /// <summary>
        /// Adds the Well-Met Traveler Logic
        /// </summary>
        /// <param name="wellMetTravelerFeat">The Well-Met Traveler feat</param>
        public static void AddWellMetTravelerLogic(TrueFeat wellMetTravelerFeat)
        {
            wellMetTravelerFeat.WithOnSheet((character) =>
            {
                if (!character.HasFeat(FeatName.Diplomacy))
                {
                    character.GrantFeat(FeatName.Diplomacy);
                }
            });

            wellMetTravelerFeat.WithPermanentQEffect("+2 Circumstance bonus to Diplomacy checks.", delegate (QEffect self)
            {
                self.BonusToSkillChecks = (Skill skill, CombatAction action, Creature? defender) =>
                {
                    if (skill == Skill.Diplomacy)
                    {
                        return new Bonus(2, BonusType.Circumstance, "Well-Met Traveler", true);
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the Light Paws Logic
        /// </summary>
        /// <param name="lightPawsFeat">The Light Paws feat</param>
        public static void AddLightPawsLogic(TrueFeat lightPawsFeat)
        {
            lightPawsFeat.WithPermanentQEffect("Stride then Step or Step then Stide, ignoring terrain.", delegate (QEffect self)
            {
                self.ProvideMainAction = (QEffect mainAction) =>
                {
                    Creature owner = mainAction.Owner;

                    QEffect createIgnoreTerrainEffect()
                    {
                        return new QEffect()
                        {
                            Id = QEffectId.IgnoresDifficultTerrain
                        };
                    }

                    async Task<bool> HandleMovement(Creature self, bool strideFirst)
                    {
                        QEffect ignoreTerrain = createIgnoreTerrainEffect();
                        self.AddQEffect(ignoreTerrain);
                        bool wasCanceled = false;
                        if (strideFirst)
                        {
                            wasCanceled = !(await self.StrideAsync("Choose where to Stride. (1/2).", allowCancel: true));
                            if (!wasCanceled)
                            {
                                await self.StepAsync("Choose where to Step. (2/2).");
                            }
                        }
                        else
                        {
                            wasCanceled = !(await self.StepAsync("Choose where to Step. (1/2).", allowCancel: true));
                            if (!wasCanceled)
                            {
                                await self.StrideAsync("Choose where to Stride. (2/2).");
                            }
                        }
                        ignoreTerrain.ExpiresAt = ExpirationCondition.Immediately;

                        return wasCanceled;
                    }

                    PossibilitySection lightPawsSection = new PossibilitySection("Light Paws");
                    CombatAction strideThenStep = new CombatAction(owner, IllustrationName.FleetStep, "Stride then Step", [], lightPawsFeat.RulesText, Target.Self())
                    .WithActionCost(2)
                    .WithEffectOnSelf(async (CombatAction action, Creature self) =>
                    {
                        action.RevertRequested = await HandleMovement(self, true);
                    });
                    CombatAction stepThenStride = new CombatAction(owner, IllustrationName.FleetStep, "Step then Stride", [], lightPawsFeat.RulesText, Target.Self())
                    .WithActionCost(2)
                    .WithEffectOnSelf(async (CombatAction action, Creature self) =>
                    {
                        action.RevertRequested = await HandleMovement(self, false);
                    });

                    lightPawsSection.AddPossibility(new ActionPossibility(strideThenStep));
                    lightPawsSection.AddPossibility(new ActionPossibility(stepThenStride));

                    SubmenuPossibility lightPawsMenu = new SubmenuPossibility(IllustrationName.FleetStep, "Light Paws");
                    lightPawsMenu.Subsections.Add(lightPawsSection);
                    return lightPawsMenu;
                };
            });
        }

        /// <summary>
        /// Adds the Lucky Break Logic
        /// </summary>
        /// <param name="luckyBreakFeat">The Lucky Break feat</param>
        public static void AddLuckyBreakLogic(TrueFeat luckyBreakFeat)
        {
            luckyBreakFeat.WithPermanentQEffect("Reroll a failed Reflex save once per day.", delegate (QEffect self)
            {
                self.RerollActiveRoll = async (QEffect activeRoll, CheckBreakdownResult result, CombatAction action, Creature defender) =>
                {
                    Creature owner = activeRoll.Owner;
                    Skill? skill = action.ActiveRollSpecification?.TaggedDetermineBonus.InvolvedSkill;

                    if (!owner.PersistentUsedUpResources.UsedUpActions.Contains("Cat's Luck") && result.CheckResult <= CheckResult.Failure && skill != null && (skill == Skill.Athletics || skill == Skill.Acrobatics) && await owner.AskForConfirmation(IllustrationName.FreeAction, "You failed the check for " + action.Name + ". Use {i}Cat's Luck{/i} to reroll the check and keep the better result?", "Yes"))
                    {
                        owner.PersistentUsedUpResources.UsedUpActions.Add("Cat's Luck");
                        return RerollDirection.RerollAndKeepBest;
                    }

                    return RerollDirection.DoNothing;
                };
            });
        }

        /// <summary>
        /// Adds the Springing Leaper Logic
        /// </summary>
        /// <param name="springingLeaperFeat">The Springing Leaper feat</param>
        public static void AddSpringingLeaperLogic(TrueFeat springingLeaperFeat)
        {
            springingLeaperFeat.WithOnSheet((character) =>
            {
                character.GrantFeat(FeatName.PowerfulLeap);
            });
            springingLeaperFeat.WithPermanentQEffect("You don't trigger reactions while Leaping.", delegate (QEffect self)
            {
                ModManager.RegisterActionOnEachActionPossibility(action =>
                {
                    if (action.Owner.HasFeat(CatfolkFeatNames.SpringingLeaper) && (action.Name.ToLower().StartsWith("leap")))
                    {
                        action.Traits.Add(Trait.DoesNotProvoke);
                    }
                });
            });
        }

        /// <summary>
        /// Adds the Well-Groomed Logic
        /// </summary>
        /// <param name="wellGroomedFeat">The Well-Groomed feat</param>
        public static void AddWellGroomedLogic(TrueFeat wellGroomedFeat)
        {
            wellGroomedFeat.WithPermanentQEffect("+1 Circumstance bonus to diseases saving throws and success becomes a critical.", delegate (QEffect self)
            {
                self.BonusToDefenses = (QEffect bonusToSave, CombatAction? action, Defense defense) =>
                {
                    if (action != null && action.SavingThrow != null && action.HasTrait(Trait.Disease))
                    {
                        return new Bonus(1, BonusType.Circumstance, "Well-Groomed", true);
                    }

                    return null;
                };
                self.AdjustSavingThrowCheckResult = (QEffect savingThrowBoost, Defense defense, CombatAction action, CheckResult result) =>
                {
                    if (action.HasTrait(Trait.Disease) && result == CheckResult.Success)
                    {
                        return CheckResult.CriticalSuccess;
                    }

                    return result;
                };
            });
        }

        /// <summary>
        /// Adds the Comfortable Cat Nap Logic
        /// </summary>
        /// <param name="comfortableCatNapFeat">The Comfortable Cat Nap feat</param>
        public static void AddComfortableCatNapLogic(TrueFeat comfortableCatNapFeat)
        {
            comfortableCatNapFeat.WithPermanentQEffect("More temp HP from Cat Nap after a long rest.", delegate (QEffect self)
            {
            });
        }
    }
}