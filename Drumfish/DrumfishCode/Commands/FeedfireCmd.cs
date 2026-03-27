using Drumfish.DrumfishCode.Cards;
using Drumfish.DrumfishCode.Powers;
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
    public static event Func<Player, decimal, Task>? Feedfired;


    public static async Task Execute(PlayerChoiceContext choiceContext, Player player, decimal amount)
    {
        // 判断是否处于 BurnoutPower
        var isInBurnOutPower = player.Creature.GetPower<BurnoutPower>();
        if (isInBurnOutPower != null)
        {
            return;
        }

        var isInHeatyStatusPower = player.Creature.GetPower<HeatyStatusPower>();
        if (isInHeatyStatusPower != null)
        {
            await PlayerCmd.GainEnergy(amount, player);
        }
        else
        {
            await CardPileCmd.Draw(choiceContext, 1, player);
        }

        await NotifyPowers(player, amount);
        await NotifyCards(player, amount);
    }

    private static async Task NotifyPowers(Player player, decimal amount)
    {
        if (Feedfired != null) await Feedfired.Invoke(player, amount);
    }

    private static async Task NotifyCards(Player? player, decimal amount)
    {
        if (player?.PlayerCombatState?.AllPiles != null)
            foreach (var pile in player.PlayerCombatState?.AllPiles!)
            {
                var drumfishCards = pile.Cards.OfType<DrumfishCard>().ToList();

                foreach (var card in drumfishCards) await card.OnFeedfire(player, amount);
            }
    }
}