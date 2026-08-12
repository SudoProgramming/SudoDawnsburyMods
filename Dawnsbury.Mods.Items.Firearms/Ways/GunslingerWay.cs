using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Mods.Feats.Classes.Gunslinger.Enums;
using Dawnsbury.Mods.Feats.Classes.Gunslinger.RegisteredComponents;
using System.Collections.Generic;

namespace Dawnsbury.Mods.Feats.Classes.Gunslinger.Ways
{
    public class GunslingerWay
    {

        /// <summary>
        /// Gets or sets the Feat object for the Gunslinger Way
        /// </summary>
        public Feat Feat { get; set; }

        /// <summary>
        /// Gets or sets the FeatName object for the Gunslinger way
        /// </summary>
        public FeatName FeatName { get; set; }

        /// <summary>
        /// Gets or sets the Flavor Text for the Gunslinger way
        /// </summary>
        public string FlavorText { get; set; }

        /// <summary>
        /// Gets or sets the Rules Text Leadin which is templated since each Gunslinger way gives 3 simular benefits
        /// </summary>
        public string RulesTextLeadIn { get; set; }

        /// <summary>
        /// Gets or sets the Slinger's Reload rules text
        /// </summary>
        public string SlingersReloadRulesText { get; set; }

        /// <summary>
        /// Gets or sets the Initial Deed rules text
        /// </summary>
        public string InitialDeedRulesText { get; set; }

        /// <summary>
        /// Gets or sets the Way Skill rules text
        /// </summary>
        public string WaySkillRulesText { get; set; }

        /// <summary>
        /// Gets or sets the Advanced Deed rules text
        /// </summary>
        public string AdvancedDeedRulesText { get; set; }

