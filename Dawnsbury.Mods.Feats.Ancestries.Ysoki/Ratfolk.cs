using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Mods.Feats.Ancestries.Ratfolk.RegisteredComponents;
using System.Collections.Generic;
using System.Linq;

namespace Dawnsbury.Mods.Feats.Ancestries.Ratfolk
{
    public static class Ratfolk
    {
        public static IEnumerable<Feat> CreateRatfolkFeats()
        {
            HeritageSelectionFeat desertRatFeat = new HeritageSelectionFeat(RatfolkFeatNames.DesertRat, "You are native to arid plains and likely grew up traveling the roads.", "You have a leaner build than other ratfolk, with longer limbs and short fur. If you have both hands free, you can increase your Speed to 30 feet as you run on all fours.");
            AddDesertRatLogic(desertRatFeat);
            yield return desertRatFeat;

            HeritageSelectionFeat longsnoutRatFeat = new HeritageSelectionFeat(RatfolkFeatNames.LongsnoutRat, "The long snouts that run in your family give you a keener sense of smell than most ratfolk.", "You gain a +2 circumstance bonus to Perception checks to Seek a creature or object within 30 feet.");
            AddLongsnoutRatLogic(longsnoutRatFeat);
            yield return longsnoutRatFeat;

            HeritageSelectionFeat sewerRatFeat = new HeritageSelectionFeat(RatfolkFeatNames.SewerRat, "You come from a long line of ratfolk with a community based in the sewers beneath a large settlement.", "You gain a +1 circumstance bonus to saving throws against diseases and poisons. If you roll a success on a saving throw against a disease or poison, you get a critical success instead. If you have a different ability that would improve the save this way (such as the battle hardened fighter class feature), if you roll a critical failure on the save you get a failure instead.");
            AddSewerRatLogic(sewerRatFeat);
            yield return sewerRatFeat;

            HeritageSelectionFeat shadowRatFeat = new HeritageSelectionFeat(RatfolkFeatNames.ShadowRat, "Your ancestors lived in dark spaces underground, granting you dark fur and a vaguely unnatural mien.", "You become trained in Intimidation. When you Demoralize an animal you don't take a penalty for not sharing a language with it.");
            AddShadowRatLogic(shadowRatFeat);
            yield return shadowRatFeat;

            HeritageSelectionFeat snowRatFeat = new HeritageSelectionFeat(RatfolkFeatNames.SnowRat, "You have a thicker coat and bulkier frame to defend against the cold.", "You gain you cold resistance equal to half your level (minimum 1).");
            AddSnowRatLogic(snowRatFeat);
            yield return snowRatFeat;

            AncestrySelectionFeat ratfolkFeat = new AncestrySelectionFeat(RatfolkFeatNames.Ratfolk,
                "Ysoki are beings that most surface-dwelling humanoids refer to as \"ratfolk.\" They are a communal people who prefer cramped conditions, with up to 100 individuals living in a given home. If they can't find homes in town, ratfolk may instead live in caves and cavern complexes, as these provide great storage for the many and varied goods they bring back from trading expeditions.\n\n{b}Sharp Teeth{/b} Your prominent incisors offer an alternative to the fists other humanoids bring to a fight. You have a jaws unarmed attack that deals 1d4 piercing damage, is in the brawling group, and has the agile and finesse traits.",
                [RatfolkTraits.Ratfolk, Trait.Humanoid],
                6,
                5,
                [new EnforcedAbilityBoost(Ability.Dexterity), new EnforcedAbilityBoost(Ability.Intelligence), new FreeAbilityBoost()],
                [desertRatFeat, longsnoutRatFeat, sewerRatFeat, shadowRatFeat, snowRatFeat])
                .WithAbilityFlaw(Ability.Strength);
            AddSharpTeethLogic(ratfolkFeat);
            yield return ratfolkFeat;

            TrueFeat cheekPouchesFeat = new TrueFeat(RatfolkFeatNames.CheekPouches, 1, "Your cheeks are stretchy, and you can store up to four small items in these cheek pouches.", "The first two non-two handed items you draw within an encounter are a {icon: FreeAction} free action instead of an action.", [RatfolkTraits.Ratfolk]);
            // TODO: Logic
            yield return cheekPouchesFeat;

            TrueFeat ratfolkLoreFeat = new TrueFeat(RatfolkFeatNames.RatfolkLore, 1, "Years of experience among ratfolk communities have made you nimble, and you've learned to run and hide when enemies threaten.", "You gain the trained proficiency rank in Acrobatics and Stealth.", [RatfolkTraits.Ratfolk]);
            // TODO: Logic
            yield return ratfolkLoreFeat;

            yield return new TrueFeat(RatfolkFeatNames.ViciousIncisors, 1, "You've let your incisors grow long enough to serve as formidable weapons", "Your jaws unarmed attack deals 1d6 piercing damage instead of 1d4, and gains the backstabber trait.", [RatfolkTraits.Ratfolk]);

            TrueFeat agileTailFeat = new TrueFeat(RatfolkFeatNames.AgileTail, 1, "Your tail is long and nimble allowing you to catch others by surprise.", "You ignore the requirement of needing a free hand for the Trip action.", [RatfolkTraits.Ratfolk, Trait.Homebrew]);
            // TODO: Logic
            yield return agileTailFeat;

            TrueFeat slyApproachFeat = new TrueFeat(RatfolkFeatNames.SlyApproach, 1, "", "{b}Trigger{/b} You successfully Tumble Through an enemy.\n\nYou gain a +1 circumstance bonus to your AC against the creature you Tumbled Through.", [RatfolkTraits.Ratfolk, Trait.Homebrew]);
            // TODO: Logic
            yield return slyApproachFeat;
        }

