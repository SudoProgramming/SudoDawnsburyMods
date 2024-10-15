using Dawnsbury.Core.CombatActions;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents
{
    public class ThaumaturgeActionIDs
    {
        /// <summary>
        /// The Exploit Vulnerability persistent Action ID
        /// </summary>
        public static readonly ActionId ExploitVulnerability = ModManager.RegisterEnumMember<ActionId>("Exploit Vulnerability AID");
    }
}
