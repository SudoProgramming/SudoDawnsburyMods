using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge.Utilities
{
    public class MirrorManager
    {
        private Creature owner;

        private bool isOriginal;

        public static void SubscribeToAllMirrorTrackingEffects(MirrorManager manager, MirrorTrackingEffect effect)
        {
            effect.OnMove += manager.HandleMovement;
            effect.OnHPChange += manager.HandleHPChange;
            effect.OnAcquireQEffect += manager.HandleQEffect;
            effect.OnTarget += manager.HandleTarget;
            effect.OnUnconscious += manager.HandleUnconscious;
        }

        public MirrorManager(Creature owner, bool isOriginal)
        {
            this.isOriginal = isOriginal;
            this.owner = owner;
        }

        public void HandleMovement(Creature pairedCreature)
        {
            if (this.isOriginal)
            {
                // Handle Swap
            }
            else
            {
                pairedCreature.RemoveAllQEffects(qe => qe is MirrorTrackingEffect mirrorTrackingEffect && mirrorTrackingEffect.PairedCreature == this.owner);
                this.owner.Battle.RemoveCreatureFromGame(this.owner);
            }
        }

        public void HandleHPChange(Creature pairedCreature)
        {
            if (owner.Damage != pairedCreature.Damage)
            {
                this.owner.SetDamageImmediately(pairedCreature.Damage);
            }
            if (owner.TemporaryHP != pairedCreature.TemporaryHP)
            {
                this.owner.TemporaryHP = pairedCreature.TemporaryHP;
            }
        }

        public void HandleQEffect(QEffect effect, Creature pairedCreature)
        {
            // Check for more here
            //QEffectId qEffectId = effect.Id;
            //switch (qEffectId)
            //{
            //    case QEffectId.Dying:
            //        this.owner.AddQEffect(QEffect.Dying(effect.Value, this.owner.HasFeat(FeatName.Diehard)));
            //        break;
            //}
            if (this.isOriginal)
            {
                effect.Owner = this.owner;
            }
            this.owner.AddQEffect(effect);
        }

        public void HandleTarget(Creature pairedCreature, CombatAction action)
        {

        }

        public void HandleUnconscious(Creature pairedCreature)
        {
            if (this.isOriginal)
            {
                Tile tempTile = owner.Occupies;
                owner.Occupies = pairedCreature.Occupies;
                pairedCreature.Occupies = tempTile;

                owner.Battle.Map.Tiles[owner.Occupies.X, owner.Occupies.Y].PrimaryOccupant = owner;
                pairedCreature.Battle.Map.Tiles[pairedCreature.Occupies.X, pairedCreature.Occupies.Y].PrimaryOccupant = pairedCreature;

                foreach (QEffect qEffect in owner.QEffects.Where(qe => qe is MirrorTrackingEffect mirrorTrackingEffect && mirrorTrackingEffect.PairedCreature == this.owner))
                {
                    qEffect.ExpiresAt = ExpirationCondition.EphemeralAtEndOfImmediateAction;
                };

                owner.Battle.RemoveCreatureFromGame(pairedCreature);
                owner.FallUnconscious();
            }
            else
            {
                foreach (QEffect qEffect in pairedCreature.QEffects.Where(qe => qe is MirrorTrackingEffect mirrorTrackingEffect && mirrorTrackingEffect.PairedCreature == this.owner))
                {
                    qEffect.ExpiresAt = ExpirationCondition.EphemeralAtEndOfImmediateAction;
                };

                this.owner.Battle.RemoveCreatureFromGame(this.owner);
            }
        }
    }
}