        /// <summary>
        /// Initalizes a new instance of the <c>GunslingerWay</c> class object
        /// </summary>
        /// <param name="gunslingerWayID">The ID of the Gunslinger</param>
        public GunslingerWay(GunslingerWayID gunslingerWayID)
        {
            // Sets each of the Gunslinger Way properties depending on the given ID
            RulesTextLeadIn = "You gain the {i}Slinger's Reload, Initial Deed, and Way Skill{/i} below:\n\n";
            switch (gunslingerWayID)
            {
                case GunslingerWayID.Drifter:
                    Feat = CreateDrifterFeat();
                    this.WithDrifersReloadingStrikeLogic();
                    this.WithDrifersIntoTheFrayLogic();
                    this.WithDriftersFinishTheJobLogic();
                    this.WithWaySkill(FeatName.Acrobatics);
                    break;
                case GunslingerWayID.Pistolero:
                    Feat = CreatePistoleroFeat();
                    this.WithPistolerosRaconteursReloadLogic();
                    this.WithPistolerosTenPacesLogic();
                    this.WithPistolerosPistolersRetortLogic();
                    this.WithWaySkillOptions([FeatName.Deception, FeatName.Intimidation]);
                    break;
                case GunslingerWayID.Sniper:
                    Feat = CreateSniperFeat();
                    this.WithSnipersCoveredReloadLogic();
                    this.WithSnipersOneShotOneKillLogic();
                    this.WithSnipersVitalShotLogic();
                    this.WithWaySkill(FeatName.Stealth);
                    break;
                case GunslingerWayID.Vanguard:
                    Feat = CreateVanguardFeat();
                    this.WithVanguardClearAPathLogic();
                    this.WithVanguardLivingFortificationLogic();
                    this.WithVanguardSpinningCrushLogic();
                    this.WithWaySkill(FeatName.Athletics);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Initalizes a new instance of the <c>GunslingerWay</c> class object
        /// </summary>
        /// <param name="gunslingerFeat">The gunslinger feat</param>
        public GunslingerWay(Feat gunslingerFeat)
        {
            Feat = gunslingerFeat;
        }

        /// <summary>
        /// Creates a generic Feat with the set properities of the Gunslinger way
        /// </summary>
        /// <returns>A fully set Feat with the Gunslinger Way details</returns>
        protected Feat CreateGenericWayFeat()
        {
            return new Feat(FeatName, FlavorText, BuildFullRulesText(), new List<Trait>(), null);
        }

        /// <summary>
        /// Creates a Drifter Gunslinger's Way
        /// </summary>
        /// <returns>The Drifter Gunslinger's Way feat</returns>
        protected Feat CreateDrifterFeat()
        {
            FeatName = GunslingerFeatNames.WayOfTheDrifter;
            FlavorText = "You're a wanderer traveling from land to land with your gun and a melee weapon as company. Maybe you learned to fight with blade and pistol as a Shackles pirate, mastered the hand cannon and katana in Minkai, or practiced with a hatchet and clan pistol in Dongun Hold. You win battles by relying on mobility and flexible use of your weapons.";
            SlingersReloadRulesText = "Reloading Strike {icon:Action}\n{b}Requirements{/b} You're wielding a firearm or crossbow in one hand, and your other hand either wields a one-handed melee weapon or is empty.\n\nStrike an opponent within reach with your one-handed melee weapon (or, if your other hand is empty, with an unarmed attack), and then Interact to reload.";
            InitialDeedRulesText = "Into the Fray {icon:FreeAction}\n{b}Trigger{/b} You roll initiative\nYou can stride as a free action toward an enemy.";
            WaySkillRulesText = "Acrobatics\nYou become trained in Acrobatics.";
            AdvancedDeedRulesText = "Finish the Job {icon:Action}\n{b}Requirements{/b} On your last action, you failed (but didn't critically fail) a Strike with a firearm or crossbow you're holding in one hand, and your other hand is either wielding a melee weapon or empty.\n\nMake a Strike with your other hand, using a one-handed melee weapon or unarmed attack. This Strike uses the same multiple attack penalty as the Strike that failed on the last action. Afterward, increase your multiple attack penalty normally.";
            return CreateGenericWayFeat();
        }

        /// <summary>
        /// Creates a Pistolero Gunslinger's Way
        /// </summary>
        /// <returns>The Pistolero Gunslinger's Way feat</returns>
        protected Feat CreatePistoleroFeat()
        {
            FeatName = GunslingerFeatNames.WayOfThePistolero;
            FlavorText = "Whether you're a professional duelist or a pistol-twirling entertainer, you have quick feet and quicker hands that never seem to let you down, and an equally sharp wit and tongue that jab your foes. You might leave a hand free or cultivate the ambidexterity for twin weapons. Either way, you stay close enough to your enemies to leverage your superior reflexes while leaving enough space to safely fire.";
            SlingersReloadRulesText = "Raconteur's Reload {icon:Action}\nInteract to reload and then attempt a Deception check to Create a Diversion or an Intimidation check to Demoralize.";
            InitialDeedRulesText = "Ten Paces {icon:FreeAction}\n{b}Trigger{/b} You roll initiative\nYou gain a +2 circumstance bonus to your initiative roll, and you can Step up to 10 feet as a free action.";
            WaySkillRulesText = "Deception or Intimidation\nYou become trained in your choice between Deception or Intimidation.";
            AdvancedDeedRulesText = "Pistoler's Retort {icon:Reaction}\n{b}Trigger{/b} A foe within the first range increment of the one-handed firearm or one-handed crossbow you're wielding critically fails an attack roll against you.\n{b}Requirements{/b} You're wielding a one-handed firearm or one-handed crossbow.\n\nMake a Strike against the triggering foe with a one-handed firearm or one-handed crossbow.";
            return CreateGenericWayFeat();
        }

        /// <summary>
        /// Creates a Sniper Gunslinger's Way
        /// </summary>
        /// <returns>The Sniper Gunslinger's Way feat</returns>
        protected Feat CreateSniperFeat()
        {
            FeatName = GunslingerFeatNames.WayOfTheSniper;
            FlavorText = "You practice a style of shooting that relies on unerring accuracy and perfect placement of your first shot. You keep hidden or at a distance, staying out of the fray and bringing unseen death to your foes.";
            SlingersReloadRulesText = "Covered Reload {icon:Action}\nReload then, either Take Cover or attempt to Hide.";
            InitialDeedRulesText = "One Shot, One Kill {icon:FreeAction}\nYou have the choice of rolling Stealth or Perception for initiative.\n\nIf you roll Stealth as initiative, you deal 1d6 percision damage with your first strike from a firearm or crossbow on your first turn.\n\nYou can begin hidden to creatures who rolled lower than you in initiative if you have standard cover or greater to them.";
            WaySkillRulesText = "Stealth\nYou become trained in Stealth.";
            AdvancedDeedRulesText = "Vital Shot {icon:TwoActions}\n{b}Requirements{/b} The target is flat-footed\n\nThis Strike deals an extra die of weapon damage, and the foe takes 2d6 persistent bleed damage. The persistent bleed becomes 3d6 at level 15.";
            return CreateGenericWayFeat();
        }

        /// <summary>
        /// Creates a Vanguard Gunslinger's Way
        /// </summary>
        /// <returns>The Vanguard Gunslinger's Way feat</returns>
        protected Feat CreateVanguardFeat()
        {
            FeatName = GunslingerFeatNames.WayOfTheVanguard;
            FlavorText = "You practice a unique combat style originated by dwarven siege engineers, using heavy weapons with wide attack areas to blast holes through enemy lines, clear an opening for your allies, and defend the conquered territory.";
            SlingersReloadRulesText = "Clear a Path {icon:Action}\n{b}Requirements{/b} You're wielding a two-handed firearm or two-handed crossbow.\n\nYou make an Athletics check to Shove an opponent within your reach using your weapon, then Interact to reload. For this Shove, you don't need a free hand, and you add the weapon's item bonus on attack rolls (if any) to the Athletics check. If your last action was a ranged Strike with the weapon, use the same multiple attack penalty as that Strike for the Shove; the Shove still counts toward your multiple attack penalty on further attacks as normal.";
            InitialDeedRulesText = "Living Fortification {icon:FreeAction}\n{b}Trigger{/b} You roll initiative\nGain a +1 circumstance bonus to AC until the start of your first turn, or a +2 circumstance bonus if the chosen weapon has the parry trait.";
            WaySkillRulesText = "Athletics\nYou become trained in Athletics.";
            AdvancedDeedRulesText = "Spinning Crush {icon:ThreeActions}\n{b}Requirements{/b} You're wielding a loaded firearm or loaded crossbow.\n\nAll creatures adjacent to you take 4d6 bludgeoning damage plus your Strength modifier; this increases to 6d6 if your firearm has a striking rune, 8d6 if it has a greater striking rune, and 10d6 if it has a major striking rune. This ability does not apply other effects that increase damage with your firearm Strikes such as weapon specialization. Creatures affected by this attack must attempt a basic Reflex save. A creature that fails its save is also pushed 10 feet directly away from you.";
            return CreateGenericWayFeat();
        }

        /// <summary>
        /// Builds the full Rules Text based off of the Gunslinger's Way details
        /// </summary>
        /// <returns>The built full rules text</returns>
        private string BuildFullRulesText()
        {
            return RulesTextLeadIn + "\n\n{b}Slinger's Reload{/b} " + SlingersReloadRulesText + "\n\n{b}Initial Deed{/b} " + InitialDeedRulesText + "\n\n{b}Way Skill{/b} " + WaySkillRulesText + "\n\n{b}At higher levels{/b}\n{b}Level 9{/b}" + AdvancedDeedRulesText;
        }
    }
}
