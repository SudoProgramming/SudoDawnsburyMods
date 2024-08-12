using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Roller;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using static Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.BarbarianFeatsDb;

namespace Dawnsbury.Mods.Feats.Classes.ExpandedClassFeats
{
    /// <summary>
    /// The Remastered Barbarian Class
    /// </summary>
    public static class BarbarianRemastered
    {
        /// <summary>
        /// A list of the original Dragon Instincts in Dawnsbury
        /// </summary>
        private static readonly List<FeatName> originalDragonInstincts = new List<FeatName>() { FeatName.DragonInstinctFire, FeatName.DragonInstinctCold, FeatName.DragonInstinctElectricity, FeatName.DragonInstinctSonic, FeatName.DragonInstinctAcid };

        /// <summary>
        /// Determines if the provided feat should be removed
        /// </summary>
        /// <param name="possibleFeatToRemove">The feat being considered for removal</param>
        /// <returns>True if the feat should be removed and false otherwise</returns>
        public static bool ShouldFeatBeRemoved(Feat possibleFeatToRemove)
        {
            // 2nd Level Second Wind, 4th Level Fast Movement, and all original Dragon Instincts
            return possibleFeatToRemove.FeatName == FeatName.SecondWind || possibleFeatToRemove.FeatName == FeatName.FastMovement || (possibleFeatToRemove is DragonInstinctFeat && originalDragonInstincts.Contains(possibleFeatToRemove.FeatName));
        }

