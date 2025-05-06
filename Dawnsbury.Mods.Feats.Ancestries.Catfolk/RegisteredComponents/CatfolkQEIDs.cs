using Dawnsbury.Core.Mechanics;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Ancestries.Catfolk.RegisteredComponents
{
    /// <summary>
    /// A static class containing all QEffect IDs used for the Ratfolk
    /// </summary>
    public static class CatfolkQEIDs
    {
        /// <summary>
        /// The Stable Trip persistent QEffect ID
        /// </summary>
        public static readonly QEffectId StableTrip = ModManager.RegisterEnumMember<QEffectId>("Stable Trip QEID");
    }
}
