using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.Enums;
using Dawnsbury.Display.Illustrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dawnsbury.Modding;
using Dawnsbury.Core;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge
{
    public class Implement : Item
    {
        public Implement(ItemName itemName, Illustration illustration, string name, string description) : base(itemName, illustration, name, 0, 0, [Trait.DoNotAddToShop, ThaumaturgeTraits.Implement])
        {
            this.Description = description;
        }
    }
}
