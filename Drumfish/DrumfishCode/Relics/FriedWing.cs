using BaseLib.Extensions;
using Drumfish.DrumfishCode.Cards.Colorless;
using Drumfish.DrumfishCode.Cards.Common;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;


namespace Drumfish.DrumfishCode.Relics;

public class FriedWing : DrumfishRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<FirewoodFeather>(),
        HoverTipFactory.FromPower<ThornsPower>()
    ];

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        var friedWing = this;
        if (side != friedWing.Owner.Creature.Side || combatState.RoundNumber > 1)
        {
            await PowerCmd.Apply<ThornsPower>(Owner.Creature, 1, Owner.Creature, null);
            return;
        }

        // 向3pile各添加一张
        List<PileType> targetPiles = [PileType.Hand, PileType.Draw, PileType.Exhaust];
        foreach (PileType targetPile in targetPiles)
        {
            var feather = combatState.CreateCard<FirewoodFeather>(Owner);
            CardCmd.Upgrade(feather);
            await CardPileCmd.AddGeneratedCardToCombat(feather, targetPile, true);
        }

        friedWing.Flash();
    }
}