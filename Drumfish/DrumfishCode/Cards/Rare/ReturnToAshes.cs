using BaseLib.Abstracts;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Rare;



[Pool(typeof(DrumfishCardPool))]
public class ReturnToAshes() : DrumfishCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move),
        new FeedfireVar(2m),
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        // Trigger Feedfire
        if (cardPlay.Card.CombatState != null)
            await FeedfireCmd.Execute(choiceContext, cardPlay.Card.Owner, DynamicVars[FeedfireVar.Key].IntValue,
                cardPlay.Card.CombatState);

        bool shouldTriggerFatal = cardPlay.Target.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal());
        AttackCommand attackCommand = await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
        if (shouldTriggerFatal && attackCommand.Results.Any((DamageResult r) => r.WasTargetKilled))
        {
            var results = new List<CardPileAddResult>();
            var cardCopy = cardPlay.Card.CreateClone();
            results.Add(await CardPileCmd.Add(Owner.RunState.CreateCard(cardCopy, Owner), PileType.Deck));
            CardCmd.PreviewCardPileAdd((IReadOnlyList<CardPileAddResult>)results, 1f);
            results = null;
        }
    }

    protected override void OnUpgrade()
    {
       DynamicVars.Damage.UpgradeValueBy(5m);
    }
}