using Dawnsbury.Core;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Coroutines.Options;
using Dawnsbury.Core.Coroutines.Requests;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge.Extensions
{
    public static class ThaumaturgeExtensions
    {
        public static void SubscribeToAll(this Creature self, MirrorTrackingEffect effect)
        {
            effect.OnMove += self.HandleMovement;
            effect.OnHPChange += self.HandleHPChange;
            effect.OnAcquireQEffect += self.HandleQEffect;
            effect.OnTarget += self.HandleTarget;
            effect.OnUnconscious += self.HandleUnconscious;
        }

        public static void SubscribeToAll(this Creature self, Creature pairedCreature)
        {
            QEffect? effect = pairedCreature.FindQEffect(ThaumaturgeQEIDs.MirrorTracking);
            if (effect != null)
            {
                self.SubscribeToAll((MirrorTrackingEffect)effect);
            }
        }

        public static void UnsubscribeToAll(this Creature self, MirrorTrackingEffect effect)
        {
            effect.OnMove -= self.HandleMovement;
            effect.OnHPChange -= self.HandleHPChange;
            effect.OnAcquireQEffect -= self.HandleQEffect;
            effect.OnTarget -= self.HandleTarget;
            effect.OnUnconscious -= self.HandleUnconscious;
        }

        public static void UnsubscribeToAll(this Creature self, Creature pairedCreature)
        {
            QEffect? effect = pairedCreature.FindQEffect(ThaumaturgeQEIDs.MirrorTracking);
            if (effect != null)
            {
                self.UnsubscribeToAll((MirrorTrackingEffect)effect);
            }
        }

        public static void HandleMovement(this Creature self, Creature pairedCreature)
        {
            self.UnsubscribeToAll(pairedCreature);
            pairedCreature.UnsubscribeToAll(self);
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

        public static void HandleHPChange(this Creature self, Creature pairedCreature)
        {
            if (self.Damage != pairedCreature.Damage)
            {
                self.SetDamageImmediately(pairedCreature.Damage);
            }
            if (self.TemporaryHP != pairedCreature.TemporaryHP)
            {
                self.TemporaryHP = pairedCreature.TemporaryHP;
            }
        }

        public static void HandleQEffect(this Creature self, QEffect effect, Creature pairedCreature)
        {
            if (!self.HasEffect(effect))
            {
                self.AddQEffect(effect);
            }
        }

        public static void HandleTarget(this Creature self, Creature pairedCreature, CombatAction action)
        {
            Target target = action.Target;
            ChosenTargets chosenTargets = action.ChosenTargets;
            if (self is MirrorClone)
            {
                if (target.IsAreaTarget && chosenTargets.ChosenCreatures.Contains(pairedCreature))
                {
                    chosenTargets.ChosenCreatures.RemoveAll(creature => creature == self);
                }
            }
        }

        public static async Task HandleUnconscious(this Creature self, Creature pairedCreature)
        {
            if (!(self is MirrorClone))
            {
                self.FallUnconscious();
                pairedCreature.UnsubscribeToAll(self);
            }
            else
            {
                self.UnsubscribeToAll(pairedCreature);
            }
        }

        public static async Task ChooseWhichVersionIsReal(this Creature self, Tile clonesDeathTile)
        {
            Tile? chosenTile = await GetTilesForSelfAndClone(self, clonesDeathTile);
            if (chosenTile != null && self.Occupies != chosenTile)
            {
                self.TranslateTo(chosenTile);
                self.AnimationData.ActualPosition = new Vector2(chosenTile.X, chosenTile.Y);
            }
        }

        private static async Task<Tile?> GetTilesForSelfAndClone(Creature self, Tile clonesDeathTile)
        {
            string messageString = "Choose which version of " + self.Name + " is real.";

            TileOption originalTile = new TileOption(self.Occupies, "Original", null, (AIUsefulness)int.MinValue, true);
            TileOption pairedTile = new TileOption(clonesDeathTile, "Clone", null, (AIUsefulness)int.MinValue, true);
            List<Option> options = new List<Option>() { originalTile, pairedTile };

            // Prompts the user to select a valid tile and returns it or null
            Option selectedOption = (await self.Battle.SendRequest(new AdvancedRequest(self, messageString, options)
            {
                IsMainTurn = false,
                IsStandardMovementRequest = false,
                TopBarIcon = IllustrationName.GenericCombatManeuver,
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
