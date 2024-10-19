using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Modding;
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
        public static readonly ItemName Amulet = ModManager.RegisterNewItemIntoTheShop("Amulet Implement", itemName => new Implement(itemName, ThaumaturgeModdedIllustrations.Amulet, "Amulet", "{b}" + ImplementDetails.AmuletInitiateBenefitName + "{/b} {icon:Reaction}\n\n" + ImplementDetails.AmuletInitiateBenefitRulesText));

        /// <summary>
        /// The Bell Implement Item
        /// </summary>
        public static readonly ItemName Bell = ModManager.RegisterNewItemIntoTheShop("Bell Implement", itemName => new Implement(itemName, ThaumaturgeModdedIllustrations.Bell, "Bell", "{b}" + ImplementDetails.BellInitiateBenefitName + "{/b} {icon:Reaction}\n\n" + ImplementDetails.BellInitiateBenefitRulesText));

        /// <summary>
        /// The Chalice Implement Item
        /// </summary>
        public static readonly ItemName Chalice = ModManager.RegisterNewItemIntoTheShop("Chalice Implement", itemName => new Implement(itemName, ThaumaturgeModdedIllustrations.Chalice, "Chalice", "{b}" + ImplementDetails.ChaliceInitiateBenefitName + "{/b} {icon:Action}\n\n" + ImplementDetails.ChaliceInitiateBenefitRulesText));

        /// <summary>
        /// The Lantern Implement Item
        /// </summary>
        public static readonly ItemName Lantern = ModManager.RegisterNewItemIntoTheShop("Lantern Implement", itemName => new Implement(itemName, ThaumaturgeModdedIllustrations.Lantern, "Lantern", ImplementDetails.LanternInitiateBenefitRulesText));

        /// <summary>
        /// The Amulet Implement Item
        /// </summary>
        public static readonly ItemName Mirror = ModManager.RegisterNewItemIntoTheShop("Mirror Implement", itemName => new Implement(itemName, ThaumaturgeModdedIllustrations.Mirror, "Mirror", "{b}" + ImplementDetails.MirrorInitiateBenefitName + "{/b} {icon:Action}\n\n" + ImplementDetails.MirrorInitiateBenefitRulesText));

        /// <summary>
        /// The Amulet Implement Item
        /// </summary>
        public static readonly ItemName Regalia = ModManager.RegisterNewItemIntoTheShop("Regalia Implement", itemName => new Implement(itemName, ThaumaturgeModdedIllustrations.Regalia, "Regalia", ImplementDetails.RegaliaInitiateBenefitRulesText));

        /// <summary>
        /// The Amulet Implement Item
        /// </summary>
        public static readonly ItemName Tome = ModManager.RegisterNewItemIntoTheShop("Tome Implement", itemName => new Implement(itemName, ThaumaturgeModdedIllustrations.Tome, "Tome", ImplementDetails.TomeInitiateBenefitRulesText));

        /// <summary>
        /// The Wand Implement Item
        /// </summary>
        public static readonly ItemName Wand = ModManager.RegisterNewItemIntoTheShop("Wand", itemName => new Implement(itemName, ThaumaturgeModdedIllustrations.Wand, "Wand", "{b}" + ImplementDetails.WandInitiateBenefitName + "{/b} {icon:TwoActions}\n\n" + ImplementDetails.WandInitiateBenefitRulesText));

        /// <summary>
        /// The Amulet Implement Item
        /// </summary>
        public static readonly ItemName LootedEsoterica = ModManager.RegisterNewItemIntoTheShop("Looted Esoterica", itemName => new Item(itemName, ThaumaturgeModdedIllustrations.LootedEsoterica, "Looted Esoterica", 0, 0, [Trait.DoNotAddToShop]));
    }
}
