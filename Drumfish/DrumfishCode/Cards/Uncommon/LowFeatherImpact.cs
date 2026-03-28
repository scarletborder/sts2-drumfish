using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Uncommon;

  
[Pool(typeof(DrumfishCardPool))]
public class LowFeatherImpact() : DrumfishCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override bool IsPlayable => (Owner.Creature.GetPower<ThornsPower>()?.Amount ?? 0m) < 1m;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ThornsPower>(2m),
        new DamageVar(5m,ValueProp.Move),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, 1).Execute(choiceContext);
        await PowerCmd.Apply<ThornsPower>(Owner.Creature, DynamicVars["ThornsPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["ThornsPower"].UpgradeValueBy(1m);
    }
}