using BaseLib.Abstracts;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Powers;
using Drumfish.DrumfishCode.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Drumfish.DrumfishCode.Cards.Colorless;

 
[Pool(typeof(TokenCardPool))]
public sealed class Wheat() : CustomCardModel(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ThornsPower>()];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new FeedfireVar(2),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.CombatState != null)
            await FeedfireCmd.Execute(choiceContext, cardPlay.Card.Owner, DynamicVars[FeedfireVar.Key].IntValue,
                cardPlay.Card.CombatState);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[FeedfireVar.Key].UpgradeValueBy(1m);
    }
}