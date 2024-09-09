using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Mods.Feats.Ancestries.Ratfolk.RegisteredComponents;
using System.Collections.Generic;

namespace Dawnsbury.Mods.Feats.Ancestries.Ratfolk
{
    public static class Ratfolk
    {
        public static IEnumerable<Feat> CreateRatfolkFeats()
        {
            HeritageSelectionFeat desertRatFeat = new HeritageSelectionFeat(RatfolkFeatNames.DesertRat, "You are native to arid plains and likely grew up traveling the roads.", "You have a leaner build than other ratfolk, with longer limbs and short fur. If you have both hands free, you can increase your Speed to 30 feet as you run on all fours.");
            // TODO: Logic
            yield return desertRatFeat;

            HeritageSelectionFeat longsnoutRatFeat = new HeritageSelectionFeat(RatfolkFeatNames.LongsnoutRat, "The long snouts that run in your family give you a keener sense of smell than most ratfolk.", "You gain a +2 circumstance bonus to Perception checks to Seek a creature or object within 30 feet.");
            // TODO: Logic
            yield return longsnoutRatFeat;

            HeritageSelectionFeat sewerRatFeat = new HeritageSelectionFeat(RatfolkFeatNames.SewerRat, "You come from a long line of ratfolk with a community based in the sewers beneath a large settlement.", "You gain a +1 circumstance bonus to saving throws against diseases and poisons. If you roll a success on a saving throw against a disease or poison, you get a critical success instead. If you have a different ability that would improve the save this way (such as the battle hardened fighter class feature), if you roll a critical failure on the save you get a failure instead.");
            // TODO: Logic
            yield return sewerRatFeat;

            HeritageSelectionFeat shadowRatFeat = new HeritageSelectionFeat(RatfolkFeatNames.ShadowRat, "Your ancestors lived in dark spaces underground, granting you dark fur and a vaguely unnatural mien.", "You become trained in Intimidation. When you Demoralize an animal you don't take a penalty for not sharing a language with it.");
            // TODO: Logic
            yield return shadowRatFeat;

            HeritageSelectionFeat snowRatFeat = new HeritageSelectionFeat(RatfolkFeatNames.SnowRat, "You have a thicker coat and bulkier frame to defend against the cold.", "You gain you cold resistance equal to half your level (minimum 1).");
            // TODO: Logic
            yield return snowRatFeat;

            AncestrySelectionFeat ratfolkFeat = new AncestrySelectionFeat(RatfolkFeatNames.Ratfolk,
                "Ysoki are beings that most surface-dwelling humanoids refer to as \"ratfolk.\" They are a communal people who prefer cramped conditions, with up to 100 individuals living in a given home. If they can't find homes in town, ratfolk may instead live in caves and cavern complexes, as these provide great storage for the many and varied goods they bring back from trading expeditions.",
                [RatfolkTraits.Ratfolk, Trait.Humanoid],
                6,
                5,
                [new EnforcedAbilityBoost(Ability.Dexterity), new EnforcedAbilityBoost(Ability.Intelligence), new FreeAbilityBoost()],
                [desertRatFeat, longsnoutRatFeat, sewerRatFeat, shadowRatFeat, snowRatFeat])
                .WithAbilityFlaw(Ability.Strength);
            // TODO: Add Sharp Teeth
            yield return ratfolkFeat;

            TrueFeat cheekPouchesFeat = new TrueFeat(RatfolkFeatNames.CheekPouches, 1, "Your cheeks are stretchy, and you can store up to four small items in these cheek pouches.", "The first four non-two handed items you draw within an encounter are a {icon: FreeAction} free action instead of an action.", [RatfolkTraits.Ratfolk]);
            // TODO: Logic
            yield return cheekPouchesFeat;

            TrueFeat ratfolkLoreFeat = new TrueFeat(RatfolkFeatNames.RatfolkLore, 1, "Years of experience among ratfolk communities have made you nimble, and you've learned to run and hide when enemies threaten.", "You gain the trained proficiency rank in Acrobatics and Stealth.", [RatfolkTraits.Ratfolk]);
            // TODO: Logic
            yield return ratfolkLoreFeat;

            TrueFeat viciousIncisorsFeat = new TrueFeat(RatfolkFeatNames.ViciousIncisors, 1, "You've let your incisors grow long enough to serve as formidable weapons", "Your jaws unarmed attack deals 1d6 piercing damage instead of 1d4, and gains the backstabber trait.", [RatfolkTraits.Ratfolk]);
            // TODO: Logic
            yield return viciousIncisorsFeat;

            TrueFeat agileTailFeat = new TrueFeat(RatfolkFeatNames.AgileTail, 1, "Your tail is long and nimble allowing you to catch others by surprise.", "You ignore the requirement of needing a free hand for the Trip action.", [RatfolkTraits.Ratfolk, Trait.Homebrew]);
            // TODO: Logic
            yield return agileTailFeat;

            TrueFeat slyApproachFeat = new TrueFeat(RatfolkFeatNames.SlyApproach, 1, "", "{b}Trigger{/b} You successfully Tumble Through an enemy.\n\nYou gain a +1 circumstance bonus to your AC against the creature you Tumbled Through.", [RatfolkTraits.Ratfolk, Trait.Homebrew]);
            // TODO: Logic
            yield return slyApproachFeat;

        }
    }
}