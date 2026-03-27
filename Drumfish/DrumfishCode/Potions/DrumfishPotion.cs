using BaseLib.Abstracts;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;

namespace Drumfish.DrumfishCode.Potions;

[Pool(typeof(DrumfishPotionPool))]
public abstract class DrumfishPotion : CustomPotionModel;