
using Drumfish.DrumfishCode.Cards.Ancient;
using Drumfish.DrumfishCode.Cards.Basic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Drumfish.DrumfishCode.Patches;

[HarmonyPatch(typeof(ArchaicTooth), "TranscendenceUpgrades", MethodType.Getter)]
public static class ArchaicToothPatch
{
    [HarmonyPostfix]
    private static void AddWatcherTranscendence(ref Dictionary<ModelId, CardModel> __result)
    {
        __result[ModelDb.Card<FlameStrike>().Id] = ModelDb.Card<PhoenixStrike>();
    }
}