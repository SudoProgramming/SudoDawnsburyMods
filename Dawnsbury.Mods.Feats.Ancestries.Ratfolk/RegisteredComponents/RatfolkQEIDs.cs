using Dawnsbury.Core.Mechanics;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Ancestries.Ratfolk.RegisteredComponents
{
    /// <summary>
    /// A static class containing all QEffect IDs used for the Ratfolk
    /// </summary>
    public static class RatfolkQEIDs
    {
        /// <summary>
        /// The Agile Tail persistent QEffect ID
        /// </summary>
        public static readonly QEffectId AgileTail = ModManager.RegisterEnumMember<QEffectId>("Agile Tail QEID");

        /// <summary>
        /// The Cheek Pouches persistent QEffect ID
        /// </summary>
        public static readonly QEffectId CheekPouches = ModManager.RegisterEnumMember<QEffectId>("Cheek Pouches QEID");

        /// <summary>
        /// The Tumbling Trickster persistent QEffect ID
        /// </summary>
        public static readonly QEffectId TumblingTrickster = ModManager.RegisterEnumMember<QEffectId>("Tumbling Trickster QEID");
    }
}