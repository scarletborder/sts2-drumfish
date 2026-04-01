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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Drumfish.DrumfishCode.Cards.Colorless;

[Pool(typeof(TokenCardPool))]
public sealed class FirewoodFeather() : CustomCardModel(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ThornsPower>()];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new FeedfireVar(1),
        new PowerVar<ThornsPower>(3m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.CombatState != null)
            await FeedfireCmd.Execute(choiceContext, cardPlay.Card.Owner, DynamicVars[FeedfireVar.Key].IntValue,
                cardPlay.Card.CombatState);

        await PowerCmd.Apply<FirewoodFeatherPower>(Owner.Creature, DynamicVars["ThornsPower"].IntValue, Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
      DynamicVars["ThornsPower"].UpgradeValueBy(1m);
    }
}