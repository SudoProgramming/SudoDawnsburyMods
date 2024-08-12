using Dawnsbury.Auxiliary;
using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Modding;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using static Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.BarbarianFeatsDb;

namespace Dawnsbury.Mods.Feats.Classes.ExpandedClassFeats
{
    public class ExpandedClassFeatsLoader
    {
        private static readonly List<FeatName> originalDragonInstincts = new List<FeatName>() { FeatName.DragonInstinctFire, FeatName.DragonInstinctCold, FeatName.DragonInstinctElectricity, FeatName.DragonInstinctSonic, FeatName.DragonInstinctAcid };

        /// <summary>
        /// Runs on launch and patches the feats
        /// </summary>
        [DawnsburyDaysModMainMethod]
        public static void LoadMod()
        {
            Debugger.Launch();
            PatchFeats();
            AddFeats(CreateRemasteredBarbarianFeats());
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
                    // HACK: Currently Rage is NOT exposed via the modding dlls so reflection is being used to call the 'EnterRage' method. THIS SHOULD BE CHANGED IF 'EnterRage' IS EVER MADE PUBLIC
                    classSelectionFeat.WithOnCreature(creature =>
                    {
                        creature.AddQEffect(new QEffect("Quick-Tempered", "At the beginning of each encounter, you can enter rage as a free action if you are not wearing heavy armor.")
                        {
                            StartOfCombat = async (QEffect qEffect) =>
                            {
                                Creature owner = qEffect.Owner;
                                if (!owner.Armor.Item.Traits.Contains(Trait.HeavyArmor) && await owner.Battle.AskForConfirmation(owner, IllustrationName.Rage, "Enter rage as a free action?", "Rage!"))
                                {
                                    typeof(BarbarianFeatsDb).GetMethod("EnterRage", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { owner });
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
                        //qEffect.YouAcquireQEffect = (QEffect self, QEffect effectToCheck) =>
                        //{
                        //    if (effectToCheck.Id == QEffectId.DenyAdvantage)
                        //    {
                        //        return new QEffect("Furious Footfalls", "You gain a +5-foot status bonus to your Speed. This bonus increases to +10 feet while you're raging.")
                        //        {
                        //            BonusToAllSpeeds = (QEffect qEffect) =>
                        //            {
                        //                int speedBonus = (qEffect.Owner.HasEffect(QEffectId.Rage)) ? 10 : 5;
                        //                return new Bonus(speedBonus, BonusType.Status, "Furious Footfalls", true);
                        //            }
                        //        };
                        //    }

                        //    return effectToCheck;
                        //};
                    });
                    //classSelectionFeat.WithOnCreature(creature =>
                    //{
                    //    creature.AddQEffect(new QEffect("Updated Rage", "Update")
                    //    {
                    //        YouAcquireQEffect = (QEffect updateRageEffect, QEffect effectToCheck) =>
                    //        {
                    //            if(effectToCheck.Name == "Rage")
                    //            {
                    //                effectToCheck.BonusToDefenses = null;
                    //            }
                    //            return effectToCheck;
                    //        }
                    //    });
                    //});
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

                    //classSelectionFeat.WithPermanentQEffect(null, (QEffect qEffect) =>
                    //{
                    //    qEffect.StartOfCombat = async (QEffect qEffect) =>
                    //    {
                    //        qEffect.Owner.Possibilities.Sections.ForEach(section =>
                    //        {
                    //            if (section.Name == "Main actions")
                    //            {
                    //                section.Possibilities.Where(possibility => possibility.Caption == "Rage" && possibility is ActionPossibility).ForEach(action =>
                    //                {
                    //                    ActionPossibility origionalPossibility = (ActionPossibility)action;
                    //                    string updatedDescription = origionalPossibility.CombatAction.Description.Replace("• You take a -1 penalty to AC.\n", string.Empty);
                    //                    string updatedShortDescription = origionalPossibility.CombatAction.ShortDescription.Replace(" and a penalty to AC until the end of the encounter", string.Empty);
                    //                    CombatAction updatedAction = origionalPossibility.CombatAction;
                    //                    updatedAction.Description = updatedDescription;
                    //                    updatedAction.ShortDescription = updatedShortDescription;
                    //                    ActionPossibility updatedPossibility = new ActionPossibility(updatedAction);

                    //                    action = updatedPossibility;

                    //                });
                    //            }

                    //            qEffect.Owner.QEffects.ForEach(qE =>
                    //            {
                    //                if (qE.ProvideMainAction != null)
                    //                {
                    //                    var ma = qE.ProvideMainAction;
                    //                    int z = 0;
                    //                }
                    //            });
                    //            qEffect.Owner.RegeneratePossibilities();
                    //        });
                    //    };
                    //});
                }
            });
        }

        private static void AddFeats(IEnumerable<Feat> feats)
        {
            foreach (Feat feat in feats)
            {
                ModManager.AddFeat(feat);
            }
        }

        private static IEnumerable<Feat> CreateRemasteredBarbarianFeats()
        {
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Adamantine dragon"), DamageKind.Bludgeoning);
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Conspirator dragon"), DamageKind.Poison);
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Diabolic dragon"), DamageKind.Fire);
            // TODO: When spirit damage is added ad Empureal
            //yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Empureal dragon"), DamageKind.Spirit);
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Fortune dragon"), DamageKind.Force);
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Horned dragon"), DamageKind.Poison);
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Mirage dragon"), DamageKind.Mental);
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Omen dragon"), DamageKind.Mental);

            yield return new TrueFeat(ModManager.RegisterFeatName("Scars of Steel"), 4, "When you are struck with the mightiest of blows, you can flex your muscles to turn aside some of the damage.", "Once per day, when an opponent critically hits you with an attack that deals physical damage, you can spend a reaction to gain resistance to the triggering attack equal to your Constitution modifier plus half your level.", new Trait[] { Trait.Barbarian, Trait.Rage })
            .WithActionCost(-2).WithPermanentQEffect("You gain resistance to the triggering attack equal to your Constitution modifier plus half your level as a reaction.", delegate (QEffect qf)
            {
                qf.YouAreDealtDamage = async (QEffect qEffect, Creature attacker, DamageStuff damage, Creature defender) =>
                {
                    int possibleResistance = qEffect.Owner.Abilities.Constitution + (int)Math.Floor(qEffect.Owner.Level / 2.0);
                    if (damage.Kind.IsPhysical() && damage.Power != null && damage.Power.CheckResult == CheckResult.CriticalSuccess && damage.Power.HasTrait(Trait.Attack) && await qf.Owner.Battle.AskToUseReaction(qf.Owner, "You were critically hit for a total damage of " + damage.Amount + ".\nUse Scars of Steel to gain " + possibleResistance + " damage resistence?"))
                    {
                        return new ReduceDamageModification(possibleResistance, "You reduced " + possibleResistance + " damage from the incoming damage.");
                    }

                    return null;
                };
                //qf.YouAreTargeted = async delegate (QEffect qf, CombatAction attack)
                //{
                //    if (attack.HasTrait(Trait.Attack) && qf.Owner.CanSee(attack.Owner) && !attack.HasTrait(Trait.AttackDoesNotTargetAC) && await qf.Owner.Battle.AskToUseReaction(qf.Owner, "You're targeted by " + attack.Owner.Name + "'s " + attack.Name + ".\nUse Nimble Dodge to gain a +2 circumstance bonus to AC?"))
                //    {
                //        qf.Owner.AddQEffect(new QEffect
                //        {
                //            ExpiresAt = ExpirationCondition.EphemeralAtEndOfImmediateAction,
                //            BonusToDefenses = (QEffect effect, CombatAction? action, Defense defense) => (defense != 0) ? null : new Bonus(2, BonusType.Circumstance, "Nimble Dodge")
                //        });
                //    }
                //};
            });
        }
    }
}
