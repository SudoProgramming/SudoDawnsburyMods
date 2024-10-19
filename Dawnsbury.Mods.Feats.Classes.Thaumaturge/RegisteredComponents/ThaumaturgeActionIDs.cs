using Dawnsbury.Core.CombatActions;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents
{
    public class ThaumaturgeActionIDs
    {
        /// <summary>
        /// The Exploit Vulnerability persistent Action ID 
        /// </summary>
        public static readonly ActionId ExploitVulnerability = ModManager.RegisterEnumMember<ActionId>("Exploit Vulnerability AID");

        /// <summary>
        /// The Instructive Strike persistent Action ID 
        /// </summary>
        public static readonly ActionId InstructiveStrike = ModManager.RegisterEnumMember<ActionId>("Instructive Strike AID");

        /// <summary>
        /// The Divine Disharmony persistent Action ID 
        /// </summary>
        public static readonly ActionId DivineDisharmony = ModManager.RegisterEnumMember<ActionId>("Divine Disharmony AID");
    }
}
