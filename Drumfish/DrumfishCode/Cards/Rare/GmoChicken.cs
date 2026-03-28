using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Cards.Colorless;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Drumfish.DrumfishCode.Cards.Rare;


[Pool(typeof(DrumfishCardPool))]
public class GmoChicken() : DrumfishCard(4, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<FirewoodFeather>(base.IsUpgraded)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Allow selecting any number of cards from the Exhaust pile
        var exhaustPile = PileType.Exhaust.GetPile(base.Owner);
        if (exhaustPile == null || exhaustPile.Cards.Count == 0) return;

        List<CardModel> list = (await CardSelectCmd.FromSimpleGrid(choiceContext, exhaustPile.Cards, base.Owner, new CardSelectorPrefs(base.SelectionScreenPrompt, 0, 999))).ToList();

        foreach (var item in list)
        {
            // Transform the selected exhaust pile card into FirewoodFeather and add to Draw pile
            var newCard = base.CombatState.CreateCard<FirewoodFeather>(base.Owner);
            if (base.IsUpgraded) CardCmd.Upgrade(newCard);

            // Remove original card from exhaust pile (it already is in exhaust) and add transformed card to Draw pile
            item.RemoveFromCurrentPile();
            await CardPileCmd.Add(newCard, PileType.Draw, CardPilePosition.Top);
        }
    }
}