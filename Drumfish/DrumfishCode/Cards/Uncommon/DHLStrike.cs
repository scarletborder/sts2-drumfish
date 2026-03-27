using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Extensions;
using Drumfish.DrumfishCode.Powers;
using Drumfish.DrumfishCode.Variables;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Uncommon;



[Pool(typeof(DrumfishCardPool))]
public class DHLStrike() : DrumfishCard(4, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(28, ValueProp.Move),
    new FeedfireVar(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<HeatyStatusPower>()];

    public override string PortraitPath => $"${Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await HeatyCmd.Exit(choiceContext, play.Card.Owner);
        if (play.Card.CombatState != null)
            await FeedfireCmd.Execute(choiceContext, Owner, DynamicVars[FeedfireVar.Key].IntValue,
                play.Card.CombatState);
        await CommonActions.CardAttack(this, play.Target).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[FeedfireVar.Key].UpgradeValueBy(1);
    }
}