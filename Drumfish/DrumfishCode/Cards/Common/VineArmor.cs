using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Extensions;
using Drumfish.DrumfishCode.Powers;
using Drumfish.DrumfishCode.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Common;


[Pool(typeof(DrumfishCardPool))]
public class VineArmor() : DrumfishCard(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new FeedfireVar(1),
        new BlockVar(8, ValueProp.Move),
        new DynamicVar("VineVar", 4),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // 免疫少于4点伤害（本回合）
        await PowerCmd.Apply<VineArmorPower>(Owner.Creature,
            DynamicVars["VineVar"].IntValue, Owner.Creature,
            play.Card);


        if (play.Card.CombatState != null)
            await FeedfireCmd.Execute(choiceContext, play.Card.Owner, DynamicVars[FeedfireVar.Key].IntValue,
                play.Card.CombatState);

        await CommonActions.CardBlock(this, play);
    }


    protected override void OnUpgrade()
    {
        DynamicVars[FeedfireVar.Key].UpgradeValueBy(1m);
        DynamicVars["VineVar"].UpgradeValueBy(2m);
        DynamicVars.Block.UpgradeValueBy(2);
    }
}