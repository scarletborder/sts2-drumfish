using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Powers;
using Drumfish.DrumfishCode.Variables;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Drumfish.DrumfishCode.Cards.Uncommon;


public class WithdrawFirewoodPower : DrumfishPower
{
    private  bool hasIgnoreFirstExhaust = false;
    
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (card.Owner == Owner.Player)
        {
            if (!hasIgnoreFirstExhaust)
            {
                hasIgnoreFirstExhaust = true;
                return;
            }
            Flash();

            CardModel card2 = card.CreateClone();
            card.RemoveFromCurrentPile();
            await Cmd.Wait(0.1f);
            await CardPileCmd.Add(card, PileType.Discard);
            await CardPileCmd.AddGeneratedCardToCombat(card2, PileType.Hand, addedByPlayer: true);
           
            await PowerCmd.Decrement(this);
        }
    }
}

[Pool(typeof(DrumfishCardPool))]
public class WithdrawFirewood() : DrumfishCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const string WithdrawFirewoodPowerName = "WithdrawFirewood";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(WithdrawFirewoodPowerName, 1m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<WithdrawFirewoodPower>(Owner.Creature,
            DynamicVars[WithdrawFirewoodPowerName].IntValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}