using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Extensions;
using Drumfish.DrumfishCode.Powers;
using Drumfish.DrumfishCode.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Common;

[Pool(typeof(DrumfishCardPool))]
public class FlameUppercut() : DrumfishCard(3, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
      new DamageVar(18, ValueProp.Move),
        new PowerVar<VulnerablePower>(2m),
        new FeedfireVar(2m)
  ];

  public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
  {
    ArgumentNullException.ThrowIfNull(play.Target, nameof(play.Target));

    await CommonActions.CardAttack(this, play.Target).Execute(choiceContext);

    await PowerCmd.Apply<VulnerablePower>(
        play.Target,
        base.DynamicVars.Vulnerable.BaseValue,
        base.Owner.Creature,
        this);

    await FeedfireCmd.Execute(choiceContext, play.Card.Owner, DynamicVars[FeedfireVar.Key].IntValue);
  }

  protected override void OnUpgrade()
  {
    DynamicVars[FeedfireVar.Key].UpgradeValueBy(1m);
  }
}
