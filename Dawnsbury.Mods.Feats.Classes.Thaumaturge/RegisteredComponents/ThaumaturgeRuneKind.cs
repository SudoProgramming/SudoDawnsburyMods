using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents
{
    public class ThaumaturgeRuneKind
    {
        /// <summary>
        /// The technical Weapon Implement persistent Rune Kind ID 
        /// </summary>
        public static readonly RuneKind WeaponImplement = ModManager.RegisterEnumMember<RuneKind>("Weapon Implement Kind ID");
    }
}
