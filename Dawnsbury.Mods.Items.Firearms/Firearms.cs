using Dawnsbury.Core;
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
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Mods.Items.Firearms.RegisteredComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using Dawnsbury.Mods.Items.Firearms.Utilities;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Dawnsbury.Mods.Items.Firearms
{
    /// <summary>
    /// The Equipment from Guns and Gears
    /// </summary>
    public static class Firearms
    {
        /// <summary>
        /// Register all the Guns and Gears items
        /// </summary>
        public static void RegisterItems()
        {
            // Simple Ranged Firearm Weapons
            //  - Air Repeater
            ModManager.RegisterNewItemIntoTheShop("Air Repeater", itemName =>
                new Item(itemName, FirearmModdedIllustrations.AirRepeater, "Air Repeater", 0, 3, Trait.Agile, Trait.Repeating, Trait.Firearm, Trait.Simple)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Piercing) { Sfx = FirearmSFXNames.SmallFirearm1 }
                        .WithRangeIncrement(6)));

            //  - Coat Pistol
            ModManager.RegisterNewItemIntoTheShop("Coat Pistol", itemName =>
                new Item(itemName, FirearmModdedIllustrations.CoatPistol, "Coat Pistol", 0, 3, Trait.FatalD8, FirearmTraits.Concussive, Trait.Firearm, Trait.Simple, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning) { Sfx = FirearmSFXNames.SmallFirearm2 }
                        .WithRangeIncrement(6)));

            //  - Fire Lance
            ModManager.RegisterNewItemIntoTheShop("Fire Lance", itemName =>
               new Item(itemName, FirearmModdedIllustrations.FireLance, "Fire Lance", 0, 3, Trait.FatalD10, Trait.Firearm, Trait.Simple, Trait.Reload2, Trait.TwoHanded)
                   .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Piercing) { Sfx = FirearmSFXNames.LargeFirearm1 }
                       .WithRangeIncrement(2)));

            //  - Flintlock Musket
            ModManager.RegisterNewItemIntoTheShop("Flintlock Musket", itemName =>
               new Item(itemName, FirearmModdedIllustrations.FlintlockMusket, "Flintlock Musket", 0, 4, Trait.FatalD10, FirearmTraits.Concussive, Trait.Firearm, Trait.Simple, Trait.Reload1, Trait.TwoHanded)
                   .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning) { Sfx = FirearmSFXNames.LargeFirearm2 }
                       .WithRangeIncrement(14)));

            //  - Flintlock Pistol
            ModManager.RegisterNewItemIntoTheShop("Flintlock Pistol", itemName =>
               new Item(itemName, FirearmModdedIllustrations.FlintlockPistol, "Flintlock Pistol", 0, 3, Trait.FatalD8, FirearmTraits.Concussive, Trait.Firearm, Trait.Simple, Trait.Reload1)
                   .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning) { Sfx = FirearmSFXNames.SmallFirearm2 }
                       .WithRangeIncrement(8)));

            //  - Hand Cannon
            ModManager.RegisterNewItemIntoTheShop("Hand Cannon", itemName =>
               new Item(itemName, FirearmModdedIllustrations.HandCannon, "Hand Cannon", 0, 3, Trait.FatalD8, FirearmTraits.Modular, Trait.Firearm, Trait.Simple, Trait.Reload1)
                   .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning) { Sfx = FirearmSFXNames.SmallFirearm2 }
                       .WithRangeIncrement(6)));

            //  - Long Air Repeater
            ModManager.RegisterNewItemIntoTheShop("Long Air Repeater", itemName =>
               new Item(itemName, FirearmModdedIllustrations.LongAirRepeater, "Long Air Repeater", 0, 5, Trait.Repeating, Trait.Firearm, Trait.Simple)
                   .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Piercing) { Sfx = FirearmSFXNames.SmallFirearm1 }
                       .WithRangeIncrement(12)));

            // Martial Ranged Firearm Weapons
            //  - Arquebus
            ModManager.RegisterNewItemIntoTheShop("Arquebus", itemName =>
                new Item(itemName, FirearmModdedIllustrations.Arquebus, "Arquebus", 0, 5, Trait.FatalD12, FirearmTraits.Kickback, FirearmTraits.Concussive, Trait.Firearm, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning) { Sfx = FirearmSFXNames.LargeFirearm1 }
                        .WithRangeIncrement(30)));

            //  - Blunderbuss
            ModManager.RegisterNewItemIntoTheShop("Blunderbuss", itemName =>
                new Item(itemName, FirearmModdedIllustrations.Blunderbuss, "Blunderbuss", 0, 4, FirearmTraits.Concussive, FirearmTraits.Scatter10, Trait.Firearm, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning) { Sfx = FirearmSFXNames.LargeFirearm2 }
                        .WithRangeIncrement(8)));

            //  - Clan Pistol
            ModManager.RegisterNewItemIntoTheShop("Clan Pistol", itemName =>
                new Item(itemName, FirearmModdedIllustrations.ClanPistol, "Clan Pistol", 0, 0, Trait.Dwarf, Trait.FatalD10, FirearmTraits.Concussive, Trait.Firearm, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning) { Sfx = FirearmSFXNames.SmallFirearm2 }
                        .WithRangeIncrement(16)));

            //  - Double-barreled Musket
            ModManager.RegisterNewItemIntoTheShop("Double-barreled Musket", itemName =>
                new Item(itemName, FirearmModdedIllustrations.DoublebarreledMusket, "Double-barreled Musket", 0, 5, Trait.FatalD10, FirearmTraits.Concussive, FirearmTraits.DoubleBarrel, Trait.Firearm, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning) { Sfx = FirearmSFXNames.LargeFirearm1 }
                        .WithRangeIncrement(12)));

            //  - Double-barreled Pistol
            ModManager.RegisterNewItemIntoTheShop("Double-barreled Pistol", itemName =>
                new Item(itemName, FirearmModdedIllustrations.DoublebarreledPistol, "Double-barreled Pistol", 0, 4, Trait.FatalD8, FirearmTraits.Concussive, FirearmTraits.DoubleBarrel, Trait.Firearm, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning) { Sfx = FirearmSFXNames.SmallFirearm1 }
                        .WithRangeIncrement(6)));

            //  - Dragon Mouth Pistol
            ModManager.RegisterNewItemIntoTheShop("Dragon Mouth Pistol", itemName =>
                new Item(itemName, FirearmModdedIllustrations.DragonMouthPistol, "Dragon Mouth Pistol", 0, 5, Trait.FatalD8, FirearmTraits.Concussive, FirearmTraits.Scatter5, Trait.Firearm, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning) { Sfx = FirearmSFXNames.SmallFirearm2 }
                        .WithRangeIncrement(4)));

            //  - Dueling Pistol
            ModManager.RegisterNewItemIntoTheShop("Dueling Pistol", itemName =>
                new Item(itemName, FirearmModdedIllustrations.DuelingPistol, "Dueling Pistol", 0, 6, Trait.FatalD10, FirearmTraits.Concussive, Trait.Firearm, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning) { Sfx = FirearmSFXNames.SmallFirearm1 }
                        .WithRangeIncrement(12)));

            //  - Harmona Gun
            ModManager.RegisterNewItemIntoTheShop("Harmona Gun", itemName =>
                new Item(itemName, FirearmModdedIllustrations.HarmonaGun, "Harmona Gun", 0, 5, FirearmTraits.Kickback, Trait.Firearm, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d10", DamageKind.Bludgeoning) { Sfx = FirearmSFXNames.LargeFirearm2 }
                        .WithRangeIncrement(30)));

            //  - Jezail
            ModManager.RegisterNewItemIntoTheShop("Jezail", itemName =>
                new Item(itemName, FirearmModdedIllustrations.Jezail, "Jezail", 0, 5, FirearmTraits.Concussive, FirearmTraits.FatalAimD12, Trait.Firearm, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning) { Sfx = FirearmSFXNames.LargeFirearm1 }
                        .WithRangeIncrement(18)));

            //  - Mithral Tree
            ModManager.RegisterNewItemIntoTheShop("Mithral Tree", itemName =>
                new Item(itemName, FirearmModdedIllustrations.MithralTree, "Mithral Tree", 0, 5, Trait.Elf, Trait.FatalD10, FirearmTraits.Concussive, FirearmTraits.Parry, Trait.Firearm, Trait.Martial, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning) { Sfx = FirearmSFXNames.LargeFirearm2 }
                        .WithRangeIncrement(30)));

            //  - Pepperbox
            ModManager.RegisterNewItemIntoTheShop("Pepperbox", itemName =>
                new Item(itemName, FirearmModdedIllustrations.Pepperbox, "Pepperbox", 0, 6, Trait.FatalD8, FirearmTraits.Concussive, Trait.Firearm, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning) { Sfx = FirearmSFXNames.LargeFirearm1 }
                        .WithRangeIncrement(12)));

            //  - Slide Pistol
            ModManager.RegisterNewItemIntoTheShop("Slide Pistol", itemName =>
                new Item(itemName, FirearmModdedIllustrations.SlidePistol, "Slide Pistol", 0, 8, Trait.FatalD10, FirearmTraits.Concussive, Trait.Firearm, Trait.Martial, Trait.Reload1)
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning) { Sfx = FirearmSFXNames.SmallFirearm1 }
                        .WithRangeIncrement(6)));

            // Advanced Ranged Firearm Weapons
            //  - Dwarven Scattergun
            ModManager.RegisterNewItemIntoTheShop("Dwarven Scattergun", itemName =>
                new Item(itemName, FirearmModdedIllustrations.DwarvenScattergun, "Dwarven Scattergun", 0, 5, Trait.Dwarf, FirearmTraits.Concussive, FirearmTraits.Kickback, FirearmTraits.Scatter10, Trait.Firearm, Trait.Advanced, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning) { Sfx = FirearmSFXNames.LargeFirearm1 }
                        .WithRangeIncrement(10)));

            //  - Flingflenser
            ModManager.RegisterNewItemIntoTheShop("Flingflenser", itemName =>
                new Item(itemName, FirearmModdedIllustrations.Flingflenser, "Flingflenser", 0, 3, Trait.Goblin, Trait.Backstabber, Trait.FatalD10, FirearmTraits.Scatter5, Trait.Firearm, Trait.Advanced, Trait.Reload1, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Slashing) { Sfx = FirearmSFXNames.LargeFirearm2 }
                        .WithRangeIncrement(6)));

            // Advanced Ranged Crossbow Weapons
            // Repeating Crossbow
            ModManager.RegisterNewItemIntoTheShop("Repeating Crossbow", itemName =>
                new Item(itemName, IllustrationName.HeavyCrossbow, "Repeating Crossbow", 0, 6, Trait.Uncommon, Trait.Repeating, Trait.Crossbow, Trait.Martial, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Piercing)
                        .WithRangeIncrement(24)));

            // Repeating Heavy Crossbow
            ModManager.RegisterNewItemIntoTheShop("Repeating Heavy Crossbow", itemName =>
                new Item(itemName, IllustrationName.HeavyCrossbow, "Repeating Heavy Crossbow", 0, 5, Trait.Uncommon, Trait.Reload1, FirearmTraits.DummyRepeating5, Trait.Crossbow, Trait.Advanced, Trait.TwoHanded)
                    .WithWeaponProperties(new WeaponProperties("1d10", DamageKind.Piercing)
                        .WithRangeIncrement(36)));

            // Attachable Weapons
            ModManager.RegisterNewItemIntoTheShop("Bayonet", itemName =>
            {
                Item bayonet = new Item(itemName, FirearmModdedIllustrations.Bayonet, "Bayonet", 0, 1, Trait.Agile, Trait.Finesse, Trait.Martial, Trait.Knife, FirearmTraits.AttachedWeapon, FirearmTraits.Bayonet);
                return bayonet
                    .WithRuneProperties(new RuneProperties("Bayonet", FirearmRuneKind.Bayonet, "An attachable blade for firearms and crossbows.", "Attach this to a firearm or crossbow {Red}(By dragging it onto the item){/Red}.\n\nAdds an agile and finesse Knife that deals 1d4 slashing. This blade will use all runes from its attached weapon.", item =>
                    {
                        item.Traits.Add(FirearmTraits.AttachedWeapon);
                        item.Traits.Add(FirearmTraits.Bayonet);
                        item.Tag = new AttachedWeapon(bayonet, item, new WeaponProperties("1d4", DamageKind.Slashing));
                    })
                        .WithCanBeAppliedTo((Item rune, Item weapon) =>
                        {
                            if (!weapon.HasTrait(Trait.Firearm) && !weapon.HasTrait(Trait.Crossbow))
                            {
                                return "Must be attached to a Firearm or Crossbow";
                            }
                            else if (weapon.HasTrait(FirearmTraits.AttachedWeapon)) {
                                return "A weapon attachment is already attached.";
                            }

                            return null;
                        }));
            });

            ModManager.RegisterNewItemIntoTheShop("Reinforced Stock", itemName =>
            {
                Item reinforcedStock = new Item(itemName, FirearmModdedIllustrations.ReinforcedStock, "Reinforced Stock", 0, 1, Trait.Finesse, Trait.Martial, Trait.Club, FirearmTraits.AttachedWeapon, FirearmTraits.ReinforcedStock);
                return reinforcedStock
                    .WithRuneProperties(new RuneProperties("Stock", FirearmRuneKind.ReinforcedStock, "A reinforced stock is a weighted crossbow, firearm haft, or grip customized for striking in melee.", "Attach this to a firearm or crossbow {Red}(By dragging it onto the item){/Red}.\n\nAdds a finesse Club that deals 1d4 bludgening or 1d8 if the item this is attached to is Two-Handed. This club will use all runes from its attached weapon.", item =>
                    {
                        item.Traits.Add(FirearmTraits.AttachedWeapon);
                        item.Traits.Add(FirearmTraits.ReinforcedStock);
                        item.Tag = new AttachedWeapon(reinforcedStock, item,
                            (Item weaponAttachedTo) =>
                            {
                                return weaponAttachedTo.HasTrait(Trait.TwoHanded);
                            }, new WeaponProperties("1d4", DamageKind.Bludgeoning), new WeaponProperties("1d8", DamageKind.Bludgeoning));
                    })
                        .WithCanBeAppliedTo((Item rune, Item weapon) =>
                        {
                            if (!weapon.HasTrait(Trait.Firearm) && !weapon.HasTrait(Trait.Crossbow))
                            {
                                return "Must be attached to a Firearm or Crossbow";
                            }
                            else if (weapon.HasTrait(FirearmTraits.AttachedWeapon))
                            {
                                return "A weapon attachment is already attached.";
                            }

                            return null;
                        }));
            });

            // Other Items
            //  - Firearm Stabalizer
            ModManager.RegisterNewItemIntoTheShop("Firearm Stabalizer", itemName =>
            {
                Item firearmStabalizer = new Item(itemName, FirearmModdedIllustrations.FirearmStabalizer, "Firearm Stabalizer", 0, 1, FirearmTraits.FirearmStabalizer);
                return firearmStabalizer
                    .WithRuneProperties(new RuneProperties("Stabalized", FirearmRuneKind.FirearmStabalizer, "The ferocious kickback of firearms is part of the functionality that makes them so appealing as weapons, but it can also present a barrier in wielding them effectively.", "Attach this to a firearm or crossbow {Red}(By dragging it onto the item){/Red}.\n\nReduce the circumstance penality from the Kickback trait to -1.", item =>
                    {
                        item.Traits.Add(FirearmTraits.FirearmStabalizer);
                    })
                    .WithCanBeAppliedTo((Item rune, Item weapon) =>
                    {
                        if (!weapon.HasTrait(Trait.Firearm) && !weapon.HasTrait(Trait.Crossbow))
                        {
                            return "Must be attached to a Firearm or Crossbow";
                        }

                        return null;
                    }));
            });

            // Other Items
            //  - Tripod
            ModManager.RegisterNewItemIntoTheShop("Tripod", itemName =>
            {
                Item tripod = new Item(itemName, FirearmModdedIllustrations.Tripod, "Tripod", 0, 2, FirearmTraits.Tripod);
                return tripod
                    .WithRuneProperties(new RuneProperties("Tripod", FirearmRuneKind.Tripod, "The ferocious kickback of firearms is part of the functionality that makes them so appealing as weapons, but it can also present a barrier in wielding them effectively.", "Attach this to a firearm or crossbow {Red}(By dragging it onto the item){/Red}.\n\nRequires an action to setup then removes the -2 circumstance penality from the Kickback trait. This will require another action to resetup if a movement action is taken.", item =>
                    {
                        item.Traits.Add(FirearmTraits.Tripod);
                    })
                    .WithCanBeAppliedTo((Item rune, Item weapon) =>
                    {
                        if (!weapon.HasTrait(Trait.Firearm) && !weapon.HasTrait(Trait.Crossbow))
                        {
                            return "Must be attached to a Firearm or Crossbow";
                        }

                        return null;
                    }));
            });
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
                        if (self.Owner.WieldsItem(FirearmTraits.DoubleBarrel))
                        {
                            AddDoubleBarrelFireStrikeAction(self);
                        }

                        // Loops through each item the owner is holding and adds logic for that trait
                        foreach (Item item in self.Owner.HeldItems)
                        {
                            // Adds the VersatileP trait to firearms with the Concussive trait
                            if (item.HasTrait(FirearmTraits.Concussive) && !item.HasTrait(Trait.VersatileP))
                            {
                                item.Traits.Add(Trait.VersatileP);
                            }

                            // Adds both VersatileP and VersatileS to firearms with the Modular trait
                            if (item.HasTrait(FirearmTraits.Modular) && !item.HasTrait(Trait.VersatileP) && !item.HasTrait(Trait.VersatileS))
                            {
                                item.Traits.Add(Trait.VersatileP);
                                item.Traits.Add(Trait.VersatileS);
                            }

                            // Adds logic for all firearms with the Double Barrel trait
                            if (item.HasTrait(FirearmTraits.DoubleBarrel))
                            {
                                AddDoubleBarrelLogic(self, item);
                            }

                            // Adds logic for all firearms with the Fatal Aim D12 trait
                            if (item.HasTrait(FirearmTraits.FatalAimD12))
                            {
                                AddFatalAimLogic(self, item);
                            }

                            // Adds logic for all firearms with the Kickback trait
                            if (item.HasTrait(FirearmTraits.Kickback))
                            {
                                AddKickbackLogic(self, item);
                            }

                            // Adds logic for all firearms with the Scatter trait
                            if (item.HasTrait(FirearmTraits.Scatter5) || item.HasTrait(FirearmTraits.Scatter10))
                            {
                                AddScatterLogic(self, item);
                            }

                            // Adds logic for all weapons with the Parry trait
                            if (item.HasTrait(FirearmTraits.Parry))
                            {
                                AddParryLogic(self, item);
                            }

                            // Adds logic for all firearms with the Misfired trait
                            if (item.HasTrait(FirearmTraits.Misfired))
                            {
                                AddMisfireLogic(self, item);
                            }

                            if (item.HasTrait(FirearmTraits.AttachedWeapon))
                            {
                                AddAttachedWeaponLogic(self, item);
                            }

                            if (item.HasTrait(FirearmTraits.DummyRepeating5))
                            {
                                AddDummyRepeatingLogic(self, item);
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
                        await AddAfterActionCleanupLogic(self, action);
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
                    if (!doubleBarrelItem.HasTrait(FirearmTraits.DoubleBarrel))
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
                    combatAction.Description = StrikeRules.CreateBasicStrikeDescription2(combatAction.StrikeModifiers, "You can fire both barrels of a double barrel weapon in a single Strike to increase the weapon damage die by one step. If the weapon has the fatal trait, this increases the fatal die by one step.", null, null, null, weaponDieIncreased: true);

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
                                attackUpgrade.Owner.AddQEffect(new QEffect(ExpirationCondition.Never)
                                {
                                    Id = FirearmQEIDs.FatalIsUpgraded
                                });
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
                    if (action.Item == item && !action.HasTrait(FirearmTraits.IgnoreDoubleBarrel))
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
                        if (cleanup.Owner.QEffects.Any(eff => eff.Id == FirearmQEIDs.FatalIsUpgraded))
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
                            cleanup.Owner.RemoveAllQEffects(qe => qe.Id == FirearmQEIDs.FatalIsUpgraded);
                        }
                    }
                },
                ProvideActionIntoPossibilitySection = delegate (QEffect self, PossibilitySection section)
                {
                    if (section.PossibilitySectionId == PossibilitySectionId.ItemActions)
                    {
                        if (item.HasTrait(FirearmTraits.DoubleBarrel) && !item.EphemeralItemProperties.NeedsReload && item.EphemeralItemProperties.AmmunitionLeftInMagazine != 2)
                        {
                            return new ActionPossibility(new CombatAction(self.Owner, item.Illustration, "Reload (1/2)", new Trait[1] { Trait.Manipulate }, "Reload the weapon with another piece of ammunition.", Target.Self((Creature cr, AI ai) => 10f)).WithActionId(FirearmActionIDs.DoubleBarrelReload).WithItem(item).WithActionCost(1).WithActionId(ActionId.Reload).WithEffectOnSelf(delegate
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
                    Item? fatalAimItem = self.Owner.HeldItems.FirstOrDefault(item => item.HasTrait(FirearmTraits.FatalAimD12));
                    if (fatalAimItem != null && !fatalAimItem.HasTrait(Trait.TwoHanded) && self.Owner.HasFreeHand && await self.Owner.Battle.AskForConfirmation(self.Owner, fatalAimItem.Illustration, "Change grip to two handed as a free action?", "Two handed"))
                    {
                        fatalAimItem.Traits.Add(Trait.TwoHanded);
                    }
                },

                // Adds or removed the Fatal D12 trait if the item has the two-handed trait
                StateCheck = (QEffect self) =>
                {
                    List<Item> fatalAimItems = self.Owner.HeldItems.Where(item => item.HasTrait(FirearmTraits.FatalAimD12)).ToList();
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
                        if (item.HasTrait(FirearmTraits.Tripod))
                        {
                            return new ActionPossibility(new CombatAction(setupTripodEffect.Owner, new SideBySideIllustration(item.Illustration, FirearmModdedIllustrations.Tripod), "Setup Tripod (" + item.Name + ")", new Trait[1]
                            {
                                                Trait.Manipulate
                            }, "Setup your Tripod which will remove the circumstance penality from Kickback till you move.", Target.Self().WithAdditionalRestriction(WhyCannotSetupTripod))
                            {
                                ShortDescription = "Setup your Tripod."
                            }.WithActionCost(1).WithEffectOnSelf((Creature owner) =>
                            {
                                if (!owner.QEffects.Any(qe => qe.Id == FirearmQEIDs.TripodSetup && qe.Tag is Item tripodItem && tripodItem == item))
                                {
                                    owner.AddQEffect(new QEffect("Tripod Setup (" + item.Name + ")", "You don't take a penalty from the Kickback trait till you move.")
                                    {
                                        ExpiresAt = ExpirationCondition.Never,
                                        Illustration = FirearmModdedIllustrations.Tripod,
                                        Id = FirearmQEIDs.TripodSetup,
                                        Tag = item
                                    });
                                }
                            }));
                        }
                    }

                    return null;
                },

                // Removes the Tripod if you attemp to move away
                AfterYouTakeAction = async (QEffect self, CombatAction action) =>
                {
                    if (item.HasTrait(FirearmTraits.Tripod) && action.HasTrait(Trait.Move))
                    {
                        self.Owner.RemoveAllQEffects(removeQE => removeQE.Id == FirearmQEIDs.TripodSetup);
                    }
                },

                // Adds the penality to attack rolls depending on if you have a tripod setup, just a firearm stabalizer, enough strength, or nothing
                BonusToAttackRolls = (QEffect self, CombatAction action, Creature? defender) =>
                {
                    if (action.Item != null && action.Item.HasTrait(FirearmTraits.Kickback) && !action.HasTrait(FirearmTraits.IgnoreKickbackPenalty) && self.Owner.Abilities.Strength < 2 && !self.Owner.QEffects.Any(qe => qe.Id == FirearmQEIDs.TripodSetup))
                    {
                        int penality = (action.Item.HasTrait(FirearmTraits.FirearmStabalizer)) ? -1 : -2;
                        return new Bonus(penality, BonusType.Circumstance, "Kickback" + ((penality == -1) ? " (Stablized)" : ""));
                    }
                    return null;
                },

                // Adds the bonus damage
                BonusToDamage = (QEffect self, CombatAction action, Creature defender) =>
                {
                    if (action.Item != null && action.Item.HasTrait(FirearmTraits.Kickback))
                    {
                        return new Bonus(1, BonusType.Untyped, "Kickback");
                    }

                    return null;
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
            Creature owner = self.Owner;
            if (item.WeaponProperties != null)
            {
                owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                {
                    AfterYouTakeAction = async (QEffect self, CombatAction action) =>
                    {
                        if (!action.HasTrait(FirearmTraits.IgnoreScatter) && action.CheckResult >= CheckResult.Success && action.Item != null && action.Item == item && action.HasTrait(Trait.Strike) && action.ChosenTargets != null && action.ChosenTargets.ChosenCreature != null && action.ChosenTargets.ChosenCreature != self.Owner && item.WeaponProperties != null)
                        {
                            Creature target = action.ChosenTargets.ChosenCreature;
                            // The best damage against the original target, the map, and tile with the targeted creature is saved for the next checks
                            List<DamageKind> damageOptions = item.DetermineDamageKinds();
                            DamageKind bestDamageToTarget = target.WeaknessAndResistance.WhatDamageKindIsBestAgainstMe(damageOptions);
                            Map map = target.Battle.Map;
                            Tile? tile = map.AllTiles.FirstOrDefault(tile => tile.PrimaryOccupant == target);

                            // Check tile is looped through to see how close it was to the target. If it was exactly 10 feet away the calculated splash damage is applied to any creatures
                            if (tile != null)
                            {
                                int distance = (item.HasTrait(FirearmTraits.Scatter10)) ? 2 : 1;
                                Tile[] tilesToScatterTo = map.AllTiles.Where(tileToCheck => tile.DistanceTo(tileToCheck) <= distance).ToArray();
                                foreach (Tile tileToScatterTo in tilesToScatterTo)
                                {
                                    Creature? potentalSplashTarget = tileToScatterTo.PrimaryOccupant;
                                    if (potentalSplashTarget != null && potentalSplashTarget is Creature splashTarget && splashTarget != target)
                                    {
                                        await CommonSpellEffects.DealDirectSplashDamage(CombatAction.CreateSimple(owner, "Scatter"), DiceFormula.FromText(item.WeaponProperties.DamageDieCount.ToString()), splashTarget, bestDamageToTarget);
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Adds the logic for all Parry weapons
        /// </summary>
        /// <param name="self">The state check</param>
        /// <param name="item">The Parry trait item</param>
        private static void AddParryLogic(QEffect self, Item item)
        {
            // Adds a QEffect that will track all the logic for the Scatter trait
            if (item.WeaponProperties != null)
            {
                self.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                {
                    // Adds a Switch Grip action to the items section to change switch between one-handed and two-handed
                    ProvideActionIntoPossibilitySection = delegate (QEffect self, PossibilitySection section)
                    {
                        if (section.PossibilitySectionId == PossibilitySectionId.ItemActions && !self.Owner.HasEffect(FirearmQEIDs.Parry))
                        {
                            return new ActionPossibility(new CombatAction(self.Owner, new SideBySideIllustration(item.Illustration, IllustrationName.SteelShield), "Parry", [], "While wielding this weapon, if your proficiency with it is trained or better, you can spend a single action to position your weapon defensively, gaining a +1 circumstance bonus to AC until the start of your next turn.", Target.Self())
                            {
                                ShortDescription = "This weapon can be used defensively to block attacks.",
                            }.WithActionCost(1).WithEffectOnSelf((Action<Creature>) (parryTarget =>
                            {
                                parryTarget.AddQEffect(new QEffect("Parry", "While wielding this weapon, if your proficiency with it is trained or better, you can spend a single action to position your weapon defensively, gaining a +1 circumstance bonus to AC until the start of your next turn.")
                                {
                                    Id = FirearmQEIDs.Parry,
                                    Tag = item,
                                    ExpiresAt = ExpirationCondition.ExpiresAtStartOfYourTurn,
                                    BonusToDefenses = (QEffect bonusToAC, CombatAction? action, Defense defense) =>
                                    {
                                        if (defense == Defense.AC)
                                        {
                                            return new Bonus(1, BonusType.Circumstance, "Parry", true);
                                        }

                                        return null;
                                    }
                                });
                            })));
                        }

                        return null;
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
            if (FirearmUtilities.IsItemFirearmOrCrossbow(item) && item.HasTrait(FirearmTraits.Misfired))
            {
                self.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                {
                    // Adds an action to clean firearm to remove the Misfired trait
                    ProvideActionIntoPossibilitySection = delegate (QEffect cleanFirearmEffect, PossibilitySection section)
                    {
                        if (section.PossibilitySectionId == PossibilitySectionId.ItemActions && FirearmUtilities.IsItemFirearmOrCrossbow(item) && item.HasTrait(FirearmTraits.Misfired))
                        {
                            return new ActionPossibility(new CombatAction(cleanFirearmEffect.Owner, new SideBySideIllustration(item.Illustration, IllustrationName.Action), "Remove Jam", [Trait.Basic, Trait.Manipulate], "Remove the jam to remove the misfired trait.", Target.Self()).WithActionCost(1).WithEffectOnSelf(async (action, self) =>
                            {
                                item.Traits.RemoveAll(trait => trait == FirearmTraits.Misfired);
                            }));
                        }

                        return null;
                    },

                    // Prevents taking any action with that item
                    PreventTakingAction = (CombatAction action) =>
                    {
                        if (action.Name != "Remove Jam" && ((action.HasTrait(Trait.Firearm) || action.HasTrait(Trait.Crossbow)) && action.HasTrait(FirearmTraits.Misfired)) || (action.Item != null && FirearmUtilities.IsItemFirearmOrCrossbow(action.Item) && action.Item.HasTrait(FirearmTraits.Misfired)))
                        {
                            return "Jammed from a misfire";
                        }

                        return null;
                    }
                });
            }
        }

        private static void AddDummyRepeatingLogic(QEffect self, Item item)
        {
            self.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
            {
                StartOfCombat = async (QEffect startOfCombat) =>
                {
                    item.Tag = 5;
                },
                StateCheckWithVisibleChanges = async (QEffect stateCheck) =>
                {
                    string itemNameWithoutReload = Regex.Replace(item.Name, @"\s\[[0-9]\/5\]$", string.Empty);
                    if (item.Tag != null && item.Tag is int ammoLeft)
                    {

                        item.Name = itemNameWithoutReload + $" [{ammoLeft}/5]";
                    }
                    else
                    {
                        item.Name = itemNameWithoutReload;
                    }
                },
                AfterYouTakeAction = async (QEffect afterAction, CombatAction action) =>
                {
                    if (action.Item != null && action.Item == item && action.Item.EphemeralItemProperties != null)
                    {
                        if (action.ActionId == ActionId.Reload && action.Item.HasTrait(Trait.Repeating) && !action.Item.EphemeralItemProperties.NeedsReload)
                        {
                            action.Item.Traits.RemoveAll(trait => trait == Trait.Repeating);
                            action.Item.Tag = 5;
                        }
                        else if (action.HasTrait(Trait.Strike))
                        {

                            if (action.Item.Tag != null && action.Item.Tag is int ammoLeft)
                            {
                                action.Item.Tag = ammoLeft - 1;
                                if (ammoLeft - 1 <= 0)
                                {
                                    action.Item.Traits.Add(Trait.Repeating);
                                }
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Adds the logic for all Attached Items to firearms
        /// </summary>
        /// <param name="self">The state check</param>
        /// <param name="item">The item attached to</param>
        private static void AddAttachedWeaponLogic(QEffect self, Item item)
        {
            // Checks for only Firearm or Crossbow items
            if (FirearmUtilities.IsItemFirearmOrCrossbow(item) && item.HasTrait(FirearmTraits.AttachedWeapon) && item.Tag is AttachedWeapon attachedWeapon)
            {
                Creature owner = self.Owner;

                if (owner.HeldItems.Contains(item))
                {
                    self.Owner.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                    {
                        ProvideMainAction = (QEffect mainAction) =>
                        {
                            if (!attachedWeapon.HasTrait(FirearmTraits.ItemUpdated))
                            {
                                attachedWeapon.Traits.Add(FirearmTraits.ItemUpdated);
                                attachedWeapon.WithWeaponProperties(attachedWeapon.GetWeaponProperties());
                                foreach (Item rune in item.Runes)
                                {
                                    if (rune.RuneProperties != null && (rune.RuneProperties.RuneKind != FirearmRuneKind.Bayonet || rune.RuneProperties.RuneKind != FirearmRuneKind.ReinforcedStock))
                                    {
                                        attachedWeapon.WithModification(new ItemModification(ItemModificationKind.Rune)
                                        {
                                            ItemName = rune.ItemName
                                        });
                                    }
                                }
                            }
                            List<Possibility> strikes =
                            [
                                ..owner.QEffects.Select(qf => {
                                    var ca = qf.ProvideStrikeModifier?.Invoke(attachedWeapon);
                                    if (ca != null) {
                                        ca.ContextMenuName = ca.Name + " (" + attachedWeapon.Name + ")";
                                    }

                                    return ca;
                                }).Where(ca => ca != null).Select(strike => new ActionPossibility(strike!))
                            ];
                            CombatAction mainStrike = owner.CreateStrike(attachedWeapon);
                            foreach (var qfStrikeModiers in owner.QEffects)
                            {
                                if (qfStrikeModiers.ProvideStrikeModifierAsPossibilities != null)
                                {
                                    strikes.AddRange(qfStrikeModiers.ProvideStrikeModifierAsPossibilities(qfStrikeModiers, attachedWeapon));
                                }
                            }

                            if (strikes.Count > 0)
                            {
                                return new SubmenuPossibility(mainStrike.Illustration, mainStrike.Name, PossibilitySize.Full)
                                {
                                    SpellIfAny = mainStrike,
                                    Subsections =
                                    {
                                        new PossibilitySection(mainStrike.Name)
                                        {
                                            Possibilities = new ActionPossibility[] { mainStrike }.Concat(strikes).ToList()
                                        }
                                    }
                                };
                            }
                            else
                            {
                                return new ActionPossibility(mainStrike, PossibilitySize.Full);
                            }
                        }
                    });
                }

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
                Item? fatalAimItem = self.Owner.HeldItems.FirstOrDefault(item => item.HasTrait(FirearmTraits.FatalAimD12));
                Item? kickbackItem = self.Owner.HeldItems.FirstOrDefault(item => item.HasTrait(FirearmTraits.Kickback));
                if (fatalAimItem != null && actionName.Contains(fatalAimItem.Name.ToLower()) && fatalAimItem.HasTrait(Trait.TwoHanded) && fatalAimItem.HasTrait(FirearmTraits.FatalAimD12))
                {
                    fatalAimItem.Traits.Remove(Trait.TwoHanded);
                }
                if (kickbackItem != null && actionName.Contains(kickbackItem.Name.ToLower()) && self.Owner.QEffects.Any(qe => qe.Id == FirearmQEIDs.TripodSetup))
                {
                    self.Owner.RemoveAllQEffects(removeQE => removeQE.Id == FirearmQEIDs.TripodSetup);
                }
            }
        }

        /// <summary>
        /// Adds cleanup logic for all after action checks
        /// </summary>
        /// <param name="self">The state check</param>
        /// <param name="action">The action just taken</param>
        private async static Task AddAfterActionCleanupLogic(QEffect self, CombatAction action)
        {
            // Collacts all the fatal aim items for cleanup and handles it
            string actionName = action.Name.ToLower();
            Item? fatalAimItem = self.Owner.HeldItems.FirstOrDefault(item => item.HasTrait(FirearmTraits.FatalAimD12));
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
            Item? fatalAimItem = self.Owner.HeldItems.FirstOrDefault(item => item.HasTrait(FirearmTraits.FatalAimD12));
            Item? kickbackItem = self.Owner.HeldItems.FirstOrDefault(item => item.HasTrait(FirearmTraits.Kickback));
            if (fatalAimItem != null && fatalAimItem.HasTrait(Trait.TwoHanded) && fatalAimItem.HasTrait(FirearmTraits.FatalAimD12))
            {
                fatalAimItem.Traits.Remove(Trait.TwoHanded);
            }
            if (kickbackItem != null && self.Owner.QEffects.Any(qe => qe.Id == FirearmQEIDs.TripodSetup))
            {
                self.Owner.RemoveAllQEffects(removeQE => removeQE.Id == FirearmQEIDs.TripodSetup);
            }
            return null;
        }

        /// <summary>
        /// Toggles the Two Handed trait for Fatal Aim D12 items
        /// </summary>
        /// <param name="self">Item owner</param>
        private static void ToggleTwoHanded(Creature self)
        {
            Item? fatalAimItem = self.HeldItems.FirstOrDefault(item => item.HasTrait(FirearmTraits.FatalAimD12));
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
            if (self.HeldItems.Count(item => item.HasTrait(FirearmTraits.FatalAimD12) && !item.HasTrait(Trait.TwoHanded)) > 0 && !self.HasFreeHand)
            {
                return "Switching from one hand to two hands requires a free hand.";
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Determines if the creature can setup the tripod
        /// </summary>
        /// <param name="self">Item owner</param>
        /// <returns>null if the tripod can be setup and a reason why it can't be</returns>
        private static string? WhyCannotSetupTripod(Creature self)
        {
            if (self.QEffects.Any(qe => qe.Id == FirearmQEIDs.TripodSetup))
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
