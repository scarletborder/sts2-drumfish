using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Drumfish.DrumfishCode.Cards.Uncommon;


[Pool(typeof(DrumfishCardPool))]
public class Guard() : DrumfishCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<IntangiblePower>()];
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = base.CombatState;
        if (combatState != null)
        {
            IEnumerable<Creature> enumerable = combatState.PlayerCreatures.Where((Creature c) =>
            {
                // 自己不受效果
                if (c.Player == base.Owner) return false;
                return c?.IsAlive ?? false;
            }).ToList();

            foreach (var item in enumerable)
            {
                await PowerCmd.Apply<IntangiblePower>(item, 1, Owner.Creature, this);
            }
            
            await PowerCmd.Apply<GuardPower>(Owner.Creature, 1, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}