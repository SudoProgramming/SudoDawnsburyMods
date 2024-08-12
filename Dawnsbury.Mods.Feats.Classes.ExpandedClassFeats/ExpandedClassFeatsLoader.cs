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
using Dawnsbury.Modding;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Dawnsbury.Mods.Feats.Classes.ExpandedClassFeats
{
    public class ExpandedClassFeatsLoader
    {
        /// <summary>
        /// Runs on launch and patches the feats
        /// </summary>
        [DawnsburyDaysModMainMethod]
        public static void LoadMod()
        {
            Debugger.Launch();
            PatchFeats();
        }

        /// <summary>
        /// Patches all feats that update an exisiting feat from this mod
        /// </summary>
        private static void PatchFeats()
        {
            AllFeats.All.ForEach((feat) =>
            {
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
                                if (!owner.Armor.Item.Traits.Contains(Trait.HeavyArmor) && await owner.Battle.AskForConfirmation(owner, IllustrationName.Rage, "Enter {i}rage{/i} as a free action?", "Rage!"))
                                {
                                    typeof(BarbarianFeatsDb).GetMethod("EnterRage", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { owner });
                                }
                            }
                        });
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
                                Creature x = self.Owner;
                                effectToCheck.BonusToDefenses = null;
                                effectToCheck.Description = effectToCheck.Description.Replace("You take a -1 penalty to AC.\n\n", string.Empty);
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
    }
}
