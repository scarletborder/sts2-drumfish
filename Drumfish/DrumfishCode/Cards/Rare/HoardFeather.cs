using System.Threading.Tasks;
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
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Rare;


public class HoardFeatherPower : DrumfishPower
{
    private decimal currentAmount = 0m;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await base.AfterApplied(applier, cardSource);
        currentAmount = base.Amount;
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        await base.AfterSideTurnStart(side, combatState);
        if (base.Owner.Side != side) return;

        // Apply Thorns equal to currentAmount
        if (currentAmount > 0m)
        {
            await PowerCmd.Apply<ThornsPower>(Owner, currentAmount, Owner, null);
        }

        // Increase the amount for next turn
        currentAmount += 1m;
    }
}


[Pool(typeof(DrumfishCardPool))]
public class HoardFeather() : DrumfishCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("HoardStart", 3m),
        new FeedfireVar(1m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Apply the recurring thorns power starting next turn
        await PowerCmd.Apply<HoardFeatherPower>(Owner.Creature, DynamicVars["HoardStart"].IntValue, Owner.Creature,
            cardPlay.Card);

        // Trigger Feedfire
        if (cardPlay.Card.CombatState != null)
            await FeedfireCmd.Execute(choiceContext, cardPlay.Card.Owner, DynamicVars[FeedfireVar.Key].IntValue,
                cardPlay.Card.CombatState);
    }

    protected override void OnUpgrade()
    {
     DynamicVars["HoardStart"].UpgradeValueBy(2m);
    }
}