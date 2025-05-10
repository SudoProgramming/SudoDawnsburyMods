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

        /// <summary>
        /// The technical Reinforced Stock persistent Rune Kind ID
        /// </summary>
        public static readonly RuneKind ReinforcedStock = ModManager.RegisterEnumMember<RuneKind>("Reinforced Stock Rune Kind ID");

        /// <summary>
        /// The technical Firearm Stabalizer persistent Rune Kind ID 
        /// </summary>
        public static readonly RuneKind FirearmStabalizer = ModManager.RegisterEnumMember<RuneKind>("Firearm Stabalizer Rune Kind ID");

        /// <summary>
        /// The technical Tripod persistent Rune Kind ID 
        /// </summary>
        public static readonly RuneKind Tripod = ModManager.RegisterEnumMember<RuneKind>("Tripod Rune Kind ID");
    }
}
