using Drumfish.DrumfishCode.Cards;
using Drumfish.DrumfishCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Drumfish.DrumfishCode.Commands;

public static class HeatyCmd
{
    /**
     * 监听 进入/退出 上火 事件，会 invoke
     * 注意：能力卡/Power 在 OnRemove 和 OnCombatEnd 时需要移除监听（使用 -=）
     */
    public static event Func<PlayerChoiceContext, Player, Task>? HeatyEntered;

    public static event Func<PlayerChoiceContext, Player, Task>? HeatyExited;

    /// <summary>
    /// 触发「进入上火」逻辑并通知订阅者与卡牌
    /// </summary>
    public static async Task Enter(PlayerChoiceContext choiceContext, Player player, CardModel card)
    {
        await PowerCmd.Apply<HeatyStatusPower>(player.Creature, 1, player.Creature, card);
        await NotifyPowersEntered(choiceContext, player);
        await NotifyCardsEntered(choiceContext, player);
    }

    /// <summary>
    /// 触发「退出上火」逻辑并通知订阅者与卡牌
    /// </summary>
    public static async Task Exit(PlayerChoiceContext choiceContext, Player player)
    {
        await PowerCmd.Remove<HeatyStatusPower>(player.Creature);
        await NotifyPowersExited(choiceContext, player);
        await NotifyCardsExited(choiceContext, player);
    }

    public static bool IsInHeaty(Player player)
    {
        return (player.Creature.GetPower<HeatyStatusPower>() != null);
    }

    private static async Task NotifyPowersEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (HeatyEntered != null) await HeatyEntered.Invoke(choiceContext, player);
    }

    private static async Task NotifyPowersExited(PlayerChoiceContext choiceContext, Player player)
    {
        if (HeatyExited != null) await HeatyExited.Invoke(choiceContext, player);
    }

    private static async Task NotifyCardsEntered(PlayerChoiceContext choiceContext, Player? player)
    {
        if (player?.PlayerCombatState?.AllPiles != null)
            foreach (var pile in player.PlayerCombatState?.AllPiles!)
            {
                var drumfishCards = pile.Cards.OfType<DrumfishCard>().ToList();

                foreach (var card in drumfishCards) await card.OnHeatyEnter(choiceContext, player);
            }
    }

    private static async Task NotifyCardsExited(PlayerChoiceContext choiceContext, Player? player)
    {
        if (player?.PlayerCombatState?.AllPiles != null)
            foreach (var pile in player.PlayerCombatState?.AllPiles!)
            {
                var drumfishCards = pile.Cards.OfType<DrumfishCard>().ToList();

                foreach (var card in drumfishCards) await card.OnHeatyExit(choiceContext, player!);
            }
    }
}