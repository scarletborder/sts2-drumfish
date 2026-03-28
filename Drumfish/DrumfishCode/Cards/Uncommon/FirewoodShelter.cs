using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Variables;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Drumfish.DrumfishCode.Cards.Uncommon;


public class FirewoodShelterPower : DrumfishPower
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

    private async Task OnFeedfire(Player player, decimal amount)
    {
        if (player != Owner.Player)
            return;

        // Gain block equal to the Feedfire amount that was just triggered
        await CreatureCmd.GainBlock(
            Owner,
            Amount,
            ValueProp.Unpowered,
            null
        );
    }
}


[Pool(typeof(DrumfishCardPool))]
public class FirewoodShelter() : DrumfishCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const string FirewoodShelterName = "FirewoodShelter";
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new DynamicVar(FirewoodShelterName, 3m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Grant the immediate 8 Block
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // Apply the temporary power that grants Block equal to Feedfire amount when Feedfire triggers this turn
        await PowerCmd.Apply<FirewoodShelterPower>(Owner.Creature, DynamicVars[FirewoodShelterName].IntValue,
            Owner.Creature,
            cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVars[FirewoodShelterName].UpgradeValueBy(1m);
    }
}