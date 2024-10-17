using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.Constants;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.Utilities;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics.Targeting;
using static Dawnsbury.Core.Mechanics.Core.CalculatedNumber;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.Enums;
using Dawnsbury.Core.Mechanics.Damage;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Display;
using Dawnsbury.Core.Coroutines.Options;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using static System.Collections.Specialized.BitVector32;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb;
using System.Reflection;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Microsoft.Xna.Framework;
using Dawnsbury.Auxiliary;
using Dawnsbury.Audio;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Core.Animations;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.Extensions;
using Dawnsbury.Core.Mechanics.Targeting.TargetingRequirements;
using Dawnsbury.Campaign.Encounters;
using static Dawnsbury.Core.Possibilities.Usability;
using System.Diagnostics;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge
{
    /// <summary>
    /// The Thaumaturge class
    /// </summary>
    public static class Thaumaturge
    {
        /// <summary>
        /// Creates the Thaumaturge Feats
        /// </summary>
        /// <returns>The Enumerable of Thaumaturge Feats</returns>
        public static IEnumerable<Feat> CreateThaumaturgeFeats()
        {
            // Creates and adds the logic for the Exploit Vulnerability class feature
            Feat exploitVulnerabilityFeat = new Feat(ThaumaturgeFeatNames.ExploitVulnerability, "You know that every creature, no matter how obscure, has a weakness. By identifying and empowering the right object, you can strike down even the most resilient of monsters.", "You gain the Exploit Vulnerability action.", [], null);
            AddExploitVulnerabilityLogic(exploitVulnerabilityFeat);
            yield return exploitVulnerabilityFeat;

            // Creates and adds the logic for the Implement's Empowerment class feature
            Feat implementsEmpowermentFeat = new Feat(ThaumaturgeFeatNames.ImplementsEmpowerment, "The power of your implement can also be turned to the more common task of combat, its power adding to and amplifying the effects of runes and other magical empowerments.", "When you Strike, you can trace mystic patterns with an implement you're holding to empower the Strike, causing it to deal 2 additional damage per weapon damage die. Channeling the power requires full use of your hands. You don't gain the benefit of implement's empowerment if you are holding anything in either hand other than a single one-handed weapon, other implements, or esoterica, and you must be holding at least one implement to gain the benefit.", [], null);
            AddImplementsEmpowermentLogic(implementsEmpowermentFeat);
            yield return implementsEmpowermentFeat;

            yield return new Feat(ThaumaturgeFeatNames.ColdWand, "Cold Wand", "Your wand is attuned to Cold.", [], null);

            yield return new Feat(ThaumaturgeFeatNames.ElectricityWand, "Electricity Wand", "Your wand is attuned to Electricity.", [], null);

            yield return new Feat(ThaumaturgeFeatNames.FireWand, "Fire Wand", "Your wand is attuned to Fire.", [], null);

            Feat amuletImplementFeat = new Feat(ThaumaturgeFeatNames.AmuletImplement, ImplementDetails.AmuletInitiateBenefitFlavorText, "You gain the " + ImplementDetails.AmuletInitiateBenefitName + " reaction.\n\n{b}" + ImplementDetails.AmuletInitiateBenefitName + "{/b} {icon:Reaction}\n" + ImplementDetails.AmuletInitiateBenefitRulesText, [ThaumaturgeTraits.Implement], null);
            AddAmuletImplementLogic(amuletImplementFeat);
            yield return amuletImplementFeat;

            Feat bellImplementFeat = new Feat(ThaumaturgeFeatNames.BellImplement, ImplementDetails.BellInitiateBenefitFlavorText, "You gain the " + ImplementDetails.BellInitiateBenefitName + " reaction.\n\n{b}" + ImplementDetails.BellInitiateBenefitName + "{/b} {icon:Reaction}\n" + ImplementDetails.BellInitiateBenefitRulesText, [ThaumaturgeTraits.Implement], null);
            AddBellImplementLogic(bellImplementFeat);
            yield return bellImplementFeat;

            Feat chaliceImplementFeat = new Feat(ThaumaturgeFeatNames.ChaliceImplement, ImplementDetails.ChaliceInitiateBenefitFlavorText, "You gain the " + ImplementDetails.ChaliceInitiateBenefitName + " action.\n\n{b}" + ImplementDetails.ChaliceInitiateBenefitName + "{/b} {icon:Action}\n" + ImplementDetails.ChaliceInitiateBenefitRulesText, [ThaumaturgeTraits.Implement], null);
            AddChaliceImplementLogic(chaliceImplementFeat);
            yield return chaliceImplementFeat;

            Feat lanternImplementFeat = new Feat(ThaumaturgeFeatNames.LanternImplement, ImplementDetails.LanternInitiateBenefitFlavorText, "You gain the following benefit.\n\n" + ImplementDetails.LanternInitiateBenefitRulesText, [ThaumaturgeTraits.Implement], null);
            AddLanternImplementLogic(lanternImplementFeat);
            yield return lanternImplementFeat;

            Feat mirrorImplementFeat = new Feat(ThaumaturgeFeatNames.MirrorImplement, ImplementDetails.MirrorInitiateBenefitFlavorText, "You gain the " + ImplementDetails.MirrorInitiateBenefitName + " action.\n\n{b}" + ImplementDetails.MirrorInitiateBenefitName + "{/b} {icon:Action}\n" + ImplementDetails.MirrorInitiateBenefitRulesText, [ThaumaturgeTraits.Implement], null);
            AddMirrorImplementLogic(mirrorImplementFeat);
            yield return mirrorImplementFeat;

            Feat regaliaImplementFeat = new Feat(ThaumaturgeFeatNames.RegaliaImplement, ImplementDetails.RegaliaInitiateBenefitFlavorText, "You gain the following benefit.\n\n" + ImplementDetails.RegaliaInitiateBenefitRulesText, [ThaumaturgeTraits.Implement], null);
            AddRegaliaImplementLogic(regaliaImplementFeat);
            yield return regaliaImplementFeat;

            Feat tomeImplementFeat = new Feat(ThaumaturgeFeatNames.TomeImplement, ImplementDetails.TomeInitiateBenefitFlavorText, "You gain the following benefit.\n\n" + ImplementDetails.TomeInitiateBenefitRulesText, [ThaumaturgeTraits.Implement], null);
            AddTomeImplementLogic(tomeImplementFeat);
            yield return tomeImplementFeat;

            Feat wandImplementFeat = new Feat(ThaumaturgeFeatNames.WandImplement, ImplementDetails.WandInitiateBenefitFlavorText, "You gain the " + ImplementDetails.WandInitiateBenefitName + " activity.\n\n{b}" + ImplementDetails.WandInitiateBenefitName + "{/b} {icon:TwoActions}\n" + ImplementDetails.WandInitiateBenefitRulesText, [ThaumaturgeTraits.Implement], null);
            AddWandImplementLogic(wandImplementFeat);
            yield return wandImplementFeat;

            Feat weaponImplementFeat = new Feat(ThaumaturgeFeatNames.WeaponImplement, ImplementDetails.WeaponInitiateBenefitFlavorText, "You gain the " + ImplementDetails.WeaponInitiateBenefitName + " reaction.\n\n{b}NOTE: The Weapon Implement will be applied to your first One-Handed weapon at the start of each encounter.{/b}\n\n{b}" + ImplementDetails.WeaponInitiateBenefitName + "{/b} {icon:Reaction}\n" + ImplementDetails.WeaponInitiateBenefitRulesText, [ThaumaturgeTraits.Implement], null);
            AddWeaponImplementLogic(weaponImplementFeat);
            yield return weaponImplementFeat;

            //// Creates the class selection feat for the Thaumaturge
            yield return new ClassSelectionFeat(ThaumaturgeFeatNames.ThaumaturgeClass, "The world is full of the unexplainable: ancient magic, dead gods, and even stranger things. In response, you've scavenged the best parts of every magical tradition and built up a collection of esoterica—a broken holy relic here, a sprig of mistletoe there—that you can use to best any creature by exploiting their weaknesses and vulnerabilities. The mystic implement you carry is both badge and weapon, its symbolic weight helping you bargain with and subdue the supernatural. Every path to power has its restrictions and costs, but you turn them all to your advantage. You're a thaumaturge, and you work wonders.",
                ThaumaturgeTraits.Thaumaturge, new EnforcedAbilityBoost(Ability.Charisma), 8,
                [Trait.Reflex, Trait.Simple, Trait.Martial, Trait.LightArmor, Trait.MediumArmor],
                [Trait.Perception, Trait.Fortitude, Trait.Will],
                3,
                "{b}1. Esoteric Lore{/b} You become trained in a special lore skill that can used to Exploit Vulnerability. This is a charisma-based skill. {i}(You add your Charisma modifier to checks using this skill.){/i}\n\n" +
                "{b}2. Exploit Vulnerability {icon:Action}{/b}\n{b}Frequency{/b} once per round; {b}Requirements{/b} You are holding your implement\n\nSelect a creature you can see and attempt an Esoteric Lore check against a standard DC for its level. You gain the following effects until you Exploit Vulnerabilities again.\n\n{b}Success{/b} Your unarmed and weapon Strikes activate the highest weakness againt the target, even though the damage type your weapon deals doesn't change. This damage affects the target of your Exploit Vulnerability, as well as any other creatures of the exact same type, but not other creatures with the same weakness. The {b}Failure{/b} result is used if the target has no weakness or if it is better.\n{b}Failure{/b} This causes the target creature, and only the target creature, to gain a weakness against your unarmed and weapon Strikes equal to 2 + half your level.\n{b}Critical Failure{/b} You become flat-footed until the beginning of your next turn.\n\n\n\n" +
                "{b}3. First Implement{/b} Choose an implement. {i}{Will appear in an open hand at the start of combat, if no open hands it will be in your bag or on the ground. The weapon implement is the only exception.){/i}\n\n" +
                "{b}4. Implement's Empowerment{/b} When you Strike, you can trace mystic patterns with an implement you're holding to empower the Strike, causing it to deal 2 additional damage per weapon damage die. Channeling the power requires full use of your hands. You don't gain the benefit of implement's empowerment if you are holding anything in either hand other than a single one-handed weapon or other implements and you must be holding at least one implement to gain the benefit.\n\n" +
                "{b}5. Thaumaturge Feat{/b}\n\n" +
                "{b}At Higher Levels:{/b}\n" +
                "{b}Level 2{/b} Thaumaturge Feat\n" +
                "{b}Level 3{/b} General feat, skill increase, Expert in Esoteric Lore\n" +
                "{b}Level 4{/b} Thaumaturge Feat", 
                null)
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFeat(exploitVulnerabilityFeat, null);
                    sheet.AddFeat(implementsEmpowermentFeat, null);
                    sheet.AddSelectionOption(new SingleFeatSelectionOption("FirstImplement", "First Implement", 1, (Feat ft) => ft.HasTrait(ThaumaturgeTraits.Implement)));
                    sheet.AddSelectionOption(new SingleFeatSelectionOption("ThaumaturgeFeat1", "Thaumaturge feat", 1, (Feat ft) => ft.HasTrait(ThaumaturgeTraits.Thaumaturge)));
                    sheet.SetProficiency(ThaumaturgeTraits.Thaumaturge, Proficiency.Trained);
                    sheet.AddAtLevel(3, delegate (CalculatedCharacterSheetValues values)
                    {
                        values.SetProficiency(Trait.Reflex, Proficiency.Expert);
                    });
                });

            TrueFeat rootToLifeFeat = new TrueFeat(ThaumaturgeFeatNames.RootToLife, 1, "Marigold, spider lily, pennyroyal—many primal traditions connect flowers and plants with the boundary between life and death, and you can leverage this association to keep an ally on this side of the line.", "You place a small plant or similar symbol on an adjacent dying creature, immediately stabilizing them; the creature is no longer dying and is instead unconscious at 0 Hit Points.\n\nIf you spend 2 actions instead of 1, you empower the act further by uttering a quick folk blessing to chase away ongoing pain, adding the auditory trait to the action. When you do so, attempt flat checks to remove each source of persistent damage affecting the target; due to the particularly effective assistance, the DC is 10 instead of the usual 15.", [Trait.Manipulate, Trait.Necromancy, Trait.Primal, ThaumaturgeTraits.Thaumaturge]);
            AddRootToLifeLogic(rootToLifeFeat);
            yield return rootToLifeFeat;

            TrueFeat scrollThaumaturgyFeat = new TrueFeat(ThaumaturgeFeatNames.ScrollThaumaturgy, 1, "Your multidisciplinary study of magic means you know how to activate the magic in scrolls with ease.", "You can activate scrolls of any magical tradition, using your thaumaturge class DC for the scroll's DC, rather than a particular spell DC. If a spell is on the spell list for multiple traditions, you choose which tradition to use at the time you activate the scroll. You can draw and activate scrolls with the same hand holding an implement.", [ThaumaturgeTraits.Thaumaturge]);
            AddScrollThaumaturgyLogic(scrollThaumaturgyFeat);
            yield return scrollThaumaturgyFeat;

            TrueFeat esotericWardenFeat = new TrueFeat(ThaumaturgeFeatNames.EsotericWarden, 1, "When you apply antithetical material against a creature successfully, you also ward yourself against its next attacks.", "When you succeed at your check to Exploit a Vulnerability, you gain a +1 status bonus to your AC against the creature's next attack and a +1 status bonus to your next saving throw against the creature; if you critically succeed, these bonuses are +2 instead. You can gain these bonuses only once per day against a particular creature, and the benefit ends if you Exploit Vulnerability again.", [ThaumaturgeTraits.Thaumaturge]);
            AddEsotericWardenLogic(esotericWardenFeat);
            yield return esotericWardenFeat;


        }

        /// <summary>
        /// Adds the logic for the Exploit Vulnerability base class feature
        /// </summary>
        /// <param name="exploitVulnerabilityFeat">The Exploit Vulnerability feat object</param>
        public static void AddExploitVulnerabilityLogic(Feat exploitVulnerabilityFeat)
        {
            exploitVulnerabilityFeat.WithPermanentQEffect("Esoteric Lore check to focus weakness", delegate (QEffect self)
            {
                self.ProvideMainAction = (QEffect exploitVulnerabilityEffect) =>
                {
                    if (ThaumaturgeUtilities.IsCreatureWeildingImplement(self.Owner))
                    {
                        CombatAction exploitVulnerabilityAction = new CombatAction(
                            self.Owner,
                            IllustrationName.GenericCombatManeuver,
                            "Exploit Vulnerability",
                            [Trait.Manipulate, ThaumaturgeTraits.Thaumaturge],
                            "{b}Frequency{/b} once per round; {b}Requirements{/b} You are holding your implement.\nYou scour your experiences and learning to identify something that might repel your foe. You retrieve an object from your esoterica with the appropriate supernatural qualities, then use your implement to stoke the remnants of its power into a blaze. Select a creature you can see and attempt an Esoteric Lore check against a standard DC for its level, as you retrieve the right object from your esoterica and use your implement to empower it. You gain the following effects until you Exploit Vulnerabilities again.\n{b}Success{/b} Your unarmed and weapon Strikes activate the highest weakness againt the target, even though the damage type your weapon deals doesn't change. This damage affects the target of your Exploit Vulnerability, as well as any other creatures of the exact same type, but not other creatures with the same weakness. The {b}Failure{/b} result is used if the target has no weakness or if it is better.\n{b}Failure{/b} This causes the target creature, and only the target creature, to gain a weakness against your unarmed and weapon Strikes equal to 2 + half your level.\n{b}Critical Failure{/b} You become flat-footed until the beginning of your next turn.",
                            Target.Ranged(100)
                            .WithAdditionalConditionOnTargetCreature((attacker, defender) => attacker.HasEffect(ThaumaturgeQEIDs.UsedExploitVulnerability) ? Usability.NotUsable("Already Exploited Vulnerability this turn") : Usability.Usable))
                        .WithActionId(ThaumaturgeActionIDs.ExploitVulnerability)
                        .WithActiveRollSpecification(new ActiveRollSpecification(ThaumaturgeUtilities.RollEsotericLore, ThaumaturgeUtilities.CalculateEsotericLoreDC))
                        .WithEffectOnEachTarget(async delegate (CombatAction action, Creature attacker, Creature defender, CheckResult result)
                        {         
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
                                        ChoiceButtonOption selectedWeakness = await attacker.AskForChoiceAmongButtons(IllustrationName.GenericCombatManeuver, "Which weakness would you like to exploit against all " + defender.BaseName + "?", weaknesses.Select(weakness => weakness.DamageKind.HumanizeTitleCase2()).ToArray());
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
                                                Illustration = IllustrationName.GenericCombatManeuver,
                                                Name = "Exploited Vulnerability",
                                                Description = "Exploited Weakness by " + attacker.Name + " - " + weakness.DamageKind.HumanizeTitleCase2() + " " + weakness.Value,
                                                Tag = attacker
                                            });
                                        }
                                        attacker.AddQEffect(new QEffect(ExpirationCondition.Never)
                                        {
                                            Id = ThaumaturgeQEIDs.ExploitVulnerabilityWeakness,
                                            Tag = defender,
                                            Illustration = IllustrationName.GenericCombatManeuver,
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
                                    Illustration = IllustrationName.GenericCombatManeuver,
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
                                        else if(owner.WeaknessAndResistance.Weaknesses.Any(weakness => weakness.DamageKind == ThaumaturgeDamageKinds.PersonalAntithesis))
                                        {
                                            owner.WeaknessAndResistance.Weaknesses.RemoveAll(weakness => weakness.DamageKind == ThaumaturgeDamageKinds.PersonalAntithesis);
                                        }
                                    }
                                });
                                attacker.AddQEffect(new QEffect(ExpirationCondition.Never)
                                {
                                    Id = ThaumaturgeQEIDs.ExploitVulnerabilityWeakness,
                                    Tag = defender,
                                    Illustration = IllustrationName.GenericCombatManeuver,
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

                        return new ActionPossibility(exploitVulnerabilityAction);
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Amulet Implement feature
        /// </summary>
        /// <param name="amuletImplementFeat">The Amulet Implement feat object</param>
        public static void AddAmuletImplementLogic(Feat amuletImplementFeat)
        {
            AddImplementEnsureLogic(amuletImplementFeat);
            amuletImplementFeat.WithPermanentQEffect(ImplementDetails.AmuletInitiateBenefitName, delegate (QEffect self)
            {
                self.StartOfCombat = async (QEffect startOfCombat) =>
                {
                    Creature owner = startOfCombat.Owner;
                    Creature[] allies = owner.Battle.AllCreatures.Where(creature => creature.FriendOf(owner)).ToArray();
                    foreach (Creature ally in allies)
                    {
                        ally.AddQEffect(new QEffect(ExpirationCondition.Never)
                        {
                            YouAreDealtDamage = async (QEffect effect, Creature attacker, DamageStuff damage, Creature defender) =>
                            {
                                if (attacker.HasEffect(ThaumaturgeQEIDs.ExploitVulnerabilityTarget))
                                {
                                    QEffect exploitEffect = attacker.QEffects.First(qe => qe.Id == ThaumaturgeQEIDs.ExploitVulnerabilityTarget);
                                    if (exploitEffect.Tag != null && exploitEffect.Tag is Creature thaumaturge && thaumaturge == owner && owner.Actions.CanTakeReaction() && ThaumaturgeUtilities.IsCreatureWeildingImplement(owner) && ally.DistanceTo(owner) <= 3 && await owner.AskToUseReaction("Use " + ImplementDetails.AmuletInitiateBenefitName + " to give resistance equal to 2 + your level?"))
                                    {
                                        return new ReduceDamageModification(2 + owner.Level, "Amulet's Abeyance");
                                    }
                                }

                                return null;
                            }
                        });
                    }
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Bell Implement feature
        /// </summary>
        /// <param name="bellImplementFeat">The Bell Implement feat object</param>
        public static void AddBellImplementLogic(Feat bellImplementFeat)
        {
            AddImplementEnsureLogic(bellImplementFeat);
            bellImplementFeat.WithPermanentQEffect(ImplementDetails.BellInitiateBenefitName, delegate (QEffect self)
            {
                self.StartOfCombat = async (QEffect startOfCombat) =>
                {
                    Creature owner = startOfCombat.Owner;
                    Creature[] allies = owner.Battle.AllCreatures.Where(creature => creature.FriendOf(owner)).ToArray();
                    foreach (Creature ally in allies)
                    {
                        ally.AddQEffect(new QEffect(ExpirationCondition.Never)
                        {
                            YouAreTargeted = async (QEffect targeted, CombatAction action) =>
                            {
                                bool actionIsSpell = action.SpellInformation != null;
                                if (action.Owner.HasEffect(ThaumaturgeQEIDs.ExploitVulnerabilityTarget))
                                {
                                    QEffect exploitEffect = action.Owner.QEffects.First(qe => qe.Id == ThaumaturgeQEIDs.ExploitVulnerabilityTarget);
                                    if (exploitEffect.Tag != null && exploitEffect.Tag is Creature thaumaturge && thaumaturge == owner && owner.Actions.CanTakeReaction() && ThaumaturgeUtilities.IsCreatureWeildingImplement(owner) && owner.DistanceTo(action.Owner) <= 6 && await owner.AskToUseReaction("Use " + ImplementDetails.BellInitiateBenefitName + ": " + (actionIsSpell ? " Target makes Fortitude save or becomes stupefied?" : " Target makes Will save or becomes your choice of enfeebled or clumsy?")))
                                    {
                                        CheckResult savingThrowResult = CommonSpellEffects.RollSavingThrow(action.Owner, new CombatAction(owner, IllustrationName.GenericCombatManeuver, ImplementDetails.BellInitiateBenefitName, [Trait.Auditory, Trait.Emotion, Trait.Enchantment, Trait.Magical, Trait.Manipulate, Trait.Mental, ThaumaturgeTraits.Thaumaturge], ImplementDetails.BellInitiateBenefitRulesText, Target.Touch()), actionIsSpell ? Defense.Fortitude : Defense.Will, creature => ThaumaturgeUtilities.CalculateClassDC(owner, ThaumaturgeTraits.Thaumaturge));
                                        if (savingThrowResult <= CheckResult.Failure)
                                        {
                                            if (actionIsSpell)
                                            {
                                                QEffect debuff = QEffect.Stupefied(savingThrowResult == CheckResult.CriticalFailure ? 2 : 1);
                                                debuff.Source = owner;
                                                debuff.ExpiresAt = ExpirationCondition.ExpiresAtStartOfSourcesTurn;
                                                action.Owner.AddQEffect(debuff);
                                            }
                                            else
                                            {
                                                int debuffLevel = savingThrowResult == CheckResult.CriticalFailure ? 2 : 1;
                                                ChoiceButtonOption userResponse = await owner.AskForChoiceAmongButtons(IllustrationName.GenericCombatManeuver, "Add Enfeebled " + debuffLevel + " or Clumsy " + debuffLevel + " to " + action.Owner.Name, ["Enfeebled " + debuffLevel, "Clumsy " + debuffLevel]);
                                                QEffect debuff = (userResponse.Index == 0) ? QEffect.Enfeebled(debuffLevel) : QEffect.Clumsy(debuffLevel);
                                                debuff.Source = owner;
                                                debuff.ExpiresAt = ExpirationCondition.ExpiresAtStartOfSourcesTurn;
                                                action.Owner.AddQEffect(debuff);

                                            }
                                        }
                                    }
                                }
                            }
                        });
                    }
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Chalice Implement feature
        /// </summary>
        /// <param name="chaliceImplementFeat">The Chalice Implement feat object</param>
        public static void AddChaliceImplementLogic(Feat chaliceImplementFeat)
        {
            AddImplementEnsureLogic(chaliceImplementFeat);
            chaliceImplementFeat.WithPermanentQEffect(ImplementDetails.ChaliceInitiateBenefitName, delegate (QEffect self)
            {
                self.ProvideActionIntoPossibilitySection = (QEffect chaliceImplementEffect, PossibilitySection possibilitySection) =>
                {
                    if (possibilitySection.PossibilitySectionId == PossibilitySectionId.MainActions)
                    {
                        PossibilitySection chaliceSection = new PossibilitySection("Calice Possibilities");

                        IllustrationName chaliceIllustrationName = IllustrationName.GenericCombatManeuver;
                        List<Trait> chaliceTraits = [Trait.Magical, Trait.Manipulate, Trait.Necromancy, ThaumaturgeTraits.Thaumaturge];

                        CombatAction sipAction = new CombatAction(self.Owner, chaliceIllustrationName, "Sip", chaliceTraits.ToArray(), ImplementDetails.ChaliceInitiateBenefitSipText, Target.AdjacentCreatureOrSelf()
                            .WithAdditionalConditionOnTargetCreature((Creature user, Creature target) =>
                            {
                                if (user.QEffects.Any(qe => qe.Name == "Chalice Used this Round"))
                                {
                                    return Usability.NotUsable("Already used this round.");
                                }
                                else if (!ThaumaturgeUtilities.IsCreatureWeildingImplement(self.Owner))
                                {
                                    return Usability.NotUsable("Not weilding Implement.");
                                }
                                return Usability.Usable;
                            }));
                        sipAction.WithActionCost(1);
                        sipAction.WithEffectOnChosenTargets(async delegate (Creature user, ChosenTargets targets)
                        {
                            if (targets.ChosenCreature != null)
                            {
                                user.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtStartOfYourTurn)
                                {
                                    Name = "Chalice Used this Round"
                                });
                                targets.ChosenCreature.GainTemporaryHP(2 + (int)(Math.Floor(user.Level / 2.0)));
                            }
                        });

                        CombatAction drainAction = new CombatAction(self.Owner, chaliceIllustrationName, "Drain", (chaliceTraits.Concat([Trait.Healing, Trait.Positive])).ToArray(), ImplementDetails.ChaliceInitiateBenefitDrainText, Target.AdjacentCreatureOrSelf()
                            .WithAdditionalConditionOnTargetCreature((Creature user, Creature target) =>
                            {
                                if (user.QEffects.Any(qe => qe.Name == "Chalice Used this Round"))
                                {
                                    return Usability.NotUsable("Already used this round.");
                                }
                                else if (user.QEffects.Any(qe => qe.Name == "Chalice is Drained"))
                                {
                                    return Usability.NotUsable("Already drained this encounter.");
                                }
                                else if (!ThaumaturgeUtilities.IsCreatureWeildingImplement(self.Owner))
                                {
                                    return Usability.NotUsable("Not weilding Implement.");
                                }
                                return Usability.Usable;
                            }));
                        drainAction.WithActionCost(1);
                        drainAction.WithEffectOnChosenTargets(async delegate (Creature user, ChosenTargets targets)
                        {
                            if (targets.ChosenCreature != null)
                            {
                                user.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtStartOfYourTurn)
                                {
                                    Name = "Chalice Used this Round"
                                });
                                user.AddQEffect(new QEffect(ExpirationCondition.Never)
                                {
                                    Name = "Chalice is Drained"
                                });
                                targets.ChosenCreature.Heal("" + (3 * user.Level), drainAction);
                            }
                        });

                        ActionPossibility sipActionPossibility = new ActionPossibility(sipAction);
                        chaliceSection.AddPossibility(sipActionPossibility);
                        ActionPossibility drainActionPossibility = new ActionPossibility(drainAction);
                        chaliceSection.AddPossibility(drainActionPossibility);

                        SubmenuPossibility chaliceMenu = new SubmenuPossibility(IllustrationName.GenericCombatManeuver, ImplementDetails.ChaliceInitiateBenefitName);
                        chaliceMenu.Subsections.Add(chaliceSection);
                        return chaliceMenu;
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Lantern Implement feature
        /// </summary>
        /// <param name="lanternImplementFeat">The Lantern Implement feat object</param>
        public static void AddLanternImplementLogic(Feat lanternImplementFeat)
        {
            AddImplementEnsureLogic(lanternImplementFeat);
            lanternImplementFeat.WithPermanentQEffect("Lantern Initiate Benefit", delegate (QEffect self)
            {
                self.BonusToAttackRolls = (QEffect bonusToSeek, CombatAction action, Creature? creature) =>
                {
                    if (action.ActionId == ActionId.Seek)
                    {
                        return new Bonus(1, BonusType.Status, "Lantern Initiate Benefit", true);
                    }

                    return null;
                };
                self.StartOfCombat = async (QEffect startOfCombat) =>
                {
                    startOfCombat.Owner.AddQEffect(new QEffect(ExpirationCondition.Never)
                    {
                        Id = ThaumaturgeQEIDs.LanternSearching,
                        Tag = new List<Tile>()
                    });
                    startOfCombat.Owner.AddQEffect(new QEffect(ExpirationCondition.Never)
                    {
                        Id = ThaumaturgeQEIDs.LocationTracking,
                        Tag = null
                    });
                };
                self.StateCheck = async (QEffect stateCheck) =>
                {
                    Creature owner = stateCheck.Owner;
                    QEffect? lanternSearchingEffect = owner.FindQEffect(ThaumaturgeQEIDs.LanternSearching);
                    QEffect? locationTrackingEffect = owner.FindQEffect(ThaumaturgeQEIDs.LocationTracking);
                    if (locationTrackingEffect != null && (locationTrackingEffect.Tag == null || (locationTrackingEffect.Tag is Tile lastTile && lastTile != owner.Occupies)) && lanternSearchingEffect != null && lanternSearchingEffect.Tag != null && lanternSearchingEffect.Tag is List<Tile> searchedTiles && ThaumaturgeUtilities.IsCreatureWeildingImplement(owner))
                    {
                        locationTrackingEffect.Tag = owner.Occupies;
                        Tile[] tilesToSearch = owner.Battle.Map.AllTiles.Where(tile => tile.DistanceTo(owner.Occupies) <= 4 && !searchedTiles.Contains(tile)).ToArray();
                        foreach (Tile tile in tilesToSearch)
                        {
                            searchedTiles.Add(tile);
                            foreach (TileQEffect tileQEffect in tile.QEffects)
                            {
                                if (tileQEffect.SeekDC != 0)
                                {
                                    CombatAction seekAction = new CombatAction(owner, IllustrationName.Seek, "Lantern Seek", [Trait.Concentrate, Trait.Secret, Trait.Basic, Trait.IsNotHostile, Trait.DoesNotBreakStealth, Trait.AttackDoesNotTargetAC], ImplementDetails.LanternInitiateBenefitRulesText, Target.Self())
                                        .WithActionId(ActionId.Seek)
                                        .WithActionCost(0)
                                        .WithActiveRollSpecification(new ActiveRollSpecification(Checks.Perception(), Checks.FlatDC(tileQEffect.SeekDC)));
                                    CheckBreakdown seekCheckBreakdown = CombatActionExecution.BreakdownAttack(seekAction, Creature.DefaultCreature);
                                    CheckBreakdownResult seekResult = new CheckBreakdownResult(seekCheckBreakdown);
                                    if (seekResult.CheckResult >= CheckResult.Success)
                                    {
                                        tile.Overhead(seekResult.CheckResult.HumanizeTitleCase2(), Color.LightBlue, owner + " rolls " + seekResult.CheckResult.HumanizeTitleCase2() + " on Lantern Seek.", "Lantern Seek", seekCheckBreakdown.DescribeWithFinalRollTotal(seekResult));
                                        await tileQEffect.WhenSeeked.InvokeIfNotNull();
                                    }
                                }
                            }
                        }
                    }
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Mirror Implement feature
        /// </summary>
        /// <param name="mirrorImplementFeat">The Mirror Implement feat object</param>
        public static void AddMirrorImplementLogic(Feat mirrorImplementFeat)
        {
            AddImplementEnsureLogic(mirrorImplementFeat);
            mirrorImplementFeat.WithPermanentQEffect(ImplementDetails.MirrorInitiateBenefitName, delegate (QEffect self)
            {
                self.ProvideMainAction = (QEffect mirrorsReflectionEffect) =>
                {
                    Creature owner = mirrorsReflectionEffect.Owner;
                    if (!ThaumaturgeUtilities.IsCreatureWeildingImplement(owner))
                    {
                        return null;
                    }

                    return new ActionPossibility(new CombatAction(owner, IllustrationName.GenericCombatManeuver, ImplementDetails.MirrorInitiateBenefitName, [Trait.Illusion, Trait.Magical, Trait.Manipulate, ThaumaturgeTraits.Thaumaturge], ImplementDetails.MirrorInitiateBenefitRulesText, Target.Tile((creature, tile) => tile.LooksFreeTo(creature) && creature.Occupies != null && creature.DistanceTo(tile) <= 3, (creature, tile) => (float)int.MinValue))
                        .WithActionCost(1)
                        .WithEffectOnChosenTargets(async delegate (Creature attacker, ChosenTargets targets)
                        {
                            QEffect? mirrorTracking = owner.FindQEffect(ThaumaturgeQEIDs.MirrorTracking);
                            if (mirrorTracking != null)
                            {
                                Creature pairedCreature = ((MirrorTrackingEffect)mirrorTracking).PairedCreature;
                                pairedCreature.RemoveAllQEffects(qe => qe.Id == ThaumaturgeQEIDs.MirrorTracking);
                                owner.Battle.RemoveCreatureFromGame(pairedCreature);
                                owner.RemoveAllQEffects(qe => qe.Id == ThaumaturgeQEIDs.MirrorTracking);
                            }
                            if (targets.ChosenTile != null)
                            {
                                Tile chosenTile = targets.ChosenTile;
                                Defenses ownerDefenses = owner.Defenses;
                                Defenses cloneDefenses = new Defenses(
                                    ownerDefenses.GetBaseValue(Defense.AC) + ThaumaturgeUtilities.DetermineBonusIncreaseForDefense(owner, Defense.AC),
                                    ownerDefenses.GetBaseValue(Defense.Fortitude) + ThaumaturgeUtilities.DetermineBonusIncreaseForDefense(owner, Defense.Fortitude),
                                    ownerDefenses.GetBaseValue(Defense.Reflex) + ThaumaturgeUtilities.DetermineBonusIncreaseForDefense(owner, Defense.Reflex),
                                    ownerDefenses.GetBaseValue(Defense.Will) + ThaumaturgeUtilities.DetermineBonusIncreaseForDefense(owner, Defense.AC));
                                Skills cloneSkills = new Skills();
                                MirrorClone mirrorClone = new MirrorClone(owner.Illustration, owner.Name, owner.Traits, owner.Level, owner.Perception, owner.Speed, cloneDefenses, owner.MaxHP, owner.Abilities, cloneSkills);
                                mirrorClone.SetDamageImmediately(owner.Damage);
                                mirrorClone.PersistentCharacterSheet = owner.PersistentCharacterSheet;
                                mirrorClone.BaseArmor = owner.BaseArmor;
                                mirrorClone.RecalculateArmor();
                                mirrorClone.EntersInitiativeOrder = false;
                                foreach (QEffect effect in owner.QEffects)
                                {
                                    if (effect.ProvideMainAction == null && effect.ProvideActionIntoPossibilitySection == null)
                                    {
                                        mirrorClone.AddQEffect(effect);
                                    }
                                }

                                owner.Battle.SpawnCreature(mirrorClone, owner.OwningFaction, chosenTile);

                                MirrorTrackingEffect ownersTrackingEffect = new MirrorTrackingEffect(owner, mirrorClone);
                                MirrorTrackingEffect mirrorTrackingEffect = new MirrorTrackingEffect(mirrorClone, owner);

                                owner.SubscribeToAll(mirrorTrackingEffect);
                                mirrorClone.SubscribeToAll(ownersTrackingEffect);

                                owner.AddQEffect(ownersTrackingEffect);
                                mirrorClone.AddQEffect(mirrorTrackingEffect);
                            }
                        }));
                };
                self.ProvideActionIntoPossibilitySection = (QEffect swapToClone, PossibilitySection possibilitySection) =>
                {
                    Creature owner = swapToClone.Owner;
                    MirrorTrackingEffect? mirrorTracking = owner.FindQEffect(ThaumaturgeQEIDs.MirrorTracking) as MirrorTrackingEffect;
                    if (possibilitySection.PossibilitySectionId == PossibilitySectionId.MainActions && mirrorTracking != null)
                    {
                        Creature pairedCreature = mirrorTracking.PairedCreature;
                        return new ActionPossibility(new CombatAction(owner, IllustrationName.GenericCombatManeuver, "Swap to Clone", [], "Swaps to the clone, in which you can continue your turn.", Target.Self())
                            .WithActionCost(0)
                            .WithEffectOnSelf(async (Creature self) =>
                            {
                                self.SwapPositions(pairedCreature);
                            }));
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Regalia Implement feature
        /// </summary>
        /// <param name="regaliaImplementFeat">The Regalia Implement feat object</param>
        public static void AddRegaliaImplementLogic(Feat regaliaImplementFeat)
        {
            AddImplementEnsureLogic(regaliaImplementFeat);
            regaliaImplementFeat.WithPermanentQEffect(ImplementDetails.RegaliaInitiateBenefitName, delegate (QEffect self)
            {
                self.StartOfCombat = async (QEffect startOfCombat) =>
                {
                    Creature owner = startOfCombat.Owner;
                    foreach (Creature ally in owner.Battle.AllCreatures.Where(creature => owner.FriendOf(creature)))
                    {
                        ally.AddQEffect(new QEffect(ExpirationCondition.Never)
                        {
                            BonusToDefenses = (QEffect bonusToDefenses, CombatAction? action, Defense defense) =>
                            {
                                if (defense == Defense.Will && action != null && action.HasTrait(Trait.Fear) && ThaumaturgeUtilities.IsCreatureWeildingImplement(owner) && ally.DistanceTo(owner) <= 3)
                                {
                                    return new Bonus(1, BonusType.Status, ImplementDetails.RegaliaInitiateBenefitName, true);
                                }

                                return null;
                            }
                        });
                    }
                };
                self.EndOfYourTurn = async (QEffect endOfTurn, Creature self) =>
                {
                    foreach (Creature ally in self.Battle.AllCreatures.Where(creature => self.FriendOf(creature) && self.DistanceTo(creature) <= 3 && creature.HasEffect(QEffectId.Frightened)))
                    {
                        if (ThaumaturgeUtilities.IsCreatureWeildingImplement(self))
                        {
                            QEffect? frightened = ally.FindQEffect(QEffectId.Frightened);
                            if (frightened != null)
                            {
                                frightened.Value -= 1;
                                if (frightened.Value <= 0)
                                {
                                    frightened.ExpiresAt = ExpirationCondition.Immediately;
                                }
                            }
                        }
                    }
                };
                self.BonusToSkillChecks = (Skill skill, CombatAction action, Creature? target) =>
                {
                    if (ThaumaturgeUtilities.IsCreatureWeildingImplement(self.Owner) && (skill == Skill.Deception || skill == Skill.Diplomacy || skill == Skill.Intimidation))
                    {
                        return new Bonus(1, BonusType.Circumstance, ImplementDetails.RegaliaInitiateBenefitName, true);
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Tome Implement feature
        /// </summary>
        /// <param name="tomeImplementFeat">The Tome Implement feat object</param>
        public static void AddTomeImplementLogic(Feat tomeImplementFeat)
        {
            tomeImplementFeat.OnSheet = (CalculatedCharacterSheetValues sheet) =>
            {
                ThaumaturgeUtilities.EnsureCorrectImplements(sheet);
                sheet.AddSelectionOption(new SingleFeatSelectionOption("Tome Extra Skill", "Tome Extra Skill", 1, (feat => feat is SkillSelectionFeat)));
                sheet.AddSelectionOption(new SingleFeatSelectionOption("Tome Extra Expert Skill", "Tome Extra Expert Skill", 3, (feat => feat is SkillIncreaseFeat)));
            };
        }

        /// <summary>
        /// Adds the logic for the Wand Implement feature
        /// </summary>
        /// <param name="wandImplementFeat">The Wand Implement feat object</param>
        public static void AddWandImplementLogic(Feat wandImplementFeat)
        {
            wandImplementFeat.OnSheet = (CalculatedCharacterSheetValues sheet) =>
            {
                ThaumaturgeUtilities.EnsureCorrectImplements(sheet);
                List<FeatName> wandFeatNames = new List<FeatName>() { ThaumaturgeFeatNames.ColdWand, ThaumaturgeFeatNames.ElectricityWand, ThaumaturgeFeatNames.FireWand } ;
                sheet.AddSelectionOption(new SingleFeatSelectionOption("Thaumaturge Wand Type", "Wand", 1, feat => wandFeatNames.Contains(feat.FeatName)));
            };
            wandImplementFeat.WithPermanentQEffect(ImplementDetails.WandInitiateBenefitName, delegate (QEffect self)
            {
                self.ProvideActionIntoPossibilitySection = (QEffect wandImplementEffect, PossibilitySection possibilitySection) =>
                {
                    if (possibilitySection.PossibilitySectionId == PossibilitySectionId.MainActions)
                    {
                        Creature owner = wandImplementEffect.Owner;
                        DamageKind? wandDamageKind = null;
                        Trait? wandTraitForType = null;
                        IllustrationName? projectileIllustration = null;
                        SfxName? wandSfx = null;
                        if (owner.HasFeat(ThaumaturgeFeatNames.ColdWand))
                        {
                            wandDamageKind = DamageKind.Cold;
                            wandTraitForType = Trait.Cold;
                            wandSfx = SfxName.RayOfFrost;
                            projectileIllustration = IllustrationName.RayOfFrost;
                        }
                        else if (owner.HasFeat(ThaumaturgeFeatNames.ElectricityWand))
                        {
                            wandDamageKind = DamageKind.Electricity;
                            wandTraitForType = Trait.Electricity;
                            wandSfx = SfxName.ElectricArc;
                            projectileIllustration = IllustrationName.ElectricArc;
                        }
                        else if (owner.HasFeat(ThaumaturgeFeatNames.FireWand)) 
                        {
                            wandDamageKind = DamageKind.Fire;
                            wandTraitForType = Trait.Fire;
                            wandSfx = SfxName.FireRay;
                            projectileIllustration = IllustrationName.FireRay;
                        }

                        PossibilitySection wandSection = new PossibilitySection("Wand Possibilities");

                        IllustrationName wandIllustrationName = IllustrationName.GenericCombatManeuver;
                        List<Trait> wandTraits = [Trait.Concentrate, Trait.Evocation, Trait.Magical, Trait.Manipulate, ThaumaturgeTraits.Thaumaturge];
                        if (wandTraitForType != null)
                        {
                            wandTraits.Add((Trait)wandTraitForType);
                        }

                        CombatAction flingMagicAction = new CombatAction(self.Owner, wandIllustrationName, "Fling Magic", wandTraits.ToArray(), ImplementDetails.WandInitiateBenefitRulesText, Target.RangedCreature(12)
                            .WithAdditionalConditionOnTargetCreature((Creature user, Creature target) =>
                            {
                                if (!ThaumaturgeUtilities.IsCreatureWeildingImplement(self.Owner))
                                {
                                    return Usability.NotUsable("Not weilding Implement.");
                                }
                                return Usability.Usable;
                            }));
                        flingMagicAction.WithActionCost(2);
                        flingMagicAction.WithSavingThrow(new SavingThrow(Defense.Reflex, creature => ThaumaturgeUtilities.CalculateClassDC(owner, ThaumaturgeTraits.Thaumaturge)));
                        flingMagicAction.WithEffectOnEachTarget(async delegate (CombatAction action, Creature attacker, Creature defender, CheckResult result)
                        {
                            if (wandDamageKind != null)
                            {
                                int level = owner.Level;
                                KindedDamage wandDamage = new KindedDamage(DiceFormula.FromText("" + (1 + (int)(Math.Floor((level - 1) / 2.0))) + "d4 + " + attacker.Abilities.Charisma, "Fling Magic"), (DamageKind)wandDamageKind);
                                DamageEvent wandDamageEvent = new DamageEvent(action, defender, result, [wandDamage], result == CheckResult.CriticalFailure, result == CheckResult.Success);
                                if (result <= CheckResult.Success)
                                {
                                    await attacker.DealDirectDamage(wandDamageEvent);
                                }
                            }
                        });

                        CombatAction boostedFlingMagicAction = new CombatAction(self.Owner, wandIllustrationName, "Boosted Fling Magic", wandTraits.ToArray(), ImplementDetails.WandInitiateBenefitRulesText, Target.RangedCreature(12)
                            .WithAdditionalConditionOnTargetCreature((Creature user, Creature target) =>
                            {
                                if (owner.HasEffect(ThaumaturgeQEIDs.BoostedWandUsed))
                                {
                                    int value = owner.GetQEffectValue(ThaumaturgeQEIDs.BoostedWandUsed);
                                    return Usability.NotUsable("Wand isn't charged yet. (" + value + " turns left)");
                                }
                                else if (!ThaumaturgeUtilities.IsCreatureWeildingImplement(self.Owner))
                                {
                                    return Usability.NotUsable("Not weilding Implement.");
                                }
                                return Usability.Usable;
                            }));
                        boostedFlingMagicAction.WithActionCost(2);
                        boostedFlingMagicAction.WithSavingThrow(new SavingThrow(Defense.Reflex, creature => ThaumaturgeUtilities.CalculateClassDC(owner, ThaumaturgeTraits.Thaumaturge)));
                        boostedFlingMagicAction.WithEffectOnEachTarget(async delegate (CombatAction action, Creature attacker, Creature defender, CheckResult result)
                        {
                            if (wandDamageKind != null)
                            {
                                int level = owner.Level;
                                KindedDamage wandDamage = new KindedDamage(DiceFormula.FromText("" + (1 + (int)(Math.Floor((level - 1) / 2.0))) + "d6 + " + attacker.Abilities.Charisma, "Boosted Fling Magic"), (DamageKind)wandDamageKind);
                                DamageEvent wandDamageEvent = new DamageEvent(action, defender, result, [wandDamage], result == CheckResult.CriticalFailure, result == CheckResult.Success);
                                if (result <= CheckResult.Success)
                                {
                                    await attacker.DealDirectDamage(wandDamageEvent);
                                }

                                DiceFormula boostedRoll = DiceFormula.FromText("1d4", "Boosted Fling Magic");
                                int boostedResult = boostedRoll.RollResult();
                                owner.Battle.Log(owner.Name + " can't Boost again for " + boostedResult + " rounds.");
                                owner.AddQEffect(new QEffect(ExpirationCondition.CountsDownAtStartOfSourcesTurn)
                                {
                                    Id = ThaumaturgeQEIDs.BoostedWandUsed,
                                    Source = owner,
                                    Value = 1 + boostedResult
                                });
                            }
                        });

                        if (wandSfx != null)
                        {
                            flingMagicAction.WithSoundEffect((SfxName)wandSfx);
                            boostedFlingMagicAction.WithSoundEffect((SfxName)wandSfx);
                        }
                        if (projectileIllustration != null)
                        {
                            flingMagicAction.ProjectileIllustration = projectileIllustration;
                            boostedFlingMagicAction.ProjectileIllustration = projectileIllustration;
                        }

                        ActionPossibility flingMagicActionPossibility = new ActionPossibility(flingMagicAction);
                        wandSection.AddPossibility(flingMagicActionPossibility);
                        ActionPossibility boostedFlingMagicPossibility = new ActionPossibility(boostedFlingMagicAction);
                        wandSection.AddPossibility(boostedFlingMagicPossibility);

                        SubmenuPossibility wandMenu = new SubmenuPossibility(IllustrationName.GenericCombatManeuver, ImplementDetails.WandInitiateBenefitName);
                        wandMenu.Subsections.Add(wandSection);
                        return wandMenu;
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Weapon Implement feature
        /// </summary>
        /// <param name="weaponImplementFeat">The Weapon Implement feat object</param>
        public static void AddWeaponImplementLogic(Feat weaponImplementFeat)
        {
            AddImplementEnsureLogic(weaponImplementFeat);
            weaponImplementFeat.WithPermanentQEffect(ImplementDetails.WeaponInitiateBenefitName, delegate (QEffect self)
            {
                self.StartOfCombat = async (QEffect startOfCombat) =>
                {
                    Creature owner = startOfCombat.Owner;
                    Item? weaponImplement = owner.HeldItems.FirstOrDefault(item => item != null && item.WeaponProperties != null && !item.HasTrait(Trait.TwoHanded));
                    if (weaponImplement == null)
                    {
                        weaponImplement = owner.CarriedItems.FirstOrDefault(item => item != null && item.WeaponProperties != null && !item.HasTrait(Trait.TwoHanded));
                    }

                    if (weaponImplement != null)
                    {
                        weaponImplement.Traits.Add(ThaumaturgeTraits.Implement);
                        owner.AddQEffect(QEffect.AttackOfOpportunity("Implement's Interruption", ImplementDetails.WeaponInitiateBenefitRulesText, (QEffect resirctions, Creature target) =>
                        {
                            QEffect? exploitTargetEffect = target.FindQEffect(ThaumaturgeQEIDs.ExploitVulnerabilityTarget);
                            if (exploitTargetEffect != null && exploitTargetEffect.Tag != null && exploitTargetEffect.Tag is Creature thaumaturge && thaumaturge == owner)
                            {
                                return true;
                            }

                            return false;
                        }
                        , false));
                    }
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Implement's Empowerment base class feature
        /// </summary>
        /// <param name="implementsEmpowermentFeat">The Implement's Empowerment feat object</param>
        public static void AddImplementsEmpowermentLogic(Feat implementsEmpowermentFeat)
        {
            // Adds a permanent Bonus to Damage effect if the criteria matches
            implementsEmpowermentFeat.WithPermanentQEffect("2 additional damage per weapon damage die", delegate (QEffect self)
            {
                self.BonusToDamage = (QEffect self, CombatAction action, Creature defender) =>
                {
                    if (action.Item != null && action.Item.WeaponProperties != null && ThaumaturgeUtilities.IsCreatureWeildingImplement(self.Owner))
                    {
                        return new Bonus(2 * action.Item.WeaponProperties.DamageDieCount, BonusType.Untyped, "Implements Empowerment");
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Root To Life feat
        /// </summary>
        /// <param name="rootToLifeFeat">The Root To Life feat object</param>
        public static void AddRootToLifeLogic(Feat rootToLifeFeat)
        {
            rootToLifeFeat.WithPermanentQEffect("Root to Life", delegate (QEffect self)
            {
                self.ProvideActionIntoPossibilitySection = (QEffect rootToLifeEffect, PossibilitySection possibilitySection) =>
                {
                    if (possibilitySection.PossibilitySectionId == PossibilitySectionId.MainActions)
                    {
                        Creature owner = rootToLifeEffect.Owner;
                        PossibilitySection rootToLifeSection = new PossibilitySection("Root to Life Possibilities");

                        IllustrationName rootToLifeIllustrationName = IllustrationName.GenericCombatManeuver;
                        List<Trait> rootToLifeTraits = [Trait.Manipulate, Trait.Necromancy, Trait.Primal, ThaumaturgeTraits.Thaumaturge];

                        CombatAction oneActionRootToLifeAction = new CombatAction(self.Owner, rootToLifeIllustrationName, "Root to Life", rootToLifeTraits.ToArray(), rootToLifeFeat.RulesText, Target.AdjacentFriend()
                            .WithAdditionalConditionOnTargetCreature((Creature user, Creature target) =>
                            {
                                if (!target.FriendOf(user) || !target.HasEffect(QEffectId.Dying))
                                {
                                    return Usability.CommonReasons.NotDying;
                                }

                                return Usability.Usable;
                            }));
                        oneActionRootToLifeAction.WithActionCost(1);
                        oneActionRootToLifeAction.WithEffectOnEachTarget(async delegate (CombatAction action, Creature attacker, Creature defender, CheckResult result)
                        {
                            QEffect? dyingEffect = defender.FindQEffect(QEffectId.Dying);
                            if (dyingEffect != null)
                            {
                                dyingEffect.Value = 0;
                            }
                        });

                        CombatAction twoActionRootToLifeAction = new CombatAction(self.Owner, rootToLifeIllustrationName, "Root to Life", rootToLifeTraits.Concat([Trait.Auditory]).ToArray(), rootToLifeFeat.RulesText, Target.AdjacentFriend()
                            .WithAdditionalConditionOnTargetCreature((Creature user, Creature target) =>
                            {
                                if (target.QEffects.Where(qe => qe.Id == QEffectId.PersistentDamage).Count() == 0 || !target.FriendOf(user) || !target.HasEffect(QEffectId.Dying))
                                {
                                    return Usability.CommonReasons.NotDying;
                                }

                                return Usability.Usable;
                            }));
                        twoActionRootToLifeAction.WithActionCost(2);
                        twoActionRootToLifeAction.WithEffectOnEachTarget(async delegate (CombatAction action, Creature attacker, Creature defender, CheckResult result)
                        {
                            QEffect? dyingEffect = defender.FindQEffect(QEffectId.Dying);
                            if (dyingEffect != null)
                            {
                                dyingEffect.Value = 0;
                            }

                            foreach (QEffect persistentDamageEffect in defender.QEffects.Where(qe => qe.Id == QEffectId.PersistentDamage))
                            {
                                persistentDamageEffect.RollPersistentDamageRecoveryCheck(true);
                            }
                        });

                        ActionPossibility oneActionRootToLifeActionPossibility = new ActionPossibility(oneActionRootToLifeAction);
                        rootToLifeSection.AddPossibility(oneActionRootToLifeActionPossibility);
                        ActionPossibility twoActionRootToLifeActionPossibility = new ActionPossibility(twoActionRootToLifeAction);
                        rootToLifeSection.AddPossibility(twoActionRootToLifeActionPossibility);

                        SubmenuPossibility rootToLifeMenu = new SubmenuPossibility(IllustrationName.GenericCombatManeuver, "Root to Life");
                        rootToLifeMenu.Subsections.Add(rootToLifeSection);
                        return rootToLifeMenu;
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Scroll Thaumaturgy feat
        /// </summary>
        /// <param name="scrollThaumaturgyFeat">The Scroll Thaumaturgy feat object</param>
        public static void AddScrollThaumaturgyLogic(Feat scrollThaumaturgyFeat)
        {
            scrollThaumaturgyFeat.OnSheet = (CalculatedCharacterSheetValues sheet) =>
            {
                sheet.SpellTraditionsKnown.Add(Trait.Spell);
                sheet.SetProficiency(Trait.Spell, Proficiency.Trained);
            };
            scrollThaumaturgyFeat.WithOnCreature(creature =>
            {
                creature.AddSpellcastingSource(SpellcastingKind.Innate, ThaumaturgeTraits.Thaumaturge, Ability.Charisma, Trait.Spell);
            });
            scrollThaumaturgyFeat.WithPermanentQEffect("Scroll Thaumaturgy", delegate (QEffect self)
            {
                self.YouBeginAction = async (QEffect youBeginAction, CombatAction action) =>
                {
                    foreach (QEffect scrollAndImplementEffect in youBeginAction.Owner.QEffects.Where(qe => qe.Id == ThaumaturgeQEIDs.HeldScrollAndImplement))
                    {
                        if (action.CastFromScroll != null && scrollAndImplementEffect != null && scrollAndImplementEffect.Tag != null && scrollAndImplementEffect.Tag is ImplementAndHeldItem implementAndHeldItem && action.CastFromScroll == implementAndHeldItem.HeldItem && implementAndHeldItem.Illustration is SideBySideIllustration sideBySideIllustration)
                        {
                            implementAndHeldItem.Implement.Illustration = sideBySideIllustration.Left;
                            implementAndHeldItem.Implement.Name = implementAndHeldItem.OriginalImplementName;
                            scrollAndImplementEffect.ExpiresAt = ExpirationCondition.EphemeralAtEndOfImmediateAction;
                        }
                    }
                };
                self.EndOfCombat = async (QEffect endOfCombat, bool didWin) =>
                {
                    if (didWin)
                    {
                        Creature owner = endOfCombat.Owner;
                        foreach (QEffect scrollAndImplementEffect in owner.QEffects.Where(qe => qe.Id == ThaumaturgeQEIDs.HeldScrollAndImplement))
                        {
                            if (scrollAndImplementEffect != null && scrollAndImplementEffect.Tag != null && scrollAndImplementEffect.Tag is ImplementAndHeldItem implementAndHeldItem)
                            {
                                owner.CarriedItems.Add(implementAndHeldItem.HeldItem);
                                owner.HeldItems.RemoveAll(item => item == implementAndHeldItem.Implement);
                                owner.CarriedItems.RemoveAll(item => item == implementAndHeldItem.Implement);
                            }
                        }
                    }
                };
                self.ProvideActionIntoPossibilitySection = (QEffect scrollThaumaturgyEffect, PossibilitySection possibilitySection) =>
                {
                    Creature owner = scrollThaumaturgyEffect.Owner;
                    if (possibilitySection.PossibilitySectionId == PossibilitySectionId.ItemActions && ThaumaturgeUtilities.IsCreatureWeildingImplement(owner))
                    {
                        SubmenuPossibility? inventory = (SubmenuPossibility?)possibilitySection.Possibilities.FirstOrDefault(possibility => possibility is SubmenuPossibility submenuPossibility && submenuPossibility.Caption == "Inventory");
                        if (inventory != null)
                        {
                            PossibilitySection? drawOrStowPossibilitySection = inventory.Subsections.FirstOrDefault(possibility => possibility.Name == "Draw or stow items");
                            if (drawOrStowPossibilitySection != null)
                            {
                                List<int> implementIndexes = new List<int>();
                                for (int i = 0; i < owner.HeldItems.Count; i++)
                                {
                                    Item? heldItem = owner.HeldItems[i];
                                    if (heldItem != null && heldItem.HasTrait(ThaumaturgeTraits.Implement))
                                    {
                                        implementIndexes.Add(i);
                                    }
                                }

                                foreach (int implementIndex in implementIndexes)
                                {
                                    if (implementIndex >= 0 && implementIndex < drawOrStowPossibilitySection.Possibilities.Count)
                                    {
                                        Possibility possibility = drawOrStowPossibilitySection.Possibilities[implementIndex];
                                        if (possibility is SubmenuPossibility itemSubMenu)
                                        {

                                            string drawOrReplace = "Draw";
                                            QEffect? matchingHeldScrollImplementEffect = owner.QEffects.FirstOrDefault(qe => qe.Id == ThaumaturgeQEIDs.HeldScrollAndImplement && qe.Tag != null && qe.Tag is ImplementAndHeldItem implementAndHeldItem && implementAndHeldItem.Implement == owner.HeldItems[implementIndex]);
                                            if (matchingHeldScrollImplementEffect != null)
                                            {
                                                drawOrReplace = "Replace";
                                            }

                                            PossibilitySection drawScrollSection = new PossibilitySection(drawOrReplace + " scroll");
                                            foreach (Item scroll in owner.CarriedItems.Where(item => item.HasTrait(Trait.Scroll)))
                                            {
                                                CombatAction drawScrollImplementAction = new CombatAction(owner, scroll.Illustration, drawOrReplace + " " + scroll.Name, [Trait.Manipulate], drawOrReplace + " a scroll into the same hand you are holding this implement in.\n----\n" + scroll.Description, Target.Self())
                                                    .WithActionCost(1)
                                                    .WithEffectOnSelf((Creature self) =>
                                                    {
                                                        if (self.HeldItems[implementIndex] is Implement implement)
                                                        {
                                                            ImplementAndHeldItem implementAndScroll = new ImplementAndHeldItem(implement, scroll);

                                                            self.CarriedItems.Remove(scroll);
                                                            if (matchingHeldScrollImplementEffect != null && matchingHeldScrollImplementEffect.Tag != null && matchingHeldScrollImplementEffect.Tag is ImplementAndHeldItem previousImplementAndHeldItem)
                                                            {
                                                                Item previousScroll = previousImplementAndHeldItem.HeldItem;
                                                                self.CarriedItems.Add(previousScroll);
                                                                self.RemoveAllQEffects(qe => qe.Id == ThaumaturgeQEIDs.HeldScrollAndImplement && qe == matchingHeldScrollImplementEffect);
                                                            }

                                                            self.AddQEffect(new QEffect(ExpirationCondition.Never)
                                                            {
                                                                Id = ThaumaturgeQEIDs.HeldScrollAndImplement,
                                                                Tag = implementAndScroll
                                                            });

                                                            implement.Illustration = implementAndScroll.Illustration;
                                                            implement.Name = implement.Name + " " + scroll.Name;
                                                        }
                                                    });

                                                Possibility drawScrollPossibility = new ActionPossibility(drawScrollImplementAction);
                                                drawScrollSection.AddPossibility(drawScrollPossibility);
                                            }

                                            itemSubMenu.Subsections.Add(drawScrollSection);
                                        }
                                    }
                                }
                            }
                        }

                        foreach (QEffect heldItemAndImplementEffect in owner.QEffects.Where(qe => qe.Id == ThaumaturgeQEIDs.HeldScrollAndImplement))
                        {
                            if (heldItemAndImplementEffect != null && heldItemAndImplementEffect.Tag != null && heldItemAndImplementEffect.Tag is ImplementAndHeldItem scrollAndImplement)
                            {
                                Item scroll = scrollAndImplement.HeldItem;
                                ScrollProperties scrollProperties = scroll.ScrollProperties;
                                if (scrollProperties != null)
                                {
                                    if (scrollProperties.Spell.CombatActionSpell.Owner != owner)
                                    {
                                        scroll.ScrollProperties.Spell = scroll.ScrollProperties.Spell.Duplicate(owner, scroll.ScrollProperties.Spell.SpellLevel, true);
                                        CombatAction itemSpell = scroll.ScrollProperties.Spell.CombatActionSpell;
                                        itemSpell.CastFromScroll = scroll;
                                        if (owner.Spellcasting != null)
                                            itemSpell.SpellcastingSource = owner.Spellcasting.Sources.FirstOrDefault<SpellcastingSource>((Func<SpellcastingSource, bool>)(source => itemSpell.Traits.Contains(source.SpellcastingTradition)));
                                        CharacterSheet persistentCharacterSheet2 = itemSpell.Owner.PersistentCharacterSheet;
                                        if (persistentCharacterSheet2 != null && !persistentCharacterSheet2.CanUse(scroll) && owner.HasEffect(QEffectId.TrickMagicItem) && itemSpell.ActionCost != 3 && owner.Spellcasting != null)
                                        {
                                            CommonSpellEffects.IncreaseActionCostByOne(itemSpell);
                                            itemSpell.Description += "\n\n{b}Uses Trick Magic Item.{/b} The spell costs 1 more action that normal, and you must succeed at a skill check to activate it.";
                                            itemSpell.SpellcastingSource = owner.Spellcasting.GetSourceByOrigin(Trait.UsesTrickMagicItem);
                                            itemSpell.Traits.Add(Trait.UsesTrickMagicItem);
                                        }
                                        if (itemSpell.SpellcastingSource == null)
                                            itemSpell.SpellcastingSource = SpellcastingSource.EmptySource;
                                    }

                                    Possibility spellPossibility = Possibilities.CreateSpellPossibility(scroll.ScrollProperties.Spell.CombatActionSpell);
                                    spellPossibility.Illustration = scroll.Illustration;
                                    spellPossibility.Caption = scroll.Name;
                                    spellPossibility.PossibilitySize = PossibilitySize.Full;
                                    spellPossibility.PossibilityGroup = "Use item";
                                    possibilitySection.AddPossibility(spellPossibility);
                                }
                            }
                        }
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Esoteric Warden base class feature
        /// </summary>
        /// <param name="esotericWardenFeat">The Esoteric Warden feat object</param>
        public static void AddEsotericWardenLogic(Feat esotericWardenFeat)
        {
            esotericWardenFeat.WithPermanentQEffect("Esoteric Warden", delegate (QEffect self)
            {
                self.AfterYouTakeAction = async (QEffect afterYouTakeAction, CombatAction action) =>
                {
                    if (action.ActionId == ThaumaturgeActionIDs.ExploitVulnerability && action.CheckResult >= CheckResult.Success && action.ChosenTargets.ChosenCreature != null)
                    {
                        int esotericWardenValue = (action.CheckResult == CheckResult.Success) ? 1 : 2;
                        Creature? target = action.ChosenTargets.ChosenCreature;
                        if (target != null)
                        {
                            afterYouTakeAction.Owner.AddQEffect(new QEffect(ExpirationCondition.Never)
                            {
                                Id = ThaumaturgeQEIDs.EsotericWardenAC,
                                Illustration = IllustrationName.GenericCombatManeuver,
                                Name = "Esoteric Warden AC (" + target.Name + ")",
                                Description = "+" + esotericWardenValue + " status bonus to AC against " + target.Name,
                                Value = esotericWardenValue,
                                Tag = action.ChosenTargets.ChosenCreature,
                                DoNotShowUpOverhead = true
                            });

                            afterYouTakeAction.Owner.AddQEffect(new QEffect(ExpirationCondition.Never)
                            {
                                Id = ThaumaturgeQEIDs.EsotericWardenSave,
                                Illustration = IllustrationName.GenericCombatManeuver,
                                Name = "Esoteric Warden Saving Throw (" + target.Name + ")",
                                Description = "+" + esotericWardenValue + " status bonus to Saving Throws against " + target.Name,
                                Value = esotericWardenValue,
                                Tag = action.ChosenTargets.ChosenCreature,
                                DoNotShowUpOverhead = true
                            });
                        }
                    }
                };
                self.BonusToDefenses = (QEffect bonusToDefenses, CombatAction? action, Defense defense) =>
                {
                    if (action != null)
                    {
                        Creature owner = bonusToDefenses.Owner;
                        QEffect? esotericWardenACEffect = owner.FindQEffect(ThaumaturgeQEIDs.EsotericWardenAC);
                        QEffect? esotericWardenSaveEffect = owner.FindQEffect(ThaumaturgeQEIDs.EsotericWardenSave);

                        if (defense == Defense.AC && esotericWardenACEffect != null && esotericWardenACEffect.Tag != null && esotericWardenACEffect.Tag is Creature wardenACCreature && wardenACCreature == action.Owner)
                        {
                            return new Bonus(esotericWardenACEffect.Value, BonusType.Status, "Esoteric Warden", true);
                        }

                        if (defense.IsSavingThrow() && esotericWardenSaveEffect != null && esotericWardenSaveEffect.Tag != null && esotericWardenSaveEffect.Tag is Creature wardenSaveCreature && wardenSaveCreature == action.Owner)
                        {
                            return new Bonus(esotericWardenSaveEffect.Value, BonusType.Status, "Esoteric Warden", true);
                        }
                    }

                    return null;
                };
                self.YouAreTargeted = async (QEffect youAreTargeted, CombatAction action) =>
                {
                    Creature owner = youAreTargeted.Owner;
                    QEffect? esotericWardenACEffect = owner.FindQEffect(ThaumaturgeQEIDs.EsotericWardenAC);
                    QEffect? esotericWardenSaveEffect = owner.FindQEffect(ThaumaturgeQEIDs.EsotericWardenSave);
                    if (action.SavingThrow != null && esotericWardenSaveEffect != null && esotericWardenSaveEffect.Tag != null && esotericWardenSaveEffect.Tag is Creature wardenSaveCreature && wardenSaveCreature == action.Owner)
                    {
                        esotericWardenSaveEffect.ExpiresAt = ExpirationCondition.Immediately;
                    }
                    else if (esotericWardenACEffect != null && esotericWardenACEffect.Tag != null && esotericWardenACEffect.Tag is Creature wardenACCreature && wardenACCreature == action.Owner)
                    {
                        esotericWardenACEffect.ExpiresAt = ExpirationCondition.Immediately;
                    }
                };
            });
        }

        private static void AddImplementEnsureLogic(Feat implementFeat)
        {
            implementFeat.WithOnSheet((character) =>
            {
                ThaumaturgeUtilities.EnsureCorrectImplements(character);
            });
        }
    }
}
