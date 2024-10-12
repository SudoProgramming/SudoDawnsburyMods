using Dawnsbury.Campaign.Encounters;
using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Display.Text;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.Enums;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dawnsbury.Display;
using Dawnsbury.Auxiliary;
using Microsoft.Xna.Framework;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Roller;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge.Utilities
{
    /// <summary>
    /// A static class for utility methods for Thaumaturges
    /// </summary>
    public static class ThaumaturgeUtilities
    {
        public static bool IsCreatureWeildingImplement(Creature creature)
        {
            if (creature.HeldItems.Count(item => item.HasTrait(ThaumaturgeTraits.Implement)) > 0)
            {
                return true;
            }

            return false;
        }

        public static CalculatedNumber RollEsotericLore(CombatAction action, Creature roller, Creature? defender)
        {
            int level = roller.Level;
            int proficiency = level + ((roller.Level >= 3) ? 4 : 2);
            return new CalculatedNumber(roller.Abilities.Charisma + proficiency, "Exploit Vulnerability Check", new List<Bonus?>());
        }

        public static CalculatedNumber CalculateEsotericLoreDC(CombatAction action, Creature roller, Creature? defender)
        {
            Creature dcByLevelTarget = defender ?? roller;
            return new CalculatedNumber(CalculateDCByLevel(dcByLevelTarget.Level), "DC by Level", new List<Bonus?>());
        }

        public static int CalculateDCByLevel(int level)
        {
            int[] dcsByLevel = [14, 15, 16, 18, 19, 20, 22, 23, 24];
            level = Math.Min(Math.Max(level, 0), dcsByLevel.Length);
            return dcsByLevel[level];
        }

        public static int CalculateClassDC(Creature creature, Trait classTrait)
        {
            return 10 + creature.Abilities.Get(creature.Abilities.KeyAbility) + creature.Proficiencies.Get(classTrait).ToNumber(creature.ProficiencyLevel);
        }

        public static FeatName LookupImplementFeatName(ImplementIDs implementID)
        {
            switch(implementID)
            {
                case ImplementIDs.Amulet:
                    return ThaumaturgeFeatNames.AmuletImplement;
                case ImplementIDs.Bell:
                    return ThaumaturgeFeatNames.BellImplement;
                case ImplementIDs.Chalice:
                    return ThaumaturgeFeatNames.ChaliceImplement;
                case ImplementIDs.Lantern:
                    return ThaumaturgeFeatNames.LanternImplement;
                case ImplementIDs.Mirror:
                    return ThaumaturgeFeatNames.MirrorImplement;
                case ImplementIDs.Regalia:
                    return ThaumaturgeFeatNames.RegaliaImplement;
                case ImplementIDs.Tome:
                    return ThaumaturgeFeatNames.TomeImplement;
                case ImplementIDs.Wand:
                    return ThaumaturgeFeatNames.WandImplement;
                case ImplementIDs.Weapon:
                    return ThaumaturgeFeatNames.WeaponImplement;
                default:
                    throw new InvalidOperationException("Implement ID is not supported: " + implementID);
            }
        }

        public static Implement CreateImplement(ImplementIDs implementID)
        {
            switch (implementID)
            {
                case ImplementIDs.Amulet:
                    return (Implement)Items.CreateNew(ImplementItemNames.Amulet);
                case ImplementIDs.Bell:
                    return (Implement)Items.CreateNew(ImplementItemNames.Bell);
                case ImplementIDs.Chalice:
                    return (Implement)Items.CreateNew(ImplementItemNames.Chalice);
                case ImplementIDs.Lantern:
                    return (Implement)Items.CreateNew(ImplementItemNames.Lantern);
                case ImplementIDs.Mirror:
                    return (Implement)Items.CreateNew(ImplementItemNames.Mirror);
                case ImplementIDs.Regalia:
                    return (Implement)Items.CreateNew(ImplementItemNames.Regalia);
                case ImplementIDs.Tome:
                    return (Implement)Items.CreateNew(ImplementItemNames.Tome);
                case ImplementIDs.Wand:
                    return (Implement)Items.CreateNew(ImplementItemNames.Wand);
                case ImplementIDs.Weapon:
                    return (Implement)Items.CreateNew(ImplementItemNames.Weapon);
                default:
                    throw new InvalidOperationException("Implement ID is not supported: " + implementID);
            }
        }

        public static ItemName GetImplementBaseItemName(ImplementIDs implementID)
        {
            switch (implementID)
            {
                case ImplementIDs.Amulet:
                    return ImplementItemNames.Amulet;
                case ImplementIDs.Bell:
                    return ImplementItemNames.Bell;
                case ImplementIDs.Chalice:
                    return ImplementItemNames.Chalice;
                case ImplementIDs.Lantern:
                    return ImplementItemNames.Lantern;
                case ImplementIDs.Mirror:
                    return ImplementItemNames.Mirror;
                case ImplementIDs.Regalia:
                    return ImplementItemNames.Regalia;
                case ImplementIDs.Tome:
                    return ImplementItemNames.Tome;
                case ImplementIDs.Wand:
                    return ImplementItemNames.Wand;
                case ImplementIDs.Weapon:
                    return ImplementItemNames.Weapon;
                default:
                    throw new InvalidOperationException("Implement ID is not supported: " + implementID);
            }
        }

        public static void EnsureCorrectImplements(CalculatedCharacterSheetValues character)
        {
            ImplementIDs[] implementIDs = [ImplementIDs.Amulet, ImplementIDs.Bell, ImplementIDs.Chalice, ImplementIDs.Lantern, ImplementIDs.Mirror, ImplementIDs.Regalia, ImplementIDs.Tome, ImplementIDs.Wand, ImplementIDs.Weapon];
            List<ImplementIDs> implementsToAdd = new List<ImplementIDs>();
            List<ImplementIDs> implementsToRemove = new List<ImplementIDs>();

            foreach (ImplementIDs implementID in implementIDs)
            {
                FeatName featName = LookupImplementFeatName(implementID);
                if (character.HasFeat(featName))
                {
                    implementsToAdd.Add(implementID);
                }
                else
                {
                    implementsToRemove.Add(implementID);
                }
            }

            foreach (ImplementIDs implementID in implementsToRemove)
            {
                RemoveImplement(character, GetImplementBaseItemName(implementID));
            }

            foreach (ImplementIDs implementID in implementsToAdd)
            {
                AddImplement(character, CreateImplement(implementID));
            }
        }

        private static void AddImplement(CalculatedCharacterSheetValues character, Implement implement)
        {
            // Add Implement to Sheet
            int[] levels = character.Sheet.InventoriesByLevel.Keys.ToArray();
            foreach (int level in levels)
            {
                Inventory inventory = character.Sheet.InventoriesByLevel[level];
                Inventory campaignInventory = character.Sheet.CampaignInventory;
                if ((inventory.LeftHand == null || inventory.LeftHand.BaseItemName != implement.BaseItemName) && (inventory.RightHand == null || inventory.RightHand.BaseItemName != implement.BaseItemName) && !inventory.Backpack.Any(item => item != null && item.BaseItemName == implement.BaseItemName) && !campaignInventory.Backpack.Any(item => item != null && item.BaseItemName == implement.BaseItemName))
                {
                    if (inventory.RightHand == null)
                    {
                        inventory.RightHand = implement;
                    }
                    else if (inventory.LeftHand == null)
                    {
                        inventory.LeftHand = implement;
                    }
                    else
                    {
                        if (inventory.CanBackpackFit(implement, 0))
                        {
                            inventory.AddAtEndOfBackpack(implement);
                        }
                        else
                        {
                            campaignInventory.AddAtEndOfBackpack(implement);
                        }
                    }
                }
            }
        }

        private static void RemoveImplement(CalculatedCharacterSheetValues character, ItemName implementItemName)
        {
            // Remove Implements from Sheet
            int[] levels = character.Sheet.InventoriesByLevel.Keys.ToArray();
            foreach (int level in levels)
            {
                Inventory inventory = character.Sheet.InventoriesByLevel[level];
                Inventory campaignInventory = character.Sheet.CampaignInventory;
                Item?[] backpackMatchingImplements = inventory.Backpack.Where(item => item != null && item.BaseItemName == implementItemName).ToArray();
                Item?[] campaignMatchingImplements = campaignInventory.Backpack.Where(item => item != null && item.BaseItemName == implementItemName).ToArray();
                if ((inventory.LeftHand != null && inventory.LeftHand.BaseItemName == implementItemName) || (inventory.RightHand != null && inventory.RightHand.BaseItemName == implementItemName) || backpackMatchingImplements.Length > 0 || campaignMatchingImplements.Length > 0)
                {
                    if (inventory.RightHand != null && inventory.RightHand.BaseItemName == implementItemName)
                    {
                        inventory.RightHand = null;
                    }
                    if (inventory.LeftHand != null && inventory.LeftHand.BaseItemName == implementItemName)
                    {
                        inventory.LeftHand = null;
                    }
                    foreach (Item? implement in backpackMatchingImplements)
                    {
                        if (implement != null)
                        {
                            inventory.Backpack.Remove(implement);
                        }
                    }
                    foreach (Item? implement in campaignMatchingImplements)
                    {
                        if (implement != null)
                        {
                            campaignInventory.Backpack.Remove(implement);
                        }
                    }
                }
            }
        }

        public static CombatAction CreateSeek(Creature owner, IllustrationName illustrationName, string name, AreaTarget areaTarget, int actionCost = 1)
        {
            return new CombatAction(owner, illustrationName, name, [Trait.Concentrate, Trait.Secret, Trait.Basic, Trait.IsNotHostile, Trait.DoesNotBreakStealth, Trait.AttackDoesNotTargetAC], "Make a Perception against against the Stealth DCs of any undetected or hidden creatures in the area." + S.FourDegreesOfSuccess("The creature stops being Undetected, and becomes Observed to you.", "If the creature is Undetected, it stops being Undetected; otherwise, if it's Hidden to you, it becomes Observed.", (string)null, (string)null), (Target)areaTarget.WithIncludeOnlyIf((Func<AreaTarget, Creature, bool>)((target, enemy) => enemy.EnemyOf(target.OwnerAction.Owner) && enemy.DetectionStatus.IsHiddenToAnEnemy)))
            {
                ActionId = ActionId.Seek,
                ActionCost = actionCost
            }.WithActiveRollSpecification(new ActiveRollSpecification(Checks.Perception(), Checks.DefenseDC(Defense.Stealth))).WithEffectOnEachTarget((Delegates.EffectOnEachTarget)(async (spell, caster, target, result) =>
            {
                switch (result)
                {
                    case CheckResult.Success:
                        if (target.DetectionStatus.Undetected)
                        {
                            target.DetectionStatus.Undetected = false;
                            target.Occupies.Overhead("detected", Color.Yellow, target?.ToString() + " is no longer undetected.");
                            break;
                        }
                        if (!target.DetectionStatus.HiddenTo.Remove(caster))
                            break;
                        target.Occupies.Overhead("seen", Color.Black, target?.ToString() + " is no longer hidden to " + caster?.ToString() + ".");
                        break;
                    case CheckResult.CriticalSuccess:
                        if (target.DetectionStatus.Undetected)
                        {
                            target.DetectionStatus.Undetected = false;
                            target.Occupies.Overhead("detected", Color.Yellow, target?.ToString() + " is no longer undetected.");
                        }
                        if (!target.DetectionStatus.HiddenTo.Remove(caster))
                            break;
                        target.Occupies.Overhead("seen", Color.Black, target?.ToString() + " is no longer hidden to " + caster?.ToString() + ".");
                        break;
                }
            })).WithEffectOnChosenTargets((Delegates.EffectOnChosenTargets)(async (spell, caster, targets) =>
            {
                foreach (Tile tile in targets.ChosenTiles)
                {
                    foreach (TileQEffect qeffect in tile.QEffects)
                    {
                        if (qeffect.SeekDC != 0)
                        {
                            CheckBreakdown breakdown = CombatActionExecution.BreakdownAttack(new CombatAction(caster, (Illustration)IllustrationName.Seek, name, Array.Empty<Trait>(), "", (Target)Target.Self()).WithActiveRollSpecification(new ActiveRollSpecification(Checks.Perception(), Checks.FlatDC(qeffect.SeekDC))), Creature.DefaultCreature);
                            CheckBreakdownResult breakdownResult = new CheckBreakdownResult(breakdown);
                            if (breakdownResult.CheckResult >= CheckResult.Success)
                            {
                                tile.Overhead(breakdownResult.CheckResult.HumanizeTitleCase2(), Color.LightBlue, caster?.ToString() + " rolls " + breakdownResult.CheckResult.HumanizeTitleCase2() + " on " + name + ".", name, breakdown.DescribeWithFinalRollTotal(breakdownResult));
                                await qeffect.WhenSeeked.InvokeIfNotNull();
                            }
                        }
                    }
                }
            }));
        }

        public static List<Resistance> GetHighestWeaknesses(Creature creature)
        {
            int highestWeaknesses = creature.WeaknessAndResistance.Weaknesses.Max(weakness => weakness.Value);
            return creature.WeaknessAndResistance.Weaknesses.Where(weakness => weakness.Value == highestWeaknesses).ToList();
        }

        public static int DetermineBonusIncreaseForDefense(Creature creature, Defense defense)
        {
            int total = 0;
            foreach (Bonus? bonus in creature.Defenses.DetermineDefenseBonuses(null, null, defense, creature))
            {
                if (bonus != null)
                {
                    total += bonus.Amount;
                }
            }
            return total;
        }
    }
}
