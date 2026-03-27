using BaseLib.Patches.UI;
using Drumfish.DrumfishCode.Keywords;
using Drumfish.DrumfishCode.Variables;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Drumfish.DrumfishCode.Cards.Uncommon;

using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Cards.Colorless;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Extensions;
using Drumfish.DrumfishCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;


public class PlantTreesPower : DrumfishPower
{

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(DrumfishKeywords.Feedfire)];

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == base.Owner.Player && cardPlay.Card.Type == CardType.Attack)
        {
            Flash();
            if (cardPlay.Card.CombatState != null)
                await FeedfireCmd.Execute(context, cardPlay.Card.Owner, base.Amount, cardPlay.Card.CombatState);
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        await PowerCmd.Remove(this);
    }
}


[Pool(typeof(DrumfishCardPool))]
public class PlantTrees() : DrumfishCard(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14, ValueProp.Move),
        new PowerVar<PlantTreesPower>(1m),
    ];

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play.Target).Execute(choiceContext);

        await PowerCmd.Apply<PlantTreesPower>(Owner.Creature, base.DynamicVars["PlantTreesPower"].BaseValue,
            base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}