using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Damage;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Rules;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Dawnsbury.Mods.GunsAndGears
{
    /// <summary>
    /// The Equipment from Guns and Gears
    /// </summary>
    public static class EquipmentGunsAndGears
    {
        private static readonly QEffect TripodSetupQEffect = new QEffect("Tripod Setup", "Your Tripod is setup, removing the circumstance penality from Kickback till you move.");

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
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/AirRepeater.png"), "Air Repeater", 0, 3, Trait.Agile, Trait.Repeating, FirearmTrait, Trait.Simple)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Piercing)
                        .WithRangeIncrement(6)));

            //  - Coat Pistol
            ModManager.RegisterNewItemIntoTheShop("Coat Pistol", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/CoatPistol.png"), "Coat Pistol", 0, 3, Trait.FatalD8, ConcussiveTrait, FirearmTrait, Trait.Simple, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning)
                        .WithRangeIncrement(6)));

            //  - Fire Lance
            ModManager.RegisterNewItemIntoTheShop("Fire Lance", itemName =>
               new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/FireLance.png"), "Fire Lance", 0, 3, Trait.FatalD10, FirearmTrait, Trait.Simple, Trait.Reload2, Trait.TwoHanded)
                   .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Piercing)
                       .WithRangeIncrement(2)));

            //  - Flintlock Musket
            ModManager.RegisterNewItemIntoTheShop("Flintlock Musket", itemName =>
               new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/FlintlockMusket.png"), "Flintlock Musket", 0, 4, Trait.FatalD10, ConcussiveTrait, FirearmTrait, Trait.Simple, Trait.Reload1, Trait.TwoHanded)
                   .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                       .WithRangeIncrement(14)));

            //  - Flintlock Pistol
            ModManager.RegisterNewItemIntoTheShop("Flintlock Pistol", itemName =>
               new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/FlintlockPistol.png"), "Flintlock Pistol", 0, 3, Trait.FatalD8, ConcussiveTrait, FirearmTrait, Trait.Simple, Trait.Reload1)
                   .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning)
                       .WithRangeIncrement(8)));

            //  - Hand Cannon
            ModManager.RegisterNewItemIntoTheShop("Hand Cannon", itemName =>
               new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/HandCannon.png"), "Hand Cannon", 0, 3, Trait.FatalD8, ModularTrait, FirearmTrait, Trait.Simple, Trait.Reload1)
                   .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                       .WithRangeIncrement(6)));

            //  - Long Air Repeater
            ModManager.RegisterNewItemIntoTheShop("Long Air Repeater", itemName =>
               new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/LongAirRepeater.png"), "Long Air Repeater", 0, 5, Trait.Repeating, FirearmTrait, Trait.Simple)
                   .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Piercing)
                       .WithRangeIncrement(12)));

            // Martial Ranged Firearm Weapons
            //  - Arquebus
            ModManager.RegisterNewItemIntoTheShop("Arquebus", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/Arquebus.png"), "Arquebus", 0, 5, Trait.FatalD12, KickbackTrait, ConcussiveTrait, FirearmTrait, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning)
                        .WithRangeIncrement(30)));

            //  - Blunderbuss
            ModManager.RegisterNewItemIntoTheShop("Blunderbuss", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/Blunderbuss.png"), "Blunderbuss", 0, 4, ConcussiveTrait, Scatter10Trait, FirearmTrait, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning)
                        .WithRangeIncrement(8)));

            //  - Clan Pistol
            ModManager.RegisterNewItemIntoTheShop("Clan Pistol", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/ClanPistol.png"), "Clan Pistol", 0, 0, Trait.Dwarf, Trait.FatalD10, ConcussiveTrait, FirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                        .WithRangeIncrement(16)));

            //  - Double-barreled Musket
            ModManager.RegisterNewItemIntoTheShop("Double-barreled Musket", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/Double-barreledMusket.png"), "Double-barreled Musket", 0, 5, Trait.FatalD10, ConcussiveTrait, DoubleBarrelTrait, FirearmTrait, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                        .WithRangeIncrement(12)));

            //  - Double-barreled Pistol
            ModManager.RegisterNewItemIntoTheShop("Double-barreled Pistol", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/Double-barreledPistol.png"), "Double-barreled Pistol", 0, 4, Trait.FatalD8, ConcussiveTrait, DoubleBarrelTrait, FirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning)
                        .WithRangeIncrement(6)));

            //  - Dragon Mouth Pistol
            ModManager.RegisterNewItemIntoTheShop("Dragon Mouth Pistol", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/DragonMouthPistol.png"), "Dragon Mouth Pistol", 0, 5, Trait.FatalD8, ConcussiveTrait, Scatter5Trait, FirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                        .WithRangeIncrement(4)));

            //  - Dueling Pistol
            ModManager.RegisterNewItemIntoTheShop("Dueling Pistol", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/DuelingPistol.png"), "Dueling Pistol", 0, 6, Trait.FatalD10, ConcussiveTrait, FirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                        .WithRangeIncrement(12)));

            //  - Harmona Gun
            ModManager.RegisterNewItemIntoTheShop("Harmona Gun", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/HarmonaGun.png"), "Harmona Gun", 0, 5, KickbackTrait, FirearmTrait, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d10", DamageKind.Bludgeoning)
                        .WithRangeIncrement(30)));

            //  - Jezail
            ModManager.RegisterNewItemIntoTheShop("Jezail", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/Jezail.png"), "Jezail", 0, 5, ConcussiveTrait, FatalAimD12Trait, FirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning)
                        .WithRangeIncrement(18)));

            //  - Mithral Tree
            // TODO: Add Parry
            ModManager.RegisterNewItemIntoTheShop("Mithral Tree", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/MithralTree.png"), "Mithral Tree", 0, 5, Trait.Elf, Trait.FatalD10, ConcussiveTrait, FirearmTrait, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                        .WithRangeIncrement(30)));

            //  - Pepperbox
            ModManager.RegisterNewItemIntoTheShop("Pepperbox", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/Pepperbox.png"), "Pepperbox", 0, 6, Trait.FatalD8, ConcussiveTrait, FirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning)
                        .WithRangeIncrement(12)));

            //  - Slide Pistol
            ModManager.RegisterNewItemIntoTheShop("Slide Pistol", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/SlidePistol.png"), "Slide Pistol", 0, 8, Trait.FatalD10, ConcussiveTrait, FirearmTrait, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)
                        .WithRangeIncrement(6)));

            // Advanced Ranged Firearm Weapons
            //  - Dwarven Scattergun
            ModManager.RegisterNewItemIntoTheShop("Dwarven Scattergun", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/DwarvenScattergun.png"), "Dwarven Scattergun", 0, 5, Trait.Dwarf, ConcussiveTrait, KickbackTrait, Scatter10Trait, FirearmTrait, Trait.Advanced, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning)
                        .WithRangeIncrement(10)));

            //  - Flingflenser
            ModManager.RegisterNewItemIntoTheShop("Flingflenser", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/Flingflenser.png"), "Flingflenser", 0, 3, Trait.Goblin, Trait.Backstabber, Trait.FatalD10, Scatter5Trait, FirearmTrait, Trait.Advanced, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Slashing)
                        .WithRangeIncrement(6)));

            // Other Items
            //  - Firearm Stabalizer
            ModManager.RegisterNewItemIntoTheShop("Firearm Stabalizer", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/Stabalizer.png"), "Firearm Stabalizer", 0, 1)
                    .WithDescription("Reduces the circumstance penality from the Kickback trait to -1."));

            // Other Items
            //  - Tripod
            ModManager.RegisterNewItemIntoTheShop("Tripod", itemName =>
                new Item(itemName, new ModdedIllustration("GunsAndGearsAssets/Tripod.png"), "Tripod", 0, 2)
                    .WithDescription("Requires an action to setup then reduces the circumstance penality from the Kickback trait to -2. This will require another action to resetup if a movement action is taken."));
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
                        QEffect fatalIsUpgraded = new QEffect("Upgraded Fatal Technical Effect", "[this condition has no description]");
                        if (self.Owner.WieldsItem(DoubleBarrelTrait))
                        {
                            self.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                            {
                                ProvideStrikeModifier = delegate (Item doubleBarrelItem)
                                {
                                    if(!doubleBarrelItem.HasTrait(DoubleBarrelTrait))
                                    {
                                        return null;
                                    }
                                    CombatAction combatAction = self.Owner.CreateStrike(doubleBarrelItem, strikeModifiers: new StrikeModifiers()
                                    {
                                        IncreaseWeaponDieByOneStep = true
                                    });
                                    combatAction.WithActionCost(1);
                                    combatAction.Name = "Double Barrel Fire";
                                    combatAction.Illustration = new SideBySideIllustration(combatAction.Illustration, IllustrationName.StarHit);
                                    combatAction.Traits.Add(Trait.Basic);
                                    combatAction.Description = StrikeRules.CreateBasicStrikeDescription(combatAction.StrikeModifiers, "You can fire both barrels of a double barrel weapon in a single Strike to increase the weapon damage die by one step. If the weapon has the fatal trait, this increases the fatal die by one step.", null, null, null);
                                    StrikeModifiers strikeModifiers = combatAction.StrikeModifiers;
                                    strikeModifiers.QEffectForStrike = new QEffect()
                                    {
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

                                                doubleBarrelItem.EphemeralItemProperties.AmmunitionLeftInMagazine = 0;
                                                attackUpgrade.Owner.AddQEffect(fatalIsUpgraded);
                                            }
                                        }
                                    };
                                    ((CreatureTarget)combatAction.Target).WithAdditionalConditionOnTargetCreature((Creature attacker, Creature defender) =>
                                    {
                                        if (doubleBarrelItem.EphemeralItemProperties.AmmunitionLeftInMagazine == 2)
                                        {
                                            return Usability.Usable;
                                        }

                                        return Usability.NotUsable("Both barrels of this weapon must be loaded.");
                                    });
                                    return combatAction;
                                }
                            });
                        }
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
                                self.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                                {
                                    AfterYouTakeAction = async (QEffect cleanup, CombatAction action) =>
                                    {
                                        if (action.Item == item)
                                        {
                                            if (action.ActionId == ActionId.Reload)
                                            {
                                                if (item.EphemeralItemProperties.AmmunitionLeftInMagazine > 2)
                                                {
                                                    item.EphemeralItemProperties.AmmunitionLeftInMagazine = 1;
                                                }
                                            }
                                            else if (action.Name.ToLower().Contains("strike"))
                                            {
                                                item.EphemeralItemProperties.AmmunitionLeftInMagazine--;
                                            }
                                            if (item.EphemeralItemProperties.NeedsReload && item.EphemeralItemProperties.AmmunitionLeftInMagazine == 1)
                                            {
                                                item.EphemeralItemProperties.NeedsReload = false;
                                            }
                                            if (cleanup.Owner.QEffects.Count(eff => eff.Name == fatalIsUpgraded.Name) > 0)
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

                                                cleanup.Owner.RemoveAllQEffects(qe => qe.Name == fatalIsUpgraded.Name);
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
                            if (item.HasTrait(FatalAimD12Trait))
                            {
                                self.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                                {
                                    ProvideMainAction = delegate (QEffect qfTechnical)
                                    {
                                        return new ActionPossibility(new CombatAction(qfTechnical.Owner, new SideBySideIllustration(item.Illustration, IllustrationName.Action), "Switch Grip", new Trait[1]
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
                                    }
                                });
                            }
                            if (item.HasTrait(KickbackTrait))
                            {
                                self.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                                {
                                    ProvideMainAction = delegate (QEffect setupTripodEffect)
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

                                        return null;
                                    },
                                    AfterYouTakeAction = async (QEffect self, CombatAction action) =>
                                    {
                                        if (self.Owner.QEffects.Count(qe => qe.Name == TripodSetupQEffect.Name) > 0 && action.HasTrait(Trait.Move))
                                        {
                                            self.Owner.RemoveAllQEffects(removeQE => removeQE.Name ==  TripodSetupQEffect.Name);
                                        }
                                    },
                                    BonusToAttackRolls = (QEffect self, CombatAction action, Creature? defender) => 
                                    {
                                        if (self.Owner.Abilities.Strength < 14 && self.Owner.QEffects.Count(qe => qe.Name == TripodSetupQEffect.Name) == 0)
                                        {
                                            int penality = (self.Owner.HeldItems.Concat(self.Owner.CarriedItems).Count(item => item.Name == "Firearm Stabalizer") > 0) ? -1 : -2;
                                            return new Bonus(penality, BonusType.Circumstance, "Kickback" + ((penality == -1) ? " (Stablized)" : ""));
                                        }
                                        return null;
                                    },
                                    BonusToDamage = (QEffect self, CombatAction action, Creature defender) =>
                                    {
                                        return new Bonus(1, BonusType.Untyped, "Kickback");
                                    }
                                });
                            }
                            if (item.HasTrait(Scatter5Trait) || item.HasTrait(Scatter10Trait))
                            {
                                if(item.WeaponProperties != null)
                                {
                                    item.WeaponProperties.AdditionalSplashDamageFormula = item.WeaponProperties.DamageDieCount.ToString();
                                    if (item.HasTrait(Scatter10Trait))
                                    {
                                        self.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                                        {
                                            AfterYouTakeAction = async (QEffect self, CombatAction action) =>
                                            {
                                                if (action.Item == item)
                                                {
                                                    if (action.Name.ToLower().Contains("strike") && action.ChosenTargets.ChosenCreature != null && item.WeaponProperties != null)
                                                    {
                                                        Creature targetCreature = action.ChosenTargets.ChosenCreature;
                                                        Map map = targetCreature.Battle.Map;
                                                        List<DamageKind> damageOptions = item.DetermineDamageKinds();
                                                        DamageKind bestDamageToTarget = targetCreature.WeaknessAndResistance.WhatDamageKindIsBestAgainstMe(damageOptions);
                                                        Tile? tile = map.AllTiles.FirstOrDefault(tile => tile.PrimaryOccupant == targetCreature);
                                                        if (tile != null)
                                                        {
                                                            Tile[] tilesToScatterTo = map.AllTiles.Where(tileToCheck => tile.DistanceTo(tileToCheck) == 2).ToArray();
                                                            foreach (Tile tileToScatterTo in tilesToScatterTo)
                                                            {
                                                                Creature? potentalSplashTarget = tileToScatterTo.PrimaryOccupant;
                                                                if (potentalSplashTarget != null && potentalSplashTarget is Creature splashTarget)
                                                                {
                                                                    await splashTarget.DealDirectDamage(action, DiceFormula.FromText(item.WeaponProperties.DamageDieCount.ToString()), splashTarget, CheckResult.Success, bestDamageToTarget);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                        }
                    },
                    YouBeginAction = async (QEffect self, CombatAction action) =>
                    {
                        string actionName = action.Name.ToLower();
                        if (actionName != null && (actionName.Contains("drop") || actionName.Contains("stow")))
                        {
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
                    YouAreDealtLethalDamage = async (QEffect self, Creature attacker, DamageStuff damage, Creature defender) =>
                    {
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

        /// <summary>
        /// Determines if the item can be switched to two handed
        /// </summary>
        /// <param name="self">Item owner</param>
        /// <returns>null if the item can be two handed or a reason why it can't be</returns>
        private static string? WhyCannotDoubleFire(Creature self)
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

        public static void SetupTripod(Creature self)
        {
            if (self.QEffects.Count(qe => qe.Name == TripodSetupQEffect.Name) == 0)
            {
                self.AddQEffect(TripodSetupQEffect);
            }
        }

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
