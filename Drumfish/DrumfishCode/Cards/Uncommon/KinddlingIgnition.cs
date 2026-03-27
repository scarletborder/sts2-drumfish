using Drumfish.DrumfishCode.Variables;

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


[Pool(typeof(DrumfishCardPool))]
public class KindlingIgnition() : DrumfishCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4, ValueProp.Move),
        new FeedfireVar(1),
        new CardsVar(2),
    ];

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play.Target).Execute(choiceContext);

        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            var match = base.CombatState?.CreateCard<Match>(Owner);
            if (match == null) continue;
            if (IsUpgraded)
            {
                CardCmd.Upgrade(match);
            }

            await CardPileCmd.AddGeneratedCardToCombat(match, PileType.Hand, true);
        }
    }
}