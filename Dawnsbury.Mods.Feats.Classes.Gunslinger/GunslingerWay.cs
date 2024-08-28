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
                case GunslingerWayID.Sniper:
                    this.Feat = this.CreateSniperFeat();
                    this.Feat.WithSnipersCoveredReloadLogic();
                    this.Feat.WithSnipersOneShotOneKillLogic();
                    this.Feat.WithWaySkill(FeatName.Stealth);
                    break;
                case GunslingerWayID.Vanguard:
                    this.Feat = this.CreateVanguardFeat();
                    this.Feat.WithVanguardClearAPathLogic();
                    this.Feat.WithVanguardLivingFortificationLogic();
                    this.Feat.WithWaySkill(FeatName.Athletics);
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

        protected Feat CreateSniperFeat()
        {
            this.FeatName = WayOfTheSniperFeatName;
            this.FlavorText = "You practice a style of shooting that relies on unerring accuracy and perfect placement of your first shot. You keep hidden or at a distance, staying out of the fray and bringing unseen death to your foes.";
            this.SlingersReloadRulesText = "Covered Reload {icon:Action}\nReload then, either Take Cover or attempt to Hide.";
            this.InitialDeedRulesText = "One Shot, One Kill {icon:FreeAction}\nYou have the choice of rolling Stealth or Perception for initiative.\n\nIf you roll Stealth as initiative, you deal 1d6 percision damage with your first strike from a firearm or crossbow on your first turn.\n\nYou can begin hidden to creatures who rolled lower than you in initiative if you have standard cover or greater to them.";
            this.WaySkillRulesText = "Stealth\nYou become trained in Stealth.";
            return CreateGenericWayFeat();
        }

        protected Feat CreateVanguardFeat()
        {
            this.FeatName = WayOfTheVanguardFeatName;
            this.FlavorText = "You practice a unique combat style originated by dwarven siege engineers, using heavy weapons with wide attack areas to blast holes through enemy lines, clear an opening for your allies, and defend the conquered territory.";
            this.SlingersReloadRulesText = "Clear a Path {icon:Action}\n{b}Requirements{/b} You're wielding a two-handed firearm or two-handed crossbow.\n\nYou make an Athletics check to Shove an opponent within your reach using your weapon, then Interact to reload. For this Shove, you don't need a free hand, and you add the weapon's item bonus on attack rolls (if any) to the Athletics check. If your last action was a ranged Strike with the weapon, use the same multiple attack penalty as that Strike for the Shove; the Shove still counts toward your multiple attack penalty on further attacks as normal.";
            this.InitialDeedRulesText = "Living Fortification {icon:FreeAction}\n{b}Trigger{/b} You roll initiative\nGain a +1 circumstance bonus to AC until the start of your first turn, or a +2 circumstance bonus if the chosen weapon has the parry trait.";
            this.WaySkillRulesText = "Athletics\nYou become trained in Athletics.";
            return CreateGenericWayFeat();
        }

        private string BuildFullRulesText()
        {
            return this.RulesTextLeadIn + "\n\n{b}Slinger's Reload{/b} " + this.SlingersReloadRulesText + "\n\n{b}Initial Deed{/b} " + this.InitialDeedRulesText + "\n\n{b}Way Skill{/b} " + this.WaySkillRulesText;
        }
    }
}
