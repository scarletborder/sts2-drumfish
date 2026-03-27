using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Extensions;
using Drumfish.DrumfishCode.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Drumfish.DrumfishCode.Cards.Common;

 
[Pool(typeof(DrumfishCardPool))]
public class DamaiNet() : DrumfishCard(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9, ValueProp.Move),
        new FeedfireVar(2m)
    ];

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 对所有敌人造成伤害
        await CommonActions.CardAttack(play.Card, play,DynamicVars.Damage.IntValue).Execute(choiceContext);

        // 触发 加薪
        if (play.Card.CombatState != null)
            await FeedfireCmd.Execute(choiceContext, play.Card.Owner, DynamicVars[FeedfireVar.Key].IntValue,
                play.Card.CombatState);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[FeedfireVar.Key].UpgradeValueBy(1m);
    }
}
