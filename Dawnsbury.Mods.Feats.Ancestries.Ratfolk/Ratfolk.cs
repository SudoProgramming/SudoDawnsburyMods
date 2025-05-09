using Dawnsbury.Auxiliary;
using Dawnsbury.Core;
using Dawnsbury.Core.Animations.Movement;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Damage;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Modding;
using Dawnsbury.Mods.Feats.Ancestries.Ratfolk.RegisteredComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.BarbarianFeatsDb.AnimalInstinctFeat;
using static Dawnsbury.Delegates;

namespace Dawnsbury.Mods.Feats.Ancestries.Ratfolk
{
    /// <summary>
    /// The Ratfolk Ancestry
    /// </summary>
    public static class Ratfolk
    {
        /// <summary>
        /// Creates all the Feats for the Ratfolk
        /// </summary>
        /// <returns>The Enumerable of Ratfolk Feats</returns>
        public static IEnumerable<Feat> CreateRatfolkFeats()
        {
            // Heritage Feats
            // Creates and adds the logic for the Desert Rat heritage
            HeritageSelectionFeat desertRatFeat = new HeritageSelectionFeat(RatfolkFeatNames.DesertRat, "You are native to arid plains and likely grew up traveling the roads.", "You have a leaner build than other ratfolk, with longer limbs and short fur. If you have both hands free, you can increase your Speed to 30 feet as you run on all fours.");
            AddDesertRatLogic(desertRatFeat);
            yield return desertRatFeat;

            // Creates and adds the logic for the Longsnout Rat heritage
            HeritageSelectionFeat longsnoutRatFeat = new HeritageSelectionFeat(RatfolkFeatNames.LongsnoutRat, "The long snouts that run in your family give you a keener sense of smell than most ratfolk.", "You gain a +2 circumstance bonus to Perception checks to Seek a creature or object within 30 feet.");
            AddLongsnoutRatLogic(longsnoutRatFeat);
            yield return longsnoutRatFeat;

            // Creates and adds the logic for the Sewer Rat heritage
            HeritageSelectionFeat sewerRatFeat = new HeritageSelectionFeat(RatfolkFeatNames.SewerRat, "You come from a long line of ratfolk with a community based in the sewers beneath a large settlement.", "You gain a +1 circumstance bonus to saving throws against diseases and poisons. If you roll a success on a saving throw against a disease or poison, you get a critical success instead. If you have a different ability that would improve the save this way (such as the battle hardened fighter class feature), if you roll a critical failure on the save you get a failure instead.");
            AddSewerRatLogic(sewerRatFeat);
            yield return sewerRatFeat;

            // Creates and adds the logic for the Shadow Rat heritage
            HeritageSelectionFeat shadowRatFeat = new HeritageSelectionFeat(RatfolkFeatNames.ShadowRat, "Your ancestors lived in dark spaces underground, granting you dark fur and a vaguely unnatural mien.", "You become trained in Intimidation. When you Demoralize an animal you don't take a penalty for not sharing a language with it.");
            AddShadowRatLogic(shadowRatFeat);
            yield return shadowRatFeat;

            // Creates and adds the logic for the Snow Rat heritage
            HeritageSelectionFeat snowRatFeat = new HeritageSelectionFeat(RatfolkFeatNames.SnowRat, "You have a thicker coat and bulkier frame to defend against the cold.", "You gain you cold resistance equal to half your level (minimum 1).");
            AddSnowRatLogic(snowRatFeat);
            yield return snowRatFeat;

            // Creates the base Ratfolk ancestry selection feat and adds the logic for the Sharp Teeth base feature
            AncestrySelectionFeat ratfolkFeat = new AncestrySelectionFeat(RatfolkFeatNames.Ratfolk,
                "Ysoki are beings that most surface-dwelling humanoids refer to as \"ratfolk.\" They are a communal people who prefer cramped conditions, with up to 100 individuals living in a given home. If they can't find homes in town, ratfolk may instead live in caves and cavern complexes, as these provide great storage for the many and varied goods they bring back from trading expeditions.\n\n{b}Sharp Teeth{/b} Your prominent incisors offer an alternative to the fists other humanoids bring to a fight. You have a jaws unarmed attack that deals 1d4 piercing damage, is in the brawling group, and has the agile and finesse traits.",
                [RatfolkTraits.Ratfolk, Trait.Humanoid],
                6,
                5,
                [new EnforcedAbilityBoost(Ability.Dexterity), new EnforcedAbilityBoost(Ability.Intelligence), new FreeAbilityBoost()],
                [desertRatFeat, longsnoutRatFeat, sewerRatFeat, shadowRatFeat, snowRatFeat])
                .WithAbilityFlaw(Ability.Strength);
            AddSharpTeethLogic(ratfolkFeat);
            yield return ratfolkFeat;

            // Level 1 Ancestry Feats
            // Creates and adds the logic for the Agile Tail Ratfolk Feat
            TrueFeat agileTailFeat = new TrueFeat(RatfolkFeatNames.AgileTail, 1, "Your tail is long and nimble allowing you to catch others by surprise.", "You ignore the requirement of needing a free hand for the Trip action.", [RatfolkTraits.Ratfolk, Trait.Homebrew]);
            AddAgileTailLogic(agileTailFeat);
            yield return agileTailFeat;

            // Creates and adds the logic for the Cheek Pouches Ratfolk Feat
            // HACK: Update if and when Draw and Replace have Item added
            TrueFeat cheekPouchesFeat = new TrueFeat(RatfolkFeatNames.CheekPouches, 1, "Your cheeks are stretchy, and you can store up to four small items in these cheek pouches.", "The first two items you draw or replace within an encounter are a {icon: FreeAction} free action instead of an action.", [RatfolkTraits.Ratfolk]);
            AddCheekPouchesLogic(cheekPouchesFeat);
            yield return cheekPouchesFeat;

            // Creates and adds the logic for the Ratfolk Lore Ratfolk Feat
            TrueFeat ratfolkLoreFeat = new TrueFeat(RatfolkFeatNames.RatfolkLore, 1, "Years of experience among ratfolk communities have made you nimble, and you've learned to run and hide when enemies threaten.", "You gain the trained proficiency rank in Acrobatics and Stealth.", [RatfolkTraits.Ratfolk]);
            AddRatfolkLoreLogic(ratfolkLoreFeat);
            yield return ratfolkLoreFeat;

            // Creates and adds the logic for the Tumbling Trickster Ratfolk Feat
            TrueFeat tumblingTricksterFeat = new TrueFeat(RatfolkFeatNames.TumblingTrickster, 1, "You are known for your agility and your trickery, so anyone running into you between check your surrounding carefully.", "After you successfully Tumble Through an enemy, you gain a +1 circumstance bonus to your AC that creature until the start of your next turn.", [RatfolkTraits.Ratfolk, Trait.Homebrew]);
            AddTumblingTricksterLogic(tumblingTricksterFeat);
            yield return tumblingTricksterFeat;

            // Creates the Vicious Incisors Ratfolk Feat
            yield return new TrueFeat(RatfolkFeatNames.ViciousIncisors, 1, "You've let your incisors grow long enough to serve as formidable weapons", "Your jaws unarmed attack deals 1d6 piercing damage instead of 1d4, and gains the backstabber trait.", [RatfolkTraits.Ratfolk]);

            // Level 5 Ancestry Feats
            // Creates and adds the logic for the Cornered Fury Ratfolk Feat
            TrueFeat corneredFuryFeat = new TrueFeat(RatfolkFeatNames.CorneredFury, 5, "When physically outmatched, you fight with unexpected ferocity.", "If a foe critically hits and damages you, that foe is flat-footed to you for 1 round.", [RatfolkTraits.Ratfolk]);
            AddCorneredFuryLogic(corneredFuryFeat);
            yield return corneredFuryFeat;

            // Creates and adds the logic for the Gnaw Ratfolk Feat
            TrueFeat gnawFeat = new TrueFeat(RatfolkFeatNames.Gnaw, 5, "With enough time and determination, you can chew through nearly anything.", "Your jaws deal an additional die of damage against constructs and objects.", [RatfolkTraits.Ratfolk]);
            gnawFeat.WithPrerequisite(RatfolkFeatNames.ViciousIncisors, "Requires Vicious Incisors");
            AddGnawLogic(gnawFeat);
            yield return gnawFeat;

            // Creates and adds the logic for the Lab Rat Ratfolk Feat
            TrueFeat labRatFeat = new TrueFeat(RatfolkFeatNames.LabRat, 5, "You've spent more than your share of time in an alchemy lab.", "You have a +1 circumstance bonus to saves against poisons and elixirs. If you roll a success on your saving throw against an elixir or poison, you get a critical success instead.", [RatfolkTraits.Ratfolk]);
            AddLabRatLogic(labRatFeat);
            yield return labRatFeat;

            // Creates and adds the logic for the Quick Stow Ratfolk Feat
            TrueFeat quickStowFeat = new TrueFeat(RatfolkFeatNames.QuickStow, 5, "You are adept at quickly moving items into your cheek pouches", "You can stow as a free action.", [RatfolkTraits.Ratfolk]);
            quickStowFeat.WithPrerequisite(RatfolkFeatNames.CheekPouches, "Requires Check Pouches");
            quickStowFeat.WithActionCost(0);
            AddQuickStowLogic(quickStowFeat);
            yield return quickStowFeat;

            // Creates and adds the logic for the Rat Magic Ratfolk Feat
            TrueFeat ratMagicFeat = new TrueFeat(RatfolkFeatNames.RatMagic, 5, "There always seemed to be a little magic within you.", "Choose any one primal cantrip. You can cast it at-will as an innate spell.", [RatfolkTraits.Ratfolk]);
            AddRatMagicLogic(ratMagicFeat);
            yield return ratMagicFeat;
        }

