using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Rules;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Display.Controls;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dawnsbury.Mods.Items.Firearms
{
    /// <summary>
    /// The Equipment from Guns and Gears
    /// </summary>
    public static class Firearms
    {
        /// <summary>
        /// A QEffect for setting up a Tripod
        /// </summary>
        private static readonly QEffect TripodSetupQEffect = new QEffect("Tripod Setup", "Your Tripod is setup, removing the circumstance penality from Kickback till you move.");

        /// <summary>
        /// A technical QEffect for determining if the Fatal dice is upgraded
        /// </summary>
        private static readonly QEffect FatalIsUpgradedTechnicalQEffect = new QEffect("Upgraded Fatal Technical Effect", "[this condition has no description]");

        /// <summary>
        /// Adds the weapon group trait of Firearm
        /// </summary>
        public static readonly Trait FirearmTrait = ModManager.RegisterTrait("Firearm", new TraitProperties("Firearm", true, relevantForShortBlock: true));

        /// <summary>
        /// Adds the technical trait for simple firearms used for proficiency
        /// </summary>
        public static readonly Trait SimpleFirearmTrait = ModManager.RegisterTrait("Simple Firearm", new TraitProperties("Simple Firearm", false) { ProficiencyName = "Simple Firearm"});

        /// <summary>
        /// Adds the technical trait for martial firearms used for proficiency
        /// </summary>
        public static readonly Trait MartialFirearmTrait = ModManager.RegisterTrait("Martial Firearm", new TraitProperties("Martial Firearm", false) { ProficiencyName = "Martial Firearm" });

        /// <summary>
        /// Adds the technical trait for advanced firearms used for proficiency
        /// </summary>
        public static readonly Trait AdvancedFirearmTrait = ModManager.RegisterTrait("Advanced Firearm", new TraitProperties("Advanced Firearm", false) { ProficiencyName = "Advanced Firearm" });

        /// <summary>
        /// Adds the technical trait for simple crossbows used for proficiency
        /// </summary>
        public static readonly Trait SimpleCrossbowTrait = ModManager.RegisterTrait("Simple Crossbow Prof", new TraitProperties("Simple Crossbow Prof", false) { ProficiencyName = "Simple Crossbow" });

        /// <summary>
        /// Adds the technical trait for martial crossbows used for proficiency
        /// </summary>
        public static readonly Trait MartialCrossbowTrait = ModManager.RegisterTrait("Martial Crossbow", new TraitProperties("Martial Crossbow", false) { ProficiencyName = "Martial Crossbow" });

        /// <summary>
        /// Adds the technical trait for advanced crossbows used for proficiency
        /// </summary>
        public static readonly Trait AdvancedCrossbowTrait = ModManager.RegisterTrait("Advanced Crossbow", new TraitProperties("Advanced Crossbow", false) { ProficiencyName = "Advanced Crossbow" });

        /// <summary>
        /// Adds the concussive trait for firearms
        /// </summary>
        public static readonly Trait ConcussiveTrait = ModManager.RegisterTrait("Concussive", new TraitProperties("Concussive", true, "Deals either bludgeoning or piercing damage, whichever is better for you.", relevantForShortBlock: true));

        /// <summary>
        /// Adds the Double Barrel trait for firearms
        /// TODO: Add 2 ammo system
        /// </summary>
        public static readonly Trait DoubleBarrelTrait = ModManager.RegisterTrait("Double Barrel", new TraitProperties("Double Barrel", true, "This weapon has two barrels that are each loaded separately. You can fire both barrels of a double barrel weapon in a single Strike to increase the weapon damage die by one step. If the weapon has the fatal trait, this increases the fatal die by one step.", relevantForShortBlock: true));

        /// <summary>
        /// Adds the Double Barrel trait for firearms
        /// </summary>
        public static readonly Trait FatalAimD12Trait = ModManager.RegisterTrait("Fatal Aim D12", new TraitProperties("Fatal Aim D12", true, "This weapon can be held in 1 or 2 hands. You can interact as an action to switch your grip on it as it is more complicated than just releasing one hand. When held with 2 hands, it gains the Fatal D12 trait.", relevantForShortBlock: true));

        /// <summary>
        /// Adds the Double Barrel trait for firearms
        /// </summary>
        public static readonly Trait KickbackTrait = ModManager.RegisterTrait("Kickback", new TraitProperties("Kickback", true, "A kickback weapon is extra powerful and difficult to use due to its high recoil. A kickback weapon deals 1 additional damage with all attacks. Firing a kickback weapon gives a –2 circumstance penalty to the attack roll, but characters with 14 or more Strength ignore the penalty. A stablizer will lower the circumstance penalty to -1, and a tripod will remove the penalty entirely.", relevantForShortBlock: true));

        /// <summary>
        /// Adds the modular trait for firearms
        /// TODO: Consider rewriting this in V3, as meeting RAW should be easier
        /// </summary>
        public static readonly Trait ModularTrait = ModManager.RegisterTrait("Modular", new TraitProperties("Modular", true, "Deals either bludgeoning, piercing or slashing damage, whichever is better for you.", relevantForShortBlock: true));

        /// <summary>
        /// Adds the Scatter 5 trait for firearms
        /// </summary>
        public static readonly Trait Scatter5Trait = ModManager.RegisterTrait("Scatter5", new TraitProperties("Scatter5", true, "This weapon fires a cluster of pellets in a wide spray. On a hit, the primary target of attacks with a scatter weapon take the listed damage, and the target and all other creatures within a 5-ft radius around it take 1 point of splash damage per weapon damage die.", relevantForShortBlock: true));

        /// <summary>
        /// Adds the Scatter 10 trait for firearms
        /// </summary>
        public static readonly Trait Scatter10Trait = ModManager.RegisterTrait("Scatter10", new TraitProperties("Scatter10", true, "This weapon fires a cluster of pellets in a wide spray. On a hit, the primary target of attacks with a scatter weapon take the listed damage, and the target and all other creatures within a 10-ft radius around it take 1 point of splash damage per weapon damage die.", relevantForShortBlock: true));

        /// <summary>
        /// Adds the Scatter 10 trait for firearms
        /// </summary>
        public static readonly Trait MisfiredTrait = ModManager.RegisterTrait("Misfired", new TraitProperties("Misfired", true, "This firearm was misfired and is now jammed. You must use an Interact action to clear the jam before you can reload the weapon and fire again.", relevantForShortBlock: true));

        // HACK: Repeating is hard coded to 5 round magazines, so right now the magazine will just be left to 5
        //public static readonly Trait Magazine6Trait = ModManager.RegisterTrait("Magazine6", new TraitProperties("Magazine", true, "This repeating weapon has a magazine capacity of 6 instead of 5.", relevantForShortBlock: true));

        //public static readonly Trait Magazine8Trait = ModManager.RegisterTrait("Magazine8", new TraitProperties("Magazine", true, "This repeating weapon has a magazine capacity of 8 instead of 5.", relevantForShortBlock: true));

        /// <summary>
        /// Register all the Guns and Gears items
        /// </summary>
        public static void RegisterItems()
        {
            // Simple Ranged Firearm Weapons
            //  - Air Repeater
            ModManager.RegisterNewItemIntoTheShop("Air Repeater", itemName =>
                new Item(itemName, new ModdedIllustration("FirearmsAssets/AirRepeater.png"), "Air Repeater", 0, 3, Trait.Agile, Trait.Repeating, FirearmTrait, SimpleFirearmTrait, Trait.Simple)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Piercing)
                        .WithRangeIncrement(6)));

            //  - Coat Pistol
            ModManager.RegisterNewItemIntoTheShop("Coat Pistol", itemName =>
                new Item(itemName, new ModdedIllustration("FirearmsAssets/CoatPistol.png"), "Coat Pistol", 0, 3, Trait.FatalD8, ConcussiveTrait, FirearmTrait, SimpleFirearmTrait, Trait.Simple, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning)
                        .WithRangeIncrement(6)));

            //  - Fire Lance
            ModManager.RegisterNewItemIntoTheShop("Fire Lance", itemName =>
               new Item(itemName, new ModdedIllustration("FirearmsAssets/FireLance.png"), "Fire Lance", 0, 3, Trait.FatalD10, FirearmTrait, SimpleFirearmTrait, Trait.Simple, Trait.Reload2, Trait.TwoHanded)
                   .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Piercing)
                       .WithRangeIncrement(2)));

            //  - Flintlock Musket
            ModManager.RegisterNewItemIntoTheShop("Flintlock Musket", itemName =>
               new Item(itemName, new ModdedIllustration("FirearmsAssets/FlintlockMusket.png"), "Flintlock Musket", 0, 4, Trait.FatalD10, ConcussiveTrait, FirearmTrait, SimpleFirearmTrait, Trait.Simple, Trait.Reload1, Trait.TwoHanded)
                   .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                       .WithRangeIncrement(14)));

            //  - Flintlock Pistol
            ModManager.RegisterNewItemIntoTheShop("Flintlock Pistol", itemName =>
               new Item(itemName, new ModdedIllustration("FirearmsAssets/FlintlockPistol.png"), "Flintlock Pistol", 0, 3, Trait.FatalD8, ConcussiveTrait, FirearmTrait, SimpleFirearmTrait, Trait.Simple, Trait.Reload1)
                   .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning)
                       .WithRangeIncrement(8)));

            //  - Hand Cannon
            ModManager.RegisterNewItemIntoTheShop("Hand Cannon", itemName =>
               new Item(itemName, new ModdedIllustration("FirearmsAssets/HandCannon.png"), "Hand Cannon", 0, 3, Trait.FatalD8, ModularTrait, FirearmTrait, SimpleFirearmTrait, Trait.Simple, Trait.Reload1)
                   .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                       .WithRangeIncrement(6)));

            //  - Long Air Repeater
            ModManager.RegisterNewItemIntoTheShop("Long Air Repeater", itemName =>
               new Item(itemName, new ModdedIllustration("FirearmsAssets/LongAirRepeater.png"), "Long Air Repeater", 0, 5, Trait.Repeating, FirearmTrait, SimpleFirearmTrait, Trait.Simple)
                   .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Piercing)
                       .WithRangeIncrement(12)));

            // Martial Ranged Firearm Weapons
            //  - Arquebus
            ModManager.RegisterNewItemIntoTheShop("Arquebus", itemName =>
                new Item(itemName, new ModdedIllustration("FirearmsAssets/Arquebus.png"), "Arquebus", 0, 5, Trait.FatalD12, KickbackTrait, ConcussiveTrait, FirearmTrait, MartialFirearmTrait, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning)
                        .WithRangeIncrement(30)));

            //  - Blunderbuss
            ModManager.RegisterNewItemIntoTheShop("Blunderbuss", itemName =>
                new Item(itemName, new ModdedIllustration("FirearmsAssets/Blunderbuss.png"), "Blunderbuss", 0, 4, ConcussiveTrait, Scatter10Trait, FirearmTrait, MartialFirearmTrait, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning)
                        .WithRangeIncrement(8)));

            //  - Clan Pistol
            ModManager.RegisterNewItemIntoTheShop("Clan Pistol", itemName =>
                new Item(itemName, new ModdedIllustration("FirearmsAssets/ClanPistol.png"), "Clan Pistol", 0, 0, Trait.Dwarf, Trait.FatalD10, ConcussiveTrait, FirearmTrait, MartialFirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                        .WithRangeIncrement(16)));

            //  - Double-barreled Musket
            ModManager.RegisterNewItemIntoTheShop("Double-barreled Musket", itemName =>
                new Item(itemName, new ModdedIllustration("FirearmsAssets/Double-barreledMusket.png"), "Double-barreled Musket", 0, 5, Trait.FatalD10, ConcussiveTrait, DoubleBarrelTrait, FirearmTrait, MartialFirearmTrait, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                        .WithRangeIncrement(12)));

            //  - Double-barreled Pistol
            ModManager.RegisterNewItemIntoTheShop("Double-barreled Pistol", itemName =>
                new Item(itemName, new ModdedIllustration("FirearmsAssets/Double-barreledPistol.png"), "Double-barreled Pistol", 0, 4, Trait.FatalD8, ConcussiveTrait, DoubleBarrelTrait, FirearmTrait, MartialFirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning)
                        .WithRangeIncrement(6)));

            //  - Dragon Mouth Pistol
            ModManager.RegisterNewItemIntoTheShop("Dragon Mouth Pistol", itemName =>
                new Item(itemName, new ModdedIllustration("FirearmsAssets/DragonMouthPistol.png"), "Dragon Mouth Pistol", 0, 5, Trait.FatalD8, ConcussiveTrait, Scatter5Trait, FirearmTrait, MartialFirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                        .WithRangeIncrement(4)));

            //  - Dueling Pistol
            ModManager.RegisterNewItemIntoTheShop("Dueling Pistol", itemName =>
                new Item(itemName, new ModdedIllustration("FirearmsAssets/DuelingPistol.png"), "Dueling Pistol", 0, 6, Trait.FatalD10, ConcussiveTrait, FirearmTrait, MartialFirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                        .WithRangeIncrement(12)));

            //  - Harmona Gun
            ModManager.RegisterNewItemIntoTheShop("Harmona Gun", itemName =>
                new Item(itemName, new ModdedIllustration("FirearmsAssets/HarmonaGun.png"), "Harmona Gun", 0, 5, KickbackTrait, FirearmTrait, MartialFirearmTrait, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d10", DamageKind.Bludgeoning)
                        .WithRangeIncrement(30)));

            //  - Jezail
            ModManager.RegisterNewItemIntoTheShop("Jezail", itemName =>
                new Item(itemName, new ModdedIllustration("FirearmsAssets/Jezail.png"), "Jezail", 0, 5, ConcussiveTrait, FatalAimD12Trait, FirearmTrait, MartialFirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning)
                        .WithRangeIncrement(18)));

            //  - Mithral Tree
            // TODO: Add Parry
            ModManager.RegisterNewItemIntoTheShop("Mithral Tree", itemName =>
                new Item(itemName, new ModdedIllustration("FirearmsAssets/MithralTree.png"), "Mithral Tree", 0, 5, Trait.Elf, Trait.FatalD10, ConcussiveTrait, FirearmTrait, MartialFirearmTrait, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                        .WithRangeIncrement(30)));

            //  - Pepperbox
            ModManager.RegisterNewItemIntoTheShop("Pepperbox", itemName =>
                new Item(itemName, new ModdedIllustration("FirearmsAssets/Pepperbox.png"), "Pepperbox", 0, 6, Trait.FatalD8, ConcussiveTrait, FirearmTrait, MartialFirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning)
                        .WithRangeIncrement(12)));

            //  - Slide Pistol
            ModManager.RegisterNewItemIntoTheShop("Slide Pistol", itemName =>
                new Item(itemName, new ModdedIllustration("FirearmsAssets/SlidePistol.png"), "Slide Pistol", 0, 8, Trait.FatalD10, ConcussiveTrait, FirearmTrait, MartialFirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                        .WithRangeIncrement(6)));

            // Advanced Ranged Firearm Weapons
            //  - Dwarven Scattergun
            ModManager.RegisterNewItemIntoTheShop("Dwarven Scattergun", itemName =>
                new Item(itemName, new ModdedIllustration("FirearmsAssets/DwarvenScattergun.png"), "Dwarven Scattergun", 0, 5, Trait.Dwarf, ConcussiveTrait, KickbackTrait, Scatter10Trait, FirearmTrait, AdvancedFirearmTrait, Trait.Advanced, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning)
                        .WithRangeIncrement(10)));

            //  - Flingflenser
            ModManager.RegisterNewItemIntoTheShop("Flingflenser", itemName =>
                new Item(itemName, new ModdedIllustration("FirearmsAssets/Flingflenser.png"), "Flingflenser", 0, 3, Trait.Goblin, Trait.Backstabber, Trait.FatalD10, Scatter5Trait, FirearmTrait, AdvancedFirearmTrait, Trait.Advanced, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Slashing)
                        .WithRangeIncrement(6)));

            // Other Items
            //  - Firearm Stabalizer
            ModManager.RegisterNewItemIntoTheShop("Firearm Stabalizer", itemName =>
                new Item(itemName, new ModdedIllustration("FirearmsAssets/Stabalizer.png"), "Firearm Stabalizer", 0, 1)
                    .WithDescription("Reduces the circumstance penality from the Kickback trait to -1."));

            // Other Items
            //  - Tripod
            ModManager.RegisterNewItemIntoTheShop("Tripod", itemName =>
                new Item(itemName, new ModdedIllustration("FirearmsAssets/Tripod.png"), "Tripod", 0, 2)
                    .WithDescription("Requires an action to setup then reduces the circumstance penality from the Kickback trait to -2. This will require another action to resetup if a movement action is taken."));
        }

        /// <summary>
        /// Sets up all the trait logic for these items
        /// </summary>
        public static void SetupTraitLogic()
        {
            // Loops through each creature and sets up the Trait logic
            ModManager.RegisterActionOnEachCreature(creature =>
            {
                creature.AddQEffect(new QEffect
                {
                    // Sets up all state checks needed for the traits
                    StateCheck = (QEffect self) =>
                    {
                        // Adds the Double Barrel Fire to all firearms with the Double Barrel trait
                        if (self.Owner.WieldsItem(DoubleBarrelTrait))
                        {
                            AddDoubleBarrelFireStrikeAction(self);
                        }

                        // Loops through each item the owner is holding and adds logic for that trait
                        foreach (Item item in self.Owner.HeldItems)
                        {
                            // Adds the VersatileP trait to firearms with the Concussive trait
                            if (item.HasTrait(ConcussiveTrait) && !item.HasTrait(Trait.VersatileP))
                            {
                                item.Traits.Add(Trait.VersatileP);
                            }

                            // Adds both VersatileP and VersatileS to firearms with the Modular trait
                            if (item.HasTrait(ModularTrait) && !item.HasTrait(Trait.VersatileP) && !item.HasTrait(Trait.VersatileS))
                            {
                                item.Traits.Add(Trait.VersatileP);
                                item.Traits.Add(Trait.VersatileS);
                            }

                            // Adds logic for all firearms with the Double Barrel trait
                            if (item.HasTrait(DoubleBarrelTrait))
                            {
                                AddDoubleBarrelLogic(self, item);
                            }

                            // Adds logic for all firearms with the Fatal Aim D12 trait
                            if (item.HasTrait(FatalAimD12Trait))
                            {
                                AddFatalAimLogic(self, item);
                            }

                            // Adds logic for all firearms with the Kickback trait
                            if (item.HasTrait(KickbackTrait))
                            {
                                AddKickbackLogic(self, item);
                            }

                            // Adds logic for all firearms with the Scatter trait
                            if (item.HasTrait(Scatter5Trait) || item.HasTrait(Scatter10Trait))
                            {
                                AddScatterLogic(self, item);
                            }

                            // Adds logic for all firearms with the Misfired trait
                            if (item.HasTrait(MisfiredTrait))
                            {
                                AddMisfireLogic(self, item);
                            }
                        }
                    },

                    // Adds cleanup logic for dropping and stowing items
                    YouBeginAction = async (QEffect self, CombatAction action) =>
                    {
                        AddBeginActionCleanupLogic(self, action);
                    },

                    // Adds cleanup logic for pick up and drawing items
                    AfterYouTakeAction = async (QEffect self, CombatAction action) =>
                    {
                        AddAfterActionCleanupLogic(self, action);
                    },

                    // Adds cleanup logic for entering dying
                    YouAreDealtLethalDamage = async (QEffect self, Creature attacker, DamageStuff damage, Creature defender) =>
                    {
                        return AddDealtLeathalDamage(self, attacker, damage, defender);
                    }
                });
            });
        }

        /// <summary>
        /// Patches all exisiting items
        /// </summary>
        public static void PatchItems()
        {
            // Add traits
        }

        /// <summary>
        /// Determines if the item is a firearm or a crossbow
        /// </summary>
        /// <param name="item">The item being checked</param>
        /// <returns>True if the item is a firearm or crossbow and false otherwise</returns>
        public static bool IsItemFirearmOrCrossbow(Item item, bool checkIfItsLoaded = false)
        {
            if (item.HasTrait(Firearms.FirearmTrait) || item.HasTrait(Trait.Crossbow))
            {
                if (checkIfItsLoaded)
                {
                    return IsItemLoaded(item);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if the item is loaded
        /// </summary>
        /// <param name="item">The item being checked</param>
        /// <returns>True if the item is loaded and false otherwise</returns>
        public static bool IsItemLoaded(Item item)
        {
            return item.EphemeralItemProperties != null && !item.EphemeralItemProperties.NeedsReload;
        }

        /// <summary>
        /// Adds the Double Barrel Fire Attack for all firearms with the Double Barrel trait
        /// </summary>
        /// <param name="self">The state check</param>
        private static void AddDoubleBarrelFireStrikeAction(QEffect self)
        {
            // Adds a QEffect that will create a new strike called Double Barrel Fire
            self.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
            {
                ProvideStrikeModifier = delegate (Item doubleBarrelItem)
                {
                    // Avoids adding Double Barrel Fire to non-Double Barrel items
                    if (!doubleBarrelItem.HasTrait(DoubleBarrelTrait))
                    {
                        return null;
                    }
                    
                    // Creates a base strike with a weapon die step upgrade
                    CombatAction combatAction = self.Owner.CreateStrike(doubleBarrelItem, strikeModifiers: new StrikeModifiers()
                    {
                        IncreaseWeaponDieByOneStep = true
                    });
                    
                    // Updates the base strike with our details for Double Barrel Fire
                    combatAction.WithActionCost(1);
                    combatAction.Name = "Double Barrel Fire";
                    combatAction.Illustration = new SideBySideIllustration(combatAction.Illustration, IllustrationName.StarHit);
                    if (!combatAction.HasTrait(Trait.Basic))
                    {
                        combatAction.Traits.Add(Trait.Basic);
                    }
                    combatAction.Description = StrikeRules.CreateBasicStrikeDescription(combatAction.StrikeModifiers, "You can fire both barrels of a double barrel weapon in a single Strike to increase the weapon damage die by one step. If the weapon has the fatal trait, this increases the fatal die by one step.", null, null, null, weaponDieIncreased: true);

                    // Updates our stike modifers by adding a QEffect to handle the fatal die upgrade logic
                    StrikeModifiers strikeModifiers = combatAction.StrikeModifiers;
                    strikeModifiers.QEffectForStrike = new QEffect()
                    {
                        // Before damage and on a critical success the item's fatal trait is upgraded a die step.
                        AfterYouMakeAttackRoll = (QEffect attackUpgrade, CheckBreakdownResult result) =>
                        {
                            Trait? fatalTrait = doubleBarrelItem.Traits.FirstOrDefault(trait => trait == Trait.FatalD8 || trait == Trait.FatalD10 || trait == Trait.FatalD12);
                            if (fatalTrait != null && result.CheckResult == CheckResult.CriticalSuccess)
                            {
                                switch (fatalTrait)
                                {
                                    case Trait.FatalD8:
                                        doubleBarrelItem.Traits.Remove(Trait.FatalD8);
                                        doubleBarrelItem.Traits.Add(Trait.FatalD10);
                                        break;
                                    case Trait.FatalD10:
                                        doubleBarrelItem.Traits.Remove(Trait.FatalD10);
                                        doubleBarrelItem.Traits.Add(Trait.FatalD12);
                                        break;
                                    case Trait.FatalD12:
                                        break;
                                    default:
                                        break;
                                }

                                // The ammo is set to 0 since this costs both barrel's ammo and a technical QEffect is added for tracking
                                doubleBarrelItem.EphemeralItemProperties.AmmunitionLeftInMagazine = 0;
                                attackUpgrade.Owner.AddQEffect(FatalIsUpgradedTechnicalQEffect);
                            }
                        }
                    };

                    // Checks if both barrels are loaded to allow for use
                    ((CreatureTarget)combatAction.Target).WithAdditionalConditionOnTargetCreature((Creature attacker, Creature defender) =>
                    {
                        if (doubleBarrelItem.EphemeralItemProperties.AmmunitionLeftInMagazine == 2)
                        {
                            return Usability.Usable;
                        }

                        return Usability.NotUsable("Both barrels of this weapon must be loaded.");
                    });

                    // The completed Double Barrel Fire action
                    return combatAction;
                }
            });
        }

        /// <summary>
        /// Adds the logic for all double barrel firearms
        /// </summary>
        /// <param name="self">The state check</param>
        /// <param name="item">The Double Barrel trait item</param>
        private static void AddDoubleBarrelLogic(QEffect self, Item item)
        {
            // Adds a QEffect that will track all the logic for the Double Barrel trait outside of Double Barrel Fire
            self.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
            {
                // Adds logic for ammo tracking on reload and fire while also cleaning up our upgraded fatal die
                AfterYouTakeAction = async (QEffect cleanup, CombatAction action) =>
                {
                    if (action.Item == item)
                    {
                        // Handles the base reload from Dawnsbury and sets the remaining ammo back down to 1 since that reload is only possible on 0 ammo
                        if (action.ActionId == ActionId.Reload)
                        {
                            if (item.EphemeralItemProperties.AmmunitionLeftInMagazine > 2)
                            {
                                item.EphemeralItemProperties.AmmunitionLeftInMagazine = 1;
                            }
                        }

                        // For each strike with this weapon the ammo is reduced
                        else if (action.Name.ToLower().Contains("strike"))
                        {
                            item.EphemeralItemProperties.AmmunitionLeftInMagazine--;
                        }

                        // If there is still 1 shot in the weapon then the NeedsRealod flag and be set back to false
                        if (item.EphemeralItemProperties.NeedsReload && item.EphemeralItemProperties.AmmunitionLeftInMagazine == 1)
                        {
                            item.EphemeralItemProperties.NeedsReload = false;
                        }

                        // Updates the item's trait to reduce the fatal die back now to it's base size
                        if (cleanup.Owner.QEffects.Count(eff => eff.Name == FatalIsUpgradedTechnicalQEffect.Name) > 0)
                        {
                            Trait? fatalTrait = item.Traits.FirstOrDefault(trait => trait == Trait.FatalD8 || trait == Trait.FatalD10 || trait == Trait.FatalD12);
                            if (fatalTrait != null && item.HasTrait(fatalTrait.Value))
                            {
                                switch (fatalTrait)
                                {
                                    case Trait.FatalD8:
                                        break;
                                    case Trait.FatalD10:
                                        item.Traits.Remove(Trait.FatalD10);
                                        item.Traits.Add(Trait.FatalD8);
                                        break;
                                    case Trait.FatalD12:
                                        item.Traits.Remove(Trait.FatalD12);
                                        item.Traits.Add(Trait.FatalD10);
                                        break;
                                    default:
                                        break;
                                }
                            }

                            // Removes the technical track used for Fatail Die tracking
                            cleanup.Owner.RemoveAllQEffects(qe => qe.Name == FatalIsUpgradedTechnicalQEffect.Name);
                        }
                    }
                },
                ProvideActionIntoPossibilitySection = delegate (QEffect self, PossibilitySection section)
                {
                    if (section.PossibilitySectionId == PossibilitySectionId.ItemActions)
                    {
                        if (item.HasTrait(DoubleBarrelTrait) && !item.EphemeralItemProperties.NeedsReload && item.EphemeralItemProperties.AmmunitionLeftInMagazine != 2)
                        {
                            return new ActionPossibility(new CombatAction(self.Owner, item.Illustration, "Reload (1/2)", new Trait[1] { Trait.Manipulate }, "Reload the weapon with another piece of ammunition.", Target.Self((Creature cr, AI ai) => 10f)).WithActionCost(1).WithActionId(ActionId.Reload).WithEffectOnSelf(delegate
                            {
                                item.EphemeralItemProperties.ReloadActionsAlreadyTaken++;
                                item.EphemeralItemProperties.ReloadActionsAlreadyTaken = 0;
                                item.EphemeralItemProperties.NeedsReload = false;
                                item.EphemeralItemProperties.AmmunitionLeftInMagazine = 2;
                            }));

                        }
                    }

                    return null;
                },
                StartOfCombat = async (QEffect self) =>
                {
                    item.EphemeralItemProperties.AmmunitionLeftInMagazine = 2;
                }
            });
        }

        /// <summary>
        /// Adds the logic for all Fatal Aim firearms
        /// </summary>
        /// <param name="self">The state check</param>
        /// <param name="item">The Fatal Aim trait item</param>
        private static void AddFatalAimLogic(QEffect self, Item item)
        {
            // Adds a QEffect that will track all the logic for the Fatal Aim trait
            self.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
            {
                // Adds a Switch Grip action to the items section to change switch between one-handed and two-handed
                ProvideActionIntoPossibilitySection = delegate (QEffect self, PossibilitySection section)
                {
                    if (section.PossibilitySectionId == PossibilitySectionId.ItemActions)
                    {
                        return new ActionPossibility(new CombatAction(self.Owner, new SideBySideIllustration(item.Illustration, IllustrationName.Action), "Switch Grip", new Trait[1]
                        {
                                            Trait.Manipulate
                        }, "You switch your grip on your weapon, either removing a hand to hold it in one hand or adding a hand to hold it with two hands.", Target.Self().WithAdditionalRestriction(WhyCannotSwitchFromOneHandedToTwoHanded))
                        {
                            ShortDescription = "Switch your grip on your weapon.",
                        }.WithActionCost(1).WithEffectOnSelf((Action<Creature>)ToggleTwoHanded));
                    }

                    return null;
                },

                // Prompts the user to choose their grip at the start of combat if they have a hand free
                StartOfCombat = async (QEffect self) =>
                {
                    Item? fatalAimItem = self.Owner.HeldItems.FirstOrDefault(item => item.HasTrait(FatalAimD12Trait));
                    if (fatalAimItem != null && !fatalAimItem.HasTrait(Trait.TwoHanded) && self.Owner.HasFreeHand && await self.Owner.Battle.AskForConfirmation(self.Owner, fatalAimItem.Illustration, "Change grip to two handed as a free action?", "Two handed"))
                    {
                        fatalAimItem.Traits.Add(Trait.TwoHanded);
                    }
                },

                // Adds or removed the Fatal D12 trait if the item has the two-handed trait
                StateCheck = (QEffect self) =>
                {
                    List<Item> fatalAimItems = self.Owner.HeldItems.Where(item => item.HasTrait(FatalAimD12Trait)).ToList();
                    foreach (Item item in fatalAimItems)
                    {
                        if (item.HasTrait(Trait.TwoHanded) && !item.HasTrait(Trait.FatalD12))
                        {
                            item.Traits.Add(Trait.FatalD12);
                        }
                        else if (!item.HasTrait(Trait.TwoHanded) && item.HasTrait(Trait.FatalD12))
                        {
                            var removed = item.Traits.Remove(Trait.FatalD12);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Adds the logic for all Kickback firearms
        /// </summary>
        /// <param name="self">The state check</param>
        /// <param name="item">The Kickback trait item</param>
        private static void AddKickbackLogic(QEffect self, Item item)
        {
            // Adds a QEffect that will track all the logic for the Kickback trait
            self.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
            {
                // Provides a Setup Tripod action to the items section that will setup the Tripod till you move
                ProvideActionIntoPossibilitySection = delegate (QEffect setupTripodEffect, PossibilitySection section)
                {
                    if (section.PossibilitySectionId == PossibilitySectionId.ItemActions)
                    {
                        if (setupTripodEffect.Owner.HeldItems.Concat(setupTripodEffect.Owner.CarriedItems).Count(item => item.Name == "Tripod") > 0)
                        {
                            return new ActionPossibility(new CombatAction(setupTripodEffect.Owner, new SideBySideIllustration(item.Illustration, IllustrationName.Action), "Setup Tripod", new Trait[1]
                            {
                                                Trait.Manipulate
                            }, "Setup your Tripod which will remove the circumstance penality from Kickback till you move.", Target.Self().WithAdditionalRestriction(WhyCannotSetupTripod))
                            {
                                ShortDescription = "Setup your Tripod."
                            }.WithActionCost(1).WithEffectOnSelf((Action<Creature>)SetupTripod));
                        }
                    }

                    return null;
                },

                // Removes the Tripod if you attemp to move away
                AfterYouTakeAction = async (QEffect self, CombatAction action) =>
                {
                    if (self.Owner.QEffects.Count(qe => qe.Name == TripodSetupQEffect.Name) > 0 && action.HasTrait(Trait.Move))
                    {
                        self.Owner.RemoveAllQEffects(removeQE => removeQE.Name == TripodSetupQEffect.Name);
                    }
                },

                // Adds the penality to attack rolls depending on if you have a tripod setup, just a firearm stabalizer, enough strength, or nothing
                BonusToAttackRolls = (QEffect self, CombatAction action, Creature? defender) =>
                {
                    if (self.Owner.Abilities.Strength < 2 && self.Owner.QEffects.Count(qe => qe.Name == TripodSetupQEffect.Name) == 0)
                    {
                        int penality = (self.Owner.HeldItems.Concat(self.Owner.CarriedItems).Count(item => item.Name == "Firearm Stabalizer") > 0) ? -1 : -2;
                        return new Bonus(penality, BonusType.Circumstance, "Kickback" + ((penality == -1) ? " (Stablized)" : ""));
                    }
                    return null;
                },

                // Adds the bonus damage
                BonusToDamage = (QEffect self, CombatAction action, Creature defender) =>
                {
                    return new Bonus(1, BonusType.Untyped, "Kickback");
                }
            });
        }

        /// <summary>
        /// Adds the logic for all Scatter firearms
        /// </summary>
        /// <param name="self">The state check</param>
        /// <param name="item">The Scatter trait item</param>
        private static void AddScatterLogic(QEffect self, Item item)
        {
            // Adds a QEffect that will track all the logic for the Scatter trait
            if (item.WeaponProperties != null)
            {
                // The result of any scatter attack will be 
                CheckBreakdownResult? lastAttackResult = null;
                self.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                {
                    // The last attack result will be saved here
                    AfterYouMakeAttackRoll = async (QEffect self, CheckBreakdownResult result) =>
                    {
                        lastAttackResult = result;
                    },

                    // After a strike with the scatter item the targeted crature is looked at
                    AfterYouTakeAction = async (QEffect self, CombatAction action) =>
                    {
                        if (action.Item == item)
                        {
                            if ((lastAttackResult == null || (lastAttackResult.CheckResult == CheckResult.Success || lastAttackResult.CheckResult == CheckResult.CriticalSuccess) && action.Name.ToLower().Contains("strike") && action.ChosenTargets.ChosenCreature != null && item.WeaponProperties != null))
                            {
                                Creature? targetCreature = action.ChosenTargets.ChosenCreature;
                                if (targetCreature != null)
                                {
                                    // The best damage against the original target, the map, and tile with the targeted creature is saved for the next checks
                                    List<DamageKind> damageOptions = item.DetermineDamageKinds();
                                    DamageKind bestDamageToTarget = targetCreature.WeaknessAndResistance.WhatDamageKindIsBestAgainstMe(damageOptions);
                                    Map map = targetCreature.Battle.Map;
                                    Tile? tile = map.AllTiles.FirstOrDefault(tile => tile.PrimaryOccupant == targetCreature);


                                    // Check tile is looped through to see how close it was to the target. If it was exactly 10 feet away the calculated splash damage is applied to any creatures
                                    if (tile != null)
                                    {
                                        Tile[] tilesToScatterTo = map.AllTiles.Where(tileToCheck => tile.DistanceTo(tileToCheck) <= 2).ToArray();
                                        foreach (Tile tileToScatterTo in tilesToScatterTo)
                                        {
                                            Creature? potentalSplashTarget = tileToScatterTo.PrimaryOccupant;
                                            if (potentalSplashTarget != null && potentalSplashTarget is Creature splashTarget && splashTarget != targetCreature)
                                            {
                                                await splashTarget.DealDirectDamage(action, DiceFormula.FromText(item.WeaponProperties.DamageDieCount.ToString()), splashTarget, CheckResult.Success, bestDamageToTarget);
                                            }
                                        }
                                    }
                                }

                                // Updates the last attack to null
                                lastAttackResult = null;
                            }
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Adds the logic for all Misfired firearms
        /// </summary>
        /// <param name="self">The state check</param>
        /// <param name="item">The Misfired trait item</param>
        private static void AddMisfireLogic(QEffect self, Item item)
        {
            // Checks for only Firearm or Crossbow items and if they have the Misfired trait
            if (IsItemFirearmOrCrossbow(item) && item.HasTrait(MisfiredTrait))
            {
                self.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                {
                    // Adds an action to clean firearm to remove the Misfired trait
                    ProvideActionIntoPossibilitySection = delegate (QEffect cleanFirearmEffect, PossibilitySection section)
                    {
                        if (section.PossibilitySectionId == PossibilitySectionId.ItemActions && IsItemFirearmOrCrossbow(item) && item.HasTrait(MisfiredTrait))
                        {
                            return new ActionPossibility(new CombatAction(cleanFirearmEffect.Owner, new SideBySideIllustration(item.Illustration, IllustrationName.Action), "Clean Firearm", [Trait.Basic, Trait.Manipulate], "Clean firearm to remove the misfired trait.", Target.Self()).WithActionCost(1).WithEffectOnSelf(async (action, self) =>
                            {
                                item.Traits.RemoveAll(trait => trait == MisfiredTrait);
                            }));
                        }

                        return null;
                    },

                    // Prevents taking any action with that item
                    PreventTakingAction = (CombatAction action) =>
                    {
                        if (action.Name != "Clean Firearm" && ((action.HasTrait(FirearmTrait) || action.HasTrait(Trait.Crossbow)) && action.HasTrait(MisfiredTrait)) || (action.Item != null && IsItemFirearmOrCrossbow(action.Item) && action.Item.HasTrait(MisfiredTrait)))
                        {
                            return "Jammed from a misfire";
                        }

                        return null;
                    }
                });
            }
        }

        /// <summary>
        /// Adds cleanup logic for all begin action checks
        /// </summary>
        /// <param name="self">The state check</param>
        /// <param name="action">The action being taken</param>
        private static void AddBeginActionCleanupLogic(QEffect self, CombatAction action)
        {
            // Checks if the last action was a drop or stow
            string actionName = action.Name.ToLower();
            if (actionName != null && (actionName.Contains("drop") || actionName.Contains("stow")))
            {
                // Collects all the fatal aim and kickback items for cleanup and handles it
                Item? fatalAimItem = self.Owner.HeldItems.FirstOrDefault(item => item.HasTrait(FatalAimD12Trait));
                Item? kickbackItem = self.Owner.HeldItems.FirstOrDefault(item => item.HasTrait(KickbackTrait));
                if (fatalAimItem != null && actionName.Contains(fatalAimItem.Name.ToLower()) && fatalAimItem.HasTrait(Trait.TwoHanded) && fatalAimItem.HasTrait(FatalAimD12Trait))
                {
                    fatalAimItem.Traits.Remove(Trait.TwoHanded);
                }
                if (kickbackItem != null && actionName.Contains(kickbackItem.Name.ToLower()) && self.Owner.QEffects.Count(qe => qe.Name == TripodSetupQEffect.Name) > 0)
                {
                    self.Owner.RemoveAllQEffects(removeQE => removeQE.Name == TripodSetupQEffect.Name);
                }
            }
        }

        /// <summary>
        /// Adds cleanup logic for all after action checks
        /// </summary>
        /// <param name="self">The state check</param>
        /// <param name="action">The action just taken</param>
        private async static void AddAfterActionCleanupLogic(QEffect self, CombatAction action)
        {
            // Collacts all the fatal aim items for cleanup and handles it
            string actionName = action.Name.ToLower();
            Item? fatalAimItem = self.Owner.HeldItems.FirstOrDefault(item => item.HasTrait(FatalAimD12Trait));
            if (actionName != null && (actionName.Contains("pick up") || actionName.Contains("draw")) && fatalAimItem != null && !fatalAimItem.HasTrait(Trait.TwoHanded) && self.Owner.HasFreeHand && await self.Owner.Battle.AskForConfirmation(self.Owner, fatalAimItem.Illustration, "Change grip to two handed as a free action?", "Two handed"))
            {
                fatalAimItem.Traits.Add(Trait.TwoHanded);
            }
        }

        /// <summary>
        /// Adds cleanup logic for entering dying
        /// </summary>
        /// <param name="self">The state check</param>
        /// <param name="attacker">The attacker</param>
        /// <param name="damage">The delt damage</param>
        /// <param name="defender">The defender</param>
        /// <returns>A null damage modification</returns>
        private static DamageModification AddDealtLeathalDamage(QEffect self, Creature attacker, DamageStuff damage, Creature defender)
        {
            // Collects all the fatal aim and kickback items for cleanup and handles it
            Item? fatalAimItem = self.Owner.HeldItems.FirstOrDefault(item => item.HasTrait(FatalAimD12Trait));
            Item? kickbackItem = self.Owner.HeldItems.FirstOrDefault(item => item.HasTrait(KickbackTrait));
            if (fatalAimItem != null && fatalAimItem.HasTrait(Trait.TwoHanded) && fatalAimItem.HasTrait(FatalAimD12Trait))
            {
                fatalAimItem.Traits.Remove(Trait.TwoHanded);
            }
            if (kickbackItem != null && self.Owner.QEffects.Count(qe => qe.Name == TripodSetupQEffect.Name) > 0)
            {
                self.Owner.RemoveAllQEffects(removeQE => removeQE.Name == TripodSetupQEffect.Name);
            }
            return null;
        }

        /// <summary>
        /// Toggles the Two Handed trait for Fatal Aim D12 items
        /// </summary>
        /// <param name="self">Item owner</param>
        private static void ToggleTwoHanded(Creature self)
        {
            Item? fatalAimItem = self.HeldItems.FirstOrDefault(item => item.HasTrait(FatalAimD12Trait));
            if (fatalAimItem != null)
            {
                if (fatalAimItem.HasTrait(Trait.TwoHanded))
                {
                    fatalAimItem.Traits.Remove(Trait.TwoHanded);
                }
                else
                {
                    fatalAimItem.Traits.Add(Trait.TwoHanded);
                }
            }
        }

        /// <summary>
        /// Determines if the item can be switched to two handed
        /// </summary>
        /// <param name="self">Item owner</param>
        /// <returns>null if the item can be two handed or a reason why it can't be</returns>
        private static string? WhyCannotSwitchFromOneHandedToTwoHanded(Creature self)
        {
            if (self.HeldItems.Count(item => item.HasTrait(FatalAimD12Trait) && !item.HasTrait(Trait.TwoHanded)) > 0 && !self.HasFreeHand)
            {
                return "Switching from one hand to two hands requires a free hand.";
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets up the Tripod and adds the QEffect
        /// </summary>
        /// <param name="self">The creature setting up the tripod</param>
        private static void SetupTripod(Creature self)
        {
            if (self.QEffects.Count(qe => qe.Name == TripodSetupQEffect.Name) == 0)
            {
                self.AddQEffect(TripodSetupQEffect);
            }
        }

        /// <summary>
        /// Determines if the creature can setup the tripod
        /// </summary>
        /// <param name="self">Item owner</param>
        /// <returns>null if the tripod can be setup and a reason why it can't be</returns>
        private static string? WhyCannotSetupTripod(Creature self)
        {
            if (self.QEffects.Count(qe => qe.Name == TripodSetupQEffect.Name) > 0)
            {
                return "Your tripod is already setup.";
            }
            else
            {
                return null;
            }
        }
    }
}
