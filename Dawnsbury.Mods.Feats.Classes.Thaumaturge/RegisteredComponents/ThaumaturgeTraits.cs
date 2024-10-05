using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents
{
    /// <summary>
    /// A static class containing all Traits used for the Thaumaturge
    /// </summary>
    public static class ThaumaturgeTraits
    {
        /// <summary>
        /// The Thaumaturge class trait 
        /// </summary>
        public static readonly Trait Thaumaturge = ModManager.RegisterTrait("Thaumaturge", new TraitProperties("Thaumaturge", relevant: true) { IsClassTrait = true });

        /// <summary>
        /// The Esoteric Lore skill trait
        /// </summary>
        public static readonly Trait EsotericLore = ModManager.RegisterTrait("Esoteric Lore", new TraitProperties("Esoteric Lore", false));

        /// <summary>
        /// The Implement trait
        /// </summary>
        public static readonly Trait Implement = ModManager.RegisterTrait("Implement", new TraitProperties("Implement", false));
    }
}