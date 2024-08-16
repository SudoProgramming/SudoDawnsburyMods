using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Display.Illustrations;
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
        /// Adds the weapon group trait of Firearm
        /// </summary>
        public static readonly Trait FirearmTrait = ModManager.RegisterTrait("Firearm", new TraitProperties("Firearm", true, relevantForShortBlock: true));

        /// <summary>
        /// Adds the concussive trait for firearms
        /// </summary>
        public static readonly Trait ConcussiveTrait = ModManager.RegisterTrait("Concussive", new TraitProperties("Concussive", true, "Deals either bludgeoning or piercing damage, whichever is better for you.", relevantForShortBlock: true));

        /// <summary>
        /// Adds the Double Barrel trait for firearms
        /// </summary>
        public static readonly Trait DoubleBarrelTrait = ModManager.RegisterTrait("Double Barrel", new TraitProperties("Double Barrel", true, "This weapon has two barrels that are each loaded separately. You can fire both barrels of a double barrel weapon in a single Strike to increase the weapon damage die by one step. If the weapon has the fatal trait, this increases the fatal die by one step.", relevantForShortBlock: true));

        /// <summary>
        /// Adds the Double Barrel trait for firearms
        /// </summary>
        public static readonly Trait FatalAimD12Trait = ModManager.RegisterTrait("Fatal Aim D12", new TraitProperties("Fatal Aim D12", true, "This weapon can be held in 1 or 2 hands. You can interact as an action to switch your grip on it as it is more complicated than just releasing one hand. When held with 2 hands, it gains the Fatal D12 trait.", relevantForShortBlock: true));

        /// <summary>
        /// Adds the modular trait for firearms
        /// TODO: Consider rewriting this in V3, as meeting RAW should be easier
        /// </summary>
        public static readonly Trait ModularTrait = ModManager.RegisterTrait("Modular", new TraitProperties("Modular", true, "Deals either bludgeoning, piercing or slashing damage, whichever is better for you.", relevantForShortBlock: true));

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
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/AirRepeater.png"), "airrepeater", 0, 3, Trait.Agile, Trait.Repeating, FirearmTrait, Trait.Simple)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Piercing)
                        .WithRangeIncrement(6)));

            //  - Coat Pistol
            ModManager.RegisterNewItemIntoTheShop("Coat Pistol", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/CoatPistol.png"), "coatpistol", 0, 3, Trait.FatalD8, ConcussiveTrait, FirearmTrait, Trait.Simple, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning)
                        .WithRangeIncrement(6)));

            //  - Fire Lance
            ModManager.RegisterNewItemIntoTheShop("Fire Lance", itemName =>
               new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/FireLance.png"), "firelance", 0, 3, Trait.FatalD10, FirearmTrait, Trait.Simple, Trait.Reload2, Trait.TwoHanded)
                   .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Piercing)
                       .WithRangeIncrement(2)));

            //  - Flintlock Musket
            ModManager.RegisterNewItemIntoTheShop("Flintlock Musket", itemName =>
               new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/FlintlockMusket.png"), "flintlockmusket", 0, 4, Trait.FatalD10, ConcussiveTrait, FirearmTrait, Trait.Simple, Trait.Reload1, Trait.TwoHanded)
                   .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                       .WithRangeIncrement(14)));

            //  - Flintlock Pistol
            ModManager.RegisterNewItemIntoTheShop("Flintlock Pistol", itemName =>
               new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/FlintlockPistol.png"), "flintlockpistol", 0, 3, Trait.FatalD8, ConcussiveTrait, FirearmTrait, Trait.Simple, Trait.Reload1)
                   .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning)
                       .WithRangeIncrement(8)));

            //  - Hand Cannon
            ModManager.RegisterNewItemIntoTheShop("Hand Cannon", itemName =>
               new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/HandCannon.png"), "handcannon", 0, 3, Trait.FatalD8, ModularTrait, FirearmTrait, Trait.Simple, Trait.Reload1)
                   .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                       .WithRangeIncrement(6)));

            //  - Long Air Repeater
            ModManager.RegisterNewItemIntoTheShop("Long Air Repeater", itemName =>
               new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/LongAirRepeater.png"), "longairrepeater", 0, 5, Trait.Repeating, FirearmTrait, Trait.Simple)
                   .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Piercing)
                       .WithRangeIncrement(12)));

            // Martial Ranged Firearm Weapons
            //  - Arquebus
            // TODO: Add Kickback
            ModManager.RegisterNewItemIntoTheShop("Arquebus", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/Arquebus.png"), "arquebus", 0, 5, Trait.FatalD12, ConcussiveTrait, FirearmTrait, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning)
                        .WithRangeIncrement(30)));

            //  - Blunderbuss
            // TODO: Add Scatter
            ModManager.RegisterNewItemIntoTheShop("Blunderbuss", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/Blunderbuss.png"), "blunderbuss", 0, 4, ConcussiveTrait, FirearmTrait, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning)
                        .WithRangeIncrement(8)));

            //  - Clan Pistol
            ModManager.RegisterNewItemIntoTheShop("Clan Pistol", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/ClanPistol.png"), "clanpistol", 0, 0, Trait.Dwarf, Trait.FatalD10, ConcussiveTrait, FirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                        .WithRangeIncrement(16)));

            //  - Double-barreled Musket
            ModManager.RegisterNewItemIntoTheShop("Double-barreled Musket", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/Double-barreledMusket.png"), "doublebarreledmusket", 0, 5, Trait.FatalD10, ConcussiveTrait, DoubleBarrelTrait, FirearmTrait, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                        .WithRangeIncrement(12)));

            //  - Double-barreled Pistol
            ModManager.RegisterNewItemIntoTheShop("Double-barreled Pistol", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/Double-barreledPistol.png"), "doublebarreledpistol", 0, 4, Trait.FatalD8, ConcussiveTrait, DoubleBarrelTrait, FirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning)
                        .WithRangeIncrement(6)));

            //  - Dragon Mouth Pistol
            // TODO: Add Scatter
            ModManager.RegisterNewItemIntoTheShop("Dragon Mouth Pistol", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/DragonMouthPistol.png"), "dragonmouthpistol", 0, 5, Trait.FatalD8, ConcussiveTrait, FirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                        .WithRangeIncrement(4)));

            //  - Dueling Pistol
            ModManager.RegisterNewItemIntoTheShop("Dueling Pistol", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/DuelingPistol.png"), "duelingpistol", 0, 6, Trait.FatalD10, ConcussiveTrait, FirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                        .WithRangeIncrement(12)));

            //  - Harmona Gun
            // TODO: Add Kickback
            ModManager.RegisterNewItemIntoTheShop("Harmona Gun", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/HarmonaGun.png"), "harmonagun", 0, 5, FirearmTrait, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d10", DamageKind.Bludgeoning)
                        .WithRangeIncrement(30)));

            //  - Jezail
            ModManager.RegisterNewItemIntoTheShop("Jezail", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/Jezail.png"), "jezail", 0, 5, ConcussiveTrait, FatalAimD12Trait, FirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning)
                        .WithRangeIncrement(18)));

            //  - Mithral Tree
            // TODO: Add Parry
            ModManager.RegisterNewItemIntoTheShop("Mithral Tree", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/MithralTree.png"), "mithraltree", 0, 5, Trait.Elf, Trait.FatalD10, ConcussiveTrait, FirearmTrait, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                        .WithRangeIncrement(30)));

            //  - Pepperbox
            // TODO: Add Capacity
            ModManager.RegisterNewItemIntoTheShop("Pepperbox", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/Pepperbox.png"), "pepperbox", 0, 6, Trait.FatalD8, ConcussiveTrait, FirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning)
                        .WithRangeIncrement(12)));

            //  - Slide Pistol
            // TODO: Add Capacity
            ModManager.RegisterNewItemIntoTheShop("Slide Pistol", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/SlidePistol.png"), "slidepistol", 0, 8, Trait.FatalD10, ConcussiveTrait, FirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                        .WithRangeIncrement(6)));

            // Advanced Ranged Firearm Weapons
            //  - Dwarven Scattergun
            // TODO: Add Kickback and Scatter
            ModManager.RegisterNewItemIntoTheShop("Dwarven Scattergun", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/DwarvenScattergun.png"), "dwarvenscattergun", 0, 5, Trait.Dwarf, ConcussiveTrait, FirearmTrait, Trait.Advanced, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning)
                        .WithRangeIncrement(10)));

            //  - Flingflenser
            // TODO: Add Scatter
            ModManager.RegisterNewItemIntoTheShop("Flingflenser", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/Flingflenser.png"), "flingflenser", 0, 3, Trait.Goblin, Trait.Backstabber, Trait.FatalD10, FirearmTrait, Trait.Advanced, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Slashing)
                        .WithRangeIncrement(6)));
        }

        /// <summary>
        /// Sets up all the trait logic for these items
        /// </summary>
        public static void SetupTraitLogic()
        {
            ModManager.RegisterActionOnEachCreature(creature =>
            {
                creature.AddQEffect(new QEffect
                {
                    StateCheck = (QEffect self) =>
                    {
                        foreach (Item item in self.Owner.HeldItems)
                        {
                            if (item.HasTrait(ConcussiveTrait) && !item.HasTrait(Trait.VersatileP))
                            {
                                item.Traits.Add(Trait.VersatileP);
                            }
                            if (item.HasTrait(ModularTrait) && !item.HasTrait(Trait.VersatileP) && !item.HasTrait(Trait.VersatileS))
                            {
                                item.Traits.Add(Trait.VersatileP);
                                item.Traits.Add(Trait.VersatileS);
                            }
                            if (item.HasTrait(DoubleBarrelTrait))
                            {
                                
                            }
                            if (item.HasTrait(FatalAimD12Trait))
                            {
                                self.Owner.AddQEffect(new QEffect
                                {
                                    ProvideMainAction = delegate (QEffect qfTechnical)
                                    {
                                        Creature owner2 = qfTechnical.Owner;
                                        int num = owner2.Level + owner2.Abilities.Constitution;
                                        return new ActionPossibility(new CombatAction(owner2, IllustrationName.Action, "Switch Grip", new Trait[1]
                                        {
                                            Trait.Manipulate
                                        }, "You switch your grip on your weapon, either removing a hand to hold it in one hand or adding a hand to hold it with two hands.", Target.Self().WithAdditionalRestriction(WhyCannotSwitchFromOneHandedToTwoHanded))
                                        {
                                            ShortDescription = "Switch your grip on your weapon.",
                                        }.WithActionCost(1).WithEffectOnSelf((Action<Creature>)ToggleTwoHanded));
                                    },
                                    StartOfCombat = async (QEffect self) =>
                                    {
                                        Item? fatalAimItem = self.Owner.HeldItems.FirstOrDefault(item => item.HasTrait(FatalAimD12Trait));
                                        if (fatalAimItem != null && !fatalAimItem.HasTrait(Trait.TwoHanded) && self.Owner.HasFreeHand && await self.Owner.Battle.AskForConfirmation(self.Owner, fatalAimItem.Illustration, "Change grip to two handed as a free action?", "Two handed"))
                                        {
                                            fatalAimItem.Traits.Add(Trait.TwoHanded);
                                        }
                                    },
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
                                    },
                                    ExpiresAt = ExpirationCondition.Ephemeral
                                });
                            }
                        }
                    },
                    YouBeginAction = async (QEffect self, CombatAction action) =>
                    {
                        string actionName = action.Name.ToLower();
                        Item? fatalAimItem = self.Owner.HeldItems.FirstOrDefault(item => item.HasTrait(FatalAimD12Trait));
                        if (actionName != null && (actionName.Contains("drop") || actionName.Contains("stow")) && fatalAimItem != null && actionName.Contains(fatalAimItem.Name.ToLower()) && fatalAimItem.HasTrait(Trait.TwoHanded) && fatalAimItem.HasTrait(FatalAimD12Trait))
                        {
                            fatalAimItem.Traits.Remove(Trait.TwoHanded);
                        }
                    },
                    AfterYouTakeAction = async (QEffect self, CombatAction action) =>
                    {
                        string actionName = action.Name.ToLower();
                        Item? fatalAimItem = self.Owner.HeldItems.FirstOrDefault(item => item.HasTrait(FatalAimD12Trait));
                        if (actionName != null && (actionName.Contains("pick up") || actionName.Contains("draw")) && fatalAimItem != null && !fatalAimItem.HasTrait(Trait.TwoHanded) && self.Owner.HasFreeHand && await self.Owner.Battle.AskForConfirmation(self.Owner, fatalAimItem.Illustration, "Change grip to two handed as a free action?", "Two handed"))
                        {
                            fatalAimItem.Traits.Add(Trait.TwoHanded);
                        }
                    },
                });
            });
        }

        /// <summary>
        /// Toggles the Two Handed trait for Fatal Aim D12 items
        /// </summary>
        /// <param name="self">Item owner</param>
        public static void ToggleTwoHanded(Creature self)
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
    }
}
