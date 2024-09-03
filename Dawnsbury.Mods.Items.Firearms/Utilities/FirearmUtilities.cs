using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Mods.Items.Firearms.RegisteredComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Items.Firearms.Utilities
{
    public static class FirearmUtilities
    {
        /// <summary>
        /// Determines if the item is a firearm or a crossbow
        /// </summary>
        /// <param name="item">The item being checked</param>
        /// <returns>True if the item is a firearm or crossbow and false otherwise</returns>
        public static bool IsItemFirearmOrCrossbow(Item item, bool checkIfItsLoaded = false)
        {
            if (item.HasTrait(FirearmTraits.Firearm) || item.HasTrait(Trait.Crossbow))
            {
                if (checkIfItsLoaded)
                {
                    return IsItemLoaded(item);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if the item is loaded
        /// </summary>
        /// <param name="item">The item being checked</param>
        /// <returns>True if the item is loaded and false otherwise</returns>
        public static bool IsItemLoaded(Item item)
        {
            return item.EphemeralItemProperties != null && !item.EphemeralItemProperties.NeedsReload;
        }

        /// <summary>
        /// Determines if the item has a multi ammo reload and if it is reloadable
        /// </summary>
        /// <param name="item">The item being check</param>
        /// <returns>True if the item is a multi ammo reloadable item and false otherwise.</returns>
        public static bool IsMultiAmmoWeaponReloadable(Item item)
        {
            int maxMagazineSize = item.HasTrait(FirearmTraits.DoubleBarrel) ? 2 : 5;
            if ((item.HasTrait(FirearmTraits.DoubleBarrel) || item.HasTrait(Trait.Repeating)) && item.EphemeralItemProperties.AmmunitionLeftInMagazine < maxMagazineSize)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Awaits the asyncronis reload action for the give item
        /// </summary>
        /// <param name="self">The creature weilding the item</param>
        /// <param name="item">The item being reloaded</param>
        public static async void AwaitReloadItem(Creature self, Item item)
        {
            if (item.HasTrait(FirearmTraits.DoubleBarrel))
            {
                item.EphemeralItemProperties.AmmunitionLeftInMagazine++;
                item.EphemeralItemProperties.NeedsReload = false;

            }
            else
            {
                await self.CreateReload(item).WithActionCost(0).WithItem(item).AllExecute();
            }
        }

        /// <summary>
        /// Discharges the provided item
        /// </summary>
        /// <param name="item">The item being discharged</param>
        public static void DischargeItem(Item item)
        {
            if (item.EphemeralItemProperties != null)
            {
                if (item.HasTrait(Trait.Reload1) || item.HasTrait(Trait.Reload2))
                {
                    item.EphemeralItemProperties.NeedsReload = true;
                }

                item.EphemeralItemProperties.AmmunitionLeftInMagazine--;
            }
        }
    }
}
