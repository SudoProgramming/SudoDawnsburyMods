using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.Feats.Ancestries.Ratfolk.RegisteredComponents
{
    /// <summary>
    /// A static class containing all Traits used for the Catfolk
    /// </summary>
    public static class CatfolkTraits
    {
        /// <summary>
        /// The Ratfolk class trait 
        /// </summary>
        public static readonly Trait Catfolk = ModManager.RegisterTrait("Catfolk", new TraitProperties("Catfolk", relevant: true) { IsAncestryTrait = true });
    }
}