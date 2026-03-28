using System.Linq;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Rare;

[Pool(typeof(DrumfishCardPool))]
public class FishEmbryoPact() : DrumfishCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var drawPile = PileType.Draw.GetPile(base.Owner);
        var exhaustPile = PileType.Exhaust.GetPile(base.Owner);

        if (drawPile == null || exhaustPile == null) return;

        // Copy lists to avoid modification during iteration
        var drawCards = drawPile.Cards.ToList();
        var exhaustCards = exhaustPile.Cards.ToList();

        // Move exhaust -> draw
        foreach (var c in exhaustCards)
        {
            c.RemoveFromCurrentPile();
            await CardPileCmd.Add(c, PileType.Draw, CardPilePosition.Top);
        }

        await Cmd.Wait(0.1f);
        // Move draw -> exhaust
        foreach (var c in drawCards)
        {
            await CardCmd.Exhaust(choiceContext, c, false, false);
        }

        await CardPileCmd.ShuffleIfNecessary(choiceContext, Owner);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}