using BaseLib.Abstracts;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Powers;
using Drumfish.DrumfishCode.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Uncommon;

public class FeatherArmamentTempThornsPower : TemporaryThornsPower
{
    public override AbstractModel OriginModel => ModelDb.Card<FeatherArmament>();
}


public class FeatherArmamentPower : DrumfishPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await base.AfterApplied(applier, cardSource);
        FeedfireCmd.Feedfired += OnFeedfire;
    }

    public override async Task AfterRemoved(Creature owner)
    {
        await base.AfterRemoved(owner);
        FeedfireCmd.Feedfired -= OnFeedfire;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        await base.AfterTurnEnd(choiceContext, side);
        if (base.Owner.Side != side)
        {
            await PowerCmd.Remove(this);
        }
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        await base.AfterCombatEnd(room);
        FeedfireCmd.Feedfired -= OnFeedfire;
    }

    private async Task OnFeedfire(PlayerChoiceContext choiceContext, Player player, decimal amount)
    {
        if (player != Owner.Player) return;

        // Apply a temporary thorns power equal to the feedfire amount
        await PowerCmd.Apply<FeatherArmamentTempThornsPower>(Owner, amount, Owner, null);
    }
}


[Pool(typeof(DrumfishCardPool))]
public class FeatherArmament() : DrumfishCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6m, ValueProp.Move),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Grant immediate block
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // Apply the observer power for this turn
        await PowerCmd.Apply<FeatherArmamentPower>(Owner.Creature, 1m, Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}