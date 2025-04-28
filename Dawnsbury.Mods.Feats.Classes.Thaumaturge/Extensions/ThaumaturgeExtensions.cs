using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Coroutines.Options;
using Dawnsbury.Core.Coroutines.Requests;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Tiles;
using Dawnsbury.IO;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge.Extensions
{
    /// <summary>
    /// A static collection of Extensions used for the Thaumaturge Class
    /// Most used with the <see cref="MirrorTrackingEffect"></see>
    /// </summary>
    public static class ThaumaturgeExtensions
    {
        /// <summary>
        /// Subscribes this to all events in the provided Mirror Tracking Effect
        /// </summary>
        /// <param name="self">This creature</param>
        /// <param name="effect">The mirror tracking effect being subscribed to</param>
        public static void SubscribeToAll(this Creature self, MirrorTrackingEffect effect)
        {
            effect.OnMove += self.HandleMovement;
            effect.OnHPChange += self.HandleHPChange;
            effect.OnDamageTaken += self.HandleDamage;
            effect.OnAcquireQEffect += self.HandleQEffect;
            effect.OnUnconscious += self.HandleUnconscious;
        }

        /// <summary>
        /// Subscribes this to all events in the provided paired creature's Mirror Tracking Effect
        /// </summary>
        /// <param name="self">This creature</param>
        /// <param name="pairedCreature">The paired creature that will be linked</param>
        public static void SubscribeToAll(this Creature self, Creature pairedCreature)
        {
            QEffect? effect = pairedCreature.FindQEffect(ThaumaturgeQEIDs.MirrorTracking);
            if (effect != null)
            {
                self.SubscribeToAll((MirrorTrackingEffect)effect);
            }
        }

        /// <summary>
        /// Unsubscribes this from all events in the provided Mirror Tracking Effect
        /// </summary>
        /// <param name="self">This creature</param>
        /// <param name="effect">The mirror tracking effect being subscribed to</param>
        public static void UnsubscribeToAll(this Creature self, MirrorTrackingEffect effect)
        {
            effect.OnMove -= self.HandleMovement;
            effect.OnHPChange -= self.HandleHPChange;
            effect.OnDamageTaken -= self.HandleDamage;
            effect.OnAcquireQEffect -= self.HandleQEffect;
            effect.OnUnconscious -= self.HandleUnconscious;
        }

        /// <summary>
        /// Unsubscribes this from all events in the provided paired creature's Mirror Tracking Effect
        /// </summary>
        /// <param name="self">This creature</param>
        /// <param name="pairedCreature">The paired creature that will be unlinked</param>
        public static void UnsubscribeToAll(this Creature self, Creature pairedCreature)
        {
            QEffect? effect = pairedCreature.FindQEffect(ThaumaturgeQEIDs.MirrorTracking);
            if (effect != null)
            {
                self.UnsubscribeToAll((MirrorTrackingEffect)effect);
            }
        }

        /// <summary>
        /// When the paired creature moves this will handle the movement 
        /// </summary>
        /// <param name="self">This creature</param>
        /// <param name="pairedCreature">The paired creature linked via the Mirror Implement</param>
        public static void HandleMovement(this Creature self, Creature pairedCreature)
        {
            // Unsubscribes both this and the paired creature from eachother
            self.UnsubscribeToAll(pairedCreature);
            pairedCreature.UnsubscribeToAll(self);

            // Removes only the clone
            if (self is MirrorClone)
            {
                self.Battle.RemoveCreatureFromGame(self);
            }
            else
            {
                // Should not be reachable by normal game logic
                Tile cloneTile = pairedCreature.Occupies;
                self.TranslateTo(cloneTile);
                self.AnimationData.ActualPosition = new Vector2(cloneTile.X, cloneTile.Y);
                self.Battle.RemoveCreatureFromGame(pairedCreature);
            }
        }

        /// <summary>
        /// When the paired creature moves this will handle the HP changes 
        /// </summary>
        /// <param name="self">This creature</param>
        /// <param name="pairedCreature">The paired creature linked via the Mirror Implement</param>
        public static void HandleHPChange(this Creature self, Creature pairedCreature)
        {
            // If the damage or the Temp HP is different from the paired creature
            if (self.Damage != pairedCreature.Damage)
            {
                self.SetDamageImmediately(pairedCreature.Damage);
            }
            if (self.TemporaryHP != pairedCreature.TemporaryHP)
            {
                self.TemporaryHP = pairedCreature.TemporaryHP;
            }
        }

        /// <summary>
        /// When the paired creature moves this will handle the HP changes 
        /// </summary>
        /// <param name="self">This creature</param>
        /// <param name="pairedCreature">The paired creature linked via the Mirror Implement</param>
        /// <param name="damage">The damage taken</param>
        public static void HandleDamage(this Creature self, Creature pairedCreature, int damage)
        {
            // Determines the bounded damage and adjusts based off of Temp HP
            int newDamage = Math.Min(damage + self.Damage, self.MaxHP);
            int adjustedDamage = newDamage - self.TemporaryHP;

            // First removes any Temp HP or reduces the damage by the Temp HP
            if (newDamage != adjustedDamage)
            {
                if (adjustedDamage <= 0)
                {
                    self.TemporaryHP -= damage;
                }
                else
                {
                    newDamage -= self.TemporaryHP;
                }
            }

            // Updates the damage
            self.SetDamageImmediately(newDamage);
        }

        /// <summary>
        /// When this gets a new QEffect it will be passed off to the paired creature
        /// </summary>
        /// <param name="self">This creature</param>
        /// <param name="effect">The QEfect possibly being passed on</param>
        /// <param name="pairedCreature">The paired creature linked via the Mirror Implement</param>
        public static void HandleQEffect(this Creature self, QEffect effect, Creature pairedCreature)
        {
            // Passes on the QEffect unless they already have the effect, it's FlankedBy, or the untyped Golden Candelabra effect
            if (!self.HasEffect(effect) && 
                (effect.Source != null && effect.Source.BaseName != "The Golden Candelabra"))
            {
                self.AddQEffect(effect);
            }
        }

        /// <summary>
        /// When the paired creature is Unconscious this will handle the mirror
        /// </summary>
        /// <param name="self">This creature</param>
        /// <param name="pairedCreature">The paired creature linked via the Mirror Implement</param>
        public static async Task HandleUnconscious(this Creature self, Creature pairedCreature)
        {
            // If this is not the mirror flaa unconscious and unsubscribe from the mirror
            if (!(self is MirrorClone))
            {
                if (Settings.Instance.DropItemsWhenKnockedOut)
                {
                    foreach (var heldItem in self.HeldItems.ToList())
                    {
                        if (heldItem.Grapplee == null)
                        {
                            self.DropItem(heldItem);
                        }
                    }

                    self.HeldItems.RemoveAll(itm => itm.Grapplee == null);
                }

                self.FallUnconscious();
                pairedCreature.UnsubscribeToAll(self);
            }
            // Otherwise the mirror unsubscribes
            else
            {
                self.UnsubscribeToAll(pairedCreature);
            }
        }

        /// <summary>
        /// Chooses between this and the clone's tile to determine which is real version
        /// </summary>
        /// <param name="self">This creature</param>
        /// <param name="cloneTile">The clone's Tile</param>
        /// <returns>A task for async calls</returns>
        public static async Task ChooseWhichVersionIsReal(this Creature self, Tile cloneTile)
        {
            // Prompts the user for which tile and moves this to it
            Tile? chosenTile = await ChooseTileFromSelfAndClone(self, cloneTile);
            if (chosenTile != null && self.Occupies != chosenTile)
            {
                self.TranslateTo(chosenTile);
                self.AnimationData.ActualPosition = new Vector2(chosenTile.X, chosenTile.Y);
            }
        }

        /// <summary>
        /// When the paired creature is targeted this will handle the target if both the original and the clone are targeted
        /// </summary>
        /// <param name="self">This creature</param>
        /// <param name="pairedCreature">The paired creature linked via the Mirror Implement</param>
        /// <param name="action">The action that is targetting</param>
        public static void HandleTarget(this Creature self, Creature pairedCreature, CombatAction action)
        {
            // Adjusts the target based on the number of actions then gets the chosen targets
            Target? target = action.Target is DependsOnActionsSpentTarget doat ? doat.TargetFromActionCount(action.SpentActions) : action.Target;
            ChosenTargets chosenTargets = action.ChosenTargets;

            // If this is the mirror and the both the original and the mirror is targeted, a trait is added to the action and the clone gains immunity to that trait
            if (self is MirrorClone && target != null)
            {
                if ((target.IsAreaTarget) && chosenTargets.ChosenCreatures.Contains(pairedCreature))
                {
                    if (!action.HasTrait(ThaumaturgeTraits.MirrorCloneImmunity))
                    {
                        action.Traits.Add(ThaumaturgeTraits.MirrorCloneImmunity);
                    }
                    self.AddQEffect(new QEffect(ExpirationCondition.EphemeralAtEndOfImmediateAction)
                    {
                        Id = ThaumaturgeQEIDs.MirrorImmunity,
                        ImmuneToTrait = ThaumaturgeTraits.MirrorCloneImmunity,
                        DoNotShowUpOverhead = true
                    });
                }
            }
        }

        /// <summary>
        /// Swaps the position of this with its paired creature
        /// </summary>
        /// <param name="self">This creature</param>
        /// <param name="pairedCreature">The paired creature linked via the Mirror Implement</param>
        /// <returns>True if the creatures were swapped and False otherwise</returns>
        public static bool SwapPositions(this Creature self, Creature pairedCreature)
        {
            // Gets the clone's tile and will do additional logic if it's not null
            Tile cloneTile = pairedCreature.Occupies;
            if (cloneTile != null)
            {
                // Finds both tracking effects and if they are not null their positions will be swapped
                MirrorTrackingEffect? mirrorTracking = self.FindQEffect(ThaumaturgeQEIDs.MirrorTracking) as MirrorTrackingEffect;
                MirrorTrackingEffect? cloneTracking = pairedCreature.FindQEffect(ThaumaturgeQEIDs.MirrorTracking) as MirrorTrackingEffect;
                if (cloneTracking != null && mirrorTracking != null)
                {
                    Vector2 pairedCreaturePosition = new Vector2(cloneTile.X, cloneTile.Y);
                    Vector2 ownerPosition = new Vector2(self.Occupies.X, self.Occupies.Y);
                    cloneTile.PrimaryOccupant = null;
                    Tile ownerTile = self.Battle.Map.Tiles[(int)ownerPosition.X, (int)ownerPosition.Y];
                    ownerTile.PrimaryOccupant = null;
                    pairedCreature.TranslateTo(ownerTile);
                    pairedCreature.AnimationData.ActualPosition = ownerPosition;
                    cloneTracking.LastLocation = ownerTile;
                    self.TranslateTo(self.Battle.Map.Tiles[(int)pairedCreaturePosition.X, (int)pairedCreaturePosition.Y]);
                    self.AnimationData.ActualPosition = pairedCreaturePosition;
                    mirrorTracking.LastLocation = self.Battle.Map.Tiles[(int)pairedCreaturePosition.X, (int)pairedCreaturePosition.Y];

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the selected tile from the two creatures
        /// </summary>
        /// <param name="self">This creature</param>
        /// <param name="otherTile">The tile being choosen between</param>
        /// <returns>The chosen tile</returns>
        private static async Task<Tile?> ChooseTileFromSelfAndClone(Creature self, Tile otherTile)
        {
            // Collects the tiles and offers the options
            string messageString = "Choose which version of " + self.Name + " is real.";

            TileOption originalTile = new TileOption(self.Occupies, "Original", null, (AIUsefulness)int.MinValue, true);
            TileOption pairedTile = new TileOption(otherTile, "Clone", null, (AIUsefulness)int.MinValue, true);
            List<Option> options = new List<Option>() { originalTile, pairedTile };

            // Prompts the user to select a valid tile and returns it or null
            Option selectedOption = (await self.Battle.SendRequest(new AdvancedRequest(self, messageString, options)
            {
                IsMainTurn = false,
                IsStandardMovementRequest = false,
                TopBarIcon = ThaumaturgeModdedIllustrations.Mirror,
                TopBarText = messageString

            })).ChosenOption;

            if (selectedOption != null)
            {
                if (selectedOption is CancelOption cancel)
                {
                    return null;
                }

                return ((TileOption)selectedOption).Tile;
            }

            return null;
        }
    }
}
