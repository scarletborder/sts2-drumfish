using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Powers;
using Drumfish.DrumfishCode.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Drumfish.DrumfishCode.Cards.Common;


[Pool(typeof(DrumfishCardPool))]
public class ForcedCombustion() : DrumfishCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new FeedfireVar(2m),
        new CardsVar(2),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Burn>(), HoverTipFactory.FromPower<HeatyStatusPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await HeatyCmd.Enter(choiceContext, Owner, this);
        List<CardModel> burnCards = [];
        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            var burn = base.CombatState?.CreateCard<Burn>(Owner);
            if (burn == null) continue;
            burnCards.Add(burn);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(burnCards, PileType.Hand, true);
        await FeedfireCmd.Execute(choiceContext, Owner, DynamicVars[FeedfireVar.Key].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[FeedfireVar.Key].UpgradeValueBy(1m);
    }
}