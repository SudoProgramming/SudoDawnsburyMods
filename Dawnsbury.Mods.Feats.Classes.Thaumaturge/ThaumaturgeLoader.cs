using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Modding;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents;
using System.Collections.Generic;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge
{
    /// <summary>
    /// Loads the Thaumaturge content
    /// </summary>
    public class ThaumaturgeLoader
    {
        /// <summary>
        /// Runs on launch and loads all the mod content
        /// </summary>
        [DawnsburyDaysModMainMethod]
        public static void LoadMod()
        {
            AddFeats(Thaumaturge.CreateThaumaturgeFeats());
            LoadImplements();
            //AllFeats.All.ForEach(Thaumaturge.PatchFeat);
        }

        private static void LoadImplements()
        {
            ItemName amulet = ThaumaturgeItemNames.Amulet;
            ItemName bell = ThaumaturgeItemNames.Bell;
            ItemName chalice = ThaumaturgeItemNames.Chalice;
            ItemName lantern = ThaumaturgeItemNames.Lantern;
            ItemName mirror = ThaumaturgeItemNames.Mirror;
            ItemName regalia = ThaumaturgeItemNames.Regalia;
            ItemName tome = ThaumaturgeItemNames.Tome;
            ItemName wand = ThaumaturgeItemNames.Wand;
            ItemName weapon = ThaumaturgeItemNames.WeaponImplementChoice;
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