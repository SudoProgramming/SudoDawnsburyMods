using Dawnsbury.Auxiliary;
using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Coroutines.Options;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Damage;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Display;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Display.Text;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.Constants;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.Enums;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static CombatAction CreateExploitVulnerabilityAction(Creature owner)
        {
            CombatAction exploitVulnerabilityAction = new CombatAction(
                owner,
                ThaumaturgeModdedIllustrations.ExploitVulnerability,
                "Exploit Vulnerability",
                [Trait.Manipulate, Trait.Basic, ThaumaturgeTraits.Thaumaturge],
                "{b}Frequency{/b} once per round; {b}Requirements{/b} You are holding your implement.\nYou scour your experiences and learning to identify something that might repel your foe. You retrieve an object from your esoterica with the appropriate supernatural qualities, then use your implement to stoke the remnants of its power into a blaze. Select a creature you can see and attempt an Esoteric Lore check against a standard DC for its level, as you retrieve the right object from your esoterica and use your implement to empower it. You gain the following effects until you Exploit Vulnerabilities again.\n{b}Success{/b} Your unarmed and weapon Strikes activate the highest weakness againt the target, even though the damage type your weapon deals doesn't change. This damage affects the target of your Exploit Vulnerability, as well as any other creatures of the exact same type, but not other creatures with the same weakness. The {b}Failure{/b} result is used if the target has no weakness or if it is better.\n{b}Failure{/b} This causes the target creature, and only the target creature, to gain a weakness against your unarmed and weapon Strikes equal to 2 + half your level.\n{b}Critical Failure{/b} You become flat-footed until the beginning of your next turn.",
                Target.Ranged(100)
                .WithAdditionalConditionOnTargetCreature((attacker, defender) => attacker.HasEffect(ThaumaturgeQEIDs.UsedExploitVulnerability) ? Usability.NotUsable("Already Exploited Vulnerability this turn") : Usability.Usable))
                .WithActionId(ThaumaturgeActionIDs.ExploitVulnerability)
                .WithActiveRollSpecification(new ActiveRollSpecification(ThaumaturgeUtilities.RollEsotericLore, ThaumaturgeUtilities.CalculateEsotericLoreDC))
                .WithEffectOnEachTarget(async delegate (CombatAction action, Creature attacker, Creature defender, CheckResult result)
                {
                    // Clear all old Exploit Vulnerabilities
                    QEffect? oldExploitVulnerability = attacker.FindQEffect(ThaumaturgeQEIDs.ExploitVulnerabilityWeakness);
                    if (oldExploitVulnerability != null)
                    {
                        oldExploitVulnerability.ExpiresAt = ExpirationCondition.Immediately;
                        foreach (Creature creature in attacker.Battle.AllCreatures)
                        {
                            QEffect? oldTarget = creature.FindQEffect(ThaumaturgeQEIDs.ExploitVulnerabilityTarget);
                            if (oldTarget != null && oldTarget.Tag != null && oldTarget.Tag == attacker)
                            {
                                oldTarget.ExpiresAt = ExpirationCondition.Immediately;
                            }
                        }
                    }
                    attacker.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtStartOfYourTurn)
                    {
                        Id = ThaumaturgeQEIDs.UsedExploitVulnerability
                    });
                    bool skipAntithesis = false;
                    if (result >= CheckResult.Success)
                    {
                        if (defender.WeaknessAndResistance.Weaknesses.Count(resistance => resistance.DamageKind != ThaumaturgeDamageKinds.PersonalAntithesis) > 0)
                        {
                            List<Resistance> weaknesses = ThaumaturgeUtilities.GetHighestWeaknesses(defender);
                            Resistance weakness = weaknesses[0];
                            if (weaknesses.Count > 1)
                            {
                                ChoiceButtonOption selectedWeakness = await attacker.AskForChoiceAmongButtons(ThaumaturgeModdedIllustrations.ExploitVulnerability, "Which weakness would you like to exploit against all " + defender.BaseName + "?", weaknesses.Select(weakness => weakness.DamageKind.HumanizeTitleCase2()).ToArray());
                                weakness = weaknesses[selectedWeakness.Index];
                            }
                            if (weakness.Value >= 2 + Math.Floor(attacker.Level / 2.0))
                            {
                                skipAntithesis = true;
                                foreach (Creature creature in attacker.Battle.AllCreatures.Where(creature => !creature.FriendOf(attacker) && creature.BaseName == defender.BaseName))
                                {
                                    creature.AddQEffect(new QEffect(ExpirationCondition.Never)
                                    {
                                        Id = ThaumaturgeQEIDs.ExploitVulnerabilityTarget,
                                        Illustration = ThaumaturgeModdedIllustrations.ExploitVulnerabilityBackground,
                                        Name = "Exploited Vulnerability",
                                        Description = "Exploited Weakness by " + attacker.Name + " - " + weakness.DamageKind.HumanizeTitleCase2() + " " + weakness.Value,
                                        Tag = attacker
                                    });
                                }
                                attacker.AddQEffect(new QEffect(ExpirationCondition.Never)
                                {
                                    Id = ThaumaturgeQEIDs.ExploitVulnerabilityWeakness,
                                    Tag = defender,
                                    Illustration = ThaumaturgeModdedIllustrations.ExploitVulnerabilityBackground,
                                    Name = "Exploit Vulnerability",
                                    Description = "Exploiting Weakness to all " + defender.BaseName + " - " + weakness.DamageKind.HumanizeTitleCase2() + " " + weakness.Value,
                                    AddExtraKindedDamageOnStrike = (CombatAction action, Creature damageTarget) =>
                                    {
                                        if (damageTarget == defender || damageTarget.BaseName == defender.BaseName)
                                        {
                                            return new KindedDamage(DiceFormula.FromText("0", "Exploit Vulnerability - Weakness " + weakness.DamageKind), weakness.DamageKind);
                                        }

                                        return null;
                                    }
                                });
                            }
                        }
                    }
                    // Add CLear Logic on Reuse
                    if (result >= CheckResult.Failure && !skipAntithesis)
                    {
                        int antithesisAmount = (int)(2 + Math.Floor(attacker.Level / 2.0));
                        defender.AddQEffect(new QEffect(ExpirationCondition.Never)
                        {
                            Id = ThaumaturgeQEIDs.ExploitVulnerabilityTarget,
                            Tag = attacker,
                            Illustration = ThaumaturgeModdedIllustrations.ExploitVulnerabilityBackground,
                            Name = "Exploited Vulnerability",
                            Description = "Exploited Weakness by " + attacker.Name + " - " + ThaumaturgeDamageKinds.PersonalAntithesis.HumanizeTitleCase2() + " " + antithesisAmount,
                            StateCheck = (QEffect stateCheck) =>
                            {
                                Creature owner = stateCheck.Owner;
                                if (owner.HasEffect(ThaumaturgeQEIDs.ExploitVulnerabilityTarget))
                                {
                                    if (!owner.WeaknessAndResistance.Weaknesses.Any(weakness => weakness.DamageKind == ThaumaturgeDamageKinds.PersonalAntithesis))
                                    {
                                        owner.WeaknessAndResistance.AddWeakness(ThaumaturgeDamageKinds.PersonalAntithesis, antithesisAmount);
                                    }
                                }
                                else if (owner.WeaknessAndResistance.Weaknesses.Any(weakness => weakness.DamageKind == ThaumaturgeDamageKinds.PersonalAntithesis))
                                    {
                                        owner.WeaknessAndResistance.Weaknesses.RemoveAll(weakness => weakness.DamageKind == ThaumaturgeDamageKinds.PersonalAntithesis);
                                    }
                                }
                            });
                            attacker.AddQEffect(new QEffect(ExpirationCondition.Never)
                            {
                                Id = ThaumaturgeQEIDs.ExploitVulnerabilityWeakness,
                                Tag = defender,
                                Illustration = ThaumaturgeModdedIllustrations.ExploitVulnerabilityBackground,
                                Name = "Exploit Vulnerability",
                                Description = "Exploiting Weakness to " + defender.Name + " - " + ThaumaturgeDamageKinds.PersonalAntithesis.HumanizeTitleCase2() + " " + antithesisAmount,
                                AddExtraKindedDamageOnStrike = (CombatAction action, Creature damageTarget) =>
                                {
                                    if (damageTarget == defender)
                                    {
                                        return new KindedDamage(DiceFormula.FromText("0", "Exploit Vulnerability - " + ThaumaturgeDamageKinds.PersonalAntithesis.HumanizeTitleCase2()), ThaumaturgeDamageKinds.PersonalAntithesis);
                                    }

                                    return null;
                                    }
                                });
                            }
                    else if (result == CheckResult.CriticalFailure)
                    {
                        QEffect flatFooted = QEffect.FlatFooted("Exploit Vulnerability");
                        flatFooted.ExpiresAt = ExpirationCondition.ExpiresAtStartOfYourTurn;
                        attacker.AddQEffect(flatFooted);
                    }
                });

            return exploitVulnerabilityAction;
        }

        public static CalculatedNumber RollEsotericLore(CombatAction action, Creature roller, Creature? defender)
        {
            int level = roller.Level;
            int proficiency = level + ((roller.Level >= 3) ? 4 : 2);
            List<Bonus?> bonusesToRoll = new List<Bonus?>();
            if (roller.HasFeat(ThaumaturgeFeatNames.TomeImplement) && IsCreatureWeildingImplement(roller))
            {
                bonusesToRoll.Add(new Bonus(1, BonusType.Circumstance, ImplementDetails.TomeInitiateBenefitName, true));
            }
            QEffect? instructiveStike = roller.FindQEffect(ThaumaturgeQEIDs.InstructiveStrike);
            if (instructiveStike != null)
            {
                bonusesToRoll.Add(new Bonus(instructiveStike.Value, BonusType.Circumstance, "Instructive Strike", true));
            }
            return new CalculatedNumber(roller.Abilities.Charisma + proficiency, "Exploit Vulnerability Check", bonusesToRoll);
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

        public static Implement CreateImplement(ImplementIDs implementID, CalculatedCharacterSheetValues sheet)
        {
            switch (implementID)
            {
                case ImplementIDs.Amulet:
                    return (Implement)Items.CreateNew(ThaumaturgeItemNames.Amulet);
                case ImplementIDs.Bell:
                    return (Implement)Items.CreateNew(ThaumaturgeItemNames.Bell);
                case ImplementIDs.Chalice:
                    return (Implement)Items.CreateNew(ThaumaturgeItemNames.Chalice);
                case ImplementIDs.Lantern:
                    return (Implement)Items.CreateNew(ThaumaturgeItemNames.Lantern);
                case ImplementIDs.Mirror:
                    return (Implement)Items.CreateNew(ThaumaturgeItemNames.Mirror);
                case ImplementIDs.Regalia:
                    return (Implement)Items.CreateNew(ThaumaturgeItemNames.Regalia);
                case ImplementIDs.Tome:
                    return (Implement)Items.CreateNew(ThaumaturgeItemNames.Tome);
                case ImplementIDs.Wand:
                    return (Implement)Items.CreateNew(ThaumaturgeItemNames.Wand);
                case ImplementIDs.Weapon:
                default:
                    throw new InvalidOperationException("Implement ID is not supported: " + implementID);
            }
        }

        public static ItemName GetImplementBaseItemName(ImplementIDs implementID, CalculatedCharacterSheetValues sheet)
        {
            switch (implementID)
            {
                case ImplementIDs.Amulet:
                    return ThaumaturgeItemNames.Amulet;
                case ImplementIDs.Bell:
                    return ThaumaturgeItemNames.Bell;
                case ImplementIDs.Chalice:
                    return ThaumaturgeItemNames.Chalice;
                case ImplementIDs.Lantern:
                    return ThaumaturgeItemNames.Lantern;
                case ImplementIDs.Mirror:
                    return ThaumaturgeItemNames.Mirror;
                case ImplementIDs.Regalia:
                    return ThaumaturgeItemNames.Regalia;
                case ImplementIDs.Tome:
                    return ThaumaturgeItemNames.Tome;
                case ImplementIDs.Wand:
                    return ThaumaturgeItemNames.Wand;
                case ImplementIDs.Weapon:
                default:
                    throw new InvalidOperationException("Implement ID is not supported: " + implementID);
            }
        }

        public static void EnsureCorrectImplements(CalculatedCharacterSheetValues character)
        {
            ImplementIDs[] implementIDs = [ImplementIDs.Amulet, ImplementIDs.Bell, ImplementIDs.Chalice, ImplementIDs.Lantern, ImplementIDs.Mirror, ImplementIDs.Regalia, ImplementIDs.Tome, ImplementIDs.Wand];
            List<ImplementIDs> implementsToAdd = new List<ImplementIDs>();
            List<ImplementIDs> implementsToRemove = new List<ImplementIDs>();

            foreach (ImplementIDs implementID in implementIDs)
            {
                FeatName featName = LookupImplementFeatName(implementID);
                List<FeatName> feats = character.AllFeatNames.ToList();
                List<string> humanized = feats.Select(feat => feat.HumanizeTitleCase2()).ToList();
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
                if (implementID != ImplementIDs.Weapon)
                {
                    RemoveImplement(character, GetImplementBaseItemName(implementID, character));
                }
            }

            foreach (ImplementIDs implementID in implementsToAdd)
            {
                if (implementID != ImplementIDs.Weapon)
                {
                    AddImplement(character, CreateImplement(implementID, character));
                }
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
