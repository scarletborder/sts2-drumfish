using System.Linq;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Rare;

[Pool(typeof(DrumfishCardPool))]
public class DreamFish() : DrumfishCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Exit Heaty
        await HeatyCmd.Exit(choiceContext, base.Owner);

        // Select 1 card from Exhaust pile and move it to hand
        var exhaustPile = PileType.Exhaust.GetPile(base.Owner);
        if (exhaustPile == null || exhaustPile.Cards.Count == 0) return;

        var prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 1, 1);
        var chosen = (await CardSelectCmd.FromSimpleGrid(choiceContext, exhaustPile.Cards, base.Owner, prefs))
            .FirstOrDefault();
        if (chosen != null)
        {
            chosen.RemoveFromCurrentPile();
            await CardPileCmd.Add(chosen, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}