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
using static Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.BarbarianFeatsDb;

namespace Dawnsbury.Mods.Feats.Classes.ExpandedClassFeats
{
    /// <summary>
    /// Updates and loads the Remastered changes into the game for the Barbarian
    /// </summary>
    public class BarbarianRemasteredLoader
    {
        /// <summary>
        /// A list of the original Dragon Instincts in Dawnsbury
        /// </summary>
        private static readonly List<FeatName> originalDragonInstincts = new List<FeatName>() { FeatName.DragonInstinctFire, FeatName.DragonInstinctCold, FeatName.DragonInstinctElectricity, FeatName.DragonInstinctSonic, FeatName.DragonInstinctAcid };

        /// <summary>
        /// Runs on launch and patches the feats
        /// </summary>
        [DawnsburyDaysModMainMethod]
        public static void LoadMod()
        {
            PatchFeats();
            AddFeats(BarbarianRemastered.CreateRemasteredBarbarianFeats());
        }

        /// <summary>
        /// Patches all feats that update an exisiting feat from this mod
        /// </summary>
        private static void PatchFeats()
        {
            AllFeats.All.RemoveAll((feat) => feat.FeatName == FeatName.SecondWind || feat.FeatName == FeatName.FastMovement || (feat is DragonInstinctFeat && originalDragonInstincts.Contains(feat.FeatName)));
            ModManager.RegisterActionOnEachActionPossibility(action =>
            {
                if (action.Name == "Rage")
                {
                    action.Description = action.Description.Replace("• You take a -1 penalty to AC.\n", string.Empty);
                    action.ShortDescription = action.ShortDescription.Replace(" and a penalty to AC until the end of the encounter", string.Empty);
                }
            });
            AllFeats.All.ForEach((feat) =>
            {
                if (feat.FeatName == FeatName.IntimidatingStrike)
                {
                    feat.Traits.Add(Trait.Barbarian);
                    for (int i = 0; i < feat.Prerequisites.Count; i++)
                    {
                        Prerequisite prereq = feat.Prerequisites[i];
                        if (prereq is ClassPrerequisite classPrerequisite)
                        {
                            if (!classPrerequisite.AllowedClasses.Contains(Trait.Barbarian))
                            {
                                List<Trait> updatedAllowedClasses = classPrerequisite.AllowedClasses;
                                updatedAllowedClasses.Add(Trait.Barbarian);
                                feat.Prerequisites[i] = new ClassPrerequisite(updatedAllowedClasses);
                            }
                        }
                    }
                }
                // Updates the 'Barbarian' class feature to include the 'Quick-Tempered' feature.
                if (feat.FeatName == FeatName.Barbarian && feat is ClassSelectionFeat classSelectionFeat)
                {
                    // Updates the Class Features description to include 'Quick-Tempered'
                    classSelectionFeat.RulesText = classSelectionFeat.RulesText.Replace("{b}2. Instinct.{/b} You select an instinct which is the source of your rage and grants you an additional power.\r\n\r\n{b}3. Barbarian feat.{/b}", "{b}2. Quick-Tempered.{/b} So long as you are able to move freely, your fury is instinctive and instantaneous. At the beginning of each encounter, you can enter rage as a free action if you are not wearing heavy armor.\r\n\r\n{b}3. Instinct.{/b} You select an instinct which is the source of your rage and grants you an additional power.\r\n\r\n{b}4. Barbarian feat.{/b}")
                    .Replace("you take a -1 penalty to AC and you can't use concentrate actions.", "you can't use concentrate actions.")
                    .Replace("Deny advantage {i}(you aren't flat-footed to hidden or flanking creatures of your level or lower){/i}", "Furious Footfalls {i}You gain a +5-foot status bonus to your Speed. This bonus increases to +10 feet while you're raging.{/i}");

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

                    classSelectionFeat.Subfeats.ForEach(subClass =>
                    {
                        if (subClass.Name == "Fury Instinct")
                        {
                            subClass.RulesText = "Increase the additional damage from Rage from 2 to 3. " + subClass.RulesText;
                            var zz = subClass.RulesText;
                            var aa = subClass;
                            subClass.WithOnCreature(delegate (Creature cr)
                            {
                                cr.AddQEffect(new QEffect
                                {
                                    StateCheck = delegate (QEffect sc)
                                    {
                                        QEffect qEffect = sc.Owner.QEffects.FirstOrDefault((QEffect qfId) => qfId.Id == QEffectId.Rage);
                                        if (qEffect != null)
                                        {
                                            qEffect.YouDealDamageWithStrike = null;
                                        }
                                    },
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
                    });

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
            });
        }

        /// <summary>
        /// Adds the provided feats via the ModManager
        /// </summary>
        /// <param name="feats">The feats to add</param>
        private static void AddFeats(IEnumerable<Feat> feats)
        {
            foreach (Feat feat in feats)
            {
                ModManager.AddFeat(feat);
            }
        }
    }
}
