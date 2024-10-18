using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.Constants;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents
{
    /// <summary>
    /// A static class containing all QEffect IDs used for Thaumaturge
    /// </summary>
    public static class ThaumaturgeItemNames
    {
        /// <summary>
        /// The Amulet Implement Item
        /// </summary>
        public static readonly ItemName Amulet = ModManager.RegisterNewItemIntoTheShop("Amulet Implement", itemName => new Implement(itemName, IllustrationName.GenericCombatManeuver, "Amulet", "{b}" + ImplementDetails.AmuletInitiateBenefitName + "{/b} {icon:Reaction}\n\n" + ImplementDetails.AmuletInitiateBenefitRulesText));

        /// <summary>
        /// The Bell Implement Item
        /// </summary>
        public static readonly ItemName Bell = ModManager.RegisterNewItemIntoTheShop("Bell Implement", itemName => new Implement(itemName, IllustrationName.GenericCombatManeuver, "Bell", "{b}" + ImplementDetails.BellInitiateBenefitName + "{/b} {icon:Reaction}\n\n" + ImplementDetails.BellInitiateBenefitRulesText));

        /// <summary>
        /// The Chalice Implement Item
        /// </summary>
        public static readonly ItemName Chalice = ModManager.RegisterNewItemIntoTheShop("Chalice Implement", itemName => new Implement(itemName, IllustrationName.GenericCombatManeuver, "Chalice", "{b}" + ImplementDetails.ChaliceInitiateBenefitName + "{/b} {icon:Action}\n\n" + ImplementDetails.ChaliceInitiateBenefitRulesText));

        /// <summary>
        /// The Lantern Implement Item
        /// </summary>
        public static readonly ItemName Lantern = ModManager.RegisterNewItemIntoTheShop("Lantern Implement", itemName => new Implement(itemName, IllustrationName.GenericCombatManeuver, "Lantern", ImplementDetails.LanternInitiateBenefitRulesText));

        /// <summary>
        /// The Amulet Implement Item
        /// </summary>
        public static readonly ItemName Mirror = ModManager.RegisterNewItemIntoTheShop("Mirror Implement", itemName => new Implement(itemName, IllustrationName.GenericCombatManeuver, "Mirror", "{b}" + ImplementDetails.MirrorInitiateBenefitName + "{/b} {icon:Action}\n\n" + ImplementDetails.MirrorInitiateBenefitRulesText));

        /// <summary>
        /// The Amulet Implement Item
        /// </summary>
        public static readonly ItemName Regalia = ModManager.RegisterNewItemIntoTheShop("Regalia Implement", itemName => new Implement(itemName, IllustrationName.GenericCombatManeuver, "Regalia", ImplementDetails.RegaliaInitiateBenefitRulesText));

        /// <summary>
        /// The Amulet Implement Item
        /// </summary>
        public static readonly ItemName Tome = ModManager.RegisterNewItemIntoTheShop("Tome Implement", itemName => new Implement(itemName, IllustrationName.GenericCombatManeuver, "Tome", ImplementDetails.TomeInitiateBenefitRulesText));

        /// <summary>
        /// The Amulet Implement Item
        /// </summary>
        public static readonly ItemName Wand = ModManager.RegisterNewItemIntoTheShop("Wand Implement", itemName => new Implement(itemName, IllustrationName.GenericCombatManeuver, "Wand", "{b}" + ImplementDetails.WandInitiateBenefitName + "{/b} {icon:TwoActions}\n\n" + ImplementDetails.WandInitiateBenefitRulesText));

        /// <summary>
        /// The Amulet Implement Item
        /// </summary>
        public static readonly ItemName Weapon = ModManager.RegisterNewItemIntoTheShop("Weapon Implement", itemName => new Implement(itemName, IllustrationName.GenericCombatManeuver, "Weapon", "{b}" + ImplementDetails.WeaponInitiateBenefitName + "{/b} {icon:Reaction}\n\n" + ImplementDetails.WeaponInitiateBenefitRulesText));

        /// <summary>
        /// The Amulet Implement Item
        /// </summary>
        public static readonly ItemName LootedEsoterica = ModManager.RegisterNewItemIntoTheShop("Looted Esoterica", itemName => new Item(itemName, IllustrationName.GenericCombatManeuver, "Looted Esoterica", 0, 0, [Trait.DoNotAddToShop]));
    }
}
