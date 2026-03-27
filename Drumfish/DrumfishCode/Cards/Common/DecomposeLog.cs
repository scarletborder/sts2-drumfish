using BaseLib.Utils;
using Drumfish.DrumfishCode.Cards.Colorless;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Keywords;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Common;


[Pool(typeof(DrumfishCardPool))]
public class DecomposeLog() : DrumfishCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [(HoverTipFactory.FromCard<Match>(base.IsUpgraded))];


    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel? cardModel =
            (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1),
                context: choiceContext, player: base.Owner,
                filter: DrumfishKeywords.IsFeedfire, source: this))
            .FirstOrDefault();
        if (cardModel != null)
        {
            await CardCmd.Exhaust(choiceContext, cardModel);
            await Cmd.Wait(0.1f);
        }

        // 加入手牌
        List<CardModel> readyToAdd = [];
        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            var match = base.CombatState?.CreateCard<Match>(Owner);
            if (match == null) continue;
            if (base.IsUpgraded)
            {
                CardCmd.Upgrade(match);
            }

            readyToAdd.Add(match);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(readyToAdd, PileType.Hand, true);
    }
}