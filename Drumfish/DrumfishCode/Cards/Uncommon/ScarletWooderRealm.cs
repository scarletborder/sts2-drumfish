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
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace Drumfish.DrumfishCode.Cards.Uncommon;

public class ScarletWooderRealmPower : DrumfishPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await base.AfterApplied(applier, cardSource);
        FeedfireCmd.Feedfired += OnFeedfire;
    }

    public override async Task AfterRemoved(Creature owner)
    {
        await base.AfterRemoved(owner);
        FeedfireCmd.Feedfired -= OnFeedfire;
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        await base.AfterCombatEnd(room);
        FeedfireCmd.Feedfired -= OnFeedfire;
    }

    private async Task OnFeedfire(Player player, decimal amount)
    {
        if (player != Owner.Player) return;

        // Play VFX/SFX and deal damage to all hittable enemies
        VfxCmd.PlayOnCreatureCenters(base.CombatState.HittableEnemies, "vfx/vfx_attack_slash");
        SfxCmd.Play("slash_attack.mp3");

        // Use the configured Amount of this power as damage (set when applied by the card)
        await CreatureCmd.Damage(null, base.CombatState.HittableEnemies, base.Amount, ValueProp.Unpowered, base.Owner,
            null);
    }
}

[Pool(typeof(DrumfishCardPool))]
public class ScarletWooderRealm() : DrumfishCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ScarletWooderRealmPower>(Owner.Creature, DynamicVars["Damage"].IntValue, Owner.Creature,
            cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}