using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents
{
    internal class ThaumaturgeDamageKinds
    {
        /// <summary>
        /// The Personal Antithesis Damage Kind
        /// </summary>
        public static readonly DamageKind PersonalAntithesis = ModManager.RegisterEnumMember<DamageKind>("Personal Antithesis");
    }
}
