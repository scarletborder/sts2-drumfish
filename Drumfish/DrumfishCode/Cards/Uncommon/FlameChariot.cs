using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Extensions;
using Drumfish.DrumfishCode.Powers;
using Drumfish.DrumfishCode.Variables;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Uncommon;


[Pool(typeof(DrumfishCardPool))]
public class FlameChariot() : DrumfishCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(4m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) =>
        {
            return card.Owner.PlayerCombatState != null ? card.Owner.PlayerCombatState.ExhaustPile.Cards.Count : 0;
        }),
        new RepeatVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<HeatyStatusPower>()];

    public override string PortraitPath => $"${Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await HeatyCmd.Exit(choiceContext, play.Card.Owner);
        await CommonActions.CardAttack(play.Card, play, DynamicVars.Repeat.IntValue).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.ExtraDamage.UpgradeValueBy(1m);
    }
}