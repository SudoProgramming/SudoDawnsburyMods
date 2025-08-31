using Dawnsbury.Core.Mechanics;
using Dawnsbury.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Classes.Gunslinger.RegisteredComponents
{
    /// <summary>
    /// A static class containing all QEffect IDs used for the Gunslinger
    /// </summary>
    public static class GunslingerQEIDs
    {
        /// <summary>
        /// The Hit the Dirt persistent QEffect ID
        /// </summary>
        public static readonly QEffectId HitTheDirt = ModManager.RegisterEnumMember<QEffectId>("Hit the Dirt QEID");

        /// <summary>
        /// The Sword and Pistol Ranged Buff persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId SwordAndPistolRangedBuff = ModManager.RegisterEnumMember<QEffectId>("Sword and Pistol - Ranged QEID");

        /// <summary>
        /// The Sword and Pistol Melee Buff persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId SwordAndPistolMeleeBuff = ModManager.RegisterEnumMember<QEffectId>("Sword and Pistol - Melee QEID");

        /// <summary>
        /// The Crossbow Crack Shot persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId CrossbowCrackShot = ModManager.RegisterEnumMember<QEffectId>("Crossbow Crack Shot QEID");

        /// <summary>
        /// The Fake Out persistent QEffect ID
        /// </summary>
        public static readonly QEffectId FakeOut = ModManager.RegisterEnumMember<QEffectId>("Fake Out QEID");

        /// <summary>
        /// The One Shot, One Kill persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId OneShotOneKill = ModManager.RegisterEnumMember<QEffectId>("One Shot, One Kill QEID");

        /// <summary>
        /// The Clear a Path persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId ClearAPath = ModManager.RegisterEnumMember<QEffectId>("Clear a Path QEID");

        /// <summary>
        /// The Pistolero's Challenge persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId PistolerosChallenge = ModManager.RegisterEnumMember<QEffectId>("Pistolero's Challenge QEID");

        /// <summary>
        /// The Finish the Job persistent QEffect ID 
        /// </summary>
        public static readonly QEffectId FinishTheJob = ModManager.RegisterEnumMember<QEffectId>("Finish the Job QEID");
    }
}
