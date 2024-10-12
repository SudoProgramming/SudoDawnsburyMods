using Dawnsbury.Core.Mechanics;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents
{
    /// <summary>
    /// A static class containing all QEffect IDs used for Thaumaturge
    /// </summary>
    public static class ThaumaturgeQEIDs
    {
        /// <summary>
        /// The Exploit Vulnerability - Weakness persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId ExploitVulnerabilityWeakness = ModManager.RegisterEnumMember<QEffectId>("Exploit Vulnerability - Weakness QEID");

        /// <summary>
        /// The Target's Exploit Vulnerability Weakness persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId ExploitVulnerabilityTarget = ModManager.RegisterEnumMember<QEffectId>("Exploit Vulnerability - Weakness Target QEID");

        /// <summary>
        /// The Exploit Vulnerability - Weakness persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId ExploitVulnerabilityAntithesis = ModManager.RegisterEnumMember<QEffectId>("Exploit Vulnerability - Antithesis QEID");

        /// <summary>
        /// The Used Exploit Vulnerability persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId UsedExploitVulnerability = ModManager.RegisterEnumMember<QEffectId>("Exploit Vulnerability Used This Turn QEID");

        /// <summary>
        /// The Lantern Searching persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId LanternSearching = ModManager.RegisterEnumMember<QEffectId>("Lantern Searching QEID");

        /// <summary>
        /// The Location Tracking persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId LocationTracking = ModManager.RegisterEnumMember<QEffectId>("Location Tracking QEID");

        /// <summary>
        /// The Mirror Tracking persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId MirrorTracking = ModManager.RegisterEnumMember<QEffectId>("Mirror Tracking QEID");

        /// <summary>
        /// The Implement Inventory Tracker persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId ImplementInventoryTracker = ModManager.RegisterEnumMember<QEffectId>("Implement Inventory Tracker QEID");

        public static readonly QEffectId DraingedMirrorImplement = ModManager.RegisterEnumMember<QEffectId>("Drainged Mirror Implement QEID");
    }
}
