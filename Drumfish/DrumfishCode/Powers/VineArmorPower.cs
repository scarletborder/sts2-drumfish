using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Powers;

public class VineArmorPower : DrumfishPower
{
    public override PowerType Type => PowerType.Buff;

    // StackType.Counter 表示它的数值(Amount)用来做计数/阈值，而不叠加持续时间
    public override PowerStackType StackType => PowerStackType.Counter;

    // 用于记录当次受伤是否触发了减免，以便播放特效
    private bool _triggeredThisHit = false;

    public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        MainFile.Logger.Info(target.ToString());
        MainFile.Logger.Info(props.ToString());
        MainFile.Logger.Info(cardSource?.ToString() ?? "null src");
        MainFile.Logger.Info($"{amount}");
        MainFile.Logger.Info($"{CombatManager.Instance.IsInProgress}");

        if (!CombatManager.Instance.IsInProgress)
        {
            return amount;
        }

        if (target != base.Owner)
        {
            return amount;
        }

        // base.Amount 即为初始化时传入的阈值 X
        // 如果受到的伤害小于该阈值 X，则将伤害降为 0
        if (amount < base.Amount)
        {
            _triggeredThisHit = true;
            return 0m;
        }

        return amount;
    }

    public override Task AfterModifyingHpLostAfterOsty()
    {
        // 只有在成功抵消伤害时才闪烁 Power 的特效
        if (_triggeredThisHit)
        {
            Flash();
            _triggeredThisHit = false;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.None && side != base.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}