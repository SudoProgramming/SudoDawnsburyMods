using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents;
using System.Linq;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge
{
    /// <summary>
    /// An Item representing an Implement and some Held Item
    /// </summary>
    public class ImplementAndHeldItem : Item
    {
        /// <summary>
        /// The Implement item
        /// </summary>
        public Item Implement { get; set; }

        /// <summary>
        /// The original Implement name
        /// </summary>
        public string OriginalImplementName { get; set; }

        /// <summary>
        /// The held item
        /// </summary>
        public Item HeldItem { get; set; }

        public bool WasStoredItem { get; set; }

        /// <summary>
        /// Initalizes an instance of the <see cref="ImplementAndHeldItem" class/>
        /// </summary>
        /// <param name="implement">The Implement</param>
        /// <param name="heldItem">The held Item</param>
        public ImplementAndHeldItem(Item implement, Item heldItem, bool wasStoredItem = false) : base(new SideBySideIllustration((implement.Illustration is SideBySideIllustration sideBySide) ? sideBySide.Left : implement.Illustration, heldItem.Illustration), heldItem.Name, heldItem.Traits.Concat([ThaumaturgeTraits.Implement]).ToArray())
        {
            Implement = implement;
            OriginalImplementName = implement.Name;
            HeldItem = heldItem;
            WasStoredItem = wasStoredItem;
        }
    }
}
