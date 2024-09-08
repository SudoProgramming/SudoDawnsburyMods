using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Ancestries.Gnome.RegisteredComponents
{
    public static class GnomeTraits
    {
        /// <summary>
        /// The Gnome class trait 
        /// </summary>
        public static readonly Trait Gnome = ModManager.RegisterTrait("Gnome", new TraitProperties("Gnome", relevant: true) { IsAncestryTrait = true });
    }
}
