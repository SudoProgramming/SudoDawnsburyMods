using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Modding;
using System.Collections.Generic;

namespace Dawnsbury.Mods.Feats.Classes.ExpandedClassFeats
{
    /// <summary>
    /// Updates and loads the Remastered changes into the game for the Barbarian
    /// </summary>
    public class BarbarianRemasteredLoader
    {
        /// <summary>
        /// Runs on launch and patches the feats
        /// </summary>
        [DawnsburyDaysModMainMethod]
        public static void LoadMod()
        {
            // First all feats that should be removed are removed, then update Rage, and finally creates and adds all the new Barbarian feats
            AllFeats.All.RemoveAll(BarbarianRemastered.ShouldFeatBeRemoved);
            AddFeats(BarbarianRemastered.CreateRemasteredBarbarianFeats());
            AllFeats.All.ForEach(BarbarianRemastered.PatchFeats);
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
