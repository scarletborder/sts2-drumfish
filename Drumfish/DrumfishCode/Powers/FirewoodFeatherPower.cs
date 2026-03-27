using Drumfish.DrumfishCode.Cards.Colorless;
using Drumfish.DrumfishCode.Cards.Common;
using MegaCrit.Sts2.Core.Models;

namespace Drumfish.DrumfishCode.Powers;

public class FirewoodFeatherPower : TemporaryThornsPower
{
    public override AbstractModel OriginModel => ModelDb.Card<FirewoodFeather>();
}