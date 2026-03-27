using BaseLib.Utils;
using Drumfish.DrumfishCode.Cards.Colorless;
using Drumfish.DrumfishCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Drumfish.DrumfishCode.Cards.Uncommon;


[Pool(typeof(DrumfishCardPool))]
public class Gardening() : DrumfishCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int num = ResolveEnergyXValue() + 1;
        List<CardModel> readyToAdd = [];
        for (var i = 0; i < num; i++)
        {
            var match = base.CombatState?.CreateCard<Match>(Owner);
            if (match == null) continue;
            if (base.IsUpgraded)
            {
                CardCmd.Upgrade(match);
            }

            readyToAdd.Add(match);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(readyToAdd, PileType.Hand, true);
    }
}