        /// <summary>
        /// Adds the Sharp Teeth Logic and adjusts it if the user has the Vicious Incisors feat
        /// </summary>
        /// <param name="ratfolkFeat">The ratfolk feat</param>
        public static void AddSharpTeethLogic(AncestrySelectionFeat ratfolkFeat)
        {
            ratfolkFeat.WithPermanentQEffect(ratfolkFeat.FlavorText, delegate (QEffect self)
            {
                string teethDamage = "1d4";
                List<Trait> sharpTeethTraits = new List<Trait>() { Trait.Unarmed, Trait.Brawling, Trait.Agile, Trait.Finesse };
                if (self.Owner.HasFeat(RatfolkFeatNames.ViciousIncisors))
                {
                    teethDamage = "1d6";
                    sharpTeethTraits.Add(Trait.Backstabber);
                }

                self.AdditionalUnarmedStrike = new Item(IllustrationName.Jaws, "Ratfolk Sharp Teeth", sharpTeethTraits.ToArray())
                    .WithWeaponProperties(new WeaponProperties(teethDamage, DamageKind.Piercing));
            });
        }

        /// <summary>
        /// Adds the Desert Rat Logic
        /// </summary>
        /// <param name="desertRatFeat">The desert rat heritage feat</param>
        public static void AddDesertRatLogic(HeritageSelectionFeat desertRatFeat)
        {
            desertRatFeat.WithPermanentQEffect(desertRatFeat.FlavorText, delegate (QEffect self)
            {
                self.BonusToAllSpeeds = (QEffect bonusToSpeed) =>
                {
                    if (self.Owner.HeldItems.Count == 0 && self.Owner.Speed < 6)
                    {
                        return new Bonus(6 -  self.Owner.Speed, BonusType.Untyped, "Desert Rat", true);
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the Longsnout Rat Logic
        /// </summary>
        /// <param name="longsnoutRat">The longsnout rat heritage feat</param>
        public static void AddLongsnoutRatLogic(HeritageSelectionFeat longsnoutRat)
        {
            longsnoutRat.WithPermanentQEffect(longsnoutRat.FlavorText, delegate (QEffect self)
            {
                self.BonusToAttackRolls = (QEffect bonusToSeek, CombatAction action, Creature? creature) =>
                {
                    if (action.ActionId == ActionId.Seek && creature != null && creature.DetectionStatus.Undetected && self.Owner.DistanceTo(creature) <= 6)
                    {
                        return new Bonus(2, BonusType.Circumstance, "Longsnout Rat", true);
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the Sewer Rat Logic
        /// </summary>
        /// <param name="sewerRat">The sewer rat heritage feat</param>
        public static void AddSewerRatLogic(HeritageSelectionFeat sewerRat)
        {
            sewerRat.WithPermanentQEffect(sewerRat.FlavorText, delegate (QEffect self)
            {
                self.AdjustSavingThrowResult = (QEffect effectBonus, CombatAction action, CheckResult result) =>
                {
                    if (action != null && action.HasTrait(Trait.Poison))
                    {
                        if (result == CheckResult.Success)
                        {
                            return result.ImproveByOneStep();
                        }
                        // HACK: If needed in the future adjust Critical Failures to Failures
                    }

                    return result;
                };

                self.BonusToDefenses = (QEffect savingThrowBonus, CombatAction? action, Defense defense) =>
                {
                    if (action != null && (defense == Defense.Fortitude || defense == Defense.Reflex || defense == Defense.Will) && action.HasTrait(Trait.Poison))
                    {
                        return new Bonus(1, BonusType.Circumstance, "Sewer Rat", true);
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the Shadow Rat Logic
        /// </summary>
        /// <param name="shadowRat">The shadow rat heritage feat</param>
        public static void AddShadowRatLogic(HeritageSelectionFeat shadowRat)
        {
            shadowRat.WithPermanentQEffect(shadowRat.FlavorText, delegate (QEffect self)
            {
                self.YouBeginAction = async (QEffect adjustAction, CombatAction action) =>
                {
                    if (action.ActionId == ActionId.Demoralize && action.ChosenTargets.GetAllTargetCreatures().Any(creature => creature.HasTrait(Trait.Animal)))
                    {
                        StrikeModifiers modifiers = action.StrikeModifiers ?? new StrikeModifiers();
                        QEffect qEffectForStrike = modifiers.QEffectForStrike ?? new QEffect();
                        qEffectForStrike.Id = QEffectId.IntimidatingGlare;
                    }
                };
            });
        }

        /// <summary>
        /// Adds the Snow Rat Logic
        /// </summary>
        /// <param name="snowRat">The snow rat heritage feat</param>
        public static void AddSnowRatLogic(HeritageSelectionFeat snowRat)
        {
            snowRat.WithPermanentQEffect(snowRat.FlavorText, delegate (QEffect self)
            {
                self.Owner.WeaknessAndResistance.AddResistance(DamageKind.Cold, 1);
            });
        }
    }
}