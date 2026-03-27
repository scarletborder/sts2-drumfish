using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Extensions;

namespace Drumfish.DrumfishCode.Cards.Common;

[Pool(typeof(Drumfish.DrumfishCode.Character.DrumfishCardPool))]
public class FlamingStrike() : Drumfish.DrumfishCode.Cards.DrumfishCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
  protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move)];
  protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<HeatyStatusPower>()];

  public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
  {
    var damage = DynamicVars.Damage.IntValue;
    if (HeatyCmd.IsInHeaty(Owner))
    {
      damage *= 2;
    }
    await CommonActions.CardAttack(this, play.Target, damage).Execute(choiceContext);
  }

  protected override void OnUpgrade()
  {
    DynamicVars.Damage.UpgradeValueBy(2m);
  }
}
