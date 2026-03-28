using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Drumfish.DrumfishCode.Cards.Uncommon;

[Pool(typeof(DrumfishCardPool))]
public class TurboNeuro() : DrumfishCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel cardToExhaust =
            (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1),
                context: choiceContext, player: base.Owner, filter: null, source: this)).FirstOrDefault();
        if (cardToExhaust != null)
        {
            await CardCmd.Exhaust(choiceContext, cardToExhaust);
        }

        CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 1);
        CardPile pile = PileType.Exhaust.GetPile(base.Owner);
        CardModel cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext, pile.Cards, base.Owner, prefs))
            .FirstOrDefault();
        if (cardModel == null) return;
        cardModel.RemoveFromCurrentPile();
        await CardPileCmd.Add(cardModel, PileType.Draw, CardPilePosition.Top);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}