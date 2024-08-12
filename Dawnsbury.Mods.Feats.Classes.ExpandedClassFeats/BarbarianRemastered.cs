using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Roller;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.BarbarianFeatsDb;

namespace Dawnsbury.Mods.Feats.Classes.ExpandedClassFeats
{
    /// <summary>
    /// Updates and loads the Remastered changes into the game for the Barbarian
    /// </summary>
    public static class BarbarianRemastered
    {
        /// <summary>
        /// Creates the Remastered Barbaian Feats
        /// </summary>
        /// <returns>The Enumerable of Barbarian Feats</returns>
        public static IEnumerable<Feat> CreateRemasteredBarbarianFeats()
        {
            // All Remastered Dragon Instincts
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Adamantine dragon"), DamageKind.Bludgeoning);
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Conspirator dragon"), DamageKind.Poison);
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Diabolic dragon"), DamageKind.Fire);
            // TODO: When spirit damage is added ad Empureal
            //yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Empureal dragon"), DamageKind.Spirit);
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Fortune dragon"), DamageKind.Force);
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Horned dragon"), DamageKind.Poison);
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Mirage dragon"), DamageKind.Mental);
            yield return new DragonInstinctFeat(ModManager.RegisterFeatName("Omen dragon"), DamageKind.Mental);

            // Class Level 4 Feat - Scars of Steel
            yield return new TrueFeat(ModManager.RegisterFeatName("Scars of Steel"), 4, "When you are struck with the mightiest of blows, you can flex your muscles to turn aside some of the damage.", "Once per day, when an opponent critically hits you with an attack that deals physical damage, you can spend a reaction to gain resistance to the triggering attack equal to your Constitution modifier plus half your level.", new Trait[] { Trait.Barbarian, Trait.Rage })
            .WithActionCost(-2).WithPermanentQEffect("You gain resistance to the triggering attack equal to your Constitution modifier plus half your level as a reaction.", delegate (QEffect qf)
            {
                // Checks the incoming damage and prompts for reaction if it's a crit and physical. Then applies the damage reduction
                qf.YouAreDealtDamage = async (QEffect qEffect, Creature attacker, DamageStuff damage, Creature defender) =>
                {
                    int possibleResistance = qEffect.Owner.Abilities.Constitution + (int)Math.Floor(qEffect.Owner.Level / 2.0);
                    if (damage.Kind.IsPhysical() && damage.Power != null && damage.Power.CheckResult == CheckResult.CriticalSuccess && damage.Power.HasTrait(Trait.Attack) && await qf.Owner.Battle.AskToUseReaction(qf.Owner, "You were critically hit for a total damage of " + damage.Amount + ".\nUse Scars of Steel to gain " + possibleResistance + " damage resistence?"))
                    {
                        return new ReduceDamageModification(possibleResistance, "You reduced " + possibleResistance + " damage from the incoming damage.");
                    }

                    return null;
                };
            });
        }
    }
}
