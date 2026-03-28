using System.Linq;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Powers;
using Drumfish.DrumfishCode.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace Drumfish.DrumfishCode.Cards.Ancient;


public class PhoenixStrikePower : DrumfishPower
{
    private Player? triggerPlayer;
    private Creature? applierCreature;

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await base.AfterApplied(applier, cardSource);
        applierCreature = applier;
        triggerPlayer = applier?.Player;
        FeedfireCmd.Feedfired += OnFeedfire;
    }

    public override async Task AfterRemoved(Creature owner)
    {
        await base.AfterRemoved(owner);
        FeedfireCmd.Feedfired -= OnFeedfire;
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        await base.AfterCombatEnd(room);
        FeedfireCmd.Feedfired -= OnFeedfire;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        await base.AfterTurnEnd(choiceContext, side);
        // Remove this power at the end of the attacker's turn (i.e., make it last only for that turn)
        if (base.Owner.Side != side)
        {
            await PowerCmd.Remove(this);
        }
    }

    private async Task OnFeedfire(Player player, decimal amount)
    {
        if (triggerPlayer == null) return;
        if (player != triggerPlayer) return;

        Flash();
        // Deal 3 damage to this power's owner (the enemy)
        await CreatureCmd.Damage(null, base.Owner, 3m, ValueProp.Unpowered, applierCreature, null);
    }
}


[Pool(typeof(DrumfishCardPool))]
public class PhoenixStrike() : DrumfishCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move),
        new PowerVar<PhoenixStrikePower>(3m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // Deal 12 damage to target
        await DamageCmd.Attack(DynamicVars.Damage.IntValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);

        // Enter Heaty
        await HeatyCmd.Enter(choiceContext, cardPlay.Card.Owner, cardPlay.Card);

        // Apply power to target that will make it take damage when Feedfire triggers this turn
        await PowerCmd.Apply<PhoenixStrikePower>(cardPlay.Target, DynamicVars["PhoenixStrikePower"].IntValue, cardPlay.Card.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}