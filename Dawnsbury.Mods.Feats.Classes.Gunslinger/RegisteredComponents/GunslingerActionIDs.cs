using Dawnsbury.Core.CombatActions;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Classes.Gunslinger.RegisteredComponents
{
    /// <summary>
    /// A static class containing all Feat Names used for the Gunslinger
    /// </summary>
    public static class GunslingerActionIDs
    {
        public static readonly ActionId WarningShot = ModManager.RegisterEnumMember<ActionId>("Warning Shot AID");
    }
}
