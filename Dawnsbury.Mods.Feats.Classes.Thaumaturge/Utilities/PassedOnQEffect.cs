using Dawnsbury.Core.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge.Utilities
{
    public class PassedOnQEffect : QEffect
    {
        public QEffect OriginalQEffect { get; set; }

        public PassedOnQEffect(QEffect passedOnQEffect) : base()
        {
            OriginalQEffect = passedOnQEffect;
        }
    }
}
