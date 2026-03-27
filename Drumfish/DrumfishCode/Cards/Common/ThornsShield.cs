using BaseLib.Abstracts;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Cards.Colorless;
using Drumfish.DrumfishCode.Character;
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
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Common;

public class ThornsShieldPower : TemporaryThornsPower
{
    public override AbstractModel OriginModel => ModelDb.Card<FirewoodFeather>();
}


[Pool(typeof(DrumfishCardPool))]
public sealed class ThornsShield() : CustomCardModel(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ThornsPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6, ValueProp.Move),
        new PowerVar<ThornsPower>(1m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 给与格挡
        await CommonActions.CardBlock(this, cardPlay);

        // 施加临时荆棘（使用新的 ThornsShieldPower 继承 TemporaryThornsPower）
        await PowerCmd.Apply<ThornsShieldPower>(Owner.Creature, DynamicVars["ThornsPower"].IntValue, Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ThornsPower"].UpgradeValueBy(2m);
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}