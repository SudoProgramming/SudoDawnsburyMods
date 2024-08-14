using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Modding;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dawnsbury.Mods.GunsAndGears
{
    /// <summary>
    /// Loads the Guns and Gears content
    /// </summary>
    public class GunsAndGearsLoader
    {
        /// <summary>
        /// Runs on launch and loads all the mod content
        /// </summary>
        [DawnsburyDaysModMainMethod]
        public static void LoadMod()
        {
            Debugger.Launch();
            EquipmentGunsAndGears.RegisterItems();
            EquipmentGunsAndGears.SetupTraitLogic();
        }
    }
}
