using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.Feats.Classes.Gunslinger.RegisteredComponents
{
    /// <summary>
    /// A static class containing all Traits used for the Gunslinger
    /// </summary>
    public static class GunslingerTraits
    {
        /// <summary>
        /// The Gunslinger class trait 
        /// </summary>
        public static readonly Trait Gunslinger = ModManager.RegisterTrait("Gunslinger", new TraitProperties("Gunslinger", relevant: true) { IsClassTrait = true });

        /// <summary>
        /// The Gunslinger class trait 
        /// </summary>
        public static readonly Trait PistoleroSkillChoice = ModManager.RegisterTrait("PistoleroSkillChoice", new TraitProperties("PistoleroSkillChoice", false));

        /// <summary>
        /// A technical trait for does not provoke
        /// </summary>
        public static readonly Trait TemporaryDoesNotProvoke = ModManager.RegisterTrait("Temporary Does Not Provoke", new TraitProperties("Temporary Does Not Provoke", false));

        /// <summary>
        /// A technical trait for parry
        /// </summary>
        public static readonly Trait TemporaryParry = ModManager.RegisterTrait("Temporary Parry", new TraitProperties("Temporary Parry", false));
    }
}
