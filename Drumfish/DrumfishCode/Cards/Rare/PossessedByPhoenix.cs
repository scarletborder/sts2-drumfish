using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Drumfish.DrumfishCode.Cards.Rare;


public class PossessedByPhoenixPower : DrumfishPower
{
    private bool hasIgnoreFirstExhaust = false;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner == Owner.Player)
        {
            if (!hasIgnoreFirstExhaust)
            {
                hasIgnoreFirstExhaust = true;
                return;
            }

            Flash();

            card.RemoveFromCurrentPile();
            await Cmd.Wait(0.1f);
            await CardPileCmd.Add(card, PileType.Discard);

            await PowerCmd.Decrement(this);
        }
    }
}


[Pool(typeof(DrumfishCardPool))]
public class PossessedByPhoenix() : DrumfishCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<PossessedByPhoenixPower>(Owner.Creature, 1m, Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}