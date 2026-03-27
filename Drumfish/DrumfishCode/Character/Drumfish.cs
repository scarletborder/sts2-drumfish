using BaseLib.Abstracts;
using Drumfish.DrumfishCode.Cards.Basic;
using Drumfish.DrumfishCode.Extensions;
using Drumfish.DrumfishCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Drumfish.DrumfishCode.Character;

public class Drumfish : PlaceholderCharacterModel
{
    public const string CharacterId = "Drumfish";

    public static readonly Color Color = new("ffffff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 70;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeDrumfish>(),
        ModelDb.Card<StrikeDrumfish>(),
        ModelDb.Card<StrikeDrumfish>(),
        ModelDb.Card<StrikeDrumfish>(),
        ModelDb.Card<DefenseDrumfish>(),
        ModelDb.Card<DefenseDrumfish>(),
        ModelDb.Card<DefenseDrumfish>(),
        ModelDb.Card<DefenseDrumfish>(),
        ModelDb.Card<FlameStrike>(),
        ModelDb.Card<Anneal>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<NewbornWing>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<DrumfishCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<DrumfishRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<DrumfishPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}