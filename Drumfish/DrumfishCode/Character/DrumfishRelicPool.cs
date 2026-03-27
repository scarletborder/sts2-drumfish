using BaseLib.Abstracts;
using Drumfish.DrumfishCode.Extensions;
using Godot;

namespace Drumfish.DrumfishCode.Character;

public class DrumfishRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Drumfish.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}