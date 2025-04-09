using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Dawnsbury.Core.Mechanics;

namespace Dawnsbury.Mods.Feats.Classes.Thaumaturge
{
    public class QEffectClone : QEffect
    {
        public QEffect OriginalEffect { get; set; }

        public QEffectClone() : base()
        {
        }

        public QEffectClone(QEffect originalQEffect) : base()
        {
            this.OriginalEffect = originalQEffect;

            if (string.IsNullOrWhiteSpace(originalQEffect.Key))
            {
                this.Key = ShortHashQEffect(originalQEffect).ToString();
            }
            else
            {
                this.Key = originalQEffect.Key;
            }

            this.Id = originalQEffect.Id;
            this.Owner = originalQEffect.Owner;
            this.Source = originalQEffect.Source;
            this.Name = originalQEffect.Name;
            this.Description = originalQEffect.Description;
            this.Value = originalQEffect.Value;
            this.CannotExpireThisTurn = originalQEffect.CannotExpireThisTurn;
            this.ExpiresAt = originalQEffect.ExpiresAt;
            this.Illustration = originalQEffect.Illustration;
            this.Innate = originalQEffect.Innate;

            this.AddExtraKindedDamageOnStrike = originalQEffect.AddExtraKindedDamageOnStrike;
            this.AddExtraStrikeDamage = originalQEffect.AddExtraStrikeDamage;
            this.AddExtraWeaponDamage = originalQEffect.AddExtraWeaponDamage;
            this.AdjustActiveRollCheckResult = originalQEffect.AdjustActiveRollCheckResult;
            this.AdjustDiceFormulaForSelfHealing = originalQEffect.AdjustDiceFormulaForSelfHealing;
            this.AdjustSavingThrowCheckResult = originalQEffect.AdjustSavingThrowCheckResult;
            this.Affliction = originalQEffect.Affliction;
            this.AfterYouAcquireEffect = originalQEffect.AfterYouAcquireEffect;
            this.AfterYouAreDealtDamageOfKind = originalQEffect.AfterYouAreDealtDamageOfKind;
            this.AfterYouAreHealed = originalQEffect.AfterYouAreHealed;
            this.AfterYouAreTargeted = originalQEffect.AfterYouAreTargeted;
            this.AfterYouDealDamage = originalQEffect.AfterYouDealDamage;
            this.AfterYouDealDamageOfKind = originalQEffect.AfterYouDealDamageOfKind;
            this.AfterYouExpendSpellcastingResources = originalQEffect.AfterYouExpendSpellcastingResources;
            this.AfterYouMakeAttackRoll = originalQEffect.AfterYouMakeAttackRoll;
            this.AfterYouMakeSavingThrow = originalQEffect.AfterYouMakeSavingThrow;
            this.AfterYouTakeAction = originalQEffect.AfterYouTakeAction;
            this.AfterYouTakeActionAgainstTarget = originalQEffect.AfterYouTakeActionAgainstTarget;
            this.AfterYouTakeAmountOfDamageOfKind = originalQEffect.AfterYouTakeAmountOfDamageOfKind;
            this.AfterYouTakeDamage = originalQEffect.AfterYouTakeDamage;
            this.AfterYouTakeDamageOfKind = originalQEffect.AfterYouTakeDamageOfKind;
            this.AfterYouTakeHostileAction = originalQEffect.AfterYouTakeHostileAction;
            this.AfterYouTakeIncomingDamageEventEvenZero = originalQEffect.AfterYouTakeIncomingDamageEventEvenZero;
            this.AppliedThisStateCheck = originalQEffect.AppliedThisStateCheck;
            this.AssociatedAura = originalQEffect.AssociatedAura;
            this.BattleformMinimumAthleticsModifier = originalQEffect.BattleformMinimumAthleticsModifier;
            this.BattleformMinimumStrikeModifier = originalQEffect.BattleformMinimumStrikeModifier;
            this.BeforeYourActiveRoll = originalQEffect.BeforeYourActiveRoll;
            this.BeforeYourSavingThrow = originalQEffect.BeforeYourSavingThrow;
            this.BonusToAbilityBasedChecksRollsAndDCs = originalQEffect.BonusToAbilityBasedChecksRollsAndDCs;
            this.BonusToAllChecksAndDCs = originalQEffect.BonusToAllChecksAndDCs;
            this.BonusToAllSpeeds = originalQEffect.BonusToAllSpeeds;
            this.BonusToAttackRolls = originalQEffect.BonusToAttackRolls;
            this.BonusToDamage = originalQEffect.BonusToDamage;
            this.BonusToDefenses = originalQEffect.BonusToDefenses;
            this.BonusToPerception = originalQEffect.BonusToPerception;
            this.BonusToSelfHealing = originalQEffect.BonusToSelfHealing;
            this.BonusToSkillChecks = originalQEffect.BonusToSkillChecks;
            this.BonusToSkills = originalQEffect.BonusToSkills;
            this.BonusToSpellSaveDCs = originalQEffect.BonusToSpellSaveDCs;
            this.BonusToSpellSaveDCsForSpecificSpell = originalQEffect.BonusToSpellSaveDCsForSpecificSpell;
            this.ConvertAllOutgoingDamageToDamageKind = originalQEffect.ConvertAllOutgoingDamageToDamageKind;
            this.CounteractLevel = originalQEffect.CounteractLevel;
            this.CountsAsABuff = originalQEffect.CountsAsABuff;
            this.CountsAsADebuff = originalQEffect.CountsAsADebuff;
            this.CountsAsBeneficialToSource = originalQEffect.CountsAsBeneficialToSource;
            this.Dismissable = originalQEffect.Dismissable;
            this.DoNotShowUpOverhead = originalQEffect.DoNotShowUpOverhead;
            this.DoNotUseResistancesAndImmunitiesAgainst = originalQEffect.DoNotUseResistancesAndImmunitiesAgainst;
            this.EndOfAnyTurn = originalQEffect.EndOfAnyTurn;
            this.EndOfCombat = originalQEffect.EndOfCombat;
            this.EndOfYourTurnBeneficialEffect = originalQEffect.EndOfYourTurnBeneficialEffect;
            this.EndOfYourTurnDetrimentalEffect = originalQEffect.EndOfYourTurnDetrimentalEffect;
            this.FizzleIncomingActions = originalQEffect.FizzleIncomingActions;
            this.FizzleOutgoingActions = originalQEffect.FizzleOutgoingActions;
            this.HalveHealingFromEffects = originalQEffect.HalveHealingFromEffects;
            this.HideFromPortrait = originalQEffect.HideFromPortrait;
            this.ImmuneToCondition = originalQEffect.ImmuneToCondition;
            this.ImmuneToTrait = originalQEffect.ImmuneToTrait;
            this.IncreaseCover = originalQEffect.IncreaseCover;
            this.IncreaseItemDamageDie = originalQEffect.IncreaseItemDamageDie;
            this.IncreaseItemDamageDieCount = originalQEffect.IncreaseItemDamageDieCount;
            this.IncreaseSpellEffectDamageDie = originalQEffect.IncreaseSpellEffectDamageDie;
            this.IsFlatFootedTo = originalQEffect.IsFlatFootedTo;
            this.IsStance = originalQEffect.IsStance;
            this.LimitsDexterityBonusToAC = originalQEffect.LimitsDexterityBonusToAC;
            this.LongTermEffectDuration = originalQEffect.LongTermEffectDuration;
            this.MetamagicProvider = originalQEffect.MetamagicProvider;
            this.OverrideItemDamageDie = originalQEffect.OverrideItemDamageDie;
            this.PreventAreaAttacksAgainstMe = originalQEffect.PreventAreaAttacksAgainstMe;
            this.PreventDeathDueToDying = originalQEffect.PreventDeathDueToDying;
            this.PreventTakingAction = originalQEffect.PreventTakingAction;
            this.PreventTargetingBy = originalQEffect.PreventTargetingBy;
            this.ProvideActionIntoPossibilitySection = originalQEffect.ProvideActionIntoPossibilitySection;
            this.ProvideContextualAction = originalQEffect.ProvideContextualAction;
            this.ProvideFortuneEffect = originalQEffect.ProvideFortuneEffect;
            this.ProvideMainAction = originalQEffect.ProvideMainAction;
            this.ProvidesArmor = originalQEffect.ProvidesArmor;
            this.ProvideSectionIntoSubmenu = originalQEffect.ProvideSectionIntoSubmenu;
            this.ProvideStrikeModifier = originalQEffect.ProvideStrikeModifier;
            this.ProvideStrikeModifierAsPossibilities = originalQEffect.ProvideStrikeModifierAsPossibilities;
            this.ProvideStrikeModifierAsPossibility = originalQEffect.ProvideStrikeModifierAsPossibility;
            this.QuickenedFor = originalQEffect.QuickenedFor;
            this.ReferencedSpell = originalQEffect.ReferencedSpell;
            this.RepresentsPoison = originalQEffect.RepresentsPoison;
            this.RerollActiveRoll = originalQEffect.RerollActiveRoll;
            this.RerollSavingThrow = originalQEffect.RerollSavingThrow;
            this.RoundsLeft = originalQEffect.RoundsLeft;
            this.SetBaseSpeedTo = originalQEffect.SetBaseSpeedTo;
            this.SightReductionTo = originalQEffect.SightReductionTo;
            this.SourceAction = originalQEffect.SourceAction;
            this.SpawnsAura = originalQEffect.SpawnsAura;
            this.StartOfSourcesTurn = originalQEffect.StartOfSourcesTurn;
            this.StartOfYourEveryTurn = originalQEffect.StartOfYourEveryTurn;
            this.StartOfYourPrimaryTurn = originalQEffect.StartOfYourPrimaryTurn;
            this.StateCheck = originalQEffect.StateCheck;
            this.StateCheckLayer = originalQEffect.StateCheckLayer;
            this.StateCheckWithVisibleChanges = originalQEffect.StateCheckWithVisibleChanges;
            this.SubsumedBy = originalQEffect.SubsumedBy;
            this.Tag = originalQEffect.Tag;
            this.ThisCreatureCannotBeMoreVisibleThan = originalQEffect.ThisCreatureCannotBeMoreVisibleThan;
            this.Traits = originalQEffect.Traits;
            this.UsedThisTurn = originalQEffect.UsedThisTurn;
            this.WhenCreatureDiesAtStateCheckAsync = originalQEffect.WhenCreatureDiesAtStateCheckAsync;
            this.WhenExpires = originalQEffect.WhenExpires;
            this.WhenMonsterDies = originalQEffect.WhenMonsterDies;
            this.WhenProvoked = originalQEffect.WhenProvoked;
            this.YouAcquireQEffect = originalQEffect.YouAcquireQEffect;
            this.YouAreDealtDamage = originalQEffect.YouAreDealtDamage;
            this.YouAreDealtDamageEvent = originalQEffect.YouAreDealtDamageEvent;
            this.YouAreDealtLethalDamage = originalQEffect.YouAreDealtLethalDamage;
            this.YouAreTargeted = originalQEffect.YouAreTargeted;
            this.YouAreTargetedByARoll = originalQEffect.YouAreTargetedByARoll;
            this.YouBeginAction = originalQEffect.YouBeginAction;
            this.YouDealDamageEvent = originalQEffect.YouDealDamageEvent;
            this.YouDealDamageWithStrike = originalQEffect.YouDealDamageWithStrike;
            this.YouDealDamageWithStrike2 = originalQEffect.YouDealDamageWithStrike2;
            this.YouHaveCriticalSpecialization = originalQEffect.YouHaveCriticalSpecialization;
            this.YourStrikeGainsDamageType = originalQEffect.YourStrikeGainsDamageType;
        }

        public static bool QuickCompareQEffects(QEffect qEffectOne, QEffect qEffectTwo)
        {
            return ShortHashQEffect(qEffectOne) == ShortHashQEffect(qEffectTwo);
        }

        public static int ShortHashQEffect(QEffect qEffect)
        {
            return HashCode.Combine(qEffect.Id, qEffect.Source, qEffect.Name, qEffect.Description, qEffect.Value, qEffect.Illustration);
        }

        public static string LongHashQEffect(QEffect qEffect)
        {
            string json = JsonSerializer.Serialize(qEffect);
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }

        }
    }
}
