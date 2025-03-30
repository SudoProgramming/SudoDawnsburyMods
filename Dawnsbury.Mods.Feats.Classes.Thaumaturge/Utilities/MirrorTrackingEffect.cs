using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.Extensions;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge.Utilities
{
    /// <summary>
    /// A tracking QEffect for the Mirror Implement
    /// </summary>
    public class MirrorTrackingEffect : QEffect
    {
        /// <summary>
        /// The paired creature either the original or the clone
        /// </summary>
        public Creature PairedCreature { get; set; }

        /// <summary>
        /// The last tracked tile from this creature
        /// </summary>
        public Tile LastLocation { get; set; }

        /// <summary>
        /// The last know HP
        /// </summary>
        public int LastHP { get; set; }

        /// <summary>
        /// The last know Temp HP
        /// </summary>
        public int LastTempHP { get; set; }

        /// <summary>
        /// The tile in which the clone died in
        /// </summary>
        public Tile? CloneDeathTile { get; set; }

        /// <summary>
        /// The event for Movement
        /// </summary>
        public event Action<Creature> OnMove;

        /// <summary>
        /// The event for Unconscious
        /// </summary>
        public event Func<Creature, Task> OnUnconscious;

        /// <summary>
        /// The event for HP changes
        /// </summary>
        public event Action<Creature> OnHPChange;

        /// <summary>
        /// The event on damage taken
        /// </summary>
        public event Action<Creature, int> OnDamageTaken;

        /// <summary>
        /// The event of acquired QEffect
        /// </summary>
        public event Action<QEffect, Creature> OnAcquireQEffect;

        /// <summary>
        /// A list of QEffects that have been passed on
        /// </summary>
        public List<QEffect> PassedOnQEffects;

        /// <summary>
        /// Initazlies an instance of the <see cref="MirrorTrackingEffect" class/>
        /// </summary>
        /// <param name="self">The original creature</param>
        /// <param name="pairedCreature">The paired creature</param>
        public MirrorTrackingEffect(Creature self, Creature pairedCreature) : base()
        {
            // Initalizes all values
            PairedCreature = pairedCreature;
            LastHP = self.HP;
            LastTempHP = self.TemporaryHP;
            LastLocation = self.Occupies;
            PassedOnQEffects = new List<QEffect>();
            CloneDeathTile = null;
            Id = ThaumaturgeQEIDs.MirrorTracking;
            ExpiresAt = ExpirationCondition.Never;
            RoundsLeft = 2;

            // On state check the location, HP, Temp HP and QEffects are all updated
            StateCheck = async (QEffect stateCheck) =>
            {
                if (this.Owner.Occupies != LastLocation)
                {
                    OnMove?.Invoke(this.Owner);
                }
                if (this.Owner.HP != LastHP || this.Owner.TemporaryHP != LastTempHP)
                {
                    LastHP = this.Owner.HP;
                    LastTempHP = this.Owner.TemporaryHP;
                    OnHPChange?.Invoke(this.Owner);
                }
                List<QEffect> qEffectsToUpdate = this.Owner.QEffects.Where(qe => qe.Owner == PairedCreature).ToList();
                foreach (QEffect qEffect in qEffectsToUpdate)
                {
                    qEffect.Owner = this.Owner;
                }
            };

            // Forces the selection of which tile contains the real user if there is a clone death tile
            StateCheckWithVisibleChanges = async (QEffect stateCheck) =>
            {
                if (CloneDeathTile != null)
                {
                    await this.Owner.ChooseWhichVersionIsReal(CloneDeathTile);
                    CloneDeathTile = null;
                    this.Owner.Battle.RemoveCreatureFromGame(PairedCreature);
                }
            };

            // On action, the reaction use is passed on and On Move if the trait has the 'Move' trait
            AfterYouTakeAction = async (QEffect afterYouTakeAction, CombatAction action) =>
            {
                if (action.ActionCost == -1)
                {
                    PairedCreature.Actions.UseUpReaction();
                }
                if (action.HasTrait(Trait.Move))
                {
                    OnMove?.Invoke(this.Owner);
                }
            };

            // Pass on acquired QEffects to the paired creature
            YouAcquireQEffect = (QEffect youAcquireEffect, QEffect acquiredEffect) =>
            {
                if (acquiredEffect.Id == ThaumaturgeQEIDs.MirrorImmunity)
                {
                    return acquiredEffect;
                }
                else if (PassedOnQEffects.Contains(acquiredEffect))
                {
                    PassedOnQEffects.Remove(acquiredEffect);
                }
                else
                {
                    QEffect? pairedMirrorTrackingEffect = PairedCreature.FindQEffect(ThaumaturgeQEIDs.MirrorTracking);
                    if (pairedMirrorTrackingEffect != null && pairedMirrorTrackingEffect is MirrorTrackingEffect effectToUpdate)
                    {
                        effectToUpdate.PassedOnQEffects.Add(acquiredEffect);
                        if (acquiredEffect.Id == QEffectId.Unconscious)
                        {
                            if (!(this.Owner is MirrorClone))
                            {
                                CloneDeathTile = PairedCreature.Occupies;
                            }

                            OnUnconscious?.Invoke(this.Owner);
                        }
                        else
                        {
                            OnAcquireQEffect?.Invoke(acquiredEffect, this.Owner);
                        }
                    }
                }

                return acquiredEffect;
            };

            // Handles the effect expiring at the start of turn
            StartOfYourPrimaryTurn = async (QEffect startOfTurn, Creature self) =>
            {
                if (!(self is MirrorClone))
                {
                    await self.ChooseWhichVersionIsReal(pairedCreature.Occupies);
                    PairedCreature.RemoveAllQEffects(qe => qe.Id == ThaumaturgeQEIDs.MirrorTracking);
                    self.Battle.RemoveCreatureFromGame(pairedCreature);
                    self.RemoveAllQEffects(qe => qe.Id == ThaumaturgeQEIDs.MirrorTracking);
                }
            };

            // Handles all targets
            YouAreTargeted = async (QEffect youAreTargeted, CombatAction action) =>
            {
                this.Owner.HandleTarget(PairedCreature, action);
            };

            // Handles all damage taken
            AfterYouTakeDamage = async (QEffect qeffect, int amount, DamageKind kind, CombatAction? action, bool critical) =>
            {
                OnDamageTaken?.Invoke(this.Owner, amount);
            };
        }
    }
}
