using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.Variant.AncestryParagon
{
    /// <summary>
    /// Loads the Ancestry Paragon content
    /// </summary>
    public class AncestryParagonLoader
    {
        /// <summary>
        /// Runs on launch and loads all the mod content
        /// </summary>
        [DawnsburyDaysModMainMethod]
        public static void LoadMod()
        {
            FeatName skippedAncestryFeat = ModManager.RegisterFeatName("Skipped Ancestry Feat", "Skipped Ancestry Feat");

            ModManager.AddFeat(new TrueFeat(skippedAncestryFeat, 1, "No Ancestry Feat", "A repeatable choice to skip selecting an Ancestry Feat", [Trait.Ancestry]).WithMultipleSelection());

            ModManager.RegisterActionOnEachCharacterSheet((CharacterSheet sheet) =>
            {
                sheet.Calculated.AddSelectionOption(new SingleFeatSelectionOption("Ancestry Paragon Level 1", "Ancestry Paragon Level 1", 1, feat => feat.HasTrait(Trait.Ancestry)).WithIsOptional());
                sheet.Calculated.AddSelectionOption(new SingleFeatSelectionOption("Ancestry Paragon Level 3", "Ancestry Paragon Level 3", 3, feat => feat.HasTrait(Trait.Ancestry)).WithIsOptional());
                sheet.Calculated.AddSelectionOption(new SingleFeatSelectionOption("Ancestry Paragon Level 7", "Ancestry Paragon Level 7", 7, feat => feat.HasTrait(Trait.Ancestry)).WithIsOptional());
            });
        }
    }
}