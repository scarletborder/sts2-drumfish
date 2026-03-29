using BaseLib.Abstracts;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Uncommon;


[Pool(typeof(DrumfishCardPool))]
public class CarryWheat() : DrumfishCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6, ValueProp.Move),
        new FeedfireVar(2),
        new RepeatVar(2),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Gain block Repeat times
        int times = (int)DynamicVars.Repeat.BaseValue;
        for (int i = 0; i < times; i++)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        }

        if (cardPlay.Card.CombatState != null)
            if (Owner.Creature.Player != null)
                await FeedfireCmd.Execute(choiceContext, Owner.Creature.Player, DynamicVars[FeedfireVar.Key].IntValue,
                    cardPlay.Card.CombatState);
    }

    protected override void OnUpgrade()
    {
        // Reduce energy cost by 1 (3 -> 2)
        EnergyCost.UpgradeBy(-1);
    }
}