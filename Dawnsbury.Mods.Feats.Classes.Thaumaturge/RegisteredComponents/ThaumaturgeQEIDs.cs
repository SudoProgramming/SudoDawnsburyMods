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
        public static readonly QEffectId ExploitVulnerabilityWeaknessTarget = ModManager.RegisterEnumMember<QEffectId>("Exploit Vulnerability - Weakness Target QEID");

        /// <summary>
        /// The Exploit Vulnerability - Weakness persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId ExploitVulnerabilityAntithesis = ModManager.RegisterEnumMember<QEffectId>("Exploit Vulnerability - Antithesis QEID");

        /// <summary>
        /// The Target's Exploit Vulnerability Antithesis persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId ExploitVulnerabilityAntithesisTarget = ModManager.RegisterEnumMember<QEffectId>("Exploit Vulnerability - Antithesis Target QEID");

        /// <summary>
        /// The Used Exploit Vulnerability persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId UsedExploitVulnerability = ModManager.RegisterEnumMember<QEffectId>("Exploit Vulnerability Used This Turn QEID");


        /// <summary>
        /// The Implement Inventory Tracker persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId ImplementInventoryTracker = ModManager.RegisterEnumMember<QEffectId>("Implement Inventory Tracker QEID");
    }
}
