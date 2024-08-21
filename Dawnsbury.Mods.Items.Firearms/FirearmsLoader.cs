using Dawnsbury.Modding;

namespace Dawnsbury.Mods.Items.Firearms
{
    /// <summary>
    /// Loads the Guns and Gears content
    /// </summary>
    public class FirearmsLoader
    {
        /// <summary>
        /// Runs on launch and loads all the mod content
        /// </summary>
        [DawnsburyDaysModMainMethod]
        public static void LoadMod()
        {
            Firearms.RegisterItems();
            Firearms.SetupTraitLogic();
            Firearms.PatchItems();
        }
    }
}
