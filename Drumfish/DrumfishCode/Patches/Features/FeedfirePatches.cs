using System.Linq;
using BaseLib.Cards.Variables;
using Drumfish.DrumfishCode.Powers;
using Drumfish.DrumfishCode.Variables;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace Drumfish.DrumfishCode.Patches.Features;

[HarmonyPatch(typeof(Hook), "BeforeCardPlayed")]
public class FeedfirePatches
{
    public static async void Postfix(CombatState combatState, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 如果卡片有 FeedFire 并且 打出者 有 HeatyStatus
        var card = cardPlay.Card;
        // 从卡牌动态变量读取 FeedFire 值（如果卡牌通过 WithVar/WithVars 添加了 FeedfireVar）
        var feedfireAmount = card.DynamicVars.TryGetValue(FeedfireVar.Key, out var val) ? val.IntValue : 0;
        if (feedfireAmount <= 0) return;

        var owner = card.Owner;

        var ownerCreature = owner.Creature;

        var isInHeat = ownerCreature.GetPower<HeatyStatusPower>();
        if (isInHeat == null)
        {
            await CardPileCmd.Draw(choiceContext, 1, owner);
        }
        else
        {
            await PlayerCmd.GainEnergy(feedfireAmount, owner);
        }
    }
}