        /// <summary>
        /// Creates the Remastered Barbaian Feats
        /// </summary>
        /// <returns>The Enumerable of Barbarian Feats</returns>
        public static IEnumerable<Feat> CreateRemasteredBarbarianFeats()
        {
            // All Remastered Dragon Instincts
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Adamantine dragon"), DamageKind.Bludgeoning);
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Conspirator dragon"), DamageKind.Poison);
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Diabolic dragon"), DamageKind.Fire);
            // TODO: When spirit damage is added ad Empureal
            //yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Empureal dragon"), DamageKind.Spirit);
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Fortune dragon"), DamageKind.Force);
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Horned dragon"), DamageKind.Poison);
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Mirage dragon"), DamageKind.Mental);
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Omen dragon"), DamageKind.Mental);

            // Class Level 4 Feat - Scars of Steel
            yield return new TrueFeat(ModManager.RegisterFeatName("Scars of Steel"), 4, "When you are struck with the mightiest of blows, you can flex your muscles to turn aside some of the damage.", "Once per day, when an opponent critically hits you with an attack that deals physical damage, you can spend a reaction to gain resistance to the triggering attack equal to your Constitution modifier plus half your level.", new Trait[] { Trait.Barbarian, Trait.Rage })
            .WithActionCost(-2).WithPermanentQEffect("You gain resistance to the triggering attack equal to your Constitution modifier plus half your level as a reaction.", delegate (QEffect qf)
            {
                // Checks the incoming damage and prompts for reaction if it's a crit and physical. Then applies the damage reduction
                qf.YouAreDealtDamage = async (QEffect qEffect, Creature attacker, DamageStuff damage, Creature defender) =>
                {
                    int possibleResistance = qEffect.Owner.Abilities.Constitution + (int)Math.Floor(qEffect.Owner.Level / 2.0);
                    if (damage.Kind.IsPhysical() && damage.Power != null && damage.Power.CheckResult == CheckResult.CriticalSuccess && damage.Power.HasTrait(Trait.Attack) && await qf.Owner.Battle.AskToUseReaction(qf.Owner, "You were critically hit for a total damage of " + damage.Amount + ".\nUse Scars of Steel to gain " + possibleResistance + " damage resistence?"))
                    {
                        return new ReduceDamageModification(possibleResistance, "You reduced " + possibleResistance + " damage from the incoming damage.");
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Patches all feats for the Barbarian Remaster
        /// </summary>
        /// <param name="feat">The feat to patch</param>
        public static void PatchFeats(Feat feat)
        {
            // Patches Intimidating Stike to be selectable by Barbarians
            if (feat.FeatName == FeatName.IntimidatingStrike)
            {
                PatchIntimidatingStrike(feat);
            }

            // Updates the 'Barbarian' class feature to include the 'Quick-Tempered' feature.
            if (feat.FeatName == FeatName.Barbarian && feat is ClassSelectionFeat classSelectionFeat)
            {
                // Add Quick-Tempered to the Barbarian
                AddQuickTemperedToClassSelection(classSelectionFeat);

                // Replace Deny Advantage and add Furious Footfalls to the Barbarian
                ReplaceDenyAdvantageWithFuriousFootfalslToClassSelection(classSelectionFeat);

                // Updates the 'Fury Instinct' sub class to match the Remaster
                classSelectionFeat.Subfeats.ForEach(subClass =>
                {
                    if (subClass.Name == "Fury Instinct")
                    {
                        UpdateFuryInstict(subClass);
                    }
                });

                // Updates Rage to match the Remaster
                UpdateRage(classSelectionFeat);

                // Updates all text descriptions for the Barbarian
                UpdateAllTextDescriptions(classSelectionFeat);
            }
        }

        /// <summary>
        /// Adds the Quick Tempered base class Feature
        /// </summary>
        /// <param name="classSelectionFeat">The Barbarian Class Selection Feat</param>
        private static void AddQuickTemperedToClassSelection(ClassSelectionFeat classSelectionFeat)
        {
            // Adds the QEffect at the start of combat to prompt for a free action rage if the user is not wearing heavy armor
            classSelectionFeat.WithOnCreature(creature =>
            {
                creature.AddQEffect(new QEffect("Quick-Tempered", "At the beginning of each encounter, you can enter rage as a free action if you are not wearing heavy armor.")
                {
                    StartOfCombat = async (QEffect qEffect) =>
                    {
                        Creature owner = qEffect.Owner;
                        if (!owner.Armor.Item.Traits.Contains(Trait.HeavyArmor) && await owner.Battle.AskForConfirmation(owner, IllustrationName.Rage, "Enter rage as a free action?", "Rage!"))
                        {
                            BarbarianFeatsDb.EnterRage(owner);
                        }
                    }
                });
            });
        }

        /// <summary>
        /// Adds the Furious Footfalls base class feature
        /// </summary>
        /// <param name="classSelectionFeat">The Barbarian Class Selection Feat</param>
        private static void ReplaceDenyAdvantageWithFuriousFootfalslToClassSelection(ClassSelectionFeat classSelectionFeat)
        {
            // Replaces Deny Advantage with Furious Footfalls as a base class feature
            classSelectionFeat.WithPermanentQEffect(null, (QEffect qEffect) =>
            {
                qEffect.StartOfCombat = async (QEffect self) =>
                {
                    self.Owner.RemoveAllQEffects((QEffect qf) => qf.Id == QEffectId.DenyAdvantage);
                    if (self.Owner.Level >= 3)
                    {
                        self.Owner.AddQEffect(new QEffect("Furious Footfalls", "You gain a +5-foot status bonus to your Speed. This bonus increases to +10 feet while you're raging.")
                        {
                            BonusToAllSpeeds = (QEffect qEffect) =>
                            {
                                int speedBonus = (qEffect.Owner.HasEffect(QEffectId.Rage)) ? 2 : 1;
                                return new Bonus(speedBonus, BonusType.Status, "Furious Footfalls", true);
                            }
                        });
                    }
                };
            });
        }

        /// <summary>
        /// Updates the 'Fury Instinct' sub class to match the Remaster
        /// </summary>
        /// <param name="classSelectionFeat">The Barbarian Class Selection Feat</param>
        private static void UpdateFuryInstict(Feat furyInstictFeat)
        {
            // Updates the Rules Text and adds the updated effect
            furyInstictFeat.RulesText = "Increase the additional damage from Rage from 2 to 3. " + furyInstictFeat.RulesText;
            furyInstictFeat.WithOnCreature(delegate (Creature cr)
            {
                cr.AddQEffect(new QEffect
                {
                    // ??? Not sure mostly copied from Dragon Instict
                    StateCheck = delegate (QEffect sc)
                    {
                        QEffect qEffect = sc.Owner.QEffects.FirstOrDefault((QEffect qfId) => qfId.Id == QEffectId.Rage);
                        if (qEffect != null)
                        {
                            qEffect.YouDealDamageWithStrike = null;
                        }
                    },

                    // ??? Not sure mostly copied from Dragon Instict
                    AddExtraStrikeDamage = delegate (CombatAction attack, Creature defender)
                    {
                        Creature owner = attack.Owner;
                        bool doesRageApplyToAcction = (bool)typeof(BarbarianFeatsDb).GetMethod("DoesRageApplyToAction", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { attack });
                        if (owner.HasEffect(QEffectId.Rage) && doesRageApplyToAcction && attack.Item != null)
                        {
                            List<DamageKind> list = attack.Item.DetermineDamageKinds();
                            DamageKind damageTypeToUse = defender.WeaknessAndResistance.WhatDamageKindIsBestAgainstMe(list);
                            DiceFormula item3 = DiceFormula.FromText((attack.HasTrait(Trait.Unarmed) || attack.HasTrait(Trait.Agile)) ? "1" : "3", (attack.HasTrait(Trait.Unarmed) || attack.HasTrait(Trait.Agile)) ? "Barbarian rage (agile)" : "Barbarian rage");
                            return (item3, damageTypeToUse);
                        }

                        return null;
                    }
                });
            });
        }

        /// <summary>
        /// Updates Rage to match the Remaster
        /// NOTE: Temp HP is added every time regardless if enough time has passed
        /// </summary>
        /// <param name="classSelectionFeat">The Barbarian Class Selection Feat</param>
        private static void UpdateRage(ClassSelectionFeat classSelectionFeat)
        {
            classSelectionFeat.WithPermanentQEffect(null, (QEffect qEffect) =>
            {
                qEffect.YouAcquireQEffect = (QEffect self, QEffect effectToCheck) =>
                {
                    if (effectToCheck.Name == "Rage")
                    {
                        effectToCheck.BonusToDefenses = null;
                        effectToCheck.Description = effectToCheck.Description.Replace("You take a -1 penalty to AC.\n\n", string.Empty);
                    }
                    else if (effectToCheck.Id == QEffectId.HasRagedThisEncounter)
                    {
                        effectToCheck = null;
                    }
                    return effectToCheck;
                };
            });
        }

        /// <summary>
        /// Updates all the text descriptions for the Barbarian changes
        /// </summary>
        /// <param name="classSelectionFeat">The Barbarian Class Selection Feat</param>
        private static void UpdateAllTextDescriptions(ClassSelectionFeat classSelectionFeat)
        {
            // Updates the 'Rage' text for the combat rage button
            ModManager.RegisterActionOnEachActionPossibility(action =>
            {
                // Updates the Class Features description
                classSelectionFeat.RulesText = classSelectionFeat.RulesText.Replace("{b}2. Instinct.{/b} You select an instinct which is the source of your rage and grants you an additional power.\r\n\r\n{b}3. Barbarian feat.{/b}", "{b}2. Quick-Tempered.{/b} So long as you are able to move freely, your fury is instinctive and instantaneous. At the beginning of each encounter, you can enter rage as a free action if you are not wearing heavy armor.\r\n\r\n{b}3. Instinct.{/b} You select an instinct which is the source of your rage and grants you an additional power.\r\n\r\n{b}4. Barbarian feat.{/b}")
                .Replace("you take a -1 penalty to AC and you can't use concentrate actions.", "you can't use concentrate actions.")
                .Replace("Deny advantage {i}(you aren't flat-footed to hidden or flanking creatures of your level or lower){/i}", "Furious Footfalls {i}You gain a +5-foot status bonus to your Speed. This bonus increases to +10 feet while you're raging.{/i}");

                // Updates the 'Rage' button description in combat
                if (action.Name == "Rage")
                {
                    action.Description = action.Description.Replace("• You take a -1 penalty to AC.\n", string.Empty);
                    action.ShortDescription = action.ShortDescription.Replace(" and a penalty to AC until the end of the encounter", string.Empty);
                }
            });
        }

        /// <summary>
        /// Patches Intimidating Stike to be selectable by Barbarians
        /// </summary>
        /// <param name="intimidatingStrikeFeat">The Intimidating Strike feat</param>
        private static void PatchIntimidatingStrike(Feat intimidatingStrikeFeat)
        {
            // Adds the Barbarian trait and cycles through the Class Prerequisites that don't have Barbarian and adds it
            intimidatingStrikeFeat.Traits.Add(Trait.Barbarian);
            for (int i = 0; i < intimidatingStrikeFeat.Prerequisites.Count; i++)
            {
                Prerequisite prereq = intimidatingStrikeFeat.Prerequisites[i];
                if (prereq is ClassPrerequisite classPrerequisite)
                {
                    if (!classPrerequisite.AllowedClasses.Contains(Trait.Barbarian))
                    {
                        List<Trait> updatedAllowedClasses = classPrerequisite.AllowedClasses;
                        updatedAllowedClasses.Add(Trait.Barbarian);
                        intimidatingStrikeFeat.Prerequisites[i] = new ClassPrerequisite(updatedAllowedClasses);
                    }
                }
            }
        }
    }
}
