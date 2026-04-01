using BaseLib.Abstracts;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Powers;
using Drumfish.DrumfishCode.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Uncommon;

public class TemperedFeatherTempThornsPower : TemporaryThornsPower
{
    public override AbstractModel OriginModel => ModelDb.Card<TemperedFeather>();
}

[Pool(typeof(DrumfishCardPool))]
public class TemperedFeather() : DrumfishCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ThornsPower>(8m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Apply temporary thorns
        await PowerCmd.Apply<TemperedFeatherTempThornsPower>(Owner.Creature, DynamicVars["ThornsPower"].IntValue,
            Owner.Creature, cardPlay.Card);

        // After applying the temporary thorns, get the current thorns amount and grant that much block
        var thorns = Owner.Creature.GetPower<ThornsPower>();
        var thornsAmount = thorns?.Amount ?? 0m;
        if (thornsAmount > 0m)
        {
            var thornBlockVar = new BlockVar(thornsAmount, ValueProp.Unpowered);
            await CreatureCmd.GainBlock(base.Owner.Creature, thornBlockVar, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ThornsPower"].UpgradeValueBy(4m);
    }
}