using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Classes.Barbarian.Remastered.RegisteredComponents
{
    public static class BarbarianRemasteredFeatNames
    {
        /// <summary>
        /// The Registered Giant Instict Feat Name
        /// </summary>
        public static readonly FeatName GiantInstict = ModManager.RegisterFeatName("Giant Instict");

        /// <summary>
        /// A list of the original Dragon Instincts in Dawnsbury
        /// </summary>
        public static readonly List<FeatName> OriginalDragonInstincts = new List<FeatName>() { FeatName.DragonInstinctFire, FeatName.DragonInstinctCold, FeatName.DragonInstinctElectricity, FeatName.DragonInstinctSonic, FeatName.DragonInstinctAcid };
    }
}
