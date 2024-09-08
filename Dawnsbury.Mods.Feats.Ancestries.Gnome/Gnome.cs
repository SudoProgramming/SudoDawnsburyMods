using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Mods.Feats.Ancestries.Gnome.RegisteredComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawnsbury.Mods.Feats.Ancestries.Gnome
{
    public static class Gnome
    {
        public static IEnumerable<Feat> CreateGnomeFeats()
        {
            yield return new AncestrySelectionFeat(GnomeFeatNames.Gnome,
                "Gnomes are short and hardy folk, with an unquenchable curiosity and eccentric habits.",
                [GnomeTraits.Gnome, Trait.Humanoid],
                8,
                5,
                [new EnforcedAbilityBoost(Ability.Constitution), new EnforcedAbilityBoost(Ability.Charisma), new FreeAbilityBoost()],
                [])
                .WithAbilityFlaw(Ability.Strength);

            yield return new TrueFeat()
        }
    }
}
