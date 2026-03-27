using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Drumfish.DrumfishCode.Variables;

public class TemporaryThornVar: DynamicVar
{
    public const string Key = "TemporaryThorn";

    public TemporaryThornVar(decimal count) : base(Key, count)
    {
        this.WithTooltip();
    }
}