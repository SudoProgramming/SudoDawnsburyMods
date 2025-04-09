using Dawnsbury.Audio;
using Dawnsbury.Auxiliary;
using Dawnsbury.Campaign.Path;
using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
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
using Dawnsbury.Modding;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.Constants;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.Enums;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.Extensions;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Collections.Specialized.BitVector32;

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

            // Creates and adds the sub feats for the Wand Implement
            yield return new Feat(ThaumaturgeFeatNames.ColdWand, "Cold Wand", "Your wand is attuned to Cold.", [], null);
            yield return new Feat(ThaumaturgeFeatNames.ElectricityWand, "Electricity Wand", "Your wand is attuned to Electricity.", [], null);
            yield return new Feat(ThaumaturgeFeatNames.FireWand, "Fire Wand", "Your wand is attuned to Fire.", [], null);

            // Creates and adds the logic for the Amulet Implement sub-class feature
            Feat amuletImplementFeat = new Feat(ThaumaturgeFeatNames.AmuletImplement, ImplementDetails.AmuletInitiateBenefitFlavorText, "You gain the " + ImplementDetails.AmuletInitiateBenefitName + " reaction.\n\n{b}" + ImplementDetails.AmuletInitiateBenefitName + "{/b} {icon:Reaction}\n" + ImplementDetails.AmuletInitiateBenefitRulesText, [ThaumaturgeTraits.Implement], null);
            AddLevelTag(amuletImplementFeat);
            AddAmuletImplementLogic(amuletImplementFeat);
            yield return amuletImplementFeat;

            // Creates and adds the logic for the Bell Implement sub-class feature
            Feat bellImplementFeat = new Feat(ThaumaturgeFeatNames.BellImplement, ImplementDetails.BellInitiateBenefitFlavorText, "You gain the " + ImplementDetails.BellInitiateBenefitName + " reaction.\n\n{b}" + ImplementDetails.BellInitiateBenefitName + "{/b} {icon:Reaction}\n" + ImplementDetails.BellInitiateBenefitRulesText, [ThaumaturgeTraits.Implement], null);
            AddLevelTag(bellImplementFeat);
            AddBellImplementLogic(bellImplementFeat);
            yield return bellImplementFeat;

            // Creates and adds the logic for the Chalice Implement sub-class feature
            Feat chaliceImplementFeat = new Feat(ThaumaturgeFeatNames.ChaliceImplement, ImplementDetails.ChaliceInitiateBenefitFlavorText, "You gain the " + ImplementDetails.ChaliceInitiateBenefitName + " action.\n\n{b}" + ImplementDetails.ChaliceInitiateBenefitName + "{/b} {icon:Action}\n" + ImplementDetails.ChaliceInitiateBenefitRulesText, [ThaumaturgeTraits.Implement], null);
            AddLevelTag(chaliceImplementFeat);
            AddChaliceImplementLogic(chaliceImplementFeat);
            yield return chaliceImplementFeat;

            // Creates and adds the logic for the Lantern Implement sub-class feature
            Feat lanternImplementFeat = new Feat(ThaumaturgeFeatNames.LanternImplement, ImplementDetails.LanternInitiateBenefitFlavorText, "You gain the following benefit.\n\n" + ImplementDetails.LanternInitiateBenefitRulesText, [ThaumaturgeTraits.Implement], null);
            AddLevelTag(lanternImplementFeat);
            AddLanternImplementLogic(lanternImplementFeat);
            yield return lanternImplementFeat;

            // Creates and adds the logic for the Mirror Implement sub-class feature
            Feat mirrorImplementFeat = new Feat(ThaumaturgeFeatNames.MirrorImplement, ImplementDetails.MirrorInitiateBenefitFlavorText, "You gain the " + ImplementDetails.MirrorInitiateBenefitName + " action.\n\n{b}" + ImplementDetails.MirrorInitiateBenefitName + "{/b} {icon:Action}\n" + ImplementDetails.MirrorInitiateBenefitRulesText, [ThaumaturgeTraits.Implement], null);
            AddLevelTag(mirrorImplementFeat);
            AddMirrorImplementLogic(mirrorImplementFeat);
            yield return mirrorImplementFeat;

            // Creates and adds the logic for the Regalia Implement sub-class feature
            Feat regaliaImplementFeat = new Feat(ThaumaturgeFeatNames.RegaliaImplement, ImplementDetails.RegaliaInitiateBenefitFlavorText, "You gain the following benefit.\n\n" + ImplementDetails.RegaliaInitiateBenefitRulesText, [ThaumaturgeTraits.Implement], null);
            AddLevelTag(regaliaImplementFeat);
            AddRegaliaImplementLogic(regaliaImplementFeat);
            yield return regaliaImplementFeat;

            // Creates and adds the logic for the Tome Implement sub-class feature
            Feat tomeImplementFeat = new Feat(ThaumaturgeFeatNames.TomeImplement, ImplementDetails.TomeInitiateBenefitFlavorText, "You gain the following benefit.\n\n" + ImplementDetails.TomeInitiateBenefitRulesText, [ThaumaturgeTraits.Implement], null);
            AddLevelTag(tomeImplementFeat);
            AddTomeImplementLogic(tomeImplementFeat);
            yield return tomeImplementFeat;

            // Creates and adds the logic for the Wand Implement sub-class feature
            Feat wandImplementFeat = new Feat(ThaumaturgeFeatNames.WandImplement, ImplementDetails.WandInitiateBenefitFlavorText, "You gain the " + ImplementDetails.WandInitiateBenefitName + " activity.\n\n{b}" + ImplementDetails.WandInitiateBenefitName + "{/b} {icon:TwoActions}\n" + ImplementDetails.WandInitiateBenefitRulesText, [ThaumaturgeTraits.Implement], null);
            AddLevelTag(wandImplementFeat);
            AddWandImplementLogic(wandImplementFeat);
            yield return wandImplementFeat;

            // Creates and adds the logic for the Weapon Implement sub-class feature
            Feat weaponImplementFeat = new Feat(ThaumaturgeFeatNames.WeaponImplement, ImplementDetails.WeaponInitiateBenefitFlavorText, "You gain the " + ImplementDetails.WeaponInitiateBenefitName + " reaction.\n\n{b}NOTE: The Weapon Implement will be applied to your first One-Handed weapon at the start of each encounter.{/b}\n\n{b}" + ImplementDetails.WeaponInitiateBenefitName + "{/b} {icon:Reaction}\n" + ImplementDetails.WeaponInitiateBenefitRulesText, [ThaumaturgeTraits.Implement], null);
            AddLevelTag(weaponImplementFeat);
            AddWeaponImplementLogic(weaponImplementFeat);
            yield return weaponImplementFeat;

            Feat amuletAdeptFeat = new Feat(ThaumaturgeFeatNames.AmuletAdept, "Your amulet offers continued protection.", "When you use Amulet's Abeyance reaction, you or your ally gains resistance 5 against the primary damage type of the trigger attack until the start of your next turn.", [ThaumaturgeTraits.AdeptImplement], null);
            amuletAdeptFeat.WithPrerequisite((CalculatedCharacterSheetValues sheet) => sheet.HasFeat(ThaumaturgeFeatNames.AmuletImplement), "Requires the Amulet Implement");
            AddAmuletAdeptLogic(amuletAdeptFeat);
            yield return amuletAdeptFeat;

            Feat bellAdeptFeat = new Feat(ThaumaturgeFeatNames.BellAdept, "Your bell resonates powerfully, causing the effect to last longer.", "The conditions from Ring Bell last 3 rounds instead of 1 round.", [ThaumaturgeTraits.AdeptImplement], null);
            bellAdeptFeat.WithPrerequisite((CalculatedCharacterSheetValues sheet) => sheet.HasFeat(ThaumaturgeFeatNames.BellImplement), "Requires the Bell Implement");
            AddBellAdeptLogic(bellAdeptFeat);
            yield return bellAdeptFeat;

            Feat chaliceAdeptFeat = new Feat(ThaumaturgeFeatNames.ChaliceAdept, "The life essence shed from blood empowers your chalice.", "If you or an ally within 30 feet takes piercing or slashing damage from a foe's critical hit or takes persistent bleed damage, Drinking from the Chalice before the end of your next turn grants that injured creature greater restoration to make up for its lost vitality. On a sip, the temporary Hit Points granted to the creature increase to 2 + your Charisma modifier + your level. When drained, the chalice heals the creature 5 Hit Points for each level you have.", [ThaumaturgeTraits.AdeptImplement], null);
            chaliceAdeptFeat.WithPrerequisite((CalculatedCharacterSheetValues sheet) => sheet.HasFeat(ThaumaturgeFeatNames.ChaliceImplement), "Requires the Chalice Implement");
            AddChaliceAdeptLogic(chaliceAdeptFeat);
            yield return chaliceAdeptFeat;

            Feat lanternAdeptFeat = new Feat(ThaumaturgeFeatNames.LanternAdept, "In addition to the initiate benefits, when you hold your lantern, its light reveals the invisible.", "The bright light increases to 30 feet. While you're holding your lantern, invisible creatures within the bright light become visible as rippling distortions, though they're still concealed.", [ThaumaturgeTraits.AdeptImplement], null);
            lanternAdeptFeat.WithPrerequisite((CalculatedCharacterSheetValues sheet) => sheet.HasFeat(ThaumaturgeFeatNames.LanternImplement), "Requires the Lantern Implement");
            AddLanternAdeptLogic(lanternAdeptFeat);
            yield return lanternAdeptFeat;

            Feat mirrorAdeptFeat = new Feat(ThaumaturgeFeatNames.MirrorAdept, "Your mirror self shatters into punishing shards when damaged.", "While Mirror's Reflection is in effect, when an enemy adjacent to one of your spaces damages you, you may choose that version of you to explode into mirror shards. This ends Mirror's Reflection (establishing the remaining version of you as the real one) and deals slashing damage to all creatures in a 5-foot emanation around where your mirror self was. The damage is equal to 2 + half your level or the damage of the triggering attack, whichever is lower. You're immune to this damage.", [ThaumaturgeTraits.AdeptImplement], null);
            mirrorAdeptFeat.WithPrerequisite((CalculatedCharacterSheetValues sheet) => sheet.HasFeat(ThaumaturgeFeatNames.MirrorImplement), "Requires the Mirror Implement");
            AddMirrorAdeptLogic(mirrorAdeptFeat);
            yield return mirrorAdeptFeat;

            Feat regaliaAdeptFeat = new Feat(ThaumaturgeFeatNames.RegaliaAdept, "Your regalia's power increases, and so do the abilities it grants.", "The circumstance bonus you gain to Deception, Diplomacy, and Intimidation increases to +2 to each skill you have master proficiency in. The +1 status bonus now applies to all saving throws against mental effects, rather than only against fear, and you and allies in your aura gain a +2 status bonus to damage rolls.", [ThaumaturgeTraits.AdeptImplement], null);
            regaliaAdeptFeat.WithPrerequisite((CalculatedCharacterSheetValues sheet) => sheet.HasFeat(ThaumaturgeFeatNames.RegaliaImplement), "Requires the Regalia Implement");
            AddRegaliaAdeptLogic(regaliaAdeptFeat);
            yield return regaliaAdeptFeat;

            Feat tomeAdeptFeat = new Feat(ThaumaturgeFeatNames.TomeAdept, "In addition to the initiate benefits, your tome inscribes insights into creatures that you can use to strike them down.", "While holding your tome, at the start of your turn each round, you may attempt a check to Exploit Vulnerability a creature of your choice. If this check succeeds, you gain a +1 circumstance bonus to your next attack roll against that creature before the start of your next turn.\n\nYou gain an additional skill increase feat.", [ThaumaturgeTraits.AdeptImplement], null);
            tomeAdeptFeat.WithPrerequisite((CalculatedCharacterSheetValues sheet) => sheet.HasFeat(ThaumaturgeFeatNames.TomeImplement), "Requires the Tome Implement");
            AddTomeAdeptLogic(tomeAdeptFeat);
            yield return tomeAdeptFeat;

            Feat wandAdeptFeat = new Feat(ThaumaturgeFeatNames.WandAdept, "You gain versatility and additional benefits when you fire your wand.", "The range of Fling Magic increases to 120 feet. Choose a second damage type from the list; whenever you Fling Magic, you can select between either of the two damage types you have chosen. Fling Magic has an additional effect if the target fails its save and takes damage, depending on the type.\n\n{b}Cold{/b} The target becomes chilled, taking a –10-foot status penalty to its Speeds for 1 round.\n{b}Electricity{/b} The target is shocked, becoming flat-footed until the end of your next turn.\n{b}Fire{/b}The target catches flame, taking 1d10 persistent fire damage (or 2d10 on a critical failure). If you have the wand paragon benefit, this increases to 2d10 persistent fire damage (or 4d10 on a critical failure).", [ThaumaturgeTraits.AdeptImplement], null);
            wandAdeptFeat.WithPrerequisite((CalculatedCharacterSheetValues sheet) => sheet.HasFeat(ThaumaturgeFeatNames.WandImplement), "Requires the Wand Implement");
            AddWandAdeptLogic(wandAdeptFeat);
            yield return wandAdeptFeat;

            Feat weaponAdeptFeat = new Feat(ThaumaturgeFeatNames.WeaponAdept, "When your implement lashes out at your foe, even a close miss brings the weapon close enough to do harm.", "When you use Implement's Interruption and fail (but don't critically fail) the Strike, you deal 1 damage of the weapon's normal type, possibly applying any bonus damage due to the target's weakness.", [ThaumaturgeTraits.AdeptImplement], null);
            weaponAdeptFeat.WithPrerequisite((CalculatedCharacterSheetValues sheet) => sheet.HasFeat(ThaumaturgeFeatNames.WeaponImplement), "Requires the Weapon Implement");
            AddWeaponAdeptLogic(weaponAdeptFeat);
            yield return weaponAdeptFeat;

            //// Creates the class selection feat for the Thaumaturge
            yield return new ClassSelectionFeat(ThaumaturgeFeatNames.ThaumaturgeClass, "The world is full of the unexplainable: ancient magic, dead gods, and even stranger things. In response, you've scavenged the best parts of every magical tradition and built up a collection of esoterica—a broken holy relic here, a sprig of mistletoe there—that you can use to best any creature by exploiting their weaknesses and vulnerabilities. The mystic implement you carry is both badge and weapon, its symbolic weight helping you bargain with and subdue the supernatural. Every path to power has its restrictions and costs, but you turn them all to your advantage. You're a thaumaturge, and you work wonders.",
                ThaumaturgeTraits.Thaumaturge, new EnforcedAbilityBoost(Ability.Charisma), 8,
                [Trait.Reflex, Trait.Simple, Trait.Martial, Trait.LightArmor, Trait.MediumArmor, Trait.Unarmed, Trait.UnarmoredDefense],
                [Trait.Perception, Trait.Fortitude, Trait.Will],
                3,
                "{b}1. Esoteric Lore{/b} You become trained in a special lore skill that can used to Exploit Vulnerability. This is a charisma-based skill. {i}(You add your Charisma modifier to checks using this skill.){/i}\n\n" +
                "{b}2. Exploit Vulnerability {icon:Action}{/b}\n{b}Frequency{/b} once per round; {b}Requirements{/b} You are holding your implement\n\nSelect a creature you can see and attempt an Esoteric Lore check against a standard DC for its level. You gain the following effects until you Exploit Vulnerabilities again.\n\n{b}Success{/b} Your unarmed and weapon Strikes activate the highest weakness againt the target, even though the damage type your weapon deals doesn't change. This damage affects the target of your Exploit Vulnerability, as well as any other creatures of the exact same type, but not other creatures with the same weakness. The {b}Failure{/b} result is used if the target has no weakness or if it is better.\n{b}Failure{/b} This causes the target creature, and only the target creature, to gain a weakness against your unarmed and weapon Strikes equal to 2 + half your level.\n{b}Critical Failure{/b} You become flat-footed until the beginning of your next turn.\n\n" +
                "{b}3. First Implement{/b} Choose an implement.\n\n" +
                "{b}4. Implement's Empowerment{/b} When you Strike, you can trace mystic patterns with an implement you're holding to empower the Strike, causing it to deal 2 additional damage per weapon damage die. Channeling the power requires full use of your hands. You don't gain the benefit of implement's empowerment if you are holding anything in either hand other than a single one-handed weapon or other implements and you must be holding at least one implement to gain the benefit.\n\n" +
                "{b}5. Thaumaturge Feat{/b}\n\n" +
                "{b}At Higher Levels:{/b}\n" +
                "{b}Level 2{/b} Thaumaturge Feat\n" +
                "{b}Level 3{/b} General feat, skill increase, Expert in Esoteric Lore\n" +
                "{b}Level 4{/b} Thaumaturge Feat\n" +
                "{b}Level 5:{/b} Ability boosts, ancestry feat, skill increase, Thaumaturge Weapon Expertise {i}(Expert in unarmed, simple and martial weapons){/i}, Second Implement {i}(you choose a second implement and gain the initiate benefit. Swapping an implement with another implement is a free action. All implements with reactions will prompt you to swap to it if it would trigger.){/i}\n" +
                "{b}Level 6:{/b} Thaumaturge feat\n" +
                "{b}Level 7:{/b} Implement Adept, general feat, skill increase, weapon specialization {i}(you deal 2 additional damage with weapons and unarmed attacks in which you are an expert; this damage increases to 3 if you're a master, and to 4 if you're legendary){/i}, master in Will, resolve {i}(Your proficiency rank for Reflex saves increases to master. When you roll a success on a Reflex save, you get a critical success instead.){/i}\n" +
                "{b}Level 8:{/b} Thaumaturge feat\n", 
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
                    sheet.AddAtLevel(5, delegate (CalculatedCharacterSheetValues values)
                    {
                        sheet.AddSelectionOption(new SingleFeatSelectionOption("SecondImplement", "Second Implement", 5, (Feat ft) => ft.HasTrait(ThaumaturgeTraits.Implement)));
                        values.SetProficiency(Trait.Unarmed, Proficiency.Expert);
                        values.SetProficiency(Trait.Simple, Proficiency.Expert);
                        values.SetProficiency(Trait.Martial, Proficiency.Expert);
                    });
                    sheet.AddAtLevel(7, delegate (CalculatedCharacterSheetValues values)
                    {
                        sheet.AddSelectionOption(new SingleFeatSelectionOption("ImplementAdept", "Implement Adept", 7, (Feat ft) => ft.HasTrait(ThaumaturgeTraits.AdeptImplement)));
                        values.SetProficiency(Trait.Will, Proficiency.Master);
                    });
                })
                .WithOnCreature(creature =>
                {
                    if (creature.Level >= 5)
                    {
                        AddSecondImplementLogic(creature);
                    }
                    if (creature.Level >= 7)
                    {
                        creature.AddQEffect(QEffect.WeaponSpecialization());
                        creature.AddQEffect(new QEffect("Resolve", "When you roll a success on a Will save, you get a critical success instead.")
                        {
                            AdjustSavingThrowCheckResult = (_, defense, _, checkResult) => defense == Defense.Will && checkResult == CheckResult.Success ? CheckResult.CriticalSuccess : checkResult
                        });
                    }
                });
            
            // Creates and adds the logic for the Ammunition Thaumaturgy feat
            TrueFeat ammunitionThaumaturgyFeat = new TrueFeat(ThaumaturgeFeatNames.AmmunitionThaumaturgy, 1, "You're so used to handling your implement, weapon, and esoterica in the heat of combat that adding a few bullets or arrows to the mix is no extra burden.", "You can use Bows using the hand holding your implement.", [ThaumaturgeTraits.Thaumaturge]);
            AddAmmunitionThaumaturgyLogic(ammunitionThaumaturgyFeat);
            yield return ammunitionThaumaturgyFeat;

            // Creates and adds the logic for the Divine Disharmony feat
            TrueFeat divineDisharmonyFeat = new TrueFeat(ThaumaturgeFeatNames.DivineDisharmony, 1, "From your collection of religious trinkets, you pull out opposing divine objects—such as the religious symbols of two deities that are hated enemies—and combine them in a display that causes discordant clashes of divine energy that are especially distracting to the faithful.", "Roll the best check between Deception or Intimidation against the Will DC of a creature you can see within 60 feet, with the following results. If the creature has access to divine spells, you gain a +2 circumstance bonus to your skill check.\n\n{b}Critical Success{/b} The creature is flat-footed to your attacks until the end of your next turn.\n\n{b}Success{/b} The creature is flat-footed against your attacks until the end of your current turn.", [Trait.Divine, Trait.Enchantment, Trait.Manipulate, ThaumaturgeTraits.Thaumaturge]);
            divineDisharmonyFeat.WithActionCost(1);
            AddDivineDisharmonyLogic(divineDisharmonyFeat);
            yield return divineDisharmonyFeat;

            // Creates and adds the logic for the Root to Life feat
            TrueFeat rootToLifeFeat = new TrueFeat(ThaumaturgeFeatNames.RootToLife, 1, "Marigold, spider lily, pennyroyal—many primal traditions connect flowers and plants with the boundary between life and death, and you can leverage this association to keep an ally on this side of the line.", "You place a small plant or similar symbol on an adjacent dying creature, immediately stabilizing them; the creature is no longer dying and is instead unconscious at 0 Hit Points.\n\nIf you spend 2 actions instead of 1, you empower the act further by uttering a quick folk blessing to chase away ongoing pain, adding the auditory trait to the action. When you do so, attempt flat checks to remove each source of persistent damage affecting the target; due to the particularly effective assistance, the DC is 10 instead of the usual 15.", [Trait.Manipulate, Trait.Necromancy, Trait.Primal, ThaumaturgeTraits.Thaumaturge]);
            AddRootToLifeLogic(rootToLifeFeat);
            yield return rootToLifeFeat;

            // Creates and adds the logic for the Scroll Thaumaturgy feat
            TrueFeat scrollThaumaturgyFeat = new TrueFeat(ThaumaturgeFeatNames.ScrollThaumaturgy, 1, "Your multidisciplinary study of magic means you know how to activate the magic in scrolls with ease.", "You can activate scrolls of any magical tradition, using your thaumaturge class DC for the scroll's DC, rather than a particular spell DC. You can draw and activate scrolls with the same hand holding an implement.", [ThaumaturgeTraits.Thaumaturge]);
            AddScrollThaumaturgyLogic(scrollThaumaturgyFeat);
            yield return scrollThaumaturgyFeat;

            // Creates and adds the logic for the Esoterica Seller feat
            TrueFeat esotericaSellerFeat = new TrueFeat(ThaumaturgeFeatNames.EsotericaSeller, 2, "You collect interesting trinkets wherever you go.", "At the end of each encounter you gain a \"Looted Esoterica\" item that's worth an amount of gold pieces equal to the combined level (levels 0 or below do not count) of all enemies you defeated in that encounter. This item has no use but you can sell it later in the shop", [ThaumaturgeTraits.Thaumaturge, Trait.Homebrew]);
            AddEsotericaSellerLogic(esotericaSellerFeat);
            yield return esotericaSellerFeat;

            // Creates and adds the logic for the Esoteric Warden feat
            TrueFeat esotericWardenFeat = new TrueFeat(ThaumaturgeFeatNames.EsotericWarden, 2, "When you apply antithetical material against a creature successfully, you also ward yourself against its next attacks.", "When you succeed at your check to Exploit a Vulnerability, you gain a +1 status bonus to your AC against the creature's next attack and a +1 status bonus to your next saving throw against the creature; if you critically succeed, these bonuses are +2 instead. You can gain these bonuses only once per day against a particular creature, and the benefit ends if you Exploit Vulnerability again.", [ThaumaturgeTraits.Thaumaturge]);
            AddEsotericWardenLogic(esotericWardenFeat);
            yield return esotericWardenFeat;

            //TrueFeat turnAwayMisfortuneFeat = new TrueFeat(ThaumaturgeFeatNames.TurnAwayMisfortune, 2, "You perform a superstition, such as casting salt over your shoulder to ward off bad luck.", "{b}Trigger{/b} You would attempt a roll affected by a misfortune effect.\n\nTurn Away Misfortune's fortune trait cancels out the misfortune effect, causing you to roll normally. As normal, you can apply only one fortune ability to a roll, so if you Turned Away Misfortune on an attack roll, you couldn't also use an ability like Halfling Luck to alter the roll further.", [Trait.Abjuration, Trait.Fortune, Trait.Manipulate, Trait.Occult, ThaumaturgeTraits.Thaumaturge]);
            //turnAwayMisfortuneFeat.WithActionCost(-1);
            //// HACK: This feat knowingly does nothing, since Misfortune is not in Dawnsbury Days. This should be fixed if added.
            //yield return turnAwayMisfortuneFeat;

            //TrueFeat breachedDefensesFeat = new TrueFeat(ThaumaturgeFeatNames.BreachedDefenses, 4, "You can find the one weak point in a creature's scales, wards, or armor to get past its resistances", "When you succeed at Exploit Vulnerability, you learn about the highest of the creature's resistances that can be bypassed (for example, if the creature has resistance to physical damage except silver), if the creature has one. If you prefer, you can choose the following benefit instead of one of the usual two benefits from Exploit Vulnerability.\n\n{b}Breached Defenses{/b} You can choose this benefit only if you succeeded at Exploit Vulnerability and learned the creature has at least one resistance that can be bypassed. Choose one such resistance. Your unarmed and weapon Strikes bypass the chosen resistance", [ThaumaturgeTraits.Thaumaturge]);
            //// HACK: Will only work in V3
            //yield return breachedDefensesFeat;

            TrueFeat instructiveStrikeFeat = new TrueFeat(ThaumaturgeFeatNames.InstructiveStrike, 4, "You attack your foe and analyze how it reacts.", "{b}Requirements{/b} You are holding your implement\n\nMake a Strike. On a hit, you can immediately attempt a check to Exploit Vulnerability on the target. On a critical hit, you gain a +2 circumstance bonus to that check.", [ThaumaturgeTraits.Thaumaturge]);
            instructiveStrikeFeat.WithActionCost(1);
            AddInstructiveStrikeLogic(instructiveStrikeFeat);
            yield return instructiveStrikeFeat;

            TrueFeat lingeringPainStrikeFeat = new TrueFeat(ThaumaturgeFeatNames.LingeringPainStrike, 4, "You attack with in such a way that your foes feel faint afterwards.", "{b}Requirements{/b} You are holding your implement\n\nMake a Strike. On a hit, the target also rolls a Fortitude save against your DC, and is sickened 1 on a failure.", [ThaumaturgeTraits.Thaumaturge, Trait.Homebrew]);
            lingeringPainStrikeFeat.WithActionCost(2);
            AddLingeringPainStrikeLogic(lingeringPainStrikeFeat);
            yield return lingeringPainStrikeFeat;

            TrueFeat oneMoreActivationFeat = new TrueFeat(ThaumaturgeFeatNames.OneMoreActivation, 6, "You've forged a deeper bond to your invested items, allowing you to activate them more than usual.", "Once each day, you can activate an non-consumable item that has already been used up.", [ThaumaturgeTraits.Thaumaturge]);
            AddOneMoreActivationLogic(oneMoreActivationFeat);
            yield return oneMoreActivationFeat;

            TrueFeat scrollEsotericaFeat = new TrueFeat(ThaumaturgeFeatNames.ScrollEsoterica, 6, "Your esoterica includes scraps of scriptures, magic tomes, druidic markings, and the like, which you can use to create temporary scrolls.", "Once each day, you can create a scroll you used this combat in the same hand you are holding your implement or your open hand. This scroll is temporary and will only last this encounter.", [ThaumaturgeTraits.Thaumaturge]);
            scrollEsotericaFeat.WithPrerequisite(ThaumaturgeFeatNames.ScrollThaumaturgy, "Requires Scroll Thaumaturgy");
            AddScrollEsotericaLogic(scrollEsotericaFeat);
            yield return scrollEsotericaFeat;

            TrueFeat sympatheticVulnerabilitiesFeat = new TrueFeat(ThaumaturgeFeatNames.SympatheticVulnerabilities, 6, "When you apply your will to invoke a vulnerability, the result is more powerful, and the vulnerability ripples out in a web from your main target to affect a broader range of creatures.", "When you Exploit Vulnerability and target a mortal weakness, your Strikes trigger against any creature with that weakness. If you Exploit Vulnerability and apply a personal antithesis, your strikes apply that custom weakness to any other creatures of the exact same type.", [ThaumaturgeTraits.Thaumaturge]);
            AddSympatheticVulnerabilitiesLogic(sympatheticVulnerabilitiesFeat);
            yield return sympatheticVulnerabilitiesFeat;

            TrueFeat profaneInsightFeat = new TrueFeat(ThaumaturgeFeatNames.ProfaneInsight, 6, "Your profound insight into your enemies means not just a blade can cut deep.", "You have a +2 circumstance bonus to checks to Demoralize the target of your Exploit Vulnerability.\n\nYou can repeatedly Demoalize the target of your Exploit Vulnerability during the same combat.", [ThaumaturgeTraits.Thaumaturge, Trait.Homebrew]);
            AddProfaneInsightLogic(profaneInsightFeat);
            yield return profaneInsightFeat;

            TrueFeat cursedEffigyFeat = new TrueFeat(ThaumaturgeFeatNames.CursedEffigy, 8, "After your attack, you grab a bit of blood, cut hair, or other piece of the creature's body.", "{b}Requirements{/b} You have successful hit the target of your Exploit Vulnerability with a strike this turn.\n\nThe target takes a -2 status penalty to all saving throws from you, as long as they remain a target of your Exploit Vulnerability.", [ThaumaturgeTraits.Thaumaturge]);
            cursedEffigyFeat.WithActionCost(1);
            AddCursedEffigyLogic(cursedEffigyFeat);
            yield return cursedEffigyFeat;

            TrueFeat knowItAllFeat = new TrueFeat(ThaumaturgeFeatNames.KnowitAll, 8, "You are a know it all, especially when it pertains to your potential enemies.", "The DC of your Exploit Vulnerability checks is calcaulted as if the creature was one level lower.", [ThaumaturgeTraits.Thaumaturge, Trait.Homebrew]);
            AddKnowItAllLogic(knowItAllFeat);
            yield return knowItAllFeat;

            TrueFeat magicalExploitsFeat = new TrueFeat(ThaumaturgeFeatNames.MagicalExploits, 8, "Even with magic, you are learning how to exploit a targets weakness.", "Attack spells you cast will trigger mortal weakness or personal antithesis if they deal damage.", [ThaumaturgeTraits.Thaumaturge, Trait.Homebrew]);
            AddMagicalExploitsLogic(magicalExploitsFeat);
            yield return magicalExploitsFeat;
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
                    if (ThaumaturgeUtilities.IsCreatureHoldingAnyImplement(self.Owner))
                    {
                        return new ActionPossibility(ThaumaturgeUtilities.CreateExploitVulnerabilityAction(exploitVulnerabilityEffect.Owner));
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for swapping implements
        /// </summary>
        /// <param name="creature">The creature containing the logic</param>
        public static void AddSecondImplementLogic(Creature creature)
        {
            creature.AddQEffect(new QEffect("Second Implement", "Swapping an Implement with another Implement is a free action, and Implement reactions prompt swapping.")
            {
                ProvideActionIntoPossibilitySection = (QEffect posibilityGeneration, PossibilitySection posibilitySection) =>
                {
                    Creature owner = posibilityGeneration.Owner;
                    List<Item> heldImplements = owner.HeldItems.Where(item => item.HasTrait(ThaumaturgeTraits.Implement)).ToList();
                    if (heldImplements.Count > 0 && posibilitySection.PossibilitySectionId == PossibilitySectionId.ItemActions)
                    {
                        Possibility? inventoryPossibility = posibilitySection.Possibilities.FirstOrDefault(possibility => possibility.Caption.ToLower() == "inventory");
                        if (inventoryPossibility != null && inventoryPossibility is SubmenuPossibility invetorySubMenu)
                        {
                            foreach (Item heldImplement in heldImplements)
                            {
                                int index = (owner.HeldItems[0] == heldImplement) ? 0 : 1;
                                var itemPossibility = invetorySubMenu.Subsections[0].Possibilities[index];
                                if (itemPossibility is SubmenuPossibility subItemMenu)
                                {
                                    PossibilitySection? replacePossibilitySection = subItemMenu.Subsections.FirstOrDefault(subSection => subSection.Name.ToLower().Contains("replace"));
                                    if (replacePossibilitySection != null)
                                    {
                                        foreach (Possibility possibility in replacePossibilitySection.Possibilities)
                                        {
                                            if (possibility is ActionPossibility actionPossibility)
                                            {
                                                CombatAction replaceAction = actionPossibility.CombatAction;
                                                if (replaceAction.Item != null && replaceAction.Item.HasTrait(ThaumaturgeTraits.Implement))
                                                {

                                                    replaceAction.WithActionCost(0);
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }

                    return null;
                },
            });
        }

        /// <summary>
        /// Adds the logic for the Amulet Implement feature
        /// </summary>
        /// <param name="amuletImplementFeat">The Amulet Implement feat object</param>
        public static void AddAmuletImplementLogic(Feat amuletImplementFeat)
        {
            AddImplementEnsureLogic(amuletImplementFeat);

            // Adds the reaction prompt to all allies
            amuletImplementFeat.WithPermanentQEffect(ImplementDetails.AmuletInitiateBenefitName + " - Resistance to damage equal to 2 + your level", delegate (QEffect self)
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
                                    if (exploitEffect.Tag != null && exploitEffect.Tag is Creature thaumaturge)
                                    {
                                        bool holdingAmulet = ThaumaturgeUtilities.AnyHeldImplementsMatchID(Enums.ImplementIDs.Amulet, owner);
                                        bool hasAmuletAdept = owner.HasFeat(ThaumaturgeFeatNames.AmuletAdept);
                                        if (thaumaturge == owner && owner.Actions.CanTakeReaction() && ThaumaturgeUtilities.IsCreatureHoldingOrCarryingImplement(Enums.ImplementIDs.Amulet, owner) && ally.DistanceTo(owner) <= 3 && await owner.AskToUseReaction((holdingAmulet ? "Use " : "Swap to Amulet to use ") + ImplementDetails.AmuletInitiateBenefitName + " to give resistance equal to " + (2 + owner.Level) + " against this attack?" + (hasAmuletAdept ? $" They will also gain 5 damage resistence to {damage.Kind} until the start of your turn." : string.Empty)))
                                        {
                                            if (await ThaumaturgeUtilities.HeldImplementOrSwap(Enums.ImplementIDs.Amulet, owner, " use Amulet reaction"))
                                            {
                                                if (owner.HasFeat(ThaumaturgeFeatNames.AmuletAdept))
                                                {
                                                    ally.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtStartOfSourcesTurn)
                                                    {
                                                        Source = owner,
                                                        Name = "Amulet Adept Resistence",
                                                        Description = $"5 damage resistence to {damage.Kind}",
                                                        Illustration = ThaumaturgeModdedIllustrations.Amulet,
                                                        StateCheck = (QEffect qfStateCheck) =>
                                                        {
                                                            qfStateCheck.Owner.WeaknessAndResistance.AddResistance(damage.Kind, 5);
                                                        },
                                                    });
                                                }

                                                return new ReduceDamageModification(2 + owner.Level, "Amulet's Abeyance");
                                            }
                                        }
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
        /// Adds the logic for the Amulet Adept feature
        /// </summary>
        /// <param name="amuletAdeptFeat">The Amulet Adept feat object</param>
        public static void AddAmuletAdeptLogic(Feat amuletAdeptFeat)
        {
            amuletAdeptFeat.WithPermanentQEffect("Give 5 damage resistence when using the amulet's reaction.", delegate (QEffect self)
            {
            });
        }

        /// <summary>
        /// Adds the logic for the Bell Implement feature
        /// </summary>
        /// <param name="bellImplementFeat">The Bell Implement feat object</param>
        public static void AddBellImplementLogic(Feat bellImplementFeat)
        {
            AddImplementEnsureLogic(bellImplementFeat);

            // Adds the bell reaction
            bellImplementFeat.WithPermanentQEffect(ImplementDetails.BellInitiateBenefitName + " - Target saves or gains an effect", delegate (QEffect self)
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
                                    bool holdingBell = ThaumaturgeUtilities.AnyHeldImplementsMatchID(Enums.ImplementIDs.Bell, owner);
                                    if (exploitEffect.Tag != null && exploitEffect.Tag is Creature thaumaturge && thaumaturge == owner && owner.Actions.CanTakeReaction() && ThaumaturgeUtilities.IsCreatureHoldingOrCarryingImplement(Enums.ImplementIDs.Bell, owner) && owner.DistanceTo(action.Owner) <= 6 && await owner.AskToUseReaction((holdingBell ? "Use " : "Swap to Bell to use ") + ImplementDetails.BellInitiateBenefitName + ": " + (actionIsSpell ? " Target makes Fortitude save or becomes stupefied?" : " Target makes Will save or becomes your choice of enfeebled or clumsy?")))
                                    {
                                        if (await ThaumaturgeUtilities.HeldImplementOrSwap(Enums.ImplementIDs.Bell, owner, " use Bell reaction"))
                                        {
                                            CheckResult savingThrowResult = CommonSpellEffects.RollSavingThrow(action.Owner, new CombatAction(owner, ThaumaturgeModdedIllustrations.Bell, ImplementDetails.BellInitiateBenefitName, [Trait.Auditory, Trait.Emotion, Trait.Enchantment, Trait.Magical, Trait.Manipulate, Trait.Mental, ThaumaturgeTraits.Thaumaturge], ImplementDetails.BellInitiateBenefitRulesText, Target.Touch()), actionIsSpell ? Defense.Fortitude : Defense.Will, ThaumaturgeUtilities.CalculateClassDC(owner, ThaumaturgeTraits.Thaumaturge));
                                            if (savingThrowResult <= CheckResult.Failure)
                                            {
                                                QEffect bellEffect = new QEffect(ExpirationCondition.CountsDownAtStartOfSourcesTurn);
                                                bellEffect.Source = owner;
                                                bellEffect.Value = owner.HasFeat(ThaumaturgeFeatNames.BellAdept) ? 3 : 1;
                                                if (actionIsSpell)
                                                {
                                                    bellEffect.StateCheck = (QEffect qfStateCheck) =>
                                                    {
                                                        action.Owner.AddQEffect(QEffect.Stupefied(savingThrowResult == CheckResult.CriticalFailure ? 2 : 1).WithExpirationEphemeral());
                                                    };
                                                }
                                                else
                                                {
                                                    int debuffLevel = savingThrowResult == CheckResult.CriticalFailure ? 2 : 1;
                                                    ChoiceButtonOption userResponse = await owner.AskForChoiceAmongButtons(ThaumaturgeModdedIllustrations.Bell, "Add Enfeebled " + debuffLevel + " or Clumsy " + debuffLevel + " to " + action.Owner.Name, ["Enfeebled " + debuffLevel, "Clumsy " + debuffLevel]);
                                                    bellEffect.StateCheck = (QEffect qfStateCheck) =>
                                                    {
                                                        action.Owner.AddQEffect((userResponse.Index == 0) ? QEffect.Enfeebled(debuffLevel).WithExpirationEphemeral() : QEffect.Clumsy(debuffLevel).WithExpirationEphemeral());
                                                    };
                                                }
                                                owner.AddQEffect(bellEffect);
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
        /// Adds the logic for the Bell Adept feature
        /// </summary>
        /// <param name="bellAdeptFeat">The Bell Adept feat object</param>
        public static void AddBellAdeptLogic(Feat bellAdeptFeat)
        {
            bellAdeptFeat.WithPermanentQEffect("Conditions from Ring Bell last 3 rounds.", delegate (QEffect self)
            {
            });
        }

        /// <summary>
        /// Adds the logic for the Chalice Implement feature
        /// </summary>
        /// <param name="chaliceImplementFeat">The Chalice Implement feat object</param>
        public static void AddChaliceImplementLogic(Feat chaliceImplementFeat)
        {
            AddImplementEnsureLogic(chaliceImplementFeat);
            chaliceImplementFeat.WithPermanentQEffect(ImplementDetails.ChaliceInitiateBenefitName + " - Sip to gain Temp HP and Drain to Heal", delegate (QEffect self)
            {
                self.ProvideActionIntoPossibilitySection = (QEffect chaliceImplementEffect, PossibilitySection possibilitySection) =>
                {
                    if (possibilitySection.PossibilitySectionId == PossibilitySectionId.MainActions)
                    {
                        PossibilitySection chaliceSection = new PossibilitySection("Chalice Possibilities");

                        Illustration chaliceIllustrationName = ThaumaturgeModdedIllustrations.Chalice;
                        List<Trait> chaliceTraits = [Trait.Magical, Trait.Manipulate, Trait.Necromancy, Trait.Basic, ThaumaturgeTraits.Thaumaturge];

                        bool holdingChalice = ThaumaturgeUtilities.AnyHeldImplementsMatchID(Enums.ImplementIDs.Chalice, self.Owner);
                        bool hasAdeptChalice = self.Owner.HasFeat(ThaumaturgeFeatNames.ChaliceAdept);
                        CombatAction sipAction = new CombatAction(self.Owner, chaliceIllustrationName, "Sip", chaliceTraits.ToArray(), (holdingChalice ? string.Empty : "{b}{Red}Swap to Chalice{/Red}{/b}\n\n") + string.Format(ImplementDetails.ChaliceInitiateBenefitSipUnformattedText, (2 + self.Owner.Level)) + (hasAdeptChalice ? "\n\nIf the target has taken critical Piercing or Slashing damage or taken persistent damage within 30 feet of you since their last turn, you instead give {Blue}" + (2 + self.Owner.Abilities.Charisma + self.Owner.Level) + "{/Blue} temporary HP."  : string.Empty), Target.AdjacentCreatureOrSelf()
                            .WithAdditionalConditionOnTargetCreature((Creature user, Creature target) =>
                            {
                                if (user.QEffects.Any(qe => qe.Name == "Chalice Used this Round"))
                                {
                                    return Usability.NotUsable("Already used this round.");
                                }
                                else if (!ThaumaturgeUtilities.IsCreatureHoldingOrCarryingImplement(Enums.ImplementIDs.Chalice, self.Owner))
                                {
                                    return Usability.NotUsable("Not weilding Implement.");
                                }
                                return Usability.Usable;
                            }));
                        sipAction.WithActionCost(1);
                        sipAction.WithEffectOnChosenTargets(async delegate (Creature user, ChosenTargets targets)
                        {
                            if (await ThaumaturgeUtilities.HeldImplementOrSwap(Enums.ImplementIDs.Chalice, user, " to sip from the Chalice"))
                            {
                                Creature? target = targets.ChosenCreature;
                                if (target != null)
                                {
                                    user.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtStartOfYourTurn)
                                    {
                                        Name = "Chalice Used this Round"
                                    });
                                    int tempHPAmount = (target.HasEffect(ThaumaturgeQEIDs.AdeptChaliceBuff)) ? 2 + user.Abilities.Charisma + user.Level : 2 + (int)(Math.Floor(user.Level / 2.0));
                                    target.GainTemporaryHP(tempHPAmount);
                                }
                            }
                        });

                        CombatAction drainAction = new CombatAction(self.Owner, chaliceIllustrationName, "Drain", (chaliceTraits.Concat([Trait.Healing, Trait.Positive])).ToArray(), (holdingChalice ? string.Empty : "{b}{Red}Swap to Chalice{/Red}{/b}\n\n") + string.Format(ImplementDetails.ChaliceInitiateBenefitDrainUnformattedText, 3 * self.Owner.Level) + (hasAdeptChalice ? "\n\nIf the target has taken critical Piercing or Slashing damage or taken persistent damage within 30 feet of you since their last turn, you instead heal {Blue}" + (5 * self.Owner.Level) + "{/Blue} HP." : string.Empty), Target.AdjacentCreatureOrSelf()
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
                                else if (!ThaumaturgeUtilities.IsCreatureHoldingOrCarryingImplement(Enums.ImplementIDs.Chalice, self.Owner))
                                {
                                    return Usability.NotUsable("Not weilding Implement.");
                                }
                                return Usability.Usable;
                            }));
                        drainAction.WithActionCost(1);
                        drainAction.WithEffectOnChosenTargets(async delegate (Creature user, ChosenTargets targets)
                        {
                            if (await ThaumaturgeUtilities.HeldImplementOrSwap(Enums.ImplementIDs.Chalice, user, " to drain the Chalice"))
                            {
                                Creature? target = targets.ChosenCreature;
                                if (target != null)
                                {
                                    user.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtStartOfYourTurn)
                                    {
                                        Name = "Chalice Used this Round"
                                    });
                                    user.AddQEffect(new QEffect(ExpirationCondition.Never)
                                    {
                                        Name = "Chalice is Drained"
                                    });
                                    int hpToHeal = (target.HasEffect(ThaumaturgeQEIDs.AdeptChaliceBuff)) ? 5 * user.Level : 3 * user.Level;
                                    await target.HealAsync("" + hpToHeal, drainAction);
                                }
                            }
                        });

                        ActionPossibility sipActionPossibility = new ActionPossibility(sipAction);
                        chaliceSection.AddPossibility(sipActionPossibility);
                        ActionPossibility drainActionPossibility = new ActionPossibility(drainAction);
                        chaliceSection.AddPossibility(drainActionPossibility);

                        SubmenuPossibility chaliceMenu = new SubmenuPossibility(ThaumaturgeModdedIllustrations.Chalice, ImplementDetails.ChaliceInitiateBenefitName);
                        chaliceMenu.Subsections.Add(chaliceSection);
                        return chaliceMenu;
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Chalice Adept feature
        /// </summary>
        /// <param name="chaliceAdeptFeat">The Chalice Adept feat object</param>
        public static void AddChaliceAdeptLogic(Feat chaliceAdeptFeat)
        {
            chaliceAdeptFeat.WithPermanentQEffect("Blood empowers the healing powers of the chalice", delegate (QEffect self)
            {
                self.StateCheck = async (QEffect qfStateCheck) =>
                {
                    Creature owner = qfStateCheck.Owner;
                    foreach (Creature ally in owner.Battle.AllCreatures.Where(creature => owner.FriendOf(creature) && !creature.HasEffect(ThaumaturgeQEIDs.AdeptChaliceTracker)))
                    {
                        ally.AddQEffect(new QEffect()
                        {
                            Id = ThaumaturgeQEIDs.AdeptChaliceTracker,
                            AfterYouTakeDamageOfKind = async (QEffect youTakeDamage, CombatAction? action, DamageKind damageKind) =>
                            {
                                Creature damageTaker = youTakeDamage.Owner;
                                if (action != null && action.CheckResult == CheckResult.CriticalSuccess && !damageTaker.HasEffect(ThaumaturgeQEIDs.AdeptChaliceBuff) && damageTaker.HasLineOfEffectTo(owner.Occupies) < CoverKind.Blocked && damageTaker.DistanceTo(owner) <= 6 && (damageKind == DamageKind.Piercing || damageKind == DamageKind.Slashing))
                                {
                                    damageTaker.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtEndOfYourTurn)
                                    {
                                        Id = ThaumaturgeQEIDs.AdeptChaliceBuff,
                                        Name = "Adept Chalice Blood Bonus",
                                        Description = "Adding temp HP when sipping the chalice, or additional healing when draining",
                                        Illustration = ThaumaturgeModdedIllustrations.Chalice,
                                        CannotExpireThisTurn = true
                                    });
                                }
                            },
                            YouAcquireQEffect = (QEffect acquireEffect, QEffect effectAdded) =>
                            {
                                if (effectAdded.Id == QEffectId.PersistentDamage && effectAdded.GetPersistentDamageKind() == DamageKind.Bleed)
                                {
                                    Creature bleeder = acquireEffect.Owner;
                                    if (!bleeder.HasEffect(ThaumaturgeQEIDs.AdeptChaliceBuff) && bleeder.HasLineOfEffectTo(owner.Occupies) < CoverKind.Blocked && bleeder.DistanceTo(owner) <= 6)
                                    {
                                        bleeder.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtEndOfYourTurn)
                                        {
                                            Id = ThaumaturgeQEIDs.AdeptChaliceBuff,
                                            Name = "Adept Chalice Blood Bonus",
                                            Description = "Adding temp HP when sipping the chalice, or additional healing when draining",
                                            Illustration = ThaumaturgeModdedIllustrations.Chalice,
                                            CannotExpireThisTurn = true
                                        });
                                    }
                                }

                                return effectAdded;
                            }
                        });
                    }
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
            lanternImplementFeat.WithPermanentQEffect("Lantern Initiate Benefit" + " - Passive improved seeking", delegate (QEffect self)
            {
                self.BonusToAttackRolls = (QEffect bonusToSeek, CombatAction action, Creature? creature) =>
                {
                    if (action.ActionId == ActionId.Seek && ThaumaturgeUtilities.AnyHeldImplementsMatchID(Enums.ImplementIDs.Lantern, action.Owner))
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
                    if (ThaumaturgeUtilities.AnyHeldImplementsMatchID(Enums.ImplementIDs.Lantern, owner))
                    {
                        bool hasLanternAdept = owner.HasFeat(ThaumaturgeFeatNames.LanternAdept);
                        if (hasLanternAdept)
                        {
                            foreach (Creature invisibleCreature in owner.Battle.AllCreatures.Where(creature => creature.HasEffect(QEffectId.Invisible) && owner.HasLineOfEffectTo(creature.Occupies) < CoverKind.Blocked && owner.DistanceTo(creature) <= 6))
                            {
                                QEffect? invisibleEffect = invisibleCreature.QEffects.FirstOrDefault(qe => qe.Id == QEffectId.Invisible);
                                if (invisibleEffect != null && !invisibleCreature.QEffects.Any(qe => qe.Id == QEffectId.FaerieFire && qe.Name == "Lantern Adept"))
                                {
                                    QEffect lanternAdeptEffect = QEffect.FaerieFire("Lantern Adept", ThaumaturgeModdedIllustrations.GetIllustration(ImplementIDs.Lantern));
                                    lanternAdeptEffect.Description = "The latern is making this creature concealed instead of invisible. {i}(Everyone has an extra 20% miss chance against you.){/i}";
                                    lanternAdeptEffect.StateCheck = (QEffect stateCheck) =>
                                    {
                                        if (stateCheck.Owner.DistanceTo(owner) > 6 || !ThaumaturgeUtilities.AnyHeldImplementsMatchID(ImplementIDs.Lantern, owner))
                                        {
                                            lanternAdeptEffect.ExpiresAt = ExpirationCondition.Immediately;
                                        }
                                    };
                                    invisibleCreature.AddQEffect(lanternAdeptEffect);
                                }
                            }
                        }

                        QEffect? lanternSearchingEffect = owner.FindQEffect(ThaumaturgeQEIDs.LanternSearching);
                        QEffect? locationTrackingEffect = owner.FindQEffect(ThaumaturgeQEIDs.LocationTracking);
                        if (locationTrackingEffect != null && (locationTrackingEffect.Tag == null || (locationTrackingEffect.Tag is Tile lastTile && lastTile != owner.Occupies)) && lanternSearchingEffect != null && lanternSearchingEffect.Tag != null && lanternSearchingEffect.Tag is List<Tile> searchedTiles && ThaumaturgeUtilities.AnyHeldImplementsMatchID(Enums.ImplementIDs.Lantern, owner))
                        {
                            locationTrackingEffect.Tag = owner.Occupies;
                            Tile[] tilesToSearch = owner.Battle.Map.AllTiles.Where(tile => tile.DistanceTo(owner.Occupies) <= (hasLanternAdept ? 6 : 4) && !searchedTiles.Contains(tile)).ToArray();
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
                    }
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Lantern Adept feature
        /// </summary>
        /// <param name="lanternAdeptFeat">The Lantern Adept feat object</param>
        public static void AddLanternAdeptLogic(Feat lanternAdeptFeat)
        {
            lanternAdeptFeat.WithPermanentQEffect("Invisible creatures within the lantern's light are concealed instead.", delegate (QEffect self)
            {
            });
        }

        /// <summary>
        /// Adds the logic for the Mirror Implement feature
        /// </summary>
        /// <param name="mirrorImplementFeat">The Mirror Implement feat object</param>
        public static void AddMirrorImplementLogic(Feat mirrorImplementFeat)
        {
            AddImplementEnsureLogic(mirrorImplementFeat);
            mirrorImplementFeat.WithPermanentQEffect(ImplementDetails.MirrorInitiateBenefitName + " - Make an illusory image of yourself", delegate (QEffect self)
            {
                void SwapWithClone(Creature selfCreature, Creature pairedCreature)
                {
                    selfCreature.SwapPositions(pairedCreature);

                    // Logic for Grabbed, Grappled, and Restrained
                    foreach (QEffect qEffect in selfCreature.QEffects.Where(qe => qe.Id == QEffectId.Grabbed || qe.Id == QEffectId.Grappled || qe.Id == QEffectId.Restrained))
                    {
                        qEffect.Owner = pairedCreature;
                        pairedCreature.AddQEffect(qEffect);
                    }
                    foreach (QEffect qEffect in pairedCreature.QEffects.Where(qe => qe.Id == QEffectId.Grabbed || qe.Id == QEffectId.Grappled || qe.Id == QEffectId.Restrained))
                    {
                        qEffect.Owner = selfCreature;
                        selfCreature.AddQEffect(qEffect);
                    }
                    foreach (Creature enemy in selfCreature.Battle.AllCreatures.Where(creature => !creature.FriendOfAndNotSelf(selfCreature)))
                    {
                        List<QEffect> qEffectsToChange = enemy.QEffects.Where(qe => (qe.Source == selfCreature || qe.Source == pairedCreature) && (qe.Id == QEffectId.Grabbed || qe.Id == QEffectId.Grappled || qe.Id == QEffectId.Restrained)).ToList();
                        foreach (QEffect qEffect in qEffectsToChange)
                        {
                            qEffect.Source = (qEffect.Source == selfCreature) ? pairedCreature : selfCreature;
                        }
                    }
                }

                self.ProvideMainAction = (QEffect mirrorsReflectionEffect) =>
                {
                    Creature owner = mirrorsReflectionEffect.Owner;
                    if (!ThaumaturgeUtilities.IsCreatureHoldingOrCarryingImplement(Enums.ImplementIDs.Mirror, owner))
                    {
                        return null;
                    }

                    bool holdingMirror = ThaumaturgeUtilities.AnyHeldImplementsMatchID(Enums.ImplementIDs.Mirror, owner);

                    return new ActionPossibility(new CombatAction(owner, ThaumaturgeModdedIllustrations.Mirror, ImplementDetails.MirrorInitiateBenefitName, [Trait.Illusion, Trait.Magical, Trait.Manipulate, Trait.Basic, ThaumaturgeTraits.Thaumaturge], (holdingMirror ? string.Empty : "{b}{Red}Swap to Mirror{/Red}{/b}\n\n") + ImplementDetails.MirrorInitiateBenefitRulesText, Target.Tile((creature, tile) => tile.LooksFreeTo(creature) && creature.Occupies != null && creature.DistanceTo(tile) <= 3, (creature, tile) => (float)int.MinValue))
                        .WithActionCost(1)
                        .WithEffectOnChosenTargets(async delegate (Creature attacker, ChosenTargets targets)
                        {
                            if (await ThaumaturgeUtilities.HeldImplementOrSwap(Enums.ImplementIDs.Mirror, attacker, " to use " + ImplementDetails.MirrorInitiateBenefitName))
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

                                    if (owner.HasFeat(ThaumaturgeFeatNames.MirrorAdept))
                                    {
                                        owner.AddQEffect(new QEffect()
                                        {
                                            AfterYouTakeDamage = async (QEffect qeffect, int amount, DamageKind kind, CombatAction? action, bool critical) =>
                                            {
                                                if (action != null && action.ChosenTargets.ChosenCreature != null && action.Owner != null && owner.EnemyOf(action.Owner) && action.Owner.IsAdjacentTo(action.ChosenTargets.ChosenCreature) && await owner.AskForConfirmation(ThaumaturgeModdedIllustrations.GetIllustration(ImplementIDs.Mirror), $"Shatter {qeffect.Owner} dealing {{Blue}}{2 + (int)(Math.Floor(owner.Level / 2.0))}{{/Blue}} slashing damage in a 5-foot emanation around {qeffect.Owner}?", "Yes"))
                                                {
                                                    CombatAction mirrorShatter = new CombatAction(action.ChosenTargets.ChosenCreature, IllustrationName.GenericCombatManeuver, "Mirror Shatter", [], "Shatter the mirror to deal slashing damage.", Target.SelfExcludingEmanation(1))
                                                    .WithActionCost(0)
                                                    .WithEffectOnEachTarget(async (CombatAction action, Creature attacker, Creature defender, CheckResult result) =>
                                                    {
                                                        if (defender != owner && defender != mirrorClone)
                                                        {
                                                            await CommonSpellEffects.DealDirectDamage(action, DiceFormula.FromText("" + (2 + (int)(Math.Floor(attacker.Level / 2.0)))), defender, result, DamageKind.Slashing);
                                                        }
                                                    });

                                                    EmanationTarget emanationTarget = (EmanationTarget)mirrorShatter.Target;
                                                    AreaSelection areaSelection = Areas.DetermineTiles(emanationTarget);
                                                    mirrorShatter.ChosenTargets.SetFromArea(emanationTarget, areaSelection?.TargetedTiles ?? new HashSet<Tile>());
                                                    await mirrorShatter.AllExecute();

                                                    if (owner == action.ChosenTargets.ChosenCreature)
                                                    {
                                                        SwapWithClone(owner, mirrorClone);
                                                    }

                                                    mirrorClone.RemoveAllQEffects(qe => qe.Id == ThaumaturgeQEIDs.MirrorTracking);
                                                    owner.Battle.RemoveCreatureFromGame(mirrorClone);
                                                    owner.RemoveAllQEffects(qe => qe.Id == ThaumaturgeQEIDs.MirrorTracking);
                                                }
                                            }
                                        });
                                    }

                                    foreach (QEffect effect in owner.QEffects)
                                    {
                                        mirrorClone.AddQEffect(new QEffectClone(effect));
                                    }

                                    foreach (Item item in owner.HeldItems)
                                    {
                                        Item mirrorItem = item.Duplicate();
                                        if (item.HasTrait(ThaumaturgeTraits.Implement))
                                        {
                                            mirrorItem.Traits.Add(ThaumaturgeTraits.Implement);
                                        }

                                        mirrorClone.HeldItems.Add(mirrorItem);

                                    }

                                    owner.Battle.SpawnCreature(mirrorClone, owner.OwningFaction, chosenTile);

                                    MirrorTrackingEffect ownersTrackingEffect = new MirrorTrackingEffect(owner, mirrorClone);
                                    MirrorTrackingEffect mirrorTrackingEffect = new MirrorTrackingEffect(mirrorClone, owner);

                                    owner.SubscribeToAll(mirrorTrackingEffect);
                                    mirrorClone.SubscribeToAll(ownersTrackingEffect);

                                    owner.AddQEffect(ownersTrackingEffect);
                                    mirrorClone.AddQEffect(mirrorTrackingEffect);
                                }
                            }
                        }));
                };
                self.ProvideActionIntoPossibilitySection = (QEffect swapToClone, PossibilitySection possibilitySection) =>
                {
                    Creature owner = swapToClone.Owner;
                    bool holdingMirror = ThaumaturgeUtilities.AnyHeldImplementsMatchID(Enums.ImplementIDs.Mirror, owner);
                    MirrorTrackingEffect? mirrorTracking = owner.FindQEffect(ThaumaturgeQEIDs.MirrorTracking) as MirrorTrackingEffect;
                    if (possibilitySection.PossibilitySectionId == PossibilitySectionId.MainActions && mirrorTracking != null)
                    {
                        if (!owner.Battle.AllCreatures.Contains(mirrorTracking.PairedCreature))
                        {
                            return null;
                        }

                        Creature pairedCreature = mirrorTracking.PairedCreature;
                        return new ActionPossibility(new CombatAction(owner, ThaumaturgeModdedIllustrations.Mirror, "Swap to Clone", [Trait.Basic, ThaumaturgeTraits.Thaumaturge], (holdingMirror ? string.Empty : "{b}{Red}Swap to Mirror{/Red}{/b}\n\n") + "Swaps to the clone, in which you can continue your turn.", Target.Self())
                            .WithActionCost(0)
                            .WithEffectOnSelf(async (Creature self) =>
                            {
                                if (await ThaumaturgeUtilities.HeldImplementOrSwap(Enums.ImplementIDs.Mirror, self, " to swap to clone"))
                                {
                                    SwapWithClone(self, pairedCreature);
                                }
                            }));
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Mirror Adept feature
        /// </summary>
        /// <param name="mirrorAdeptFeat">The Mirror Adept feat object</param>
        public static void AddMirrorAdeptLogic(Feat mirrorAdeptFeat)
        {
            mirrorAdeptFeat.WithPermanentQEffect("You may shatter your reflect when it takes damage.", delegate (QEffect self)
            {
            });
        }

        /// <summary>
        /// Adds the logic for the Regalia Implement feature
        /// </summary>
        /// <param name="regaliaImplementFeat">The Regalia Implement feat object</param>
        public static void AddRegaliaImplementLogic(Feat regaliaImplementFeat)
        {
            AddImplementEnsureLogic(regaliaImplementFeat);
            regaliaImplementFeat.WithPermanentQEffect(ImplementDetails.RegaliaInitiateBenefitName + " - Passively ward of fear", delegate (QEffect self)
            {
                self.StateCheck = async (QEffect stateCheck) =>
                {
                    Creature owner = stateCheck.Owner;
                    foreach (Creature ally in owner.Battle.AllCreatures.Where(creature => owner.FriendOf(creature) && !creature.HasEffect(ThaumaturgeQEIDs.AdeptRegaliaTracker)))
                    {
                        ally.AddQEffect(new QEffect(ExpirationCondition.Never)
                        {
                            Id = ThaumaturgeQEIDs.AdeptRegaliaTracker,
                            BonusToDefenses = (QEffect bonusToDefenses, CombatAction? action, Defense defense) =>
                            {
                                if (ThaumaturgeUtilities.AnyHeldImplementsMatchID(Enums.ImplementIDs.Regalia, owner) && defense == Defense.Will && action != null && (action.HasTrait(Trait.Fear) || (owner.HasFeat(ThaumaturgeFeatNames.RegaliaAdept) && action.HasTrait(Trait.Mental))) && ThaumaturgeUtilities.AnyHeldImplementsMatchID(Enums.ImplementIDs.Regalia, owner) && owner.HasLineOfEffectTo(ally.Occupies) < CoverKind.Blocked && ally.DistanceTo(owner) <= 3)
                                {

                                    return new Bonus(1, BonusType.Status, ImplementDetails.RegaliaInitiateBenefitName, true);
                                }

                                return null;
                            },
                            BonusToDamage = (QEffect bonusToDamage, CombatAction action, Creature defender) =>
                            {
                                if (owner.HasFeat(ThaumaturgeFeatNames.RegaliaAdept) && ThaumaturgeUtilities.AnyHeldImplementsMatchID(Enums.ImplementIDs.Regalia, owner) && owner.HasLineOfEffectTo(ally.Occupies) < CoverKind.Blocked && ally.DistanceTo(owner) <= 3)
                                {
                                    return new Bonus(2, BonusType.Status, "Regalia Implement", true);
                                }

                                return null;
                            }
                        });
                    }
                };
                self.EndOfYourTurnBeneficialEffect = async (QEffect endOfTurn, Creature self) =>
                {
                    if (ThaumaturgeUtilities.AnyHeldImplementsMatchID(Enums.ImplementIDs.Regalia, self))
                    {
                        foreach (Creature ally in self.Battle.AllCreatures.Where(creature => self.FriendOf(creature) && self.DistanceTo(creature) <= 3 && creature.HasEffect(QEffectId.Frightened)))
                        {
                            if (ThaumaturgeUtilities.AnyHeldImplementsMatchID(Enums.ImplementIDs.Regalia, self))
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
                    }
                };
                self.BonusToSkillChecks = (Skill skill, CombatAction action, Creature? target) =>
                {
                    if (ThaumaturgeUtilities.AnyHeldImplementsMatchID(Enums.ImplementIDs.Regalia, self.Owner) && (skill == Skill.Deception || skill == Skill.Diplomacy || skill == Skill.Intimidation))
                    {
                        int bonusValue = (self.Owner.HasFeat(ThaumaturgeFeatNames.RegaliaAdept) && self.Owner.Proficiencies.Get(Skills.SkillToTrait(skill)) >= Proficiency.Master) ? 2 : 1;
                        return new Bonus(bonusValue, BonusType.Circumstance, ImplementDetails.RegaliaInitiateBenefitName, true);
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Regalia Adept feature
        /// </summary>
        /// <param name="regaliaAdeptFeat">The Regalia Adept feat object</param>
        public static void AddRegaliaAdeptLogic(Feat regaliaAdeptFeat)
        {
            regaliaAdeptFeat.WithPermanentQEffect("Your regalia bonuses are increased and you give a +2 status bonus to damage.", delegate (QEffect self)
            {
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
                sheet.AddSelectionOption(new SingleFeatSelectionOption("Tome First Extra Skill", "Tome First Extra Skill", 1, (feat => feat is SkillSelectionFeat)));
                sheet.AddSelectionOption(new SingleFeatSelectionOption("Tome Second Extra Skill", "Tome Second Extra Skill", 1, (feat => feat is SkillSelectionFeat)));
                sheet.AddSkillIncreaseOption(3);
                sheet.AddSkillIncreaseOption(5);
            };
            tomeImplementFeat.WithPermanentQEffect(ImplementDetails.TomeInitiateBenefitName + " - Improved Exploit Vulnerability and extra skills", delegate (QEffect self) {
            });
        }

        /// <summary>
        /// Adds the logic for the Tome Adept feature
        /// </summary>
        /// <param name="tomeAdeptFeat">The Tome Adept feat object</param>
        public static void AddTomeAdeptLogic(Feat tomeAdeptFeat)
        {
            tomeAdeptFeat.OnSheet = (CalculatedCharacterSheetValues sheet) =>
            {
                sheet.AddSkillIncreaseOption(7);
            };
            tomeAdeptFeat.WithPermanentQEffect("You may exploit vulnerability at the start of your turn as a free action, and if you succeed you gain a +1 circumstance bonus to attack rolls against them.", delegate (QEffect self)
            {
                self.StartOfYourPrimaryTurn = async (QEffect qfStartOfTurn, Creature owner) =>
                {
                    if (ThaumaturgeUtilities.AnyHeldImplementsMatchID(ImplementIDs.Tome, owner))
                    {
                        CombatAction exploitAction = ThaumaturgeUtilities.CreateExploitVulnerabilityAction(owner).WithActionCost(0);
                        await owner.Battle.GameLoop.FullCast(exploitAction);
                        Creature? exploitTarget = exploitAction.ChosenTargets.ChosenCreature;
                        if (!exploitAction.RevertRequested && exploitAction.CheckResult >= CheckResult.Success && exploitTarget != null)
                        {
                            owner.AddQEffect(new QEffect("Adept Tome", $"+1 circumstance bonus to your next attack roll against {exploitTarget.Name}")
                            {
                                ExpiresAt = ExpirationCondition.ExpiresAtStartOfYourTurn,
                                Illustration = ThaumaturgeModdedIllustrations.Tome,
                                BonusToAttackRolls = (QEffect qfBonusToAttack, CombatAction action, Creature? defender) =>
                                {
                                    if (defender != null && defender == exploitTarget && action.HasTrait(Trait.Attack))
                                    {
                                        return new Bonus(1, BonusType.Circumstance, "Tome Adept", true);
                                    }

                                    return null;
                                },
                                AfterYouTakeAction = async (QEffect qfAfterAction, CombatAction action) =>
                                {
                                    if (action.HasTrait(Trait.Attack) && action.ChosenTargets.Targets(exploitTarget))
                                    {
                                        qfAfterAction.ExpiresAt = ExpirationCondition.Immediately;
                                    }
                                }
                            });
                        }
                    }
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Wand Implement feature
        /// </summary>
        /// <param name="wandImplementFeat">The Wand Implement feat object</param>
        public static void AddWandImplementLogic(Feat wandImplementFeat)
        {
            wandImplementFeat.OnSheet = (CalculatedCharacterSheetValues sheet) =>
            {
                List<FeatName> wandFeatNames = new List<FeatName>() { ThaumaturgeFeatNames.ColdWand, ThaumaturgeFeatNames.ElectricityWand, ThaumaturgeFeatNames.FireWand } ;
                sheet.AddSelectionOption(new SingleFeatSelectionOption("Thaumaturge Wand Type", "Wand", sheet.CurrentLevel, feat => wandFeatNames.Contains(feat.FeatName)));
                ThaumaturgeUtilities.EnsureCorrectImplements(sheet);
            };
            wandImplementFeat.WithPermanentQEffect(ImplementDetails.WandInitiateBenefitName + " - Flind magic with your wand", delegate (QEffect self)
            {
                self.ProvideActionIntoPossibilitySection = (QEffect wandImplementEffect, PossibilitySection possibilitySection) =>
                {
                    if (possibilitySection.PossibilitySectionId == PossibilitySectionId.MainActions)
                    {
                        Creature owner = wandImplementEffect.Owner;
                        List<FeatName> wandTypesKnown = new List<FeatName>();
                        if (owner.HasFeat(ThaumaturgeFeatNames.ColdWand))
                        {
                            wandTypesKnown.Add(ThaumaturgeFeatNames.ColdWand);
                        }
                        if (owner.HasFeat(ThaumaturgeFeatNames.ElectricityWand))
                        {
                            wandTypesKnown.Add(ThaumaturgeFeatNames.ElectricityWand);
                        }
                        if (owner.HasFeat(ThaumaturgeFeatNames.FireWand))
                        {
                            wandTypesKnown.Add(ThaumaturgeFeatNames.FireWand);
                        }

                        SubmenuPossibility wandMenu = new SubmenuPossibility(ThaumaturgeModdedIllustrations.Wand, ImplementDetails.WandInitiateBenefitName);
                        foreach (FeatName wandType in wandTypesKnown)
                        {
                            void HandleWandAdeptEffect(DamageKind damageKind, Illustration wandIllustration, Creature attacker, Creature defender, CheckResult result)
                            {
                                if (attacker.HasFeat(ThaumaturgeFeatNames.WandAdept))
                                {
                                    QEffect adeptEffect = new QEffect();
                                    switch(damageKind)
                                    {
                                        case DamageKind.Cold:
                                            adeptEffect.Name = "Speed Debuff (Cold Adept Wand)";
                                            adeptEffect.Description = $"-10-foot status penalty untill the start of {attacker.Name}'s turn";
                                            adeptEffect.Source = attacker;
                                            adeptEffect.ExpiresAt = ExpirationCondition.ExpiresAtStartOfSourcesTurn;
                                            adeptEffect.BonusToAllSpeeds = (QEffect afSpeedDebuff) =>
                                            {
                                                return new Bonus(-2, BonusType.Status, "Adept Wand (Cold)", false);
                                            };
                                            break;
                                        case DamageKind.Electricity:
                                            adeptEffect = QEffect.FlatFooted("Electricity Adept Wand");
                                            adeptEffect.Name = "Flat-footed (Electricity Adept Wand)";
                                            adeptEffect.Description = $"You have a -2 circumstance penalty to AC until the end of your next turn.";
                                            adeptEffect.ExpiresAt = ExpirationCondition.ExpiresAtEndOfYourTurn;
                                            break;
                                        case DamageKind.Fire:
                                            string damage = (result == CheckResult.CriticalFailure) ? "2d10" : "1d10";
                                            adeptEffect = QEffect.PersistentDamage(DiceFormula.FromText(damage), DamageKind.Fire);
                                            break;
                                        default:
                                            break;
                                    }

                                    if (damageKind != DamageKind.Untyped)
                                    {
                                        adeptEffect.Illustration = wandIllustration;
                                        defender.AddQEffect(adeptEffect);
                                    }
                                }
                            }

                            DamageKind wandDamageKind = DamageKind.Untyped;
                            Trait? wandTraitForType = null;
                            Illustration wandIllustration = ThaumaturgeModdedIllustrations.Wand;
                            IllustrationName? projectileIllustration = null;
                            SfxName? wandSfx = null;

                            if (wandType == ThaumaturgeFeatNames.ColdWand)
                            {
                                wandDamageKind = DamageKind.Cold;
                                wandTraitForType = Trait.Cold;
                                wandSfx = SfxName.RayOfFrost;
                                wandIllustration = ThaumaturgeModdedIllustrations.WandCold;
                                projectileIllustration = IllustrationName.RayOfFrost;
                            }
                            else if (wandType == ThaumaturgeFeatNames.ElectricityWand)
                            {
                                wandDamageKind = DamageKind.Electricity;
                                wandTraitForType = Trait.Electricity;
                                wandSfx = SfxName.ElectricArc;
                                wandIllustration = ThaumaturgeModdedIllustrations.WandElectricity;
                                projectileIllustration = IllustrationName.ElectricArc;
                            }
                            else if (wandType == ThaumaturgeFeatNames.FireWand)
                            {
                                wandDamageKind = DamageKind.Fire;
                                wandTraitForType = Trait.Fire;
                                wandSfx = SfxName.FireRay;
                                wandIllustration = ThaumaturgeModdedIllustrations.WandFire;
                                projectileIllustration = IllustrationName.FireRay;
                            }

                            bool hasWandAdept = owner.HasFeat(ThaumaturgeFeatNames.WandAdept);
                            int wandRange = hasWandAdept ? 24 : 12;

                            PossibilitySection wandSection = new PossibilitySection($"{wandDamageKind.HumanizeTitleCase2()}");
                            List<Trait> wandTraits = [Trait.Concentrate, Trait.Evocation, Trait.Magical, Trait.Manipulate, Trait.Basic, ThaumaturgeTraits.Thaumaturge];
                            if (wandTraitForType != null)
                            {
                                wandTraits.Add((Trait)wandTraitForType);
                            }

                            bool holdingWand = ThaumaturgeUtilities.AnyHeldImplementsMatchID(Enums.ImplementIDs.Wand, owner);
                            CombatAction flingMagicAction = new CombatAction(self.Owner, wandIllustration, $"Fling Magic  ({wandDamageKind.HumanizeTitleCase2()})", wandTraits.ToArray(), (holdingWand ? string.Empty : "{b}{Red}Swap to Wand{/Red}{/b}\n\n") + ImplementDetails.WandInitiateBenefitRulesText, Target.RangedCreature(wandRange)
                                .WithAdditionalConditionOnTargetCreature((Creature user, Creature target) =>
                                {
                                    if (!ThaumaturgeUtilities.IsCreatureHoldingOrCarryingImplement(Enums.ImplementIDs.Wand, self.Owner))
                                    {
                                        return Usability.NotUsable("Not weilding Implement.");
                                    }
                                    return Usability.Usable;
                                }));
                            flingMagicAction.WithActionCost(2);
                            flingMagicAction.WithSavingThrow(new SavingThrow(Defense.Reflex, creature => ThaumaturgeUtilities.CalculateClassDC(owner, ThaumaturgeTraits.Thaumaturge)));
                            flingMagicAction.WithEffectOnEachTarget(async delegate (CombatAction action, Creature attacker, Creature defender, CheckResult result)
                            {
                                if (wandDamageKind != DamageKind.Untyped && await ThaumaturgeUtilities.HeldImplementOrSwap(Enums.ImplementIDs.Wand, attacker, " to Fling Magic"))
                                {
                                    int level = owner.Level;
                                    KindedDamage wandDamage = new KindedDamage(DiceFormula.FromText("" + (1 + (int)(Math.Floor((level - 1) / 2.0))) + "d4 + " + attacker.Abilities.Charisma, "Fling Magic"), (DamageKind)wandDamageKind);
                                    DamageEvent wandDamageEvent = new DamageEvent(action, defender, result, [wandDamage], result == CheckResult.CriticalFailure, result == CheckResult.Success);
                                    if (result <= CheckResult.Success)
                                    {
                                        await CommonSpellEffects.DealDirectDamage(wandDamageEvent);
                                        HandleWandAdeptEffect(wandDamageKind, wandIllustration, attacker, defender, result);
                                    }
                                }
                            });

                            CombatAction boostedFlingMagicAction = new CombatAction(self.Owner, wandIllustration, $"Boosted Fling Magic ({wandDamageKind.HumanizeTitleCase2()})", wandTraits.ToArray(), (holdingWand ? string.Empty : "{b}{Red}Swap to Wand{/Red}{/b}\n\n") + ImplementDetails.WandInitiateBenefitRulesText, Target.RangedCreature(wandRange)
                                .WithAdditionalConditionOnTargetCreature((Creature user, Creature target) =>
                                {
                                    if (owner.HasEffect(ThaumaturgeQEIDs.BoostedWandUsed))
                                    {
                                        int value = owner.GetQEffectValue(ThaumaturgeQEIDs.BoostedWandUsed);
                                        return Usability.NotUsable("Wand isn't charged yet. (" + value + " turns left)");
                                    }
                                    else if (!ThaumaturgeUtilities.IsCreatureHoldingOrCarryingImplement(Enums.ImplementIDs.Wand, self.Owner))
                                    {
                                        return Usability.NotUsable("Not weilding Implement.");
                                    }
                                    return Usability.Usable;
                                }));
                            boostedFlingMagicAction.WithActionCost(2);
                            boostedFlingMagicAction.WithSavingThrow(new SavingThrow(Defense.Reflex, creature => ThaumaturgeUtilities.CalculateClassDC(owner, ThaumaturgeTraits.Thaumaturge)));
                            boostedFlingMagicAction.WithEffectOnEachTarget(async delegate (CombatAction action, Creature attacker, Creature defender, CheckResult result)
                            {
                                if (wandDamageKind != DamageKind.Untyped && await ThaumaturgeUtilities.HeldImplementOrSwap(Enums.ImplementIDs.Wand, attacker, " to Boosted Fling Magic"))
                                {
                                    int level = owner.Level;
                                    KindedDamage wandDamage = new KindedDamage(DiceFormula.FromText("" + (1 + (int)(Math.Floor((level - 1) / 2.0))) + "d6 + " + attacker.Abilities.Charisma, "Boosted Fling Magic"), (DamageKind)wandDamageKind);
                                    DamageEvent wandDamageEvent = new DamageEvent(action, defender, result, [wandDamage], result == CheckResult.CriticalFailure, result == CheckResult.Success);
                                    if (result <= CheckResult.Success)
                                    {
                                        await CommonSpellEffects.DealDirectDamage(wandDamageEvent);
                                        HandleWandAdeptEffect(wandDamageKind, wandIllustration, attacker, defender, result);
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
                            wandMenu.Subsections.Add(wandSection);
                        }

                        return wandMenu;
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Wand Adept feature
        /// </summary>
        /// <param name="wandAdeptFeat">The Wand Adept feat object</param>
        public static void AddWandAdeptLogic(Feat wandAdeptFeat)
        {
            wandAdeptFeat.OnSheet = (CalculatedCharacterSheetValues sheet) =>
            {
                List<FeatName> wandFeatNames = new List<FeatName>() { ThaumaturgeFeatNames.ColdWand, ThaumaturgeFeatNames.ElectricityWand, ThaumaturgeFeatNames.FireWand };
                sheet.AddSelectionOption(new SingleFeatSelectionOption("Thaumaturge Adept Wand Type", "Adept Wand", sheet.CurrentLevel, feat => wandFeatNames.Contains(feat.FeatName)));
                ThaumaturgeUtilities.EnsureCorrectImplements(sheet);
            };
            wandAdeptFeat.WithPermanentQEffect("Creatures that fail their saves against your fling magic have an additiional effects.", delegate (QEffect self)
            {

            });
        }

        /// <summary>
        /// Adds the logic for the Weapon Implement feature
        /// </summary>
        /// <param name="weaponImplementFeat">The Weapon Implement feat object</param>
        public static void AddWeaponImplementLogic(Feat weaponImplementFeat)
        {
            AddImplementEnsureLogic(weaponImplementFeat);
            weaponImplementFeat.WithPermanentQEffect(ImplementDetails.WeaponInitiateBenefitName + " - Reactively Strike with your One-Hand weapon", delegate (QEffect self)
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
                        owner.AddQEffect(new QEffect("Implement's Interruption {icon:Reaction}", ImplementDetails.WeaponInitiateBenefitRulesText)
                        {
                            Id = QEffectId.AttackOfOpportunity,
                            WhenProvoked = async (QEffect aooEffect, CombatAction provokingAction) =>
                            {
                                Creature attacker = aooEffect.Owner;
                                Creature provoker = provokingAction.Owner;
                                if (attacker.PrimaryWeapon != null && attacker.QEffects.Any(qe => (qe is ExploitEffect exploitEffect && exploitEffect.Target.BaseName == provoker.BaseName) || (qe is QEffectClone clonedEffect && clonedEffect.OriginalEffect is ExploitEffect clonedExploitEffect && clonedExploitEffect.Target.BaseName == provoker.BaseName)))
                                {
                                    MirrorTrackingEffect? pairedCreatureEffect = (MirrorTrackingEffect?)attacker.QEffects.FirstOrDefault(qe => qe is MirrorTrackingEffect mirrorTrackingEffect);
                                    if (((attacker is not MirrorClone && attacker.Actions.CanTakeReaction()) || (pairedCreatureEffect != null && pairedCreatureEffect.PairedCreature.Actions.CanTakeReaction())) && await ThaumaturgeUtilities.HeldImplementOrSwap(Enums.ImplementIDs.Weapon, attacker, " since you have been provoked and can make an attack of opportunity", false))
                                    {
                                        CheckResult? strikeCheckResult = await CommonCombatActions.OfferAndMakeReactiveStrike(attacker, provoker, "{b}" + provoker.Name + "{/b} uses {b}" + provokingAction.Name + "{/b} which provokes.\nUse your reaction to make an attack of opportunity?", "*attack of opportunity*", 1);
                                        if (strikeCheckResult != null && strikeCheckResult == CheckResult.CriticalSuccess)
                                        {
                                            if (provokingAction.HasTrait(Trait.Manipulate))
                                            {
                                                provokingAction.Disrupted = true;
                                            }
                                        }
                                        else if (strikeCheckResult == CheckResult.Failure && ((attacker is not MirrorClone && attacker.HasFeat(ThaumaturgeFeatNames.WeaponAdept) || (pairedCreatureEffect != null && pairedCreatureEffect.PairedCreature.HasFeat(ThaumaturgeFeatNames.WeaponAdept)))) && attacker.PrimaryWeapon != null)
                                        {
                                            List<DamageKind> damageTypes = attacker.PrimaryWeapon.DetermineDamageKinds();
                                            QEffect? exploitEffect = attacker.QEffects.FirstOrDefault(qe => (qe is ExploitEffect exploitEffect && exploitEffect.Target.BaseName == provoker.BaseName) || (qe is QEffectClone clonedEffect && clonedEffect.OriginalEffect is ExploitEffect clonedExploitEffect && clonedExploitEffect.Target.BaseName == provoker.BaseName));

                                            if (exploitEffect != null && exploitEffect is ExploitEffect actualExploitEffect)
                                            {
                                                damageTypes.Add(actualExploitEffect.ExploitedDamageKind);
                                            }
                                            else if (exploitEffect != null && exploitEffect is QEffectClone clonedEffect && clonedEffect.OriginalEffect is ExploitEffect clonedExploitEffect)
                                            {
                                                damageTypes.Add(clonedExploitEffect.ExploitedDamageKind);
                                            }
                                            DamageKind damageKindToUse = provoker.WeaknessAndResistance.WhatDamageKindIsBestAgainstMe(damageTypes);
                                            await CommonSpellEffects.DealDirectSplashDamage(CombatAction.CreateSimple(attacker, "Weapon Adept"), DiceFormula.FromText("1"), provoker, damageKindToUse);
                                        }

                                        if (attacker is MirrorClone && pairedCreatureEffect != null)
                                        {
                                            pairedCreatureEffect.PairedCreature.Actions.UseUpReaction();
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
        /// Adds the logic for the Weapon Adept feature
        /// </summary>
        /// <param name="weaponAdeptFeat">The Weapon Adept feat object</param>
        public static void AddWeaponAdeptLogic(Feat weaponAdeptFeat)
        {
            weaponAdeptFeat.WithPermanentQEffect("You deal 1 damage when you miss your Attack of Opportunity (Possibly triggering weaknesses)", delegate (QEffect self)
            {
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
                    if (action.Item != null && action.Item.WeaponProperties != null && ThaumaturgeUtilities.IsCreatureHoldingAnyImplement(self.Owner))
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
            rootToLifeFeat.WithPermanentQEffect("Heal those that are dying", delegate (QEffect self)
            {
                self.ProvideActionIntoPossibilitySection = (QEffect rootToLifeEffect, PossibilitySection possibilitySection) =>
                {
                    if (possibilitySection.PossibilitySectionId == PossibilitySectionId.MainActions)
                    {
                        Creature owner = rootToLifeEffect.Owner;
                        PossibilitySection rootToLifeSection = new PossibilitySection("Root to Life Possibilities");

                        IllustrationName rootToLifeIllustrationName = IllustrationName.Tree1;
                        List<Trait> rootToLifeTraits = [Trait.Manipulate, Trait.Necromancy, Trait.Primal, Trait.Basic, ThaumaturgeTraits.Thaumaturge];

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

                        SubmenuPossibility rootToLifeMenu = new SubmenuPossibility(rootToLifeIllustrationName, "Root to Life");
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
            scrollThaumaturgyFeat.WithPermanentQEffect("Cast and hold scrolls with your Implement hand", delegate (QEffect self)
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
                self.StartOfCombat = async (QEffect startOfCombat) =>
                {
                    Creature self = startOfCombat.Owner;
                    List<Item> allItems = self.HeldItems.Concat(self.CarriedItems).ToList();
                    foreach (Item implement in allItems.Where(item => item.HasTrait(ThaumaturgeTraits.Implement) && item.StoredItems.Count == 1))
                    {
                        Item scroll = implement.StoredItems[0];
                        implement.StoredItems.Remove(scroll);
                        ImplementAndHeldItem implementAndScroll = new ImplementAndHeldItem(implement, scroll, wasStoredItem: true);
                        QEffect? matchingHeldScrollImplementEffect = self.QEffects.FirstOrDefault(qe => qe.Id == ThaumaturgeQEIDs.HeldScrollAndImplement && qe.Tag != null && qe.Tag is ImplementAndHeldItem implementAndHeldItem && implementAndHeldItem.Implement == implement);


                        if (matchingHeldScrollImplementEffect != null && matchingHeldScrollImplementEffect.Tag != null && matchingHeldScrollImplementEffect.Tag is ImplementAndHeldItem previousImplementAndHeldItem)
                        {
                            Item previousScroll = previousImplementAndHeldItem.HeldItem;
                            implementAndScroll.OriginalImplementName = previousImplementAndHeldItem.OriginalImplementName;
                            self.CarriedItems.Add(previousScroll);
                            self.RemoveAllQEffects(qe => qe.Id == ThaumaturgeQEIDs.HeldScrollAndImplement && qe == matchingHeldScrollImplementEffect);
                        }

                        self.AddQEffect(new QEffect(ExpirationCondition.Never)
                        {
                            Id = ThaumaturgeQEIDs.HeldScrollAndImplement,
                            Tag = implementAndScroll
                        });

                        implement.Illustration = implementAndScroll.Illustration;
                        implement.Name = implementAndScroll.OriginalImplementName + " " + scroll.Name;
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
                                if (implementAndHeldItem.WasStoredItem)
                                {
                                    implementAndHeldItem.Implement.StoredItems.Add(implementAndHeldItem.HeldItem);
                                }
                                else
                                {
                                    owner.CarriedItems.Add(implementAndHeldItem.HeldItem);
                                    owner.HeldItems.RemoveAll(item => item == implementAndHeldItem.Implement);
                                    owner.CarriedItems.RemoveAll(item => item == implementAndHeldItem.Implement);
                                }
                            }
                        }
                    }
                };
                self.ProvideActionIntoPossibilitySection = (QEffect scrollThaumaturgyEffect, PossibilitySection possibilitySection) =>
                {
                    Creature owner = scrollThaumaturgyEffect.Owner;
                    if (possibilitySection.PossibilitySectionId == PossibilitySectionId.ItemActions && ThaumaturgeUtilities.IsCreatureHoldingAnyImplement(owner))
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
                                                CombatAction drawScrollImplementAction = new CombatAction(owner, scroll.Illustration, drawOrReplace + " " + scroll.Name, [Trait.Manipulate, Trait.Basic], drawOrReplace + " a scroll into the same hand you are holding this implement in.\n----\n" + scroll.Description, Target.Self())
                                                    .WithActionCost(1)
                                                    .WithEffectOnSelf(async (Creature self) =>
                                                    {
                                                        if (self.HeldItems[implementIndex].HasTrait(ThaumaturgeTraits.Implement))
                                                        {
                                                            Item implement = self.HeldItems[implementIndex];
                                                            ImplementAndHeldItem implementAndScroll = new ImplementAndHeldItem(implement, scroll);

                                                            self.CarriedItems.Remove(scroll);
                                                            if (matchingHeldScrollImplementEffect != null && matchingHeldScrollImplementEffect.Tag != null && matchingHeldScrollImplementEffect.Tag is ImplementAndHeldItem previousImplementAndHeldItem)
                                                            {
                                                                Item previousScroll = previousImplementAndHeldItem.HeldItem;
                                                                implementAndScroll.OriginalImplementName = previousImplementAndHeldItem.OriginalImplementName;
                                                                self.CarriedItems.Add(previousScroll);
                                                                self.RemoveAllQEffects(qe => qe.Id == ThaumaturgeQEIDs.HeldScrollAndImplement && qe == matchingHeldScrollImplementEffect);
                                                            }

                                                            self.AddQEffect(new QEffect(ExpirationCondition.Never)
                                                            {
                                                                Id = ThaumaturgeQEIDs.HeldScrollAndImplement,
                                                                Tag = implementAndScroll
                                                            });

                                                            implement.Illustration = implementAndScroll.Illustration;
                                                            implement.Name = implementAndScroll.OriginalImplementName + " " + scroll.Name;
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
                            if (heldItemAndImplementEffect != null && heldItemAndImplementEffect.Tag != null && heldItemAndImplementEffect.Tag is ImplementAndHeldItem scrollAndImplement && owner.HeldItems.Any(item => item == scrollAndImplement.Implement))
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
            esotericWardenFeat.WithPermanentQEffect("Increase defenses after exploiting a vulnerability", delegate (QEffect self)
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
                                Illustration = ThaumaturgeModdedIllustrations.EsotericWarden,
                                Name = "Esoteric Warden AC (" + target.Name + ")",
                                Description = "+" + esotericWardenValue + " status bonus to AC against " + target.Name,
                                Value = esotericWardenValue,
                                Tag = action.ChosenTargets.ChosenCreature,
                                DoNotShowUpOverhead = true
                            });

                            afterYouTakeAction.Owner.AddQEffect(new QEffect(ExpirationCondition.Never)
                            {
                                Id = ThaumaturgeQEIDs.EsotericWardenSave,
                                Illustration = ThaumaturgeModdedIllustrations.EsotericWarden,
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

        /// <summary>
        /// Adds the logic for the Instructive Strike feat
        /// </summary>
        /// <param name="instructiveStrikeFeat">The Instructive Strike feat object</param>
        public static void AddInstructiveStrikeLogic(Feat instructiveStrikeFeat)
        {
            instructiveStrikeFeat.WithPermanentQEffect("Strike and Exploit Vulnerability", delegate (QEffect self)
            {
                self.ProvideStrikeModifier = (Item item) =>
                {
                    Creature owner = self.Owner;
                    if (item.WeaponProperties != null)
                    {
                        CombatAction instructiveStrike = owner.CreateStrike(item);
                        ((CreatureTarget)instructiveStrike.Target).WithAdditionalConditionOnTargetCreature((Creature attacker, Creature defender) =>
                        {
                            if (!ThaumaturgeUtilities.IsCreatureHoldingAnyImplement(attacker))
                            {
                                return Usability.NotUsable("Not weilding Implement.");
                            }

                            return Usability.Usable;
                        });
                        instructiveStrike.Name = "Instructive Strike";
                        if (!instructiveStrike.HasTrait(Trait.Basic))
                        {
                            instructiveStrike.Traits.Add(Trait.Basic);
                        }
                        instructiveStrike.ActionId = ThaumaturgeActionIDs.InstructiveStrike;
                        return instructiveStrike;
                    }

                    return null;
                };

                self.AfterYouTakeAction = async (QEffect afterYouTakeAction, CombatAction action) =>
                {
                    Creature owner = self.Owner;
                    if (action.ActionId == ThaumaturgeActionIDs.InstructiveStrike && ThaumaturgeUtilities.IsCreatureHoldingAnyImplement(owner) && action.CheckResult >= CheckResult.Success)
                    {
                        QEffect instructiveStrikeEffect = new QEffect(ExpirationCondition.ExpiresAtStartOfYourTurn)
                        {
                            Id = ThaumaturgeQEIDs.InstructiveStrike,
                            Value = (action.CheckResult == CheckResult.Success ? 1 : 2)
                        };
                        Creature? target = action.ChosenTargets.ChosenCreature;
                        if (target != null)
                        {
                            owner.AddQEffect(instructiveStrikeEffect);
                            CombatAction exploitVulnerabilityAction = ThaumaturgeUtilities.CreateExploitVulnerabilityAction(owner);
                            exploitVulnerabilityAction.ActionCost = 0;
                            exploitVulnerabilityAction.Target = action.Target;
                            exploitVulnerabilityAction.ChosenTargets.ChosenCreature = target;
                            await exploitVulnerabilityAction.AllExecute();
                            owner.RemoveAllQEffects(qe => qe.Id == ThaumaturgeQEIDs.InstructiveStrike);
                        }
                    }
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Ammunition Thaumaturgy feat
        /// </summary>
        /// <param name="instructiveStrikeFeat">The Ammunition Thaumaturgy feat object</param>
        public static void AddAmmunitionThaumaturgyLogic(Feat ammunitionThaumaturgyFeat)
        {
            ammunitionThaumaturgyFeat.WithPermanentQEffect("Use bows freely with your Implement hand", delegate (QEffect self)
            {
                self.StateCheck = (QEffect stateCheck) =>
                {
                    Creature owner = self.Owner;
                    if (ThaumaturgeUtilities.IsCreatureHoldingAnyImplement(owner))
                    {
                        foreach (Item item in owner.HeldItems)
                        {
                            if (item.HasTrait(Trait.OneHandPlus) && !item.HasTrait(ThaumaturgeTraits.TemporaryIgnoreOneHandPlus) && !item.HasTrait(ThaumaturgeTraits.Implement))
                            {
                                item.Traits.Remove(Trait.OneHandPlus);
                                item.Traits.Add(ThaumaturgeTraits.TemporaryIgnoreOneHandPlus);
                            }
                        }
                    }
                    else
                    {
                        foreach (Item item in owner.HeldItems)
                        {
                            if (item.HasTrait(ThaumaturgeTraits.TemporaryIgnoreOneHandPlus) && !item.HasTrait(Trait.OneHandPlus))
                            {
                                item.Traits.Remove(ThaumaturgeTraits.TemporaryIgnoreOneHandPlus);
                                item.Traits.Add(Trait.OneHandPlus);
                            }
                        }
                    }
                    foreach (Item item in owner.CarriedItems)
                    {
                        if (item.HasTrait(ThaumaturgeTraits.TemporaryIgnoreOneHandPlus))
                        {
                            item.Traits.Remove(ThaumaturgeTraits.TemporaryIgnoreOneHandPlus);
                            item.Traits.Add(Trait.OneHandPlus);
                        }
                    }
                    foreach (Tile tile in owner.Battle.Map.Tiles)
                    {
                        foreach (Item item in tile.DroppedItems)
                        {
                            if (item.HasTrait(ThaumaturgeTraits.TemporaryIgnoreOneHandPlus))
                            {
                                item.Traits.Remove(ThaumaturgeTraits.TemporaryIgnoreOneHandPlus);
                                item.Traits.Add(Trait.OneHandPlus);
                            }
                        }
                    }
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Divine Disharmony feat
        /// </summary>
        /// <param name="divineDisharmonyFeat">The Divine Disharmony feat object</param>
        public static void AddDivineDisharmonyLogic(Feat divineDisharmonyFeat)
        {
            divineDisharmonyFeat.WithPermanentQEffect("Make enemies flat-footed with you Divine knowledge", delegate (QEffect self)
            {
                self.ProvideMainAction = (QEffect provideMainAction) =>
                {
                    Creature owner = provideMainAction.Owner;
                    PossibilitySection divineDisharmonySection = new PossibilitySection("Divine Disharmony Possibilities");

                    CombatAction divineDisharmonyAction = new CombatAction(owner, ThaumaturgeModdedIllustrations.DivineDisharmony, "Divine Disharmony", [Trait.Divine, Trait.Enchantment, Trait.Manipulate, Trait.Basic, ThaumaturgeTraits.Thaumaturge], divineDisharmonyFeat.RulesText,Target.RangedCreature(12));
                    divineDisharmonyAction.WithActionCost(1);
                    divineDisharmonyAction.WithActionId(ThaumaturgeActionIDs.DivineDisharmony);
                    divineDisharmonyAction.WithActiveRollSpecification(new ActiveRollSpecification(Checks.SkillCheck(Skill.Deception, Skill.Intimidation), Checks.DefenseDC(Defense.Will)));
                    divineDisharmonyAction.WithEffectOnEachTarget(async delegate (CombatAction action, Creature attacker, Creature defender, CheckResult result)
                    {
                        if (result >= CheckResult.Success)
                        {
                            QEffect flatFooted = QEffect.FlatFooted("Divine Disharmony");
                            flatFooted.Owner = defender;
                            flatFooted.ExpiresAt = ExpirationCondition.ExpiresAtEndOfYourTurn;
                            if (result == CheckResult.CriticalSuccess)
                            {
                                flatFooted.CannotExpireThisTurn = true;
                            }

                            defender.AddQEffect(flatFooted);
                        }
                    });

                    return new ActionPossibility(divineDisharmonyAction);
                };
                self.BonusToSkillChecks = (Skill skill, CombatAction action, Creature? target) =>
                {
                    if (action.ActionId == ThaumaturgeActionIDs.DivineDisharmony && target != null && target.Spellcasting != null && target.Spellcasting.PrimarySpellcastingSource.SpellcastingTradition == Trait.Divine)
                    {
                        return new Bonus(2, BonusType.Circumstance, "Divine Disharmony", true);
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Lingering Pain Strike feat
        /// </summary>
        /// <param name="lingeringPainStrikeFeat">The Lingering Pain Strike feat object</param>
        public static void AddLingeringPainStrikeLogic(Feat lingeringPainStrikeFeat)
        {
            lingeringPainStrikeFeat.WithPermanentQEffect("Strike and potentially sicken", delegate (QEffect self)
            {
                self.ProvideStrikeModifier = (Item item) =>
                {
                    if (item.WeaponProperties != null)
                    {
                        CombatAction lingeringPainStrikeStrike = self.Owner.CreateStrike(item);
                        lingeringPainStrikeStrike.WithActionCost(2);
                        lingeringPainStrikeStrike.Traits.Add(Trait.Mental);
                        lingeringPainStrikeStrike.Traits.Add(Trait.Fear);
                        lingeringPainStrikeStrike.Traits.Add(Trait.Emotion);
                        lingeringPainStrikeStrike.Traits.Add(Trait.Basic);
                        lingeringPainStrikeStrike.Illustration = (Illustration)new SideBySideIllustration(lingeringPainStrikeStrike.Illustration, (Illustration)IllustrationName.Sickened);
                        lingeringPainStrikeStrike.Name = "Lingering Pain Strike";
                        lingeringPainStrikeStrike.StrikeModifiers.OnEachTarget += (async (attacker, defender, checkResult) =>
                        {
                            if (checkResult >= CheckResult.Success)
                            {
                                ActiveRollSpecification activeRollSpecification = new ActiveRollSpecification(Checks.SavingThrow(Defense.Fortitude), Checks.FlatDC(ThaumaturgeUtilities.CalculateClassDC(attacker, ThaumaturgeTraits.Thaumaturge)));
                                int classDC = ThaumaturgeUtilities.CalculateClassDC(attacker, ThaumaturgeTraits.Thaumaturge);
                                CheckResult savingThrowResult = CommonSpellEffects.RollSavingThrow(defender, lingeringPainStrikeStrike, Defense.Fortitude, classDC);
                                if (savingThrowResult <= CheckResult.Failure)
                                {
                                    defender.AddQEffect(QEffect.Sickened(1, classDC));
                                }
                            }

                        });

                        return lingeringPainStrikeStrike;
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Esoterica Seller feat
        /// </summary>
        /// <param name="esotericaSellerFeat">The Esoterica Seller feat object</param>
        public static void AddEsotericaSellerLogic(Feat esotericaSellerFeat)
        {
            esotericaSellerFeat.WithPermanentQEffect("Earn more money collecting esoterica", delegate (QEffect self)
            {
                self.StartOfCombat = async (QEffect startOfCombat) =>
                {
                    int totalWorth = 0;
                    foreach(Creature creature in startOfCombat.Owner.Battle.AllCreatures.Where(enemy => !startOfCombat.Owner.FriendOfAndNotSelf(enemy)))
                    {
                        totalWorth += Math.Max(creature.Level, 0);
                    }

                    self.Owner.AddQEffect(new QEffect(ExpirationCondition.Never)
                    {
                        Id = ThaumaturgeQEIDs.EsotericSeller,
                        Value = totalWorth
                    });
                };
                self.EndOfCombat = async (QEffect endOfCombat, bool didWin) =>
                {
                    if (didWin)
                    {
                        Creature owner = endOfCombat.Owner;
                        QEffect? esotericSeller = owner.FindQEffect(ThaumaturgeQEIDs.EsotericSeller);
                        if (esotericSeller != null && esotericSeller.Value > 0)
                        {
                            Item lootedEsoteric = Items.CreateNew(ThaumaturgeItemNames.LootedEsoterica);
                            lootedEsoteric.Price = esotericSeller.Value;
                            if (owner.PersistentCharacterSheet != null)
                            {
                                owner.PersistentCharacterSheet.CampaignInventory.AddAtEndOfBackpack(lootedEsoteric);
                                if (CampaignState.Instance != null)
                                {
                                    CampaignState.Instance.CommonLoot.Add(lootedEsoteric);
                                }
                            }
                        }
                    }
                };
            });
        }

        /// <summary>
        /// Adds the logic for the One More Activation feat
        /// </summary>
        /// <param name="oneMoreActivationFeat">The One More Activation feat object</param>
        public static void AddOneMoreActivationLogic(Feat oneMoreActivationFeat)
        {
            oneMoreActivationFeat.WithPermanentQEffect("Once per day, you can reactivate an item.", delegate (QEffect self)
            {
                self.ProvideActionIntoPossibilitySection = (QEffect qfProvideAction, PossibilitySection section) =>
                {
                    if (section.PossibilitySectionId == PossibilitySectionId.ItemActions)
                    {
                        Creature owner = qfProvideAction.Owner;

                        if (owner.PersistentUsedUpResources.UsedUpActions.Contains("One More Activation"))
                        {
                            return null;
                        }

                        List<Item> allItems = owner.HeldItems.Concat(owner.CarriedItems).ToList();
                        List<Item> usedUpItems = allItems.Where(item => item.IsUsedUp == true).ToList();
                        foreach (Item item in allItems)
                        {
                            foreach (Item subItem in item.StoredItems)
                            {
                                if (subItem.IsUsedUp)
                                {
                                    usedUpItems.Add(subItem);
                                }
                            }
                        }

                        if (usedUpItems.Count > 0)
                        {
                            SubmenuPossibility oneMoreActivationMenu = new SubmenuPossibility(IllustrationName.GenericCombatManeuver, "Reactivate an item that already been used up.");
                            PossibilitySection oneMoreActivationSection = new PossibilitySection($"One more activation");
                            foreach (Item item in usedUpItems)
                            {
                                CombatAction oneMoreActivationAction = new CombatAction(owner, item.Illustration, "One More Activation (" + item.Name + ")", [Trait.Basic], $"Reactivate {item.Name} so it can be used again today. You can not use 'One More Activation' again for the rest of the day.", Target.Self()
                                    .WithAdditionalRestriction((Creature user) =>
                                    {
                                        if (user.PersistentUsedUpResources.UsedUpActions.Contains("One More Activation"))
                                        {
                                            return "Already used 'One More Activation' today.";
                                        }

                                        return null;
                                    }))
                                    .WithActionCost(0)
                                    .WithEffectOnSelf(async (Creature self) =>
                                    {
                                        item.RevertUseUp();
                                        self.PersistentUsedUpResources.UsedUpActions.Add("One More Activation");
                                    });
                                ActionPossibility oneMoreActivationPossibility = new ActionPossibility(oneMoreActivationAction);
                                oneMoreActivationSection.AddPossibility(oneMoreActivationPossibility);
                            }

                            oneMoreActivationMenu.Subsections.Add(oneMoreActivationSection);
                            return oneMoreActivationMenu;
                        }
                    }
                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Scroll Esoterica feat
        /// </summary>
        /// <param name="scrollEsotericaFeat">The Scroll Esoterica feat object</param>
        public static void AddScrollEsotericaLogic(Feat scrollEsotericaFeat)
        {
            scrollEsotericaFeat.WithPermanentQEffect("Once per day, you recreate a scroll.", delegate (QEffect self)
            {
                self.StartOfCombat = async (QEffect startOfCombat) =>
                {
                    startOfCombat.Tag = new List<Item>();
                };

                self.AfterYouTakeAction = async (QEffect youTakeAction, CombatAction action) =>
                {
                    if (youTakeAction.Tag != null && youTakeAction.Tag is List<Item> usedScrollsThisCombat && action.CastFromScroll != null)
                    {
                        usedScrollsThisCombat.Add(action.CastFromScroll);
                    }
                };

                self.ProvideActionIntoPossibilitySection = (QEffect qfProvideAction, PossibilitySection section) =>
                {
                    if (section.PossibilitySectionId == PossibilitySectionId.ItemActions)
                    {
                        Creature owner = qfProvideAction.Owner;

                        if (owner.PersistentUsedUpResources.UsedUpActions.Contains("Scroll Esoterica"))
                        {
                            return null;
                        }

                        if (qfProvideAction.Tag != null && qfProvideAction.Tag is List<Item> usedScrollsThisCombat && usedScrollsThisCombat.Count > 0)
                        {
                            SubmenuPossibility scrollEsotericaMenu = new SubmenuPossibility(IllustrationName.Scroll, "Recreate a scroll you used this combat.");
                            PossibilitySection scrollEsotericaSection = new PossibilitySection($"Scroll Esoterica");
                            foreach (Item item in usedScrollsThisCombat)
                            {
                                CombatAction scrollEsotericaAction = new CombatAction(owner, item.Illustration, "Scroll Esoterica (" + item.Name + ")", [Trait.Basic], $"Recreate {item.Name} and place it in your hand holding your implement or a free hand. You can not use 'Scroll Esoterica' again for the rest of the day.", Target.Self()
                                    .WithAdditionalRestriction((Creature user) =>
                                    {
                                        if (user.PersistentUsedUpResources.UsedUpActions.Contains("Scroll Esoterica"))
                                        {
                                            return "Already used 'Scroll Esoterica' today.";
                                        }
                                        List<Item> implementAndHeldItems = owner.QEffects.Where(qe => qe.Id == ThaumaturgeQEIDs.HeldScrollAndImplement && qe.Tag is ImplementAndHeldItem implementAndHeldItem).Select(qe => ((ImplementAndHeldItem)qe.Tag).Implement).ToList();
                                        if (!user.HasFreeHand && !owner.HeldItems.Any(item => item.HasTrait(ThaumaturgeTraits.Implement) && !implementAndHeldItems.Contains(item)))
                                        {
                                            return "No free hand or implement without a scroll";
                                        }

                                        return null;
                                    }))
                                    .WithActionCost(0)
                                    .WithEffectOnSelf(async (Creature self) =>
                                    {
                                        if (self.HasFreeHand)
                                        {
                                            self.AddHeldItem(item);
                                        }

                                        Item? implement = self.HeldItems.FirstOrDefault(item => item.HasTrait(ThaumaturgeTraits.Implement) && item.StoredItems.Count == 0);
                                        if (implement != null)
                                        {

                                            ImplementAndHeldItem implementAndScroll = new ImplementAndHeldItem(implement, item, wasStoredItem: true);
                                            QEffect? matchingHeldScrollImplementEffect = self.QEffects.FirstOrDefault(qe => qe.Id == ThaumaturgeQEIDs.HeldScrollAndImplement && qe.Tag != null && qe.Tag is ImplementAndHeldItem implementAndHeldItem && implementAndHeldItem.Implement == implement);


                                            if (matchingHeldScrollImplementEffect != null && matchingHeldScrollImplementEffect.Tag != null && matchingHeldScrollImplementEffect.Tag is ImplementAndHeldItem previousImplementAndHeldItem)
                                            {
                                                Item previousScroll = previousImplementAndHeldItem.HeldItem;
                                                implementAndScroll.OriginalImplementName = previousImplementAndHeldItem.OriginalImplementName;
                                                self.CarriedItems.Add(previousScroll);
                                                self.RemoveAllQEffects(qe => qe.Id == ThaumaturgeQEIDs.HeldScrollAndImplement && qe == matchingHeldScrollImplementEffect);
                                            }

                                            self.AddQEffect(new QEffect(ExpirationCondition.Never)
                                            {
                                                Id = ThaumaturgeQEIDs.HeldScrollAndImplement,
                                                Tag = implementAndScroll
                                            });

                                            implement.Illustration = implementAndScroll.Illustration;
                                            implement.Name = implementAndScroll.OriginalImplementName + " " + item.Name;
                                        }

                                        self.PersistentUsedUpResources.UsedUpActions.Add("Scroll Esoterica");
                                    });
                                ActionPossibility scrollEsotericaPossibility = new ActionPossibility(scrollEsotericaAction);
                                scrollEsotericaSection.AddPossibility(scrollEsotericaPossibility);
                            }

                            scrollEsotericaMenu.Subsections.Add(scrollEsotericaSection);
                            return scrollEsotericaMenu;
                        }
                    }
                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Sympathetic Vulnerabilities class feat
        /// </summary>
        /// <param name="sympatheticVulnerabilitiesFeat">The Sympathetic Vulnerabilities feat object</param>
        public static void AddSympatheticVulnerabilitiesLogic(Feat sympatheticVulnerabilitiesFeat)
        {
            sympatheticVulnerabilitiesFeat.WithPermanentQEffect("Your mortal weaknesses and personal antithesis apply to more creatures.", delegate (QEffect self)
            {
            });
        }

        /// <summary>
        /// Adds the logic for the Profane Insight class feat
        /// </summary>
        /// <param name="profaneInsightFeat">The Profane Insight feat object</param>
        public static void AddProfaneInsightLogic(Feat profaneInsightFeat)
        {
            profaneInsightFeat.WithPermanentQEffect("You gain a +2 circumstance bonus to Demoalize against your Exploit Vulnerability targets.", delegate (QEffect self)
            {
                self.BonusToSkillChecks = (Skill skill, CombatAction action, Creature? defender) =>
                {
                    if (action.ActionId == ActionId.Demoralize && defender != null && defender.QEffects.Any(qe => qe.Id == ThaumaturgeQEIDs.ExploitVulnerabilityTarget && qe.Tag is Creature source && source == action.Owner))
                    {
                        return new Bonus(2, BonusType.Circumstance, "Profane insight", true);
                    }

                    return null;
                };
                self.AfterYouTakeActionAgainstTarget = async (QEffect afterAction, CombatAction action, Creature defender, CheckResult result) =>
                {
                    if (action.ActionId == ActionId.Demoralize && defender.QEffects.Any(qe => qe.Id == ThaumaturgeQEIDs.ExploitVulnerabilityTarget && qe.Tag is Creature source && source == action.Owner))
                    {
                        defender.RemoveAllQEffects(qe => qe.Id == QEffectId.ImmunityToTargeting && qe.SourceAction != null && qe.SourceAction.ActionId == ActionId.Demoralize && qe.Source == afterAction.Owner);
                    }
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Cursed Effigy class feat
        /// </summary>
        /// <param name="cursedEffigyFeat">The Cursed Effigy feat object</param>
        public static void AddCursedEffigyLogic(Feat cursedEffigyFeat)
        {
            cursedEffigyFeat.WithPermanentQEffect("You can give the targets of your Exploit Vulnerability a -2 status penalty.", delegate (QEffect self)
            {
                self.AfterYouTakeActionAgainstTarget = async (QEffect afterAction, CombatAction action, Creature defender, CheckResult result) =>
                {
                    if (action.HasTrait(Trait.Strike))
                    {
                        QEffect? exploitTargetEffect = defender.QEffects.FirstOrDefault(qe => qe.Id == ThaumaturgeQEIDs.ExploitVulnerabilityTarget && qe.Tag is Creature source && source == action.Owner);
                        if (exploitTargetEffect != null)
                        {
                            QEffect canUseCursedEffigy = new QEffect(ExpirationCondition.ExpiresAtEndOfSourcesTurn)
                            {
                                Id = ThaumaturgeQEIDs.CanUseCursedEffigy,
                                Source = afterAction.Owner
                            };

                            exploitTargetEffect.WhenExpires += (QEffect qfExpires) =>
                            {
                                canUseCursedEffigy.ExpiresAt = ExpirationCondition.Immediately;
                            };

                            defender.AddQEffect(canUseCursedEffigy);
                        }
                    }
                };
                self.ProvideMainAction = (QEffect provideMainAction) =>
                {
                    Creature owner = provideMainAction.Owner;
                    return new ActionPossibility(new CombatAction(owner, IllustrationName.GenericCombatManeuver, "Cursed Effight", [Trait.Curse], cursedEffigyFeat.RulesText, Target.Ranged(200)
                        .WithAdditionalConditionOnTargetCreature((Creature attacker, Creature defender) =>
                        {
                            if (defender.QEffects.Any(qe => qe.Id == ThaumaturgeQEIDs.CanUseCursedEffigy && qe.Source != null && qe.Source == attacker))
                            {
                                return Usability.Usable;
                            }

                            return Usability.NotUsableOnThisCreature("Not valid");
                        }))
                        .WithActionCost(1)
                        .WithEffectOnEachTarget(async (CombatAction action, Creature attacker, Creature defender, CheckResult result) =>
                        {
                            QEffect? exploitTargetEffect = defender.QEffects.FirstOrDefault(qe => qe.Id == ThaumaturgeQEIDs.ExploitVulnerabilityTarget && qe.Tag is Creature source && source == action.Owner);
                            if (exploitTargetEffect != null)
                            {
                                QEffect savingThrowPenalty = new QEffect("Cursed Effigy", $"You have a -2 status penalty against saving throws from {attacker.Name}")
                                {
                                    Illustration = IllustrationName.GenericCombatManeuver,
                                    Source = attacker,
                                    BonusToDefenses = (QEffect penalty, CombatAction? action, Defense defense) =>
                                    {
                                        if (defense == Defense.Reflex || defense == Defense.Fortitude || defense == Defense.Will)
                                        {
                                            if (action != null && action.Owner == penalty.Source)
                                            {
                                                return new Bonus(-2, BonusType.Status, "Cursed Effigy", false);
                                            }
                                        }

                                        return null;
                                    }
                                };

                                exploitTargetEffect.WhenExpires += (QEffect qfExpires) =>
                                {
                                    savingThrowPenalty.ExpiresAt = ExpirationCondition.Immediately;
                                };

                                defender.AddQEffect(savingThrowPenalty);
                            } 
                        }));
                };
            });
        }

        /// <summary>
        /// Adds the logic for the Know It All class feat
        /// </summary>
        /// <param name="knowItAllFeat">The Know It All feat object</param>
        public static void AddKnowItAllLogic(Feat knowItAllFeat)
        {
            knowItAllFeat.WithPermanentQEffect("You can exploit vulnerabilities easier.", delegate (QEffect self)
            {
            });
        }

        /// <summary>
        /// Adds the logic for the Magical Exploits class feat
        /// </summary>
        /// <param name="magicalExploitsFeat">The Magical Exploits feat object</param>
        public static void AddMagicalExploitsLogic(Feat magicalExploitsFeat)
        {
            magicalExploitsFeat.WithPermanentQEffect("Attack spells trigger your weakness and antithesis.", delegate (QEffect self)
            {
            });
        }

        private static void AddImplementEnsureLogic(Feat implementFeat)
        {
            implementFeat.WithOnSheet((character) =>
            {
                ThaumaturgeUtilities.EnsureCorrectImplements(character);
            });
            implementFeat.WithOnCreature((character, creature) =>
            {
                ThaumaturgeUtilities.EnsureCorrectImplements(character);
            });
        }

        private static void AddLevelTag(Feat feat)
        {
            feat.WithOnSheet(values =>
            {
                string tagName = values.CurrentLevel > 1 ? "Second Implement" : "First Implement";
                values.Tags[tagName] = feat.FeatName;
            });
        }
    }
}
