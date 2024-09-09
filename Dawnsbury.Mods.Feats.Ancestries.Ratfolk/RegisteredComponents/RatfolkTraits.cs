using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.Feats.Ancestries.Ratfolk.RegisteredComponents
{
    /// <summary>
    /// A static class containing all Traits used for the Ratfolk
    /// </summary>
    public static class RatfolkTraits
    {
        /// <summary>
        /// The Ratfolk class trait 
        /// </summary>
        public static readonly Trait Ratfolk = ModManager.RegisterTrait("Ratfolk", new TraitProperties("Ratfolk", relevant: true) { IsAncestryTrait = true });
    }
}