using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.GunsAndGears
{
    /// <summary>
    /// The Equipment from Guns and Gears
    /// </summary>
    public static class EquipmentGunsAndGears
    {
        /// <summary>
        /// Adds all the Guns and Gears items
        /// </summary>
        public static void AddItems()
        {
            // TODO: Add to all Firearm trait and group

            // Simple Ranged Firearm Weapons
            //  - Air Repeater
            // TODO: Add Magazine
            ModManager.RegisterNewItemIntoTheShop("Air Repeater", itemName =>
                new Item(itemName, IllustrationName.Unknown, "airrepeater", 0, 3, Trait.Agile, Trait.Repeating, Trait.Simple)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Piercing)
                        .WithRangeIncrement(6)));

            //  - Coat Pistol
            // TODO: Add Concussive
            ModManager.RegisterNewItemIntoTheShop("Coat Pistol", itemName =>
                new Item(itemName, IllustrationName.Unknown, "coatpistol", 0, 3, Trait.FatalD8, Trait.Simple, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Piercing)
                        .WithRangeIncrement(6)));

            //  - Fire Lance
            ModManager.RegisterNewItemIntoTheShop("Fire Lance", itemName =>
               new Item(itemName, IllustrationName.Unknown, "firelance", 0, 3, Trait.FatalD10, Trait.Simple, Trait.Reload2, Trait.TwoHanded)
                   .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Piercing)
                       .WithRangeIncrement(2)));

            //  - Flintlock Musket
            // TODO: Add Concussive
            ModManager.RegisterNewItemIntoTheShop("Flintlock Musket", itemName =>
               new Item(itemName, IllustrationName.Unknown, "flintlockmusket", 0, 4, Trait.FatalD10, Trait.Simple, Trait.Reload1, Trait.TwoHanded)
                   .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Piercing)
                       .WithRangeIncrement(14)));

            //  - Flintlock Pistol
            // TODO: Add Concussive
            ModManager.RegisterNewItemIntoTheShop("Flintlock Pistol", itemName =>
               new Item(itemName, IllustrationName.Unknown, "flintlockpistol", 0, 3, Trait.FatalD8, Trait.Simple, Trait.Reload1)
                   .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Piercing)
                       .WithRangeIncrement(8)));

            //  - Hand Cannon
            // TODO: Add Modular
            ModManager.RegisterNewItemIntoTheShop("Hand Cannon", itemName =>
               new Item(itemName, IllustrationName.Unknown, "handcannon", 0, 3, Trait.FatalD8, Trait.Simple, Trait.Reload1)
                   .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                       .WithRangeIncrement(6)));

            //  - Long Air Repeater
            // TODO: Add Magazine
            ModManager.RegisterNewItemIntoTheShop("Long Air Repeater", itemName =>
               new Item(itemName, IllustrationName.Unknown, "longairrepeater", 0, 5, Trait.Repeating, Trait.Simple)
                   .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Piercing)
                       .WithRangeIncrement(12)));

            // Martial Ranged Firearm Weapons
            //  - Arquebus
            // TODO: Add Concussive and Kickback
            ModManager.RegisterNewItemIntoTheShop("Arquebus", itemName =>
                new Item(itemName, IllustrationName.Unknown, "arquebus", 0, 5, Trait.FatalD12, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Piercing)
                        .WithRangeIncrement(30)));

            //  - Blunderbuss
            // TODO: Add Concussive and Scatter
            ModManager.RegisterNewItemIntoTheShop("Blunderbuss", itemName =>
                new Item(itemName, IllustrationName.Unknown, "blunderbuss", 0, 4, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Piercing)
                        .WithRangeIncrement(8)));

            //  - Clan Pistol
            // TODO: Add Concussive
            ModManager.RegisterNewItemIntoTheShop("Clan Pistol", itemName =>
                new Item(itemName, IllustrationName.Unknown, "clanpistol", 0, 0, Trait.Dwarf, Trait.FatalD10, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Piercing)
                        .WithRangeIncrement(16)));

            //  - Double-barreled Musket
            // TODO: Add Concussive and Double Barrel
            ModManager.RegisterNewItemIntoTheShop("Double-barreled Musket", itemName =>
                new Item(itemName, IllustrationName.Unknown, "doublebarreledmusket", 0, 5, Trait.FatalD10, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Piercing)
                        .WithRangeIncrement(12)));

            //  - Double-barreled Pistol
            // TODO: Add Concussive and Double Barrel
            ModManager.RegisterNewItemIntoTheShop("Double-barreled Pistol", itemName =>
                new Item(itemName, IllustrationName.Unknown, "doublebarreledpistol", 0, 4, Trait.FatalD8, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Piercing)
                        .WithRangeIncrement(6)));

            //  - Dragon Mouth Pistol
            // TODO: Add Concussive and Scatter
            ModManager.RegisterNewItemIntoTheShop("Dragon Mouth Pistol", itemName =>
                new Item(itemName, IllustrationName.Unknown, "dragonmouthpistol", 0, 5, Trait.FatalD8, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Piercing)
                        .WithRangeIncrement(4)));

            //  - Dueling Pistol
            // TODO: Add Concussive
            ModManager.RegisterNewItemIntoTheShop("Dueling Pistol", itemName =>
                new Item(itemName, IllustrationName.Unknown, "duelingpistol", 0, 6, Trait.FatalD10, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Piercing)
                        .WithRangeIncrement(12)));

            //  - Harmona Gun
            // TODO: Add Kickback
            ModManager.RegisterNewItemIntoTheShop("Harmona Gun", itemName =>
                new Item(itemName, IllustrationName.Unknown, "harmonagun", 0, 5, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d10", DamageKind.Bludgeoning)
                        .WithRangeIncrement(30)));

            //  - Jezail
            // TODO: Add Concussive and Fatal Aim
            ModManager.RegisterNewItemIntoTheShop("Jezail", itemName =>
                new Item(itemName, IllustrationName.Unknown, "jezail", 0, 5, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Piercing)
                        .WithRangeIncrement(18)));

            //  - Mithral Tree
            // TODO: Add Concussive and Parry
            ModManager.RegisterNewItemIntoTheShop("Mithral Tree", itemName =>
                new Item(itemName, IllustrationName.Unknown, "mithraltree", 0, 5, Trait.Elf, Trait.FatalD10, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Piercing)
                        .WithRangeIncrement(30)));

            //  - Pepperbox
            // TODO: Add Capacity and Concussive
            ModManager.RegisterNewItemIntoTheShop("Pepperbox", itemName =>
                new Item(itemName, IllustrationName.Unknown, "pepperbox", 0, 6, Trait.FatalD8, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Piercing)
                        .WithRangeIncrement(12)));

            //  - Slide Pistol
            // TODO: Add Capacity and Concussive
            ModManager.RegisterNewItemIntoTheShop("Slide Pistol", itemName =>
                new Item(itemName, IllustrationName.Unknown, "slidepistol", 0, 8, Trait.FatalD10, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Piercing)
                        .WithRangeIncrement(6)));
        }
    }
}
