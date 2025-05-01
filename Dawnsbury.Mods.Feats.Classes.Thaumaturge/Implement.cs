using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.Enums;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge
{
    /// <summary>
    /// The Implement class used for all Implement Items
    /// </summary>
    public class Implement : Item
    {
        /// <summary>
        /// Gets or sets the Implement ID
        /// </summary>
        public ImplementIDs ImplementID { get; set; }

        /// <summary>
        /// Initalizes an instance of the <see cref="Implement" class/>
        /// </summary>
        /// <param name="implementID">The Implement ID</param>
        /// <param name="itemName">The item name</param>
        /// <param name="illustration">The illustration</param>
        /// <param name="name">The implement name</param>
        /// <param name="description">The implement description</param>
        public Implement(ImplementIDs implementID, ItemName itemName, Illustration illustration, string name, string description) : base(itemName, illustration, name, 0, 0, [Trait.DoNotAddToShop, ThaumaturgeTraits.Implement])
        {
            this.ImplementID = implementID;
            this.Description = description;
            this.WithStoresItem((Item implement, Item storedItem) =>
            {
                if (implement.StoredItems.Count >= 1)
                {
                    return "Already holding a scroll";
                }
                else if (storedItem.ScrollProperties == null)
                {
                    return "Implements can hold a single scroll for the 'Scroll Thaumaturgy' feat. Normal item swapping isn't allowed.";
                }

                return null;
            });
        }
    }
}
