using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Extensions;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

// 注意：请根据 STS2 原版或你模组的实际命名空间引入荆棘 Power。
// 例如：using MegaCrit.Sts2.Core.Entities.Powers;
// 这里假设荆棘能力的类名为 ThornsPower

namespace Drumfish.DrumfishCode.Cards.Uncommon;

[Pool(typeof(DrumfishCardPool))]
public sealed class FeatherBarrage() : DrumfishCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new RepeatVar(3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.IntValue)
            .WithHitCount((int)((DynamicVars.Repeat.IntValue) + Owner.Creature.GetPower<ThornsPower>()?.Amount ?? 0m))
            .FromCard(this)
            .TargetingRandomOpponents(base.CombatState)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // ==========================================
        // 2. 后置效果：损失所有荆棘
        // ==========================================
        var playerCreature = cardPlay.Card.Owner.Creature;
        var thornsPower = playerCreature.GetPower<ThornsPower>();

        if (thornsPower != null)
        {
            // 使用底层的 PowerCmd 移除状态
            // （具体方法名请视 STS2 当前版本的 API 而定，可能是 Remove, RemovePower 或 Clear）
            await PowerCmd.Remove<ThornsPower>(playerCreature);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(1m);
    }
}