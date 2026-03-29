using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Commands;
using Drumfish.DrumfishCode.Powers;
using Drumfish.DrumfishCode.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Drumfish.DrumfishCode.Cards.Rare;

public class YuyukoPower : TemporaryStrengthPower, ICustomModel
{
    public override AbstractModel OriginModel => ModelDb.Card<Yuyuko>();

    protected override bool IsPositive => false;
}

[Pool(typeof(DrumfishCardPool))]
public class Yuyuko() : DrumfishCard(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        // damage is 8 per X
        new DamageVar(8m, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        int x = ResolveEnergyXValue();
        if (x <= 0) return;

        for (int i = 0; i < x; i++)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this)
                .TargetingAllOpponents(base.CombatState)
                .Execute(choiceContext);
            // Apply YuyukoPower to target to remove Strength equal to damage for this turn
            await PowerCmd.Apply<YuyukoPower>(cardPlay.Target, DynamicVars.Damage.BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}