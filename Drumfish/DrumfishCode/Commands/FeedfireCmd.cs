using Drumfish.DrumfishCode.Cards;
using Drumfish.DrumfishCode.History;
using Drumfish.DrumfishCode.Powers;
using Drumfish.DrumfishCode.History.Entries; // 引入我们之前写的扩展方法命名空间
using MegaCrit.Sts2.Core.Combat; // 引入 CombatManager
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Drumfish.DrumfishCode.Commands;

public static class FeedfireCmd
{
    /**
     * 监听 加薪 事件，加薪时会invoke
     * 能力卡，记得在 OnRemove(移除效果) 和 OnCombatEnd 时候用 '-=' 移除监听
     */
    public static event Func<PlayerChoiceContext, Player, decimal, Task>? Feedfired;

    public static async Task Execute(PlayerChoiceContext choiceContext, Player player, decimal amount, CombatState state)
    {
        // 1. 结算前置阻挡逻辑：判断是否处于 BurnoutPower
        var isInBurnOutPower = player.Creature.GetPower<BurnoutPower>();
        if (isInBurnOutPower != null)
        {
            return; // 加薪被阻挡，直接返回，不记录进历史
        }

        // === 【注入点：写入战斗历史记录】 ===
        // 确保加薪数值大于0才记录
        if (amount > 0m && CombatManager.Instance != null)
        {
            // 调用通过 Harmony 反向补丁暴露出来的扩展方法
            CombatManager.Instance.History.GainFeedfire(state, amount, player);
        }

        // 2. 结算加薪实际效果
        var isInHeatyStatusPower = player.Creature.GetPower<HeatyStatusPower>();
        if (isInHeatyStatusPower != null)
        {
            // 处于特殊状态时获得能量
            await PlayerCmd.GainEnergy(amount, player);
        }
        else
        {
            // 注意：如果 CardPileCmd.Draw 签名要求 int，可能需要 (int)amount。这里保持你原样
            await CardPileCmd.Draw(choiceContext, 1, player); 
        }

        // 3. 广播给其他卡牌和遗物
        await NotifyPowers(choiceContext, player, amount);
        await NotifyCards(choiceContext, player, amount);
    }

    private static async Task NotifyPowers(PlayerChoiceContext choiceContext, Player player, decimal amount)
    {
        if (Feedfired != null) await Feedfired.Invoke(choiceContext, player, amount);
    }

    private static async Task NotifyCards(PlayerChoiceContext choiceContext, Player? player, decimal amount)
    {
        if (player?.PlayerCombatState?.AllPiles != null)
            foreach (var pile in player.PlayerCombatState?.AllPiles!)
            {
                var drumfishCards = pile.Cards.OfType<DrumfishCard>().ToList();

                foreach (var card in drumfishCards) await card.OnFeedfire(choiceContext, player, amount);
            }
    }
}