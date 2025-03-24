using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Display.Illustrations;
using System.Linq;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge
{
    /// <summary>
    /// A Mirror clone from the Mirror Implement
    /// </summary>
    public class MirrorClone : Creature
    {
        /// <summary>
        /// Initalizes an instance of the <see cref="MirrorClone" class/>
        /// </summary>
        /// <param name="illustration">The clone's illustration</param>
        /// <param name="baseName">The clone's base name</param>
        /// <param name="traits">The clone's traits</param>
        /// <param name="level">The clone's level</param>
        /// <param name="perception">The clone's perception value</param>
        /// <param name="speed">The clone's speed</param>
        /// <param name="defenses">The clone's defenses</param>
        /// <param name="hp">The clone's hp value</param>
        /// <param name="abilities">The clone's abilities</param>
        /// <param name="skills">The clone's skills</param>
        public MirrorClone(Illustration illustration, string baseName, Traits traits, int level, int perception, int speed, Defenses defenses, int hp, Abilities abilities, Skills skills) : base(illustration, baseName + " (Mirror Clone)", traits.ToList(), level, perception, speed, defenses, hp, abilities, skills) { }
    }
}
