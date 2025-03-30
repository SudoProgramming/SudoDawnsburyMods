using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Items.Firearms.RegisteredComponents
{
    public class FirearmRuneKind
    {
        /// <summary>
        /// The technical Bayonet persistent Rune Kind ID 
        /// </summary>
        public static readonly RuneKind Bayonet = ModManager.RegisterEnumMember<RuneKind>("Bayonet Rune Kind ID");
    }
}
