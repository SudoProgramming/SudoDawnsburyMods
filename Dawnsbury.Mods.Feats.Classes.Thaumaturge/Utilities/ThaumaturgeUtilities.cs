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
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge.Utilities
{
    /// <summary>
    /// A static class for utility methods for Thaumaturges
    /// </summary>
    public static class ThaumaturgeUtilities
    {
        public static bool IsCreatureHoldingAnyImplement(Creature creature)
        {
            if (creature.HeldItems.Count(item => item.HasTrait(ThaumaturgeTraits.Implement)) > 0)
            {
                return true;
            }

            return false;
        }

        public static bool IsCreatureHoldingOrCarryingImplement(ImplementIDs implementID, Creature creature)
        {
            return AnyHeldImplementsMatchID(implementID, creature) || AnyCarriedImplementsMatchID(implementID, creature);
        }

        public static bool DoesImplementMatchID(ImplementIDs implementID, Item item)
        {
            if (item is Implement implement && implement.ImplementID == implementID)
            {
                return true;
            }
            else if (item is not Implement && implementID == ImplementIDs.Weapon && item.HasTrait(ThaumaturgeTraits.Implement))
            {
                return true;
            }

            return false;
        }

        public static bool AnyHeldImplementsMatchID(ImplementIDs implementID, Creature owner)
        {
            return owner.HeldItems.Any(item => DoesImplementMatchID(implementID, item));
        }

        public static bool AnyCarriedImplementsMatchID(ImplementIDs implementID, Creature owner)
        {
            return owner.CarriedItems.Any(item => DoesImplementMatchID(implementID, item));
        }

        public static async Task<bool> HeldImplementOrSwap(ImplementIDs implementID, Creature owner, string promptText = "", bool ignoreSingleImplementPrompt = true)
        {
            if (AnyHeldImplementsMatchID(implementID, owner))
            {
                return true;
            }

            if (owner.Level >= 5 && !Thaumaturge.HasDedicationFeat(owner))
            {
                return await HandleImplementPromptAndSwap(implementID, owner, ignoreSingleImplementPrompt, promptText);
            }

            return false;
        }

        public static async Task<bool> HandleImplementPromptAndSwap(ImplementIDs implementID, Creature owner, bool ignoreSingleImplementPrompt, string promptText = "")
        {
            if (!AnyCarriedImplementsMatchID(implementID, owner))
            {
                return false;
            }

            Item[] heldImplements = owner.HeldItems.Where(item => item.HasTrait(ThaumaturgeTraits.Implement)).ToArray();

            Item? implementToSwap = null;
            if (heldImplements.Length == 1 && (ignoreSingleImplementPrompt || await owner.AskForConfirmation(ThaumaturgeModdedIllustrations.GetIllustration(implementID), $"Swap {heldImplements[0]} with {implementID.HumanizeTitleCase2()}{promptText}", "Yes")))
            {
                implementToSwap = heldImplements[0];
            }
            else if (heldImplements.Length > 1)
            {
                List<string> buttonOptions = heldImplements.Select(item => item.Name).ToList();
                buttonOptions.Add("Pass");
                ChoiceButtonOption option = await owner.AskForChoiceAmongButtons(ThaumaturgeModdedIllustrations.GetIllustration(implementID), $"Swap which held implement with {implementID.HumanizeTitleCase2()}{promptText}", buttonOptions.ToArray());
                if (option.Index != buttonOptions.Count - 1)
                {
                    implementToSwap = heldImplements[option.Index];
                }
            }

            if (implementToSwap != null)
            {
                Item? carriedImplement = owner.CarriedItems.FirstOrDefault(item => DoesImplementMatchID(implementID, item));
                if (carriedImplement != null)
                {
                    owner.CarriedItems.Remove(carriedImplement);
                    owner.HeldItems.Remove(implementToSwap);
                    owner.HeldItems.Add(carriedImplement);
                    owner.CarriedItems.Add(implementToSwap);

                    return true;
                }
            }

            return false;
        }

        public static CombatAction CreateExploitVulnerabilityAction(Creature owner, bool isGlimpse)
        {
            CombatAction exploitVulnerabilityAction = new CombatAction(
                owner,
                ThaumaturgeModdedIllustrations.ExploitVulnerability,
                !isGlimpse ? "Exploit Vulnerability" : "Glimpse Vulnerability",
                [Trait.Manipulate, Trait.Basic, Trait.UnaffectedByConcealment, ThaumaturgeTraits.Thaumaturge],
                "{b}Frequency{/b} once per round; {b}Requirements{/b} You are holding your implement.\n" + (!isGlimpse ? "Select a creature you can see and attempt an Esoteric Lore check against a standard DC for its level, as you retrieve the right object from your esoterica and use your implement to empower it. You gain the following effects until you Exploit Vulnerabilities again.\n{b}Success{/b} Your unarmed and weapon Strikes activate the highest weakness againt the target, even though the damage type your weapon deals doesn't change. This damage affects the target of your Exploit Vulnerability, as well as any other creatures of the exact same type, but not other creatures with the same weakness. The {b}Failure{/b} result is used if the target has no weakness or if it is better.\n{b}Failure{/b} This causes the target creature, and only the target creature, to gain a weakness against your unarmed and weapon Strikes equal to 2 + half your level.\n{b}Critical Failure{/b} You become flat-footed until the beginning of your next turn." : "Select a creature you can see. Until you Glimpse Vulnerability again, that target gains weakness 2 against your unarmed and weapon Strikes."),
                Target.Ranged(100)
                .WithAdditionalConditionOnTargetCreature((attacker, defender) => attacker.HasEffect(ThaumaturgeQEIDs.UsedExploitVulnerability) ? Usability.NotUsable("Already Exploited Vulnerability this turn") : Usability.Usable))
                .WithActionId(ThaumaturgeActionIDs.ExploitVulnerability)
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

                        List<QEffect> oldWardenEffects = attacker.QEffects.Where(qe => qe.Id == ThaumaturgeQEIDs.EsotericWardenAC || qe.Id == ThaumaturgeQEIDs.EsotericWardenSave).ToList();
                        foreach (QEffect oldWardenEffect in oldWardenEffects)
                        {
                            oldWardenEffect.ExpiresAt = ExpirationCondition.Immediately;
                        }
                    }
                    attacker.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtStartOfYourTurn)
                    {
                        Id = ThaumaturgeQEIDs.UsedExploitVulnerability
                    });
                    bool skipAntithesis = false;
                    bool applyToAll = attacker.HasFeat(ThaumaturgeFeatNames.SympatheticVulnerabilities);
                    if (!isGlimpse && result >= CheckResult.Success)
                    {
                        if (defender.WeaknessAndResistance.Weaknesses.Count(resistance => resistance.DamageKind != ThaumaturgeDamageKinds.PersonalAntithesis && resistance.DamageKind != ThaumaturgeDamageKinds.GlimpseVulnerability) > 0)
                        {
                            List<Resistance> weaknesses = ThaumaturgeUtilities.GetHighestWeaknesses(defender);
                            Resistance weakness = weaknesses[0];
                            if (weaknesses.Count > 1)
                            {
                                ChoiceButtonOption selectedWeakness = await attacker.AskForChoiceAmongButtons(ThaumaturgeModdedIllustrations.ExploitVulnerability, "Which weakness would you like to exploit against all " + (applyToAll ? "creatures with that weakness" : defender.BaseName) + "?", weaknesses.Where(weak => weak is not ResistanceToAll).Select(weakness =>
                                    {
                                        if (weakness is SpecialResistance specialWeakness)
                                        {
                                            return $"{specialWeakness.Name} {specialWeakness.Value}";
                                        }
                                        else
                                        {
                                            return $"{weakness.DamageKind.HumanizeTitleCase2()} {weakness.Value}";
                                        }
                                    }).ToArray());
                                weakness = weaknesses[selectedWeakness.Index];
                            }

                            int personalAntithesisValue = (int)(2 + Math.Floor(attacker.Level / 2.0));
                            if (weakness.Value >= personalAntithesisValue || await attacker.AskForConfirmation(ThaumaturgeModdedIllustrations.ExploitVulnerability, $"The {(weakness is SpecialResistance spec ? spec.Name : weakness.DamageKind.HumanizeLowerCase2())} {weakness.Value} weakness is less than the {personalAntithesisValue} weakness you could apply with Personal Antithesis. Which would you like to use?", (weakness is SpecialResistance spe ? spe.Name : weakness.DamageKind.HumanizeLowerCase2()), "Personal Antithesis"))
                            {
                                skipAntithesis = true;
                                foreach (Creature creature in attacker.Battle.AllCreatures.Where(creature => !creature.FriendOf(attacker) && creature.BaseName == defender.BaseName))
                                {
                                    creature.AddQEffect(new QEffect(ExpirationCondition.Never)
                                    {
                                        Id = ThaumaturgeQEIDs.ExploitVulnerabilityTarget,
                                        Illustration = ThaumaturgeModdedIllustrations.ExploitVulnerabilityBackground,
                                        Name = "Exploited Vulnerability",
                                        Description = "Exploited Weakness by " + attacker.Name + " - " + (weakness is SpecialResistance changed ? changed.Name : weakness.DamageKind.HumanizeLowerCase2()) + " " + weakness.Value,
                                        Tag = attacker
                                    });
                                }

                                ExploitEffect exploitEffect = new ExploitEffect(attacker, defender, isGlimpse, weakness: weakness, applyToAll: applyToAll);
                                QEffect applyEffect = new QEffect()
                                {
                                    Description = defender.Name,
                                    Illustration = IllustrationName.GenericCombatManeuver,
                                    StateCheck = async (QEffect stateCheck) =>
                                    {
                                        foreach (Creature creature in attacker.Battle.AllCreatures.Where(creature => !creature.FriendOf(attacker) && creature.BaseName == defender.BaseName))
                                        {
                                            if (!creature.HasEffect(ThaumaturgeQEIDs.ExploitVulnerabilityTarget) || (creature.FindQEffect(ThaumaturgeQEIDs.ExploitVulnerabilityTarget)?.Tag is Creature a && a == attacker))
                                            {
                                                creature.AddQEffect(new QEffect(ExpirationCondition.Never)
                                                {
                                                    Id = ThaumaturgeQEIDs.ExploitVulnerabilityTarget,
                                                    Illustration = ThaumaturgeModdedIllustrations.ExploitVulnerabilityBackground,
                                                    Name = "Exploited Vulnerability",
                                                    Description = "Exploited Weakness by " + attacker.Name + " - " + (weakness is SpecialResistance changed ? changed.Name : weakness.DamageKind.HumanizeLowerCase2()) + " " + weakness.Value,
                                                    Tag = attacker
                                                });
                                            }
                                        }
                                    }
                                };
                                exploitEffect.WhenExpires = (QEffect expired) =>
                                {
                                    applyEffect.ExpiresAt = ExpirationCondition.Immediately;
                                };
                                attacker.AddQEffect(exploitEffect);
                                attacker.AddQEffect(applyEffect);
                            }
                        }
                    }
                    // Add CLear Logic on Reuse
                    if (isGlimpse || (result >= CheckResult.Failure && !skipAntithesis))
                    {
                        int antithesisAmount = !isGlimpse ? (int)(2 + Math.Floor(attacker.Level / 2.0)) : 2;
                        DamageKind damageKind = !isGlimpse ? ThaumaturgeDamageKinds.PersonalAntithesis : ThaumaturgeDamageKinds.GlimpseVulnerability;

                        List<Creature> applyWeaknessTo = new List<Creature>() { defender };

                        if (applyToAll)
                        {
                            applyWeaknessTo.AddRange(attacker.Battle.AllCreatures.Where(creature => creature != defender && !creature.FriendOf(attacker) && creature.BaseName == defender.BaseName));
                        }

                        foreach (Creature applyWeakness in applyWeaknessTo)
                        {
                            applyWeakness.AddQEffect(new QEffect(ExpirationCondition.Never)
                            {
                                Id = ThaumaturgeQEIDs.ExploitVulnerabilityTarget,
                                Tag = attacker,
                                Illustration = ThaumaturgeModdedIllustrations.ExploitVulnerabilityBackground,
                                Name = "Exploited Vulnerability",
                                Description = "Exploited Weakness by " + attacker.Name + " - " + damageKind.HumanizeTitleCase2() + " " + antithesisAmount,
                                StateCheck = (QEffect stateCheck) =>
                                {
                                    Creature owner = stateCheck.Owner;
                                    if (owner.HasEffect(ThaumaturgeQEIDs.ExploitVulnerabilityTarget))
                                    {
                                        if (!owner.WeaknessAndResistance.Weaknesses.Any(weakness => weakness.DamageKind == damageKind))
                                        {
                                            owner.WeaknessAndResistance.AddWeakness(damageKind, antithesisAmount);
                                        }
                                    }
                                    else if (owner.WeaknessAndResistance.Weaknesses.Any(weakness => weakness.DamageKind == damageKind))
                                    {
                                        owner.WeaknessAndResistance.Weaknesses.RemoveAll(weakness => weakness.DamageKind == damageKind);
                                    }
                                }
                            });

                        }

                        ExploitEffect exploitEffect = new ExploitEffect(attacker, defender, isGlimpse, antithesisAmount: antithesisAmount, applyToAll: applyToAll);
                        QEffect applyEffect = new QEffect()
                        {
                            Illustration = IllustrationName.GenericCombatManeuver,
                            Description = defender.Name,
                            StateCheck = async (QEffect stateCheck) =>
                            {
                                if (applyToAll)
                                {
                                    foreach (Creature applyWeakness in attacker.Battle.AllCreatures.Where(creature => creature != defender && !creature.FriendOf(attacker) && creature.BaseName == defender.BaseName))
                                    {
                                        if (!applyWeakness.HasEffect(ThaumaturgeQEIDs.ExploitVulnerabilityTarget) || (applyWeakness.FindQEffect(ThaumaturgeQEIDs.ExploitVulnerabilityTarget)?.Tag is Creature a && a == attacker))
                                        {
                                            applyWeakness.AddQEffect(new QEffect(ExpirationCondition.Never)
                                            {
                                                Id = ThaumaturgeQEIDs.ExploitVulnerabilityTarget,
                                                Tag = attacker,
                                                Illustration = ThaumaturgeModdedIllustrations.ExploitVulnerabilityBackground,
                                                Name = "Exploited Vulnerability",
                                                Description = "Exploited Weakness by " + attacker.Name + " - " + damageKind.HumanizeTitleCase2() + " " + antithesisAmount,
                                                StateCheck = (QEffect stateCheck) =>
                                                {
                                                    Creature owner = stateCheck.Owner;
                                                    if (owner.HasEffect(ThaumaturgeQEIDs.ExploitVulnerabilityTarget))
                                                    {
                                                        if (!owner.WeaknessAndResistance.Weaknesses.Any(weakness => weakness.DamageKind == damageKind))
                                                        {
                                                            owner.WeaknessAndResistance.AddWeakness(damageKind, antithesisAmount);
                                                        }
                                                    }
                                                    else if (owner.WeaknessAndResistance.Weaknesses.Any(weakness => weakness.DamageKind == damageKind))
                                                    {
                                                        owner.WeaknessAndResistance.Weaknesses.RemoveAll(weakness => weakness.DamageKind == damageKind);
                                                    }
                                                }
                                            });
                                        }
                                    }
                                }
                            }
                        };
                        exploitEffect.WhenExpires = (QEffect expired) =>
                        {
                            applyEffect.ExpiresAt = ExpirationCondition.Immediately;
                        };
                        attacker.AddQEffect(exploitEffect);
                        attacker.AddQEffect(applyEffect);

                    }
                    else if (!isGlimpse && result == CheckResult.CriticalFailure)
                    {
                        QEffect flatFooted = QEffect.FlatFooted("Exploit Vulnerability");
                        flatFooted.ExpiresAt = ExpirationCondition.ExpiresAtStartOfYourTurn;
                        attacker.AddQEffect(flatFooted);
                    }
                });

            if (!isGlimpse)
            {
                exploitVulnerabilityAction.WithActiveRollSpecification(new ActiveRollSpecification(ThaumaturgeUtilities.RollEsotericLore, ThaumaturgeUtilities.CalculateEsotericLoreDC));
            }

            return exploitVulnerabilityAction;
        }

        public static CalculatedNumber RollEsotericLore(CombatAction action, Creature roller, Creature? defender)
        {
            int level = roller.Level;
            int expertiseBonus = 2;
            if (roller.Level >= 7)
            {
                expertiseBonus = 6;
            }
            else if (roller.Level >= 3)
            {
                expertiseBonus = 4;
            }
            int proficiency = level + expertiseBonus;
            List<Bonus?> bonusesToRoll = new List<Bonus?>();
            if (roller.HasFeat(ThaumaturgeFeatNames.TomeImplement) && AnyHeldImplementsMatchID(ImplementIDs.Tome, roller))
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
            bool hasKnowItAll = roller.HasFeat(ThaumaturgeFeatNames.KnowitAll);
            int dcLevel = hasKnowItAll ? dcByLevelTarget.Level -1 : dcByLevelTarget.Level;
            string dcText = hasKnowItAll ? "DC by Level (Know it All)" : "DC by Level";
            return new CalculatedNumber(Checks.LevelBasedDC(dcLevel), dcText, new List<Bonus?>());
        }

        public static int CalculateClassDC(Creature creature, Trait classTrait)
        {
            return 10 + creature.Abilities.Get(creature.Abilities.KeyAbility) + creature.Proficiencies.Get(classTrait).ToNumber(creature.ProficiencyLevel);
        }

        public static FeatName LookupImplementFeatName(ImplementIDs implementID, bool isDedication)
        {
            switch(implementID)
            {
                case ImplementIDs.Amulet:
                    return !isDedication ? ThaumaturgeFeatNames.AmuletImplement : ThaumaturgeFeatNames.AmuletImplementDedication;
                case ImplementIDs.Bell:
                    return !isDedication ? ThaumaturgeFeatNames.BellImplement : ThaumaturgeFeatNames.BellImplementDedication;
                case ImplementIDs.Chalice:
                    return !isDedication ? ThaumaturgeFeatNames.ChaliceImplement : ThaumaturgeFeatNames.ChaliceImplementDedication;
                case ImplementIDs.Lantern:
                    return !isDedication ? ThaumaturgeFeatNames.LanternImplement : ThaumaturgeFeatNames.LanternImplementDedication;
                case ImplementIDs.Mirror:
                    return !isDedication ? ThaumaturgeFeatNames.MirrorImplement : ThaumaturgeFeatNames.MirrorImplementDedication;
                case ImplementIDs.Regalia:
                    return !isDedication ? ThaumaturgeFeatNames.RegaliaImplement : ThaumaturgeFeatNames.RegaliaImplementDedication;
                case ImplementIDs.Tome:
                    return !isDedication ? ThaumaturgeFeatNames.TomeImplement : ThaumaturgeFeatNames.TomeImplementDedication;
                case ImplementIDs.Wand:
                    return !isDedication ? ThaumaturgeFeatNames.WandImplement : ThaumaturgeFeatNames.WandImplementDedication;
                case ImplementIDs.Weapon:
                    return !isDedication ? ThaumaturgeFeatNames.WeaponImplement : ThaumaturgeFeatNames.WeaponImplementDedication;
                default:
                    throw new InvalidOperationException("Implement ID is not supported: " + implementID);
            }
        }

        public static Implement CreateImplement(ImplementIDs implementID)
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

        public static ItemName GetImplementBaseItemName(ImplementIDs implementID)
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

        public static void EnsureCorrectImplements(CalculatedCharacterSheetValues character, bool isDedication)//, bool delete = false)
        {
            ImplementIDs[] implementIDs = [ImplementIDs.Amulet, ImplementIDs.Bell, ImplementIDs.Chalice, ImplementIDs.Lantern, ImplementIDs.Mirror, ImplementIDs.Regalia, ImplementIDs.Tome, ImplementIDs.Wand, ImplementIDs.Weapon];
            List<ImplementIDs> implementsToAdd = new List<ImplementIDs>();
            List<ImplementIDs> implementsToRemove = new List<ImplementIDs>();

            foreach (ImplementIDs implementID in implementIDs)
            {
                FeatName featName = LookupImplementFeatName(implementID, isDedication);
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
                    RemoveImplement(character, GetImplementBaseItemName(implementID));
                }
            }

            foreach (ImplementIDs implementID in implementsToAdd)
            {
                if (implementID != ImplementIDs.Weapon)
                {
                    AddImplement(character, CreateImplement(implementID), LookupImplementFeatName(implementID, isDedication));
                }
                else
                {
                    AddImplement(character, Items.CreateNew(ThaumaturgeItemNames.WeaponImplementChoice), LookupImplementFeatName(implementID, isDedication));
                }
            }

            UpdateAllImplementsForPossibleScrolls(character);
        }

        private static void UpdateAllImplementsForPossibleScrolls(CalculatedCharacterSheetValues character)
        {
            bool shouldUpdate(Item item)
            {
                return item.HasTrait(ThaumaturgeTraits.Implement) && !item.HasTrait(ThaumaturgeTraits.WeaponImplement);
            }

            void UpdateImplement(Inventory inventory)
            {
                bool hasScrollThaumaturgy = character.HasFeat(ThaumaturgeFeatNames.ScrollThaumaturgy);

                if (inventory.RightHand != null && shouldUpdate(inventory.RightHand))
                {
                    Implement.SetStores(inventory.RightHand, hasScrollThaumaturgy);
                }
                if (inventory.LeftHand != null && shouldUpdate(inventory.LeftHand))
                {
                    Implement.SetStores(inventory.LeftHand, hasScrollThaumaturgy);
                }
                foreach (Item? item in inventory.Backpack)
                {
                    if (item != null && shouldUpdate(item))
                    {
                        Implement.SetStores(item, hasScrollThaumaturgy);
                    }
                }
            }

            // Add Implement to Sheet
            int[] levels = character.Sheet.InventoriesByLevel.Keys.ToArray();
            Inventory campaignInventory = character.Sheet.CampaignInventory;
            UpdateImplement(campaignInventory);

            foreach (int level in levels)
            {
                Inventory inventory = character.Sheet.InventoriesByLevel[level];
                UpdateImplement(inventory);
            }
        }

        private static void AddImplement(CalculatedCharacterSheetValues character, Item implement, FeatName implementFeatName)
        {
            // Add Implement to Sheet
            int[] levels = character.Sheet.InventoriesByLevel.Keys.ToArray();
            Inventory campaignInventory = character.Sheet.CampaignInventory;
            AddImplementIntoInventory(campaignInventory, implement);

            foreach (int level in levels)
            {
                Inventory inventory = character.Sheet.InventoriesByLevel[level];

                if (level == 1 || inventory.LeftHand != null || inventory.RightHand != null || inventory.Backpack.Count != 0)
                {
                    if (level >= 5 || (character.Tags.ContainsKey("First Implement") && character.Tags["First Implement"] is FeatName firstImplementFeatName && firstImplementFeatName == implementFeatName) || (character.Tags.ContainsKey("Dedication Implement") && character.Tags["Dedication Implement"] is FeatName dedicationImplement && dedicationImplement == implementFeatName))
                    {
                        AddImplementIntoInventory(inventory, implement);
                    }
                }
            }
        }

        private static void RemoveImplement(CalculatedCharacterSheetValues character, ItemName implementName)
        {
            // Remove Implement to Sheet
            int[] levels = character.Sheet.InventoriesByLevel.Keys.ToArray();
            Inventory campaignInventory = character.Sheet.CampaignInventory;
            RemoveImplementFromInventory(campaignInventory, implementName);

            foreach (int level in levels)
            {
                Inventory inventory = character.Sheet.InventoriesByLevel[level];
                RemoveImplementFromInventory(inventory, implementName);
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

        private static bool AddImplementIntoInventory(Inventory inventory, Item implement)
        {
            if (implement.BaseItemName == ThaumaturgeItemNames.WeaponImplementChoice && 
                ((inventory.LeftHand?.Runes.Any(rune => rune.BaseItemName == implement.BaseItemName) ?? false) || 
                (inventory.RightHand?.Runes.Any(rune => rune.BaseItemName == implement.BaseItemName) ?? false) || 
                inventory.Backpack.Any(item => item?.Runes.Any(rune => rune.BaseItemName == implement.BaseItemName) ?? false)))
            {
                return false;
            }
            else if (implement.BaseItemName == ThaumaturgeItemNames.WeaponImplementChoice && 
                (inventory.LeftHand?.StoredItems.Any(item => item.Runes.Any(rune => rune.BaseItemName == implement.BaseItemName)) ?? false) || 
                (inventory.RightHand?.StoredItems.Any(item => item.Runes.Any(rune => rune.BaseItemName == implement.BaseItemName)) ?? false) ||
                inventory.Backpack.Any(item => item?.StoredItems.Any(item => item.Runes.Any(rune => rune.BaseItemName == implement.BaseItemName)) ?? false))
            {
                return false;
            }
            else if ((inventory.LeftHand?.StoredItems.Any(item => item.BaseItemName == implement.BaseItemName) ?? false)||
                (inventory.RightHand?.StoredItems.Any(item => item.BaseItemName == implement.BaseItemName) ?? false) ||
                (inventory.Backpack.Any(item => item?.StoredItems.Any(item => item.BaseItemName == implement.BaseItemName) ?? false)))
            {
                return false;
            }
            else if ((inventory.LeftHand == null || inventory.LeftHand.BaseItemName != implement.BaseItemName) && (inventory.RightHand == null || inventory.RightHand.BaseItemName != implement.BaseItemName) && !inventory.Backpack.Any(item => item != null && item.BaseItemName == implement.BaseItemName))
            {
                if (inventory.RightHand == null && implement.ItemName != ThaumaturgeItemNames.WeaponImplementChoice && (inventory.LeftHand == null || !inventory.LeftHand.HasTrait(Trait.TwoHanded)))
                {
                    inventory.RightHand = implement;
                }
                else if (inventory.LeftHand == null && implement.ItemName != ThaumaturgeItemNames.WeaponImplementChoice)
                {
                    inventory.LeftHand = implement;
                }
                else
                {
                    if (inventory.CanBackpackFit(implement, 0))
                    {
                        inventory.AddAtEndOfBackpack(implement);
                    }
                }

                return true;
            }

            return false;
        }

        private static void RemoveImplementFromInventory(Inventory inventory, ItemName implementName, bool skipDeletingIfScroll = true)
        {
            Item? GetScroll(Item implement)
            {
                if (implement.StoredItems.Count == 1)
                {
                    return implement.StoredItems[0];
                }

                return null;
            }

            bool hasStoredItems(Item item)
            {
                return item.StoredItems.Count > 0;
            }

            Item? scrollToAdd = null;
            if (inventory.RightHand?.BaseItemName == implementName)
            {
                Item rightHand = inventory.RightHand;
                bool storedItems = hasStoredItems(rightHand);
                if (!storedItems || !skipDeletingIfScroll)
                {
                    scrollToAdd = GetScroll(rightHand);
                    inventory.RightHand = null;
                }
            }
            else if (inventory.LeftHand?.BaseItemName == implementName)
            {
                Item leftHand = inventory.LeftHand;
                bool storedItems = hasStoredItems(leftHand);
                if (!storedItems || !skipDeletingIfScroll)
                {
                    scrollToAdd = GetScroll(leftHand);
                    inventory.LeftHand = null;
                }
            }
            else
            {
                Item? possibleImplement = inventory.Backpack.FirstOrDefault(item => item != null && item.BaseItemName == implementName);
                if (possibleImplement != null && (!hasStoredItems(possibleImplement) || !skipDeletingIfScroll))
                {
                    scrollToAdd = GetScroll(possibleImplement);
                }
                inventory.Backpack.RemoveAll(item => item != null && item.BaseItemName == implementName && (!hasStoredItems(item) || !skipDeletingIfScroll));
            }

            if (scrollToAdd != null)
            {
                inventory.AddAtEndOfBackpack(scrollToAdd);
            }
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
