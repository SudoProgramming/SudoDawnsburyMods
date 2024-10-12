using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Display.Illustrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge
{
    public class MirrorClone : Creature
    {
        public MirrorClone(Illustration illustration, string baseName, Traits traits, int level, int perception, int speed, Defenses defenses, int hp, Abilities abilities, Skills skills) : base(illustration, baseName + " (Mirror Clone)", traits.ToList(), level, perception, speed, defenses, hp, abilities, skills)
        {

        }
    }
}
