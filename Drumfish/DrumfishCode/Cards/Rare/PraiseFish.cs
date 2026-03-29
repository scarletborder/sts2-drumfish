using System.Linq;
using System.Collections.Generic;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Drumfish.DrumfishCode.Cards.Rare;

public class PraiseFishPower : DrumfishPower
{
    private bool hasTriggeredThisTurn = false;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        await base.AfterSideTurnStart(side, combatState);
        if (base.Owner.Side == side)
        {
            hasTriggeredThisTurn = false;
        }
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (hasTriggeredThisTurn) return;
        if (card.Owner != base.Owner.Player) return;

        hasTriggeredThisTurn = true;
        Flash();

        // Create a clone of the exhausted card and add it to draw pile
        var cardCloneModel = card.CreateClone();
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardToCombat(cardCloneModel, PileType.Draw, addedByPlayer: true),
            2.2f);
    }
}

[Pool(typeof(DrumfishCardPool))]
public class PraiseFish() : DrumfishCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<PraiseFishPower>(Owner.Creature, 1m, Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
       AddKeyword(CardKeyword.Innate);
    }
}