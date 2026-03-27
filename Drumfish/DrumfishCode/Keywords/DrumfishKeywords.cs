using BaseLib.Patches.Content;
using Drumfish.DrumfishCode.Variables;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Drumfish.DrumfishCode.Keywords;


public static class DrumfishKeywords
{
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Feedfire;

    public static bool IsFeedfire(this CardModel card)
    {
        return card.Keywords.Contains(Feedfire) || card.DynamicVars.ContainsKey(FeedfireVar.Key);
    }
}