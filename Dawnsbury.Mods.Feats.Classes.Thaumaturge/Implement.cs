using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Mods.Feats.Classes.Thaumaturge.RegisteredComponents;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge
{
    /// <summary>
    /// The Implement class used for all Implement Items
    /// </summary>
    public class Implement : Item
    {
        /// <summary>
        /// Initalizes an instance of the <see cref="Implement" class/>
        /// </summary>
        /// <param name="itemName">The item name</param>
        /// <param name="illustration">The illustration</param>
        /// <param name="name">The implement name</param>
        /// <param name="description">The implement description</param>
        public Implement(ItemName itemName, Illustration illustration, string name, string description) : base(itemName, illustration, name, 0, 0, [Trait.DoNotAddToShop, ThaumaturgeTraits.Implement])
        {
            this.Description = description;
        }
    }
}
