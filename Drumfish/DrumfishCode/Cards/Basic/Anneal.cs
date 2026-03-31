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
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Basic;

[Pool(typeof(DrumfishCardPool))]
public class Anneal() : DrumfishCard(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new FeedfireVar(1),
        new BlockVar(4, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Card.CombatState != null)
            await FeedfireCmd.Execute(choiceContext, play.Card.Owner, DynamicVars[FeedfireVar.Key].IntValue,
                play.Card.CombatState);

        await CommonActions.CardBlock(this, play);
        var ownerCreature = play.Card.Owner.Creature;
        var isInHeat = ownerCreature.GetPower<HeatyStatusPower>();
        if (isInHeat != null)
        {
            await HeatyCmd.Exit(choiceContext, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
    }

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}