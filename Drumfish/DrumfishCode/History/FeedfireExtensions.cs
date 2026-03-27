

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Players;
using Drumfish.DrumfishCode.History.Entries;
using Drumfish.DrumfishCode.Patches;

namespace Drumfish.DrumfishCode.History;


public static class FeedfireExtensions
{
    public static void GainFeedfire(this CombatHistory history, CombatState combatState, decimal amount, Player player)
    {
        if (amount <= 0m) return; 
        
        var entry = new FeedfireEntry(amount, player, combatState.RoundNumber, combatState.CurrentSide, history);
        
        // 调用我们利用 [HarmonyReversePatch] 暴露出来的 AddEntry 补丁
        history.AddEntry(entry); 
    }
}