using Dawnsbury.Core;
using Dawnsbury.Core.Animations.Movement;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Modding;
using Dawnsbury.Mods.Feats.Ancestries.Ratfolk.RegisteredComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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

                self.AdditionalUnarmedStrike = new Item(IllustrationName.Jaws, "Ratfolk Sharp Teeth", sharpTeethTraits.ToArray())
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
                self.AdjustSavingThrowResult = (QEffect effectBonus, CombatAction action, CheckResult result) =>
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
        /// Asyncronisly gets the first creature in the tumble through path
        /// </summary>
        /// <param name="self">The creature tumbling through</param>
        /// <param name="tile">The end goal time</param>
        /// <returns>The first creature tumbled through or null</returns>
        private static async Task<Creature?> GetTumbleThroughCreature(Creature self, Tile tile)
        {
            // HACK: Pathfinding is internal so this should be updated when and if it is made public
            MovementStyle movementStyle = new MovementStyle()
            {
                MaximumSquares = self.Speed
            };
            PathfindingDescription pathfindingDescription = new PathfindingDescription()
            {
                Squares = movementStyle.MaximumSquares,
                Style = movementStyle
            };
            IList<Tile>? pathToTiles = (IList<Tile>?)(typeof(ModManager).Assembly.GetType("Dawnsbury.Core.Intelligence.Pathfinding").GetMethod("GetPath", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Invoke(null, [self, tile, tile.Battle, pathfindingDescription]));

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