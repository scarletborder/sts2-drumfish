

// sts2, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
namespace Drumfish.DrumfishCode.Powers;

public sealed class GuardPower : DrumfishPower
{
    // 这是一个负面效果
    public override PowerType Type => PowerType.Debuff;

    // Single 表示不需要层数/数值叠加，有和没有仅为 true/false 的状态
    public override PowerStackType StackType => PowerStackType.Single;

    // 用于记录当次受伤是否触发了伤害翻倍，以便播放特效
    private bool _triggeredThisHit = false;

    public override decimal ModifyHpLostBeforeOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!CombatManager.Instance.IsInProgress)
        {
            return amount;
        }
        if (target != base.Owner)
        {
            return amount;
        }

        // 判定伤害来源不为空，且伤害来源的阵营与持有者的阵营不同（即来源于敌人）
        // 如果想写死判断只针对“敌方怪物阵营”，可以使用 dealer.Side == CombatSide.Enemy
        if (dealer != null && dealer.Side != base.Owner.Side)
        {
            _triggeredThisHit = true;
            return amount * 2m; // 伤害翻倍
        }

        return amount;
    }

    public override Task AfterModifyingHpLostAfterOsty()
    {
        // 如果刚刚触发了翻倍效果，闪烁该 Power 的特效提示玩家
        if (_triggeredThisHit)
        {
            Flash();
            _triggeredThisHit = false;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        // 在拥有此 Power 的角色回合结束时消失
        if (side != CombatSide.None && side != base.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}