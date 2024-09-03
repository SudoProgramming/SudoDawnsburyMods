using Dawnsbury.Audio;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Items.Firearms.RegisteredComponents
{
    public static class FirearmSFXNames
    {
        /// <summary>
        /// Adds the 1st small firearm sound effect
        /// </summary>
        public static readonly SfxName SmallFirearm1 = ModManager.RegisterNewSoundEffect("FirearmsAssets/Firearm_Small_SFX_1.wav");

        /// <summary>
        /// Adds the 2nd small firearm sound effect
        /// </summary>
        public static readonly SfxName SmallFirearm2 = ModManager.RegisterNewSoundEffect("FirearmsAssets/Firearm_Small_SFX_2.wav");

        /// <summary>
        /// Adds the 1st large firearm sound effect
        /// </summary>
        public static readonly SfxName LargeFirearm1 = ModManager.RegisterNewSoundEffect("FirearmsAssets/Firearm_Large_SFX_1.wav");

        /// <summary>
        /// Adds the 2nd large firearm sound effect
        /// </summary>
        public static readonly SfxName LargeFirearm2 = ModManager.RegisterNewSoundEffect("FirearmsAssets/Firearm_Large_SFX_2.wav");
    }
}
