using BaseLib.Abstracts;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Drumfish.DrumfishCode.Cards.Rare;

public class DiademFishPower : DrumfishPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await base.AfterApplied(applier, cardSource);
        HeatyCmd.HeatyEntered += OnHeatyEntered;
    }

    public override async Task AfterRemoved(Creature owner)
    {
        await base.AfterRemoved(owner);
        HeatyCmd.HeatyEntered -= OnHeatyEntered;
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        await base.AfterCombatEnd(room);
        HeatyCmd.HeatyEntered -= OnHeatyEntered;
    }

    private async Task OnHeatyEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;

        await PlayerCmd.GainEnergy(1m, player);
    }
}

[Pool(typeof(DrumfishCardPool))]
public class DiademFish() : DrumfishCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<HeatyStatusPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Immediately exit Heaty
        await HeatyCmd.Exit(choiceContext, Owner);

        // Apply the power that grants energy when entering Heaty
        await PowerCmd.Apply<DiademFishPower>(Owner.Creature, DynamicVars.Energy.IntValue, Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1m);
    }
}