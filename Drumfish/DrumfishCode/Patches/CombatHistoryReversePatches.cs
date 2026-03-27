using HarmonyLib;
using MegaCrit.Sts2.Core.Combat.History;

namespace Drumfish.DrumfishCode.Patches;

[HarmonyPatch(typeof(CombatHistory))]
public static class CombatHistoryReversePatches
{
    /// <summary>
    /// 这是一个反向补丁。Harmony 会将 CombatHistory.Add(CombatHistoryEntry) 的私有实现克隆到这里。
    /// 因为我们将第一个参数设为了 this CombatHistory instance，它还能直接当作扩展方法使用！
    /// </summary>
    [HarmonyReversePatch]
    [HarmonyPatch("Add")] // 指定目标为 CombatHistory 的私有方法 "Add"
    public static void AddEntry(this CombatHistory instance, CombatHistoryEntry entry)
    {
        // 这里的代码永远不会执行，Harmony 会在游戏启动加载模组时替换这个方法体。
        throw new System.NotImplementedException("Stub for Reverse Patch");
    }
}