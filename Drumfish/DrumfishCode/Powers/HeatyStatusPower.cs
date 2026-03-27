using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Drumfish.DrumfishCode.Powers;

/// <summary>
/// 上火 (Shanghuo) Power
/// Effect: After a card's effects are calculated, if this power is still active, exhaust that card.
/// </summary>

public sealed class HeatyStatusPower : DrumfishPower
{
    private class Data
    {
        public CardModel? CardToExhaust;
    }

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;

    /// <summary>
    /// Triggered when a card is about to be played.
    /// Records the card to potentially exhaust it after calculation.
    /// </summary>
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner)
        {
            return Task.CompletedTask;
        }

        var internalData = GetInternalData<Data>();
        internalData.CardToExhaust = cardPlay.Card;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Triggered after a card's effects have been calculated.
    /// If the power is still active, exhaust the card.
    /// </summary>
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var internalData = GetInternalData<Data>();

        // Check if this is the card we tracked
        if (cardPlay.Card != internalData.CardToExhaust)
        {
            return;
        }

        // Check if the card owner is still the power owner
        if (cardPlay.Card.Owner?.Creature != Owner)
        {
            internalData.CardToExhaust = null;
            return;
        }

        // If power is still active (Amount > 0), exhaust the card
        if (Amount > 0)
        {
            await CardCmd.Exhaust(context, cardPlay.Card, false, false);
        }

        // Clear the tracked card
        internalData.CardToExhaust = null;
    }
}