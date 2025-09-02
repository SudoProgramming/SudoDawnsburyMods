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
        public static readonly Trait Implement = ModManager.RegisterTrait("Implement", new TraitProperties("Implement", true));

        /// <summary>
        /// The Weapon Implement trait
        /// </summary>
        public static readonly Trait WeaponImplement = ModManager.RegisterTrait("Weapon Implement", new TraitProperties("Weapon Implement", true));

        /// <summary>
        /// The Adept Implement trait
        /// </summary>
        public static readonly Trait AdeptImplement = ModManager.RegisterTrait("AdeptImplement", new TraitProperties("Adept Implement", false));

        /// <summary>
        /// The Tome Skill trait
        /// </summary>
        public static readonly Trait TomeSkill = ModManager.RegisterTrait("TomeSkill", new TraitProperties("Tome Skill", false));

        /// <summary>
        /// The Primary Tome Skill trait
        /// </summary>
        public static readonly Trait PrimaryTomeSkill = ModManager.RegisterTrait("PrimaryTomeSkill", new TraitProperties("Primary Tome Skill", false));

        /// <summary>
        /// The Secondary Tome Skill trait
        /// </summary>
        public static readonly Trait SecondaryTomeSkill = ModManager.RegisterTrait("SecondaryTomeSkill", new TraitProperties("Secondary Tome Skill", false));

        /// <summary>
        /// The Mirror Clone Immunity trait
        /// </summary>
        public static readonly Trait MirrorCloneImmunity = ModManager.RegisterTrait("Mirror Clone Immunity", new TraitProperties("Mirror Clone Immunity", false));

        /// <summary>
        /// The Temporary Ignore One Hand Plus trait
        /// </summary>
        public static readonly Trait TemporaryIgnoreOneHandPlus = ModManager.RegisterTrait("Temporary Ignore One Hand Plus", new TraitProperties("Temporary Ignore One Hand Plus", false));

        /// <summary>
        /// The Duplicated Start Of Combat Scroll trait
        /// </summary>
        public static readonly Trait DuplicatedStartOfCombatScroll = ModManager.RegisterTrait("Duplicated Start Of Combat Scroll", new TraitProperties("Duplicated Start Of Combat Scroll", false));

        /// <summary>
        /// The Unaffected By Concealmen trait
        /// </summary>
        public static readonly Trait TemporaryUnaffectedByConcealment = ModManager.RegisterTrait("Temporary Unaffected By Concealmen", new TraitProperties("Temporary Unaffected By Concealmen", false));
    }
}