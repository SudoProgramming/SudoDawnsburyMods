using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Display.Illustrations;
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
