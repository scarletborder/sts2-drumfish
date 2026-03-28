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
using MegaCrit.Sts2.Core.ValueProps;


namespace Drumfish.DrumfishCode.Relics;


public class NewbornWing : DrumfishRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<FirewoodFeather>()];

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        var newbornWing = this;
        if (side != newbornWing.Owner.Creature.Side || combatState.RoundNumber > 1)
            return;

        // 向2pile各添加一张
        List<PileType> targetPiles = [PileType.Hand, PileType.Draw];
        foreach (PileType targetPile in targetPiles)
        {
            var feather = combatState.CreateCard<FirewoodFeather>(Owner);
            await CardPileCmd.AddGeneratedCardToCombat(feather, targetPile, true);
        }

        newbornWing.Flash();
    }
}