using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.Feats.Classes.Gunslinger.RegisteredComponents
{
    /// <summary>
    /// A static class containing all Feat Names used for the Gunslinger
    /// </summary>
    public static class GunslingerFeatNames
    {
        /// <summary>
        /// The Gunslinger Class Selection Feat
        /// </summary>
        public static readonly FeatName GunslingerClass = ModManager.RegisterFeatName("GunslingerClassFeat", "Gunslinger");

        /// <summary>
        /// The Singular Expertise base class feature
        /// </summary>
        public static readonly FeatName SingularExpertise = ModManager.RegisterFeatName("Singular Expertise", "Singular Expertise");

        /// <summary>
        /// The Coated Munitions class feat name
        /// </summary>
        public static readonly FeatName CoatedMunitions = ModManager.RegisterFeatName("Coated Munitions", "Coated Munitions");

        /// <summary>
        /// The Hit the Dirt class feat name
        /// </summary>
        public static readonly FeatName HitTheDirt = ModManager.RegisterFeatName("Hit the Dirt", "Hit the Dirt");

        /// <summary>
        /// The Cover Fire class feat name
        /// </summary>
        public static readonly FeatName CoverFire = ModManager.RegisterFeatName("Cover Fire", "Cover Fire");

        /// <summary>
        /// The Crossbow Crack Shot class feat name
        /// </summary>
        public static readonly FeatName CrossbowCrackShot = ModManager.RegisterFeatName("Crossbow Crack Shot", "Crossbow Crack Shot");

        /// <summary>
        /// The Sword and Pistol class feat name
        /// </summary>
        public static readonly FeatName SwordAndPistol = ModManager.RegisterFeatName("Sword and Pistol", "Sword and Pistol");

        /// <summary>
        /// The Defensive Armaments class feat name
        /// </summary>
        public static readonly FeatName DefensiveArmaments = ModManager.RegisterFeatName("Defensive Armaments", "Defensive Armaments");

        /// <summary>
        /// The Fake Out class feat name
        /// </summary>
        public static readonly FeatName FakeOut = ModManager.RegisterFeatName("Fake Out", "Fake Out");

        /// <summary>
        /// The Pistol Twirl class feat name
        /// </summary>
        public static readonly FeatName PistolTwirl = ModManager.RegisterFeatName("Pistol Twirl", "Pistol Twirl");

        /// <summary>
        /// The Risky Reload class feat name
        /// </summary>
        public static readonly FeatName RiskyReload = ModManager.RegisterFeatName("Risky Reload", "Risky Reload");

        /// <summary>
        /// The Warning Shot class feat name
        /// </summary>
        public static readonly FeatName WarningShot = ModManager.RegisterFeatName("Warning Shot", "Warning Shot");

        /// <summary>
        /// The Alchemical Shot class feat name
        /// </summary>
        public static readonly FeatName AlchemicalShot = ModManager.RegisterFeatName("Alchemical Shot", "Alchemical Shot");

        /// <summary>
        /// The Black Powder Boost class feat name
        /// </summary>
        public static readonly FeatName BlackPowderBoost = ModManager.RegisterFeatName("Black Powder Boost", "Black Powder Boost {icon:Action} or {icon:TwoActions}");

        /// <summary>
        /// The Paired Shots class feat name
        /// HACK: Currently the percision damage is added from both attacks. Dawnsbury doesn't break out precision damage to check which is higher
        /// </summary>
        public static readonly FeatName PairedShots = ModManager.RegisterFeatName("Paired Shots", "Paired Shots");

        /// <summary>
        /// The Running Reload class feat name
        /// </summary>
        public static readonly FeatName RunningReload = ModManager.RegisterFeatName("Running Reload", "Running Reload");

        /// <summary>
        /// The Way of the Drifter Subclass Feat Name
        /// </summary>
        public static readonly FeatName WayOfTheDrifter = ModManager.RegisterFeatName("Way of the Drifter", "Way of the Drifter");

        /// <summary>
        /// The Way of the Pistolero Subclass Feat Name
        /// </summary>
        public static readonly FeatName WayOfThePistolero = ModManager.RegisterFeatName("Way of the Pistolero", "Way of the Pistolero");

        /// <summary>
        /// The Way of the Sniper Subclass Feat Name
        /// </summary>
        public static readonly FeatName WayOfTheSniper = ModManager.RegisterFeatName("Way of the Sniper", "Way of the Sniper");

        ///// <summary>
        ///// The Way of the Triggerbrand Subclass Feat Name
        ///// TODO: Add Combination Weapons and this Subclass
        ///// </summary>
        //public static readonly FeatName WayOfTheTriggerbrand = ModManager.RegisterFeatName("Way of the Triggerbrand", "Way of the Triggerbrand");

        /// <summary>
        /// The Way of the Vanguard Subclass Feat Name
        /// </summary>
        public static readonly FeatName WayOfTheVanguard = ModManager.RegisterFeatName("Way of the Vanguard", "Way of the Vanguard");

        /// <summary>
        /// A technical feat for rolling Stealth for Initiative as a Sniper Gunslinger
        /// </summary>
        public static readonly FeatName GunslingerSniperStealthInitiative = ModManager.RegisterFeatName("Gunslinger Sniper Stealth Init", "Stealth for Initiative");

        /// <summary>
        /// A technical feat for rolling Stealth for Initiative as a Sniper Gunslinger
        /// </summary>
        public static readonly FeatName GunslingerSniperPerceptionInitiative = ModManager.RegisterFeatName("Gunslinger Sniper Perception Init", "Perception for Initiative");
    }
}
