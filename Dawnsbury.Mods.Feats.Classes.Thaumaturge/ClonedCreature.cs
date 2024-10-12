using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Core.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge
{
    public class ClonedCreature : Creature
    {
        public ClonedCreature(Creature baseCreature) : base(baseCreature.Illustration, baseCreature.Name, baseCreature.Traits.ToList(), baseCreature.Level, baseCreature.Perception, baseCreature.Speed, new Defenses(0,0,0,0), baseCreature.HP, new Abilities(0,0,0,0,0,0), new Skills())
        {
            baseCreature.AddQEffect(new QEffect(ExpirationCondition.Never)
            {
                StateCheck = (QEffect stateCheck) =>
                {
                    if (this != baseCreature)
                    {
                    }
                }
            });
        }
    }
}
