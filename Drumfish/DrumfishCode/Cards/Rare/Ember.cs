using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Cards.Colorless;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Drumfish.DrumfishCode.Cards.Rare;


public class EmberPower : DrumfishPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        // Only care about cards exhausted by this player
        if (card.Owner != Owner.Player) return;

        // Ignore Match cards
        if (card.Rarity == CardRarity.Token) return;

        // Create a Match and add it to hand
        for (int i = 0; i < Amount; i++)
        {
            var match = card.CombatState?.CreateCard<Match>(Owner.Player);
            if (match != null)
            {
                await CardPileCmd.AddGeneratedCardToCombat(match, PileType.Hand, true);
            }
        }
      
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        await base.AfterTurnEnd(choiceContext, side);
        if (base.Owner.Side != side) return;
        await PowerCmd.Remove(this);
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        await base.AfterCombatEnd(room);
        // ensure removal
        await PowerCmd.Remove(this);
    }
}


[Pool(typeof(DrumfishCardPool))]
public class Ember() : DrumfishCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EmberPower>(Owner.Creature, DynamicVars.Cards.IntValue, Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
     EnergyCost.UpgradeBy(-1);
    }
}