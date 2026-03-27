using System;
using System.Linq;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Drumfish.DrumfishCode.Variables;

/**
 * <summary>
 * 加薪
 * 数据正常显示，逻辑由patch做
 * </summary>
 */
public class FeedfireVar : DynamicVar
{
    public const string Key = "Feedfire";

    public FeedfireVar(decimal feedfireCount) : base(Key, feedfireCount)
    {
        this.WithTooltip();
    }
}