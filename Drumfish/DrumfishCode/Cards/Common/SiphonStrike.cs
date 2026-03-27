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
public class SiphonStrike() : DrumfishCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
      new DamageVar(9, ValueProp.Move),
        new FeedfireVar(1m),
        new PowerVar<WeakPower>(2m),
    ];

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
    await CommonActions.CardAttack(this, cardPlay.Target, 2).Execute(choiceContext);
    await PowerCmd.Apply<WeakPower>(cardPlay.Target, base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
  }

  protected override void OnUpgrade()
  {
    DynamicVars.Damage.UpgradeValueBy(3m);
    DynamicVars.Weak.UpgradeValueBy(1m);
  }
}