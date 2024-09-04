using Dawnsbury.Core.Mechanics;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.Items.Firearms.RegisteredComponents
{
    /// <summary>
    /// A static class containing all QEffect IDs used for Firearms
    /// </summary>
    public static class FirearmQEIDs
    {
        /// <summary>
        /// The Tripod Setup persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId TripodSetup = ModManager.RegisterEnumMember<QEffectId>("Tripod Setup QEID");

        /// <summary>
        /// The technical Fatal Is Upgraded persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId FatalIsUpgraded = ModManager.RegisterEnumMember<QEffectId>("Fatal Is Upgraded QEID");


        /// <summary>
        /// The Parry persistent QEffect ID
        /// </summary>
        public static readonly QEffectId Parry = ModManager.RegisterEnumMember<QEffectId>("Parry QEID");
    }
}
