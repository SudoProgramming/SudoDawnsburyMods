using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.Constants;
using static Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.BarbarianFeatsDb.AnimalInstinctFeat;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents
{
    /// <summary>
    /// A static class containing all Item Names used for Thaumaturge
    /// </summary>
    public static class ThaumaturgeItemNames
    {
        private static readonly string level9TextLeadIn = "\n\n{b}At higher levels:{/b}\n{b}Intensify Vulnerability{/b} {icon:Action}\n";

        /// <summary>
        /// The Amulet Implement Item
        /// </summary>
        public static readonly ItemName Amulet = ModManager.RegisterNewItemIntoTheShop("Amulet Implement", itemName => new Implement(Enums.ImplementIDs.Amulet, itemName, ThaumaturgeModdedIllustrations.Amulet, "Amulet", "{b}" + ImplementDetails.AmuletInitiateBenefitName + "{/b} {icon:Reaction}\n\n" + ImplementDetails.AmuletInitiateBenefitRulesText + level9TextLeadIn + ImplementDetails.AmuletIntensifyVulnerabilityRulesText));

        /// <summary>
        /// The Bell Implement Item
        /// </summary>
        public static readonly ItemName Bell = ModManager.RegisterNewItemIntoTheShop("Bell Implement", itemName => new Implement(Enums.ImplementIDs.Bell, itemName, ThaumaturgeModdedIllustrations.Bell, "Bell", "{b}" + ImplementDetails.BellInitiateBenefitName + "{/b} {icon:Reaction}\n\n" + ImplementDetails.BellInitiateBenefitRulesText + level9TextLeadIn + ImplementDetails.BellIntensifyVulnerabilityRulesText));

        /// <summary>
        /// The Chalice Implement Item
        /// </summary>
        public static readonly ItemName Chalice = ModManager.RegisterNewItemIntoTheShop("Chalice Implement", itemName => new Implement(Enums.ImplementIDs.Chalice, itemName, ThaumaturgeModdedIllustrations.Chalice, "Chalice", "{b}" + ImplementDetails.ChaliceInitiateBenefitName + "{/b} {icon:Action}\n\n" + ImplementDetails.ChaliceInitiateBenefitRulesText + level9TextLeadIn + ImplementDetails.ChaliceIntensifyVulnerabilityRulesText));

        /// <summary>
        /// The Lantern Implement Item
        /// </summary>
        public static readonly ItemName Lantern = ModManager.RegisterNewItemIntoTheShop("Lantern Implement", itemName => new Implement(Enums.ImplementIDs.Lantern, itemName, ThaumaturgeModdedIllustrations.Lantern, "Lantern", ImplementDetails.LanternInitiateBenefitRulesText + level9TextLeadIn + ImplementDetails.LanternIntensifyVulnerabilityRulesText));

        /// <summary>
        /// The Amulet Implement Item
        /// </summary>
        public static readonly ItemName Mirror = ModManager.RegisterNewItemIntoTheShop("Mirror Implement", itemName => new Implement(Enums.ImplementIDs.Mirror, itemName, ThaumaturgeModdedIllustrations.Mirror, "Mirror", "{b}" + ImplementDetails.MirrorInitiateBenefitName + "{/b} {icon:Action}\n\n" + ImplementDetails.MirrorInitiateBenefitRulesText + level9TextLeadIn + ImplementDetails.MirrorIntensifyVulnerabilityRulesText));

        /// <summary>
        /// The Amulet Implement Item
        /// </summary>
        public static readonly ItemName Regalia = ModManager.RegisterNewItemIntoTheShop("Regalia Implement", itemName => new Implement(Enums.ImplementIDs.Regalia, itemName, ThaumaturgeModdedIllustrations.Regalia, "Regalia", ImplementDetails.RegaliaInitiateBenefitRulesText + level9TextLeadIn + ImplementDetails.RegaliaIntensifyVulnerabilityRulesText));

        /// <summary>
        /// The Amulet Implement Item
        /// </summary>
        public static readonly ItemName Tome = ModManager.RegisterNewItemIntoTheShop("Tome Implement", itemName => new Implement(Enums.ImplementIDs.Tome, itemName, ThaumaturgeModdedIllustrations.Tome, "Tome", ImplementDetails.TomeInitiateBenefitRulesText + level9TextLeadIn + ImplementDetails.TomeIntensifyVulnerabilityRulesText));

        /// <summary>
        /// The Wand Implement Item
        /// </summary>
        public static readonly ItemName Wand = ModManager.RegisterNewItemIntoTheShop("Wand Implement", itemName => new Implement(Enums.ImplementIDs.Wand, itemName, ThaumaturgeModdedIllustrations.Wand, "Wand", "{b}" + ImplementDetails.WandInitiateBenefitName + "{/b} {icon:TwoActions}\n\n" + ImplementDetails.WandInitiateBenefitRulesText + level9TextLeadIn + ImplementDetails.WandIntensifyVulnerabilityRulesText));

        public static readonly ItemName WeaponImplementChoice = ModManager.RegisterNewItemIntoTheShop("Weapon Implement", itemName =>
        {
            return new Item(itemName, ThaumaturgeModdedIllustrations.ExploitVulnerability, "Weapon Implement", 1, 0, Trait.DoNotAddToShop)
                .WithRuneProperties(new RuneProperties("implement", ThaumaturgeRuneKind.WeaponImplement, "The weapon Implement for a Thaumaturge", "Drag onto a one handed weapon to turn it into a Weapon Implement. (NOTE: At the start of an encounter only the first item with this on it will become an implement. Having multiple on weapons will not work.)", item =>
                {
                    item.Traits.Add(ThaumaturgeTraits.WeaponImplement);
                })
                .WithCanBeAppliedTo((Item rune, Item weapon) =>
                {
                    if (weapon.HasTrait(Trait.TwoHanded))
                    {
                        return "Can not be two handed";
                    }
                    else if (weapon.WeaponProperties == null)
                    {
                        return "Must be a weapon";
                    }

                    return null;
                }));
        });

        /// <summary>
        /// The Looted Esoterica Item
        /// </summary>
        public static readonly ItemName LootedEsoterica = ModManager.RegisterNewItemIntoTheShop("Looted Esoterica", itemName => new Item(itemName, ThaumaturgeModdedIllustrations.LootedEsoterica, "Looted Esoterica", 0, 0, [Trait.DoNotAddToShop]));
    }
}
