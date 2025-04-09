using Dawnsbury.Core.Mechanics;
using Dawnsbury.Modding;

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

        /// <summary>
        /// The Mirror Immunity persistent QEffect ID
        /// </summary>
        public static readonly QEffectId MirrorImmunity = ModManager.RegisterEnumMember<QEffectId>("Mirror Immunity QEID");

        /// <summary>
        /// The Drainged Mirror Implement persistent QEffect ID
        /// </summary>
        public static readonly QEffectId DraingedMirrorImplement = ModManager.RegisterEnumMember<QEffectId>("Drainged Mirror Implement QEID");

        /// <summary>
        /// The Boosted Wand Used persistent QEffect ID
        /// </summary>
        public static readonly QEffectId BoostedWandUsed = ModManager.RegisterEnumMember<QEffectId>("Boosted Wand Used QEID");

        /// <summary>
        /// The Held Scroll And Implement persistent QEffect ID
        /// </summary>
        public static readonly QEffectId HeldScrollAndImplement = ModManager.RegisterEnumMember<QEffectId>("Held Scroll And Implement QEID");

        /// <summary>
        /// The Esoteric Warden AC persistent QEffect ID
        /// </summary>
        public static readonly QEffectId EsotericWardenAC = ModManager.RegisterEnumMember<QEffectId>("Esoteric Warden AC QEID");

        /// <summary>
        /// The Esoteric Warden Save persistent QEffect ID
        /// </summary>
        public static readonly QEffectId EsotericWardenSave = ModManager.RegisterEnumMember<QEffectId>("Esoteric Warden Save QEID");

        /// <summary>
        /// The Instructive Strike persistent QEffect ID
        /// </summary>
        public static readonly QEffectId InstructiveStrike = ModManager.RegisterEnumMember<QEffectId>("Instructive Strike QEID");

        /// <summary>
        /// The Esoteric Seller persistent QEffect ID
        /// </summary>
        public static readonly QEffectId EsotericSeller = ModManager.RegisterEnumMember<QEffectId>("Esoteric Seller QEID");

        /// <summary>
        /// The Adept Chalice Buff persistent QEffect ID
        /// </summary>
        public static readonly QEffectId AdeptChaliceBuff = ModManager.RegisterEnumMember<QEffectId>("Adept Chalice Buff QEID");

        /// <summary>
        /// The Adept Chalice Tracker persistent QEffect ID
        /// </summary>
        public static readonly QEffectId AdeptChaliceTracker = ModManager.RegisterEnumMember<QEffectId>("Adept Chalice Tracker QEID");

        /// <summary>
        /// The Adept Regalia Tracker persistent QEffect ID
        /// </summary>
        public static readonly QEffectId AdeptRegaliaTracker = ModManager.RegisterEnumMember<QEffectId>("Adept Regalia QEID");

        /// <summary>
        /// The Can Use Cursed Effigy persistent QEffect ID
        /// </summary>
        public static readonly QEffectId CanUseCursedEffigy = ModManager.RegisterEnumMember<QEffectId>("Can Use Cursed Effigy QEID");
    }
}
