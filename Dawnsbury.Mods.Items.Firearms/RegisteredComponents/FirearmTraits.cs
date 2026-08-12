using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.Items.Firearms.RegisteredComponents
{
    public static class FirearmTraits
    {
        /// <summary>
        /// Adds a dummary repeating trait for repeating weapons that have a reload.
        /// </summary>
        public static readonly Trait DummyRepeating5 = ModManager.RegisterTrait("DummyRepeating5", new TraitProperties("Repeating", true, "This weapon holds 5 projectiles. After you fire all five, you'll need to spend 3 actions to load a new magazine before you can fire again.") { RelevantForItemBlock = true });

        /// <summary>
        /// Adds the concussive trait for firearms
        /// </summary>
        public static readonly Trait Concussive = ModManager.RegisterTrait("Concussive", new TraitProperties("Concussive", true, "Deals either bludgeoning or piercing damage, whichever is better for you.", relevantForShortBlock: true) { RelevantForItemBlock = true });

        /// <summary>
        /// Adds the Double Barrel trait for firearms
        /// TODO: Add 2 ammo system
        /// </summary>
        public static readonly Trait DoubleBarrel = ModManager.RegisterTrait("Double Barrel", new TraitProperties("Double Barrel", true, "This weapon has two barrels that are each loaded separately. You can fire both barrels of a double barrel weapon in a single Strike to increase the weapon damage die by one step. If the weapon has the fatal trait, this increases the fatal die by one step.", relevantForShortBlock: true) { RelevantForItemBlock = true });

        /// <summary>
        /// Adds the Double Barrel trait for firearms
        /// </summary>
        public static readonly Trait FatalAimD12 = ModManager.RegisterTrait("Fatal Aim D12", new TraitProperties("Fatal Aim D12", true, "This weapon can be held in 1 or 2 hands. You can interact as an action to switch your grip on it as it is more complicated than just releasing one hand. When held with 2 hands, it gains the Fatal D12 trait.", relevantForShortBlock: true) { RelevantForItemBlock = true });

        /// <summary>
        /// Adds the Double Barrel trait for firearms
        /// </summary>
        public static readonly Trait Kickback = ModManager.RegisterTrait("Kickback", new TraitProperties("Kickback", true, "A kickback weapon is extra powerful and difficult to use due to its high recoil. A kickback weapon deals 1 additional damage with all attacks. Firing a kickback weapon gives a –2 circumstance penalty to the attack roll, but characters with 14 or more Strength ignore the penalty. A stablizer will lower the circumstance penalty to -1, and a tripod will remove the penalty entirely.", relevantForShortBlock: true) { RelevantForItemBlock = true });

        /// <summary>
        /// Adds the modular trait for firearms
        /// TODO: Consider rewriting this in V3, as meeting RAW should be easier
        /// </summary>
        public static readonly Trait Modular = ModManager.RegisterTrait("Modular", new TraitProperties("Modular", true, "Deals either bludgeoning, piercing or slashing damage, whichever is better for you.", relevantForShortBlock: true) { RelevantForItemBlock = true });

        /// <summary>
        /// Adds the Scatter 5 trait for firearms
        /// </summary>
        public static readonly Trait Scatter5 = ModManager.RegisterTrait("Scatter5", new TraitProperties("Scatter5", true, "This weapon fires a cluster of pellets in a wide spray. On a hit, the primary target of attacks with a scatter weapon take the listed damage, and the target and all other creatures within a 5-ft radius around it take 1 point of splash damage per weapon damage die.", relevantForShortBlock: true) { RelevantForItemBlock = true });

        /// <summary>
        /// Adds the Scatter 10 trait for firearms
        /// </summary>
        public static readonly Trait Scatter10 = ModManager.RegisterTrait("Scatter10", new TraitProperties("Scatter10", true, "This weapon fires a cluster of pellets in a wide spray. On a hit, the primary target of attacks with a scatter weapon take the listed damage, and the target and all other creatures within a 10-ft radius around it take 1 point of splash damage per weapon damage die.", relevantForShortBlock: true) { RelevantForItemBlock = true });

        /// <summary>
        /// Adds the Parry trait for firearms
        /// </summary>
        public static readonly Trait Parry = ModManager.RegisterTrait("Parry", new TraitProperties("Parry", true, "This weapon can be used defensively to block attacks. While wielding this weapon, if your proficiency with it is trained or better, you can spend a single action to position your weapon defensively, gaining a +1 circumstance bonus to AC until the start of your next turn.", relevantForShortBlock: true) { RelevantForItemBlock = true });

        /// <summary>
        /// Adds the Misfired trait for firearms
        /// </summary>
        public static readonly Trait Misfired = ModManager.RegisterTrait("Misfired", new TraitProperties("Misfired", true, "This firearm was misfired and is now jammed. You must use an Interact action to clear the jam before you can reload the weapon and fire again.", relevantForShortBlock: true));

        /// <summary>
        /// Adds the Ignore Kickback Penalty trait for actions
        /// </summary>
        public static readonly Trait IgnoreKickbackPenalty = ModManager.RegisterTrait("IgnoreKickbackPenalty", new TraitProperties("IgnoreKickbackPenalty", false));

        /// <summary>
        /// Adds the Ignore Double Barrel trait for actions
        /// </summary>
        public static readonly Trait IgnoreDoubleBarrel = ModManager.RegisterTrait("IgnoreDoubleBarrel", new TraitProperties("IgnoreDoubleBarrel", false));

        /// <summary>
        /// Adds the Ignore Scatter trait for actions
        /// </summary>
        public static readonly Trait IgnoreScatter = ModManager.RegisterTrait("IgnoreScatter", new TraitProperties("IgnoreScatter", false));

        /// <summary>
        /// Adds the Attached Weapon trait for actions
        /// </summary>
        public static readonly Trait AttachedWeapon = ModManager.RegisterTrait("Attached Weapon", new TraitProperties("Attached Weapon", false));

        /// <summary>
        /// Adds the Bayonet trait for actions
        /// </summary>
        public static readonly Trait Bayonet = ModManager.RegisterTrait("Bayonet", new TraitProperties("Bayonet", false));

        /// <summary>
        /// Adds the Reinforced Stock trait for actions
        /// </summary>
        public static readonly Trait ReinforcedStock = ModManager.RegisterTrait("Reinforced Stock", new TraitProperties("Reinforced Stock", false));

        /// <summary>
        /// Adds the Firearm Stabalizer trait for actions
        /// </summary>
        public static readonly Trait FirearmStabalizer = ModManager.RegisterTrait("Firearm Stabalizer", new TraitProperties("Firearm Stabalizer", false));

        /// <summary>
        /// Adds the Tripod trait for actions
        /// </summary>
        public static readonly Trait Tripod = ModManager.RegisterTrait("Tripod", new TraitProperties("Tripod", false));

        /// <summary>
        /// Adds the Item Updated trait for actions
        /// </summary>
        public static readonly Trait ItemUpdated = ModManager.RegisterTrait("ItemUpdated", new TraitProperties("ItemUpdated", false));

        // HACK: Repeating is hard coded to 5 round magazines, so right now the magazine will just be left to 5
        //public static readonly Trait Magazine6 = ModManager.RegisterTrait("Magazine6", new TraitProperties("Magazine", true, "This repeating weapon has a magazine capacity of 6 instead of 5.", relevantForShortBlock: true));

        //public static readonly Trait Magazine8 = ModManager.RegisterTrait("Magazine8", new TraitProperties("Magazine", true, "This repeating weapon has a magazine capacity of 8 instead of 5.", relevantForShortBlock: true));

        // Items
        public static readonly Trait WeaponAirRepeater = ModManager.RegisterTrait("AirRepeater", new TraitProperties("Air Repeater", false));
        public static readonly Trait WeaponCoatPistol = ModManager.RegisterTrait("CoatPistol", new TraitProperties("Coat Pistol", false));
        public static readonly Trait WeaponFireLance = ModManager.RegisterTrait("FireLance", new TraitProperties("Fire Lance", false));
        public static readonly Trait WeaponFlintlockMusket = ModManager.RegisterTrait("FlintlockMusket", new TraitProperties("Flintlock Musket", false));
        public static readonly Trait WeaponFlintlockPistol = ModManager.RegisterTrait("FlintlockPistol", new TraitProperties("Flintlock Pistol", false));
        public static readonly Trait WeaponHandCannon = ModManager.RegisterTrait("HandCannon", new TraitProperties("Hand Cannon", false));
        public static readonly Trait WeaponLongAirRepeater = ModManager.RegisterTrait("LongAirRepeater", new TraitProperties("Long Air Repeater", false));
        public static readonly Trait WeaponArquebus = ModManager.RegisterTrait("Arquebus", new TraitProperties("Arquebus", false));
        public static readonly Trait WeaponBlunderbuss = ModManager.RegisterTrait("Blunderbuss", new TraitProperties("Blunderbuss", false));
        public static readonly Trait WeaponClanPistol = ModManager.RegisterTrait("ClanPistol", new TraitProperties("Clan Pistol", false));
        public static readonly Trait WeaponDoublebarreledMusket = ModManager.RegisterTrait("DoublebarreledMusket", new TraitProperties("Double-barreled Musket", false));
        public static readonly Trait WeaponDoublebarreledPistol = ModManager.RegisterTrait("DoublebarreledPistol", new TraitProperties("Double-barreled Pistol", false));
        public static readonly Trait WeaponDragonMouthPistol = ModManager.RegisterTrait("DragonMouthPistol", new TraitProperties("Dragon Mouth Pistol", false));
        public static readonly Trait WeaponDuelingPistol = ModManager.RegisterTrait("DuelingPistol", new TraitProperties("Dueling Pistol", false));
        public static readonly Trait WeaponHarmonaGun = ModManager.RegisterTrait("HarmonaGun", new TraitProperties("Harmona Gun", false));
        public static readonly Trait WeaponJezail = ModManager.RegisterTrait("Jezail", new TraitProperties("Jezail", false));
        public static readonly Trait WeaponMithralTree = ModManager.RegisterTrait("MithralTree", new TraitProperties("Mithral Tree", false));
        public static readonly Trait WeaponPepperbox = ModManager.RegisterTrait("Pepperbox", new TraitProperties("Pepperbox", false));
        public static readonly Trait WeaponSlidePistol = ModManager.RegisterTrait("SlidePistol", new TraitProperties("Slide Pistol", false));
        public static readonly Trait WeaponRepeatingHeavyCrossbow = ModManager.RegisterTrait("RepeatingHeavyCrossbow", new TraitProperties("Repeating Heavy Crossbow", false));
        public static readonly Trait WeaponDwarvenScattergun = ModManager.RegisterTrait("DwarvenScattergun", new TraitProperties("Dwarven Scattergun", false));
        public static readonly Trait WeaponFlingflenser = ModManager.RegisterTrait("Flingflenser", new TraitProperties("Flingflenser", false));
        public static readonly Trait WeaponRepeatingCrossbow = ModManager.RegisterTrait("RepeatingCrossbow", new TraitProperties("Repeating Crossbow", false));
    }
}