        /// <summary>
        /// Adds the Sharp Teeth Logic and adjusts it if the user has the Vicious Incisors feat
        /// </summary>
        /// <param name="ratfolkFeat">The ratfolk feat</param>
        public static void AddSharpTeethLogic(AncestrySelectionFeat ratfolkFeat)
        {
            // Adds a permanent effect to add a Sharp Teeth. The teeth are upgraded if the owner has the Vicious Incisors feat
            ratfolkFeat.WithPermanentQEffect(null, delegate (QEffect self)
            {
                string teethDamage = "1d4";
                List<Trait> sharpTeethTraits = new List<Trait>() { Trait.Unarmed, Trait.Brawling, Trait.Agile, Trait.Finesse };
                if (self.Owner.HasFeat(RatfolkFeatNames.ViciousIncisors))
                {
                    teethDamage = "1d6";
                    sharpTeethTraits.Add(Trait.Backstabber);
                }

                self.AdditionalUnarmedStrike = new Item(IllustrationName.Jaws, "Jaws", sharpTeethTraits.ToArray())
                    .WithWeaponProperties(new WeaponProperties(teethDamage, DamageKind.Piercing));
            });
        }

        /// <summary>
        /// Adds the Desert Rat Logic
        /// </summary>
        /// <param name="desertRatFeat">The desert rat heritage feat</param>
        public static void AddDesertRatLogic(HeritageSelectionFeat desertRatFeat)
        {
            // Adds a permanent effect to raise the speed to 30 feet if nothing is held
            desertRatFeat.WithPermanentQEffect("30 feet Speed if nothing is held.", delegate (QEffect self)
            {
                self.BonusToAllSpeeds = (QEffect bonusToSpeed) =>
                {
                    if (self.Owner.HeldItems.Count == 0 && self.Owner.Speed < 6)
                    {
                        return new Bonus(6 -  self.Owner.Speed, BonusType.Untyped, "Desert Rat", true);
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the Longsnout Rat Logic
        /// </summary>
        /// <param name="longsnoutRat">The longsnout rat heritage feat</param>
        public static void AddLongsnoutRatLogic(HeritageSelectionFeat longsnoutRat)
        {
            // Adds a permanenet effect to increase Seek checks within 30 feet
            longsnoutRat.WithPermanentQEffect("+2 Circumstance to Seek within 30 ft.", delegate (QEffect self)
            {
                self.BonusToAttackRolls = (QEffect bonusToSeek, CombatAction action, Creature? creature) =>
                {
                    if (action.ActionId == ActionId.Seek && creature != null && creature.DetectionStatus.Undetected && self.Owner.DistanceTo(creature) <= 6)
                    {
                        return new Bonus(2, BonusType.Circumstance, "Longsnout Rat", true);
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the Sewer Rat Logic
        /// </summary>
        /// <param name="sewerRat">The sewer rat heritage feat</param>
        public static void AddSewerRatLogic(HeritageSelectionFeat sewerRat)
        {
            // Adds a permanent effect to add a +1 circumstance bonus to poison effect and bump successes to critical successes
            sewerRat.WithPermanentQEffect("+1 Circumstance bonus to poison effects", delegate (QEffect self)
            {
                self.AdjustSavingThrowCheckResult = (QEffect effectBonus, Defense defense, CombatAction action, CheckResult result) =>
                {
                    if (action != null && action.HasTrait(Trait.Poison))
                    {
                        if (result == CheckResult.Success)
                        {
                            return result.ImproveByOneStep();
                        }
                        // HACK: If needed in the future adjust Critical Failures to Failures
                    }

                    return result;
                };

                self.BonusToDefenses = (QEffect savingThrowBonus, CombatAction? action, Defense defense) =>
                {
                    if (action != null && (defense == Defense.Fortitude || defense == Defense.Reflex || defense == Defense.Will) && action.HasTrait(Trait.Poison))
                    {
                        return new Bonus(1, BonusType.Circumstance, "Sewer Rat", true);
                    }

                    return null;
                };
            });
        }

        /// <summary>
        /// Adds the Shadow Rat Logic
        /// </summary>
        /// <param name="shadowRat">The shadow rat heritage feat</param>
        public static void AddShadowRatLogic(HeritageSelectionFeat shadowRat)
        {
            // Adds a permanent effect to gain trained in Intimidation and ignore the no common language penalty against animals for Demoralize
            shadowRat.WithOnSheet((character) =>
            {
                if (!character.HasFeat(FeatName.Intimidation))
                {
                    character.GrantFeat(FeatName.Intimidation);
                }
            });
            shadowRat.WithPermanentQEffect("No language penalty against animals", delegate (QEffect self)
            {
                self.YouBeginAction = async (QEffect adjustAction, CombatAction action) =>
                {
                    if (action.ActionId == ActionId.Demoralize && action.ChosenTargets.GetAllTargetCreatures().Any(creature => creature.HasTrait(Trait.Animal)))
                    {
                        StrikeModifiers modifiers = action.StrikeModifiers ?? new StrikeModifiers();
                        QEffect qEffectForStrike = modifiers.QEffectForStrike ?? new QEffect();
                        qEffectForStrike.Id = QEffectId.IntimidatingGlare;
                        modifiers.QEffectForStrike = qEffectForStrike;
                    }
                };
            });
        }

        /// <summary>
        /// Adds the Snow Rat Logic
        /// </summary>
        /// <param name="snowRat">The snow rat heritage feat</param>
        public static void AddSnowRatLogic(HeritageSelectionFeat snowRat)
        {
            // Adds a permanent effect to gain half level cold resistance
            snowRat.WithPermanentQEffect("Cold Resistance", delegate (QEffect self)
            {
                int resistence = Math.Max((self.Owner.Level / 2), 1);
                self.Owner.WeaknessAndResistance.AddResistance(DamageKind.Cold, resistence);
            });
        }

        /// <summary>
        /// Adds the Agile Tail Logic
        /// </summary>
        /// <param name="agileTail">The agile tail feat</param>
        public static void AddAgileTailLogic(TrueFeat agileTail)
        {
            // Adds a permanent effect that allows the user to Trip without having a free hand
            agileTail.WithPermanentQEffect("Trip without free hand", delegate (QEffect self)
            {
                self.Id = RatfolkQEIDs.AgileTail;
            });
            ModManager.RegisterActionOnEachActionPossibility(action =>
            {
                if (action.ActionId == ActionId.Trip && action.Owner != null && action.Owner.HasEffect(RatfolkQEIDs.AgileTail))
                {
                    action.Target = Target.Touch()
                        .WithAdditionalConditionOnTargetCreature(((attacker, defender) => defender.HasEffect(QEffectId.Prone) ? Usability.CommonReasons.TargetIsAlreadyProne : Usability.Usable));
                }
            });
        }

        /// <summary>
        /// Adds the Cheek Pouches Logic
        /// </summary>
        /// <param name="cheekPouchesFeat">The cheek pouches feat</param>
        public static void AddCheekPouchesLogic(TrueFeat cheekPouchesFeat)
        {
            // Adds a permanent effect that allows the first two draw actions to be done as a free action
            cheekPouchesFeat.WithPermanentQEffect("First two draws are free actions", delegate (QEffect self)
            {
                self.Id = RatfolkQEIDs.CheekPouches;
                self.Value = 2;
                self.AfterYouTakeAction = async (QEffect countTracking, CombatAction action) =>
                {
                    // HACK: Currently the item drawn is not stored add later:  && action.Item != null && !action.Item.HasTrait(Trait.TwoHanded)
                    if ((action.Name.ToLower().StartsWith("draw") || action.Name.ToLower().StartsWith("replace")) && self.Value > 0)
                    {
                        self.Value--;
                        if (self.Value <= 0)
                        {
                            self.Owner.RemoveAllQEffects(qe => qe.Id == RatfolkQEIDs.CheekPouches);
                        }
                    }
                };
            });
            ModManager.RegisterActionOnEachActionPossibility(action =>
            {
                // HACK: Currently the item drawn is not stored add later:  && action.Item != null && !action.Item.HasTrait(Trait.TwoHanded)
                if (action.Owner.HasEffect(RatfolkQEIDs.CheekPouches) && (action.Name.ToLower().StartsWith("draw") || action.Name.ToLower().StartsWith("replace")))
                {
                    action.ActionCost = 0;
                }
            });
        }

        /// <summary>
        /// Adds the Tumbling Trickster Logic
        /// </summary>
        /// <param name="tumblingTricksterFeat">The tumbling trickster feat</param>
        public static void AddTumblingTricksterLogic(TrueFeat tumblingTricksterFeat)
        {
            // Adds a permanent effect that gives the user +1 circumstance to AC against creatures tumbled through
            tumblingTricksterFeat.WithPermanentQEffect("+1 Circumstance to AC after Tumble Through", delegate (QEffect self)
            {
                // As you begin striding a tracking effect is added if a tumble through is needed
                self.YouBeginAction = async (QEffect tumblingTrickersEffect, CombatAction action) =>
                {
                    Creature? tumbleThroughCreature = null;

                    if (action.Name == "Tumble Through" && action.ChosenTargets != null && action.ChosenTargets.ChosenCreature != null)
                    {
                        tumbleThroughCreature = action.ChosenTargets.ChosenCreature;
                    }

                    // Checks that the action is a Stride and that the chosen tile is valid
                    else  if (action.ActionId == ActionId.Stride && action.ChosenTargets != null && action.ChosenTargets.ChosenTile != null && !tumblingTrickersEffect.Owner.HasEffect(RatfolkQEIDs.TumblingTrickster))
                    {
                        // Uses the Pathfinding to determine if the path will go through a creature requiring a Tumble Through
                        tumbleThroughCreature = await GetTumbleThroughCreature(tumblingTrickersEffect.Owner, action.ChosenTargets.ChosenTile);
                    }

                    // If a tumble through is needed from Pathfinding a tracking effect is added that will be checked after movement is complete
                    // The two details we need to remember are the starting tile (Tuple item 1) and the creature tumbled through (Tuple item 2)
                    if (tumbleThroughCreature != null)
                    {
                        tumblingTrickersEffect.Owner.AddQEffect(new QEffect(ExpirationCondition.EphemeralAtEndOfImmediateAction)
                        {
                            Id = RatfolkQEIDs.TumblingTrickster,
                            Tag = new Tuple<CombatAction, Creature> (action, tumbleThroughCreature)
                        });
                    }
                };

                // After a stride is complete the tracking effect placed before striding is checked and a bonus might be set
                self.AfterYouTakeAction = async (QEffect postTumbleThrough, CombatAction action) =>
                {
                    if (action.ActionId == ActionId.Stride && action.ChosenTargets != null && action.ChosenTargets.ChosenTile != null)
                    {
                        // The tumbling trickster tracking effect set before striding is gathered and extrapolates the tag named into tumbleDetails. Then the starting tile and the current tile are checked. If they are the same the tumble through failed
                        QEffect? tumblingEffect = postTumbleThrough.Owner.QEffects.FirstOrDefault(qe => qe.Id == RatfolkQEIDs.TumblingTrickster);

                        if (tumblingEffect != null && tumblingEffect.Tag is Tuple<CombatAction, Creature> tumbleDetails)
                        {
                            Creature? creatureForBonus = null;
                            CombatAction tumbleAction = tumbleDetails.Item1;
                            if (tumbleAction.Name == "Tumble Through" && tumbleAction.CheckResult >= CheckResult.Success)
                            {
                                creatureForBonus = tumbleDetails.Item2;
                            }
                            else if (tumbleAction.ActionId == ActionId.Stride && action.ChosenTargets.ChosenTile == postTumbleThrough.Owner.Occupies)
                            {
                                creatureForBonus = tumbleDetails.Item2;
                            }
                            if (creatureForBonus != null)
                            {
                                // A successful tumble through means an effect bonus is added for AC when that creature attacks
                                postTumbleThrough.Owner.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtStartOfYourTurn)
                                {
                                    BonusToDefenses = (QEffect bonusToAC, CombatAction? action, Defense defense) =>
                                    {
                                        if (defense == Defense.AC && action != null && action.Owner != null && action.Owner == creatureForBonus)
                                        {
                                            return new Bonus(1, BonusType.Circumstance, "Tumbling Trickster", true);
                                        }

                                        return null;
                                    }
                                });
                            }
                        }
                    }
                };
            });
        }

        /// <summary>
        /// Adds the Ratfolk Lore Logic
        /// </summary>
        /// <param name="ratfolkLore">The ratfolk lore feat</param>
        public static void AddRatfolkLoreLogic(TrueFeat ratfolkLore)
        {
            // Adds both Acrobatics and Stealth as trained
            ratfolkLore.WithOnSheet((character) =>
            {
                if (!character.HasFeat(FeatName.Acrobatics))
                {
                    character.GrantFeat(FeatName.Acrobatics);
                }
                if (!character.HasFeat(FeatName.Stealth))
                {
                    character.GrantFeat(FeatName.Stealth);
                }
            });
        }

        /// <summary>
        /// Adds the Cornered Fury Logic
        /// </summary>
        /// <param name="corneredFuryFeat">The Cornered Fury feat</param>
        public static void AddCorneredFuryLogic(TrueFeat corneredFuryFeat)
        {
            corneredFuryFeat.WithPermanentQEffect("Foe's are flat-footed for a round if they critically hit you.", delegate (QEffect self)
            {
                self.AfterYouTakeDamage = async (QEffect qeffect, int amount, DamageKind kind, CombatAction? action, bool critical) =>
                {
                    if (action != null && action.HasTrait(Trait.Attack) && action.CheckResult == CheckResult.CriticalSuccess)
                    {
                        QEffect flatFooted = QEffect.FlatFooted("Cornered Fury");
                        flatFooted.IsFlatFootedTo = (QEffect qf, Creature? attacker, CombatAction? power) =>
                        {
                            if (attacker != null && attacker == qeffect.Owner)
                            {
                                return "Cornered Fury";
                            }

                            return null;
                        };
                        flatFooted.Name += " (Cornered Fury)";
                        flatFooted.Description = $"You have a -2 circumstance penalty to AC against {qeffect.Owner.Name} until the start of your next turn.";
                        flatFooted.ExpiresAt = ExpirationCondition.ExpiresAtStartOfYourTurn;
                        action.Owner.AddQEffect(flatFooted);
                    }
                };
            });
        }

        /// <summary>
        /// Adds the Gnaw Logic
        /// </summary>
        /// <param name="gnawFeat">The Gnaw feat</param>
        public static void AddGnawLogic(TrueFeat gnawFeat)
        {
            gnawFeat.WithPermanentQEffect("Your jaws deal an additional die of damage against constructs.", delegate (QEffect self)
            {
                self.YouDealDamageEvent = async (QEffect qfDamage, DamageEvent damageEvent) =>
                {
                    Creature owner = qfDamage.Owner;
                    if (damageEvent.CombatAction != null && damageEvent.CombatAction.Item != null && damageEvent.CombatAction.Item.Name.ToLower().Contains("ratfolk sharp teeth") && damageEvent.TargetCreature.HasTrait(Trait.Construct) || damageEvent.TargetCreature.HasTrait(Trait.Object))
                    {
                        damageEvent.KindedDamages.Add(new KindedDamage(DiceFormula.FromText("1d6", "Gnaw"), damageEvent.KindedDamages[0].DamageKind));
                    }
                };
            });
        }

        /// <summary>
        /// Adds the Lab Rat Logic
        /// </summary>
        /// <param name="labRatFeat">The Lab Rat feat</param>
        public static void AddLabRatLogic(TrueFeat labRatFeat)
        {
            labRatFeat.WithPermanentQEffect("You have a +1 circumstance bonus against poisons and elixirs", delegate (QEffect self)
            {
                self.BonusToDefenses = (QEffect defenseBonus, CombatAction? action, Defense defense) =>
                {
                    if (action != null && (action.HasTrait(Trait.Poison) || action.HasTrait(Trait.Elixir)))
                    {
                        return new Bonus(1, BonusType.Circumstance, "Lab Rat", true);
                    }

                    return null;
                };
                self.AdjustSavingThrowCheckResult = (QEffect qfCheckUp, Defense defense, CombatAction action, CheckResult checkResult) =>
                {
                    if (checkResult == CheckResult.Success && (action.HasTrait(Trait.Poison) || action.HasTrait(Trait.Elixir)))
                    {
                        return CheckResult.CriticalSuccess;
                    }

                    return checkResult;
                };
            });
        }

        /// <summary>
        /// Adds the Quick Stow Logic
        /// </summary>
        /// <param name="quickStowFeat">The Quick Stow feat</param>
        public static void AddQuickStowLogic(TrueFeat quickStowFeat)
        {
            quickStowFeat.WithPermanentQEffect("You can stow as a free action.", delegate (QEffect self)
            {
                ModManager.RegisterActionOnEachActionPossibility(action =>
                {
                    if (action.Owner.HasFeat(RatfolkFeatNames.QuickStow) && action.Name.ToLower().StartsWith("stow"))
                    {
                        action.ActionCost = 0;
                    }
                });
            });
        }

        /// <summary>
        /// Adds the Rat Magic Logic
        /// </summary>
        /// <param name="ratMagicFeat">The Rat Magic feat</param>
        public static void AddRatMagicLogic(TrueFeat ratMagicFeat)
        {
            ratMagicFeat.WithOnSheet((character) =>
            {
                character.SetProficiency(Trait.Spell, Proficiency.Trained);
                character.InnateSpells.GetOrCreate(RatfolkTraits.Ratfolk, () => new InnateSpells(Trait.Primal));
                character.AddSelectionOption(new AddInnateSpellOption("ExtraRatfolkCantrip", "Rat magic cantrip", -1, RatfolkTraits.Ratfolk, 0, spell => spell.HasTrait(Trait.Primal)));
            });
        }

        /// <summary>
        /// Asyncronisly gets the first creature in the tumble through path
        /// </summary>
        /// <param name="self">The creature tumbling through</param>
        /// <param name="tile">The end goal time</param>
        /// <returns>The first creature tumbled through or null</returns>
        private static async Task<Creature?> GetTumbleThroughCreature(Creature self, Tile tile)
        {
            MovementStyle movementStyle = new MovementStyle()
            {
                MaximumSquares = self.Speed
            };
            PathfindingDescription pathfindingDescription = new PathfindingDescription()
            {
                Squares = movementStyle.MaximumSquares,
                Style = movementStyle
            };
            IList<Tile>? pathToTiles = Pathfinding.GetPath(self, tile, tile.Battle, pathfindingDescription);

            // After pathfinding the first non-friendly creature is returned
            if (pathToTiles != null)
            {
                foreach (Tile pathTile in pathToTiles)
                {
                    Creature? tileOccupant = pathTile.PrimaryOccupant;
                    if (tileOccupant != null && tileOccupant != self && !self.FriendOf(tileOccupant))
                    {
                        return pathTile.PrimaryOccupant;
                    }
                }
            }

            return null;
        }
    }
}