using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents;
using System.Linq;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge
{
    public class ImplementAndHeldItem : Item
    {
        public Item Implement { get; set; }

        public string OriginalImplementName { get; set; }

        public Item HeldItem { get; set; }

        public ImplementAndHeldItem(Item implement, Item heldItem) : base(new SideBySideIllustration((implement.Illustration is SideBySideIllustration sideBySide) ? sideBySide.Left : implement.Illustration, heldItem.Illustration), heldItem.Name, heldItem.Traits.Concat([ThaumaturgeTraits.Implement]).ToArray())
        {
            Implement = implement;
            OriginalImplementName = implement.Name;
            HeldItem = heldItem;
        }
    }
}
