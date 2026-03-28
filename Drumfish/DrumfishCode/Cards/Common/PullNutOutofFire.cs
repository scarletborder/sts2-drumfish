using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Common;

[Pool(typeof(DrumfishCardPool))]
public class PullNutOutofFire() : DrumfishCard(1, CardType.Attack, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(2m),
        new CardsVar(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(choiceContext, base.Owner.Creature, base.DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);

        var exhaustPile = PileType.Exhaust.GetPile(Owner);
        var cards = exhaustPile.Cards;
    
        // 获取动态变量中定义的数量，并确保不超过当前堆中的总数
        int countToRetrieve = (int)base.DynamicVars.Cards.BaseValue;
        int actualCount = Math.Min(countToRetrieve, cards.Count);

        if (actualCount <= 0)
        {
            return;
        }
        
        for (int i = 0; i < actualCount; i++)
        {
            // 每次都取最后一张 (因为取走一张后，原本的倒数第二变成了倒数第一)
            var cardInTop = cards[^1]; 
        
            cardInTop.RemoveFromCurrentPile();
            await CardPileCmd.Add(cardInTop, PileType.Hand);
            await Cmd.Wait(0.1f);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}