using Dawnsbury.Core.CombatActions;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.Items.Firearms.RegisteredComponents
{
    /// <summary>
    /// A static class containing all Action IDs used for Firearms
    /// </summary>
    public static class FirearmActionIDs
    {
        /// <summary>
        /// The Double Barrel Reload persistent Action ID
        /// </summary>
        public static readonly ActionId DoubleBarrelReload = ModManager.RegisterEnumMember<ActionId>("Double Barrel Realod AID");
    }
}
