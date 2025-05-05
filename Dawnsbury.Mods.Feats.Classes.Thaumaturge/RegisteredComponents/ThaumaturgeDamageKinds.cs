using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents
{
    /// <summary>
    /// A static class containing all Damage Kinds used for the Thaumaturge
    /// </summary>
    public class ThaumaturgeDamageKinds
    {
        /// <summary>
        /// The Personal Antithesis Damage Kind
        /// </summary>
        public static readonly DamageKind PersonalAntithesis = ModManager.RegisterEnumMember<DamageKind>("Personal Antithesis");

        /// <summary>
        /// The Glimpse Vulnerability Damage Kind
        /// </summary>
        public static readonly DamageKind GlimpseVulnerability = ModManager.RegisterEnumMember<DamageKind>("Glimpse Vulnerability");
    }
}
