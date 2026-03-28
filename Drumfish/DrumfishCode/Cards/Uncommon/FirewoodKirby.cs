using BaseLib.Abstracts;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Powers;
using System.Linq;
using Drumfish.DrumfishCode.Character;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Uncommon;


public class FirewoodKirbyPower : DrumfishPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        await base.AfterTurnEnd(choiceContext, side);

        // Only trigger at the end of the owner's turn
        if (base.Owner.Side != side) return;

        // Exit Heaty (上火) state for the player
        var player = base.Owner.Player;
        if (player != null && HeatyCmd.IsInHeaty(player))
        {
            await HeatyCmd.Exit(choiceContext, player);
        }
    }
    
    public override async Task BeforeFlushLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player || !Hook.ShouldFlush(player.Creature.CombatState, player))
        {
            return;
        }
        List<CardModel> list = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 0, base.Amount), context: choiceContext, player: base.Owner.Player, filter: RetainFilter, source: this)).ToList();
        if (list.Count == 0)
        {
            return;
        }
        foreach (CardModel item in list)
        {
            item.GiveSingleTurnRetain();
        }
    }


    private bool RetainFilter(CardModel card)
    {
        return !card.ShouldRetainThisTurn;
    }
}


[Pool(typeof(DrumfishCardPool))]
public class FirewoodKirby() : DrumfishCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FirewoodKirbyPower>(Owner.Creature, DynamicVars.Cards.IntValue, Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
       EnergyCost.UpgradeBy(-1);
    }
}