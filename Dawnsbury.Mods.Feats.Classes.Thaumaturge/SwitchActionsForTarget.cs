using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge
{
    public class SwitchActionsForTarget
    {
        public Delegates.EffectOnChosenTargets? OriginalEffectOnChosenTargets { get; set; }

        public Delegates.EffectOnEachTarget? OriginalEffectOnEachTarget { get; set; }

        public SwitchActionsForTarget()
        {
        }
    }
}
