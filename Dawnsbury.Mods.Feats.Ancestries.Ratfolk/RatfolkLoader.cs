using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Modding;
using System.Collections.Generic;

namespace Dawnsbury.Mods.Feats.Ancestries.Ratfolk
{
    /// <summary>
    /// Loads the Ratfolk content
    /// </summary>
    public class RatfolkLoader
    {
        /// <summary>
        /// Runs on launch and loads all the mod content
        /// </summary>
        [DawnsburyDaysModMainMethod]
        public static void LoadMod()
        {
            AddFeats(Ratfolk.CreateRatfolkFeats());
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