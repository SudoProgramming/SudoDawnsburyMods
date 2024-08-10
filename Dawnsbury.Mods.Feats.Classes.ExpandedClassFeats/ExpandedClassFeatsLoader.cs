using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
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
                    classSelectionFeat.RulesText = classSelectionFeat.RulesText.Replace("{b}2. Instinct.{/b} You select an instinct which is the source of your rage and grants you an additional power.\r\n\r\n{b}3. Barbarian feat.{/b}", "{b}2. Quick-Tempered.{/b} So long as you are able to move freely, your fury is instinctive and instantaneous. At the beginning of each encounter, you can enter rage as a free action if you are not wearing heavy armor.\r\n\r\n{b}3. Instinct.{/b} You select an instinct which is the source of your rage and grants you an additional power.\r\n\r\n{b}4. Barbarian feat.{/b}");

                    // Adds the QEffect at the start of combat to prompt for a free action rage if the user is not wearing heavy armor
                    // HACK: Currently Rage is NOT exposed via the modding dlls so reflection is being used to call the 'EnterRage' method. THIS SHOULD BE CHANGED IF 'EnterRage' IS EVER MADE PUBLIC
                    classSelectionFeat.WithPermanentQEffect("At the beginning of each encounter, you can enter rage as a free action", (QEffect qEffect) =>
                    {
                        qEffect.StartOfCombat = async (QEffect qEffect) =>
                        {
                            Creature owner = qEffect.Owner;
                            if (!owner.Armor.Item.Traits.Contains(Trait.HeavyArmor) && await owner.Battle.AskForConfirmation(owner, IllustrationName.Rage, "Enter {i}rage{/i} as a free action?", "Rage!"))
                            {
                                typeof(BarbarianFeatsDb).GetMethod("EnterRage", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { owner });
                            }
                        };
                    });
                }
            });
        }
    }
}
