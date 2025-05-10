using Dawnsbury.Core;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Rules;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using Dawnsbury.Mods.Items.Firearms.RegisteredComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using Dawnsbury.Mods.Items.Firearms.Utilities;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using System.Threading.Tasks;
using System.ComponentModel.Design;

namespace Dawnsbury.Mods.Items.Firearms
{
    /// <summary>
    /// An attached weapon
    /// </summary>
    public class AttachedWeapon : Item
    {
        public Item Weapon { get; set; }

        public Item ItemAttachedTo { get; set; }

        private WeaponProperties BaseWeaponProperties { get; set; }

        private WeaponProperties? ChangedWeaponProperties { get; set; }

        private Func<Item, bool>? ShouldUseChangedProperties { get; set; }

        public AttachedWeapon(Item attachedWeapon, Item itemAttachedTo, WeaponProperties weaponProperties) : base(attachedWeapon.ItemName, attachedWeapon.Illustration, attachedWeapon.Name, attachedWeapon.Level, attachedWeapon.Price, attachedWeapon.Traits.ToArray())
        {
            Weapon = attachedWeapon;
            ItemAttachedTo = itemAttachedTo;
            BaseWeaponProperties = weaponProperties;
        }

        public AttachedWeapon(Item attachedWeapon, Item itemAttachedTo, Func<Item, bool> shouldUseChangedProperties, WeaponProperties baseWeaponProperties, WeaponProperties changedWeaponProperties) : base(attachedWeapon.ItemName, attachedWeapon.Illustration, attachedWeapon.Name, attachedWeapon.Level, attachedWeapon.Price, attachedWeapon.Traits.ToArray())
        {
            Weapon = attachedWeapon;
            ItemAttachedTo = itemAttachedTo;
            BaseWeaponProperties = baseWeaponProperties;
            ChangedWeaponProperties = changedWeaponProperties;
            ShouldUseChangedProperties = shouldUseChangedProperties;
        }

        public WeaponProperties GetWeaponProperties()
        {
            if (ShouldUseChangedProperties == null || !ShouldUseChangedProperties(this.ItemAttachedTo))
            {
                return this.BaseWeaponProperties;
            }
            else if (ChangedWeaponProperties != null)
            {
                return this.ChangedWeaponProperties;
            }
            else
            {
                return this.BaseWeaponProperties;
            }
        }
    }
}
