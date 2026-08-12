using Dawnsbury.Core.CombatActions;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Classes.Gunslinger.RegisteredComponents
{
    /// <summary>
    /// A static class containing all Feat Names used for the Gunslinger
    /// </summary>
    public static class GunslingerActionIDs
    {
        /// <summary>
        /// The Warning Shot persistent Action ID
        /// </summary>
        public static readonly ActionId WarningShot = ModManager.RegisterEnumMember<ActionId>("Warning Shot AID");

        /// <summary>
        /// The Black Powder Boost persistent Action ID
        /// </summary>
        public static readonly ActionId BlackPowderBoost = ModManager.RegisterEnumMember<ActionId>("Black Powder Boost AID");

        /// <summary>
        /// The Pistolero's Challenge persistent Action ID
        /// </summary>
        public static readonly ActionId PistolerosChallenge = ModManager.RegisterEnumMember<ActionId>("Pistolero's Challenge AID");

        /// <summary>
        /// The Scatter Blast persistent Action ID
        /// </summary>
        public static readonly ActionId ScatterBlast = ModManager.RegisterEnumMember<ActionId>("Scatter Blast AID");
    }
}
