using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Mods.Feats.Classes.Gunslinger.Enums;
using Dawnsbury.Mods.Feats.Classes.Gunslinger.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Classes.Gunslinger
{
    public class GunslingerWay
    {
        /// <summary>
        /// The Way of the Drifter Subclass Feat Name
        /// </summary>
        public static readonly FeatName WayOfTheDrifterFeatName = ModManager.RegisterFeatName("Way of the Drifter", "Way of the Drifter");

        /// <summary>
        /// The Way of the Pistolero Subclass Feat Name
        /// </summary>
        public static readonly FeatName WayOfThePistoleroFeatName = ModManager.RegisterFeatName("Way of the Pistolero", "Way of the Pistolero");

        /// <summary>
        /// The Way of the Sniper Subclass Feat Name
        /// </summary>
        public static readonly FeatName WayOfTheSniperFeatName = ModManager.RegisterFeatName("Way of the Sniper", "Way of the Sniper");

        ///// <summary>
        ///// The Way of the Triggerbrand Subclass Feat Name
        ///// TODO: Add Combination Weapons and this Subclass
        ///// </summary>
        //public static readonly FeatName WayOfTheTriggerbrandFeatName = ModManager.RegisterFeatName("Way of the Triggerbrand", "Way of the Triggerbrand");

        /// <summary>
        /// The Way of the Vanguard Subclass Feat Name
        /// </summary>
        public static readonly FeatName WayOfTheVanguardFeatName = ModManager.RegisterFeatName("Way of the Vanguard", "Way of the Vanguard");

        public Feat Feat { get; set; }

        public FeatName FeatName { get; set; }

        public string FlavorText {  get; set; }

        public string RulesTextLeadIn { get; set; }

        public string SlingersReloadRulesText { get; set; }

        public string InitialDeedRulesText { get; set; }

        public string WaySkillRulesText { get; set; }

        public GunslingerWay(GunslingerWayID gunslingerWayID)
        {
            this.RulesTextLeadIn = "You gain the {i}Slinger's Reload, Initial Deed, and Way Skill{/i} below:\n\n";
            switch (gunslingerWayID)
            {
                case GunslingerWayID.Drifter:
                    this.Feat = this.CreateDrifterFeat();
                    this.Feat.WithDrifersReloadingStrikeLogic();
                    this.Feat.WithDrifersIntoTheFrayLogic();
                    this.Feat.WithWaySkill(FeatName.Acrobatics);
                    break;
                case GunslingerWayID.Pistolero:
                    this.Feat = this.CreatePistoleroFeat();
                    this.Feat.WithPistolerosRaconteursReloadLogic();
                    this.Feat.WithPistolerosTenPacesLogic();
                    this.Feat.WithWaySkillOptions(new List<FeatName> { FeatName.Deception, FeatName.Intimidation });
                    break;
                default:
                    break;
            }
        }

        public GunslingerWay(Feat gunslingerFeat)
        {
            Feat = gunslingerFeat;
        }

        protected Feat CreateGenericWayFeat()
        {
            return new Feat(this.FeatName, this.FlavorText, this.BuildFullRulesText(), new List<Trait>(), null);
        }

        protected Feat CreateDrifterFeat()
        {
            this.FeatName = WayOfTheDrifterFeatName;
            this.FlavorText = "You're a wanderer traveling from land to land with your gun and a melee weapon as company. Maybe you learned to fight with blade and pistol as a Shackles pirate, mastered the hand cannon and katana in Minkai, or practiced with a hatchet and clan pistol in Dongun Hold. You win battles by relying on mobility and flexible use of your weapons.";
            this.SlingersReloadRulesText = "Reloading Strike {icon:Action}\n{b}Requirements{/b} You're wielding a firearm or crossbow in one hand, and your other hand either wields a one-handed melee weapon or is empty.\n\nStrike an opponent within reach with your one-handed melee weapon (or, if your other hand is empty, with an unarmed attack), and then Interact to reload.";
            this.InitialDeedRulesText = "Into the Fray {icon:FreeAction}\n{b}Trigger{/b} You roll initiative\nYou can stride as a free action toward an enemy.";
            this.WaySkillRulesText = "Acrobatics\nYou become trained in Acrobatics.";
            return CreateGenericWayFeat();
        }

        protected Feat CreatePistoleroFeat()
        {
            this.FeatName = WayOfThePistoleroFeatName;
            this.FlavorText = "Whether you're a professional duelist or a pistol-twirling entertainer, you have quick feet and quicker hands that never seem to let you down, and an equally sharp wit and tongue that jab your foes. You might leave a hand free or cultivate the ambidexterity for twin weapons. Either way, you stay close enough to your enemies to leverage your superior reflexes while leaving enough space to safely fire.";
            this.SlingersReloadRulesText = "Raconteur's Reload {icon:Action}\nInteract to reload and then attempt a Deception check to Create a Diversion or an Intimidation check to Demoralize.";
            this.InitialDeedRulesText = "Ten Paces {icon:FreeAction}\n{b}Trigger{/b} You roll initiative\nYou gain a +2 circumstance bonus to your initiative roll, and you can Step up to 10 feet as a free action.";
            this.WaySkillRulesText = "Deception or Intimidation\nYou become trained in your choice between Deception or Intimidation.";
            return CreateGenericWayFeat();
        }

        private string BuildFullRulesText()
        {
            return this.RulesTextLeadIn + "\n\n{b}Slinger's Reload{/b} " + this.SlingersReloadRulesText + "\n\n{b}Initial Deed{/b} " + this.InitialDeedRulesText + "\n\n{b}Way Skill{/b} " + this.WaySkillRulesText;
        }
    }
}
