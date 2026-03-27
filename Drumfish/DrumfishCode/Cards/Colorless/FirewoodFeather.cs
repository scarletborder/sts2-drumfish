using Drumfish.DrumfishCode.Powers;
using Drumfish.DrumfishCode.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Drumfish.DrumfishCode.Cards.Colorless;


public class FirewoodFeather() : DrumfishCard(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ThornsPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new FeedfireVar(1),
        new PowerVar<ThornsPower>(1m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<HeatyStatusPower>(Owner.Creature, 1, Owner.Creature, this);
        await PowerCmd.Apply<FirewoodFeatherPower>(Owner.Creature, DynamicVars["ThornsPower"].BaseValue, Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ThornsPower"].UpgradeValueBy(1m);
    }
}