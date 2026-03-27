using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.History.Entries;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Uncommon;

[Pool(typeof(DrumfishCardPool))]
public class ColorOutOfFirewood() : DrumfishCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private static decimal FirewoodThisTurn(CardModel card, Creature? _)
    {
        return CombatManager.Instance.History.Entries
            .OfType<FeedfireEntry>()
            .Where(e => e.HappenedThisTurn(card.CombatState) && e.Actor == card.Owner.Creature)
            .Select(e => (decimal)e.Amount)
            .Sum();
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(6m),
        new ExtraDamageVar(2m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(FirewoodThisTurn),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 升级后基础伤害 +3
        base.DynamicVars.CalculationBase.UpgradeValueBy(4m);
    }
}