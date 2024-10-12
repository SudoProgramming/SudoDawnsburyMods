using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge.Utilities
{
    public class MirrorTrackingEffect : QEffect
    {
        public Creature PairedCreature { get; set; }

        public Tile LastLocation { get; set; }

        public int LastHP { get; set; }

        public int LastTempHP { get; set; }

        public event Action<Creature> OnMove;

        public event Action<Creature> OnUnconscious;

        public event Action<Creature> OnHPChange;

        public event Action<QEffect, Creature> OnAcquireQEffect;

        public event Action<Creature, CombatAction> OnTarget;

        public List<QEffect> PassedOnQEffects;

        public MirrorTrackingEffect(Creature self, Creature pairedCreature) : base()
        {
            PairedCreature = pairedCreature;
            LastHP = self.HP;
            LastTempHP = self.TemporaryHP;
            LastLocation = self.Occupies;
            PassedOnQEffects = new List<QEffect>();

            Id = ThaumaturgeQEIDs.MirrorTracking;
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
            };
            YouAcquireQEffect = (QEffect youAcquireEffect, QEffect acquiredEffect) =>
            {
                if (PassedOnQEffects.Contains(acquiredEffect))
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
            YouAreTargeted = async (QEffect youAreTargeted, CombatAction action) =>
            {
                OnTarget?.Invoke(this.Owner, action);
            };
        }
    }
}
