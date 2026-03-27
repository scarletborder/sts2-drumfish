using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Drumfish.DrumfishCode.Powers;

public abstract class TemporaryThornsPower : DrumfishPower, ITemporaryPower
{
    private bool _shouldIgnoreNextInstance;

    public override PowerType Type
    {
        get
        {
            if (!IsPositive)
            {
                return PowerType.Debuff;
            }

            return PowerType.Buff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Counter;

    public abstract AbstractModel OriginModel { get; }

    public PowerModel InternallyAppliedPower => ModelDb.Power<ThornsPower>();

    protected virtual bool IsPositive => true;

    private int Sign
    {
        get
        {
            if (!IsPositive)
            {
                return -1;
            }

            return 1;
        }
    }

    public override LocString Title
    {
        get
        {
            var originModel = OriginModel;
            return originModel switch
            {
                CardModel cardModel => cardModel.TitleLocString,
                PotionModel potionModel => potionModel.Title,
                RelicModel relicModel => relicModel.Title,
                _ => throw new InvalidOperationException()
            };
        }
    }

    public override LocString Description => new LocString("powers",
        IsPositive ? "TEMPORARY_THORNS_POWER.description" : "TEMPORARY_THORNS_DOWN.description");

    protected override string SmartDescriptionLocKey
    {
        get
        {
            return !IsPositive ? "TEMPORARY_THORNS_DOWN.smartDescription" : "TEMPORARY_THORNS_POWER.smartDescription";
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var list = new List<IHoverTip>();
            var originModel = OriginModel;

            var collection = originModel switch
            {
                CardModel card => [HoverTipFactory.FromCard(card)],
                PotionModel model => [HoverTipFactory.FromPotion(model)],
                RelicModel relic => HoverTipFactory.FromRelic(relic),
                _ => throw new InvalidOperationException()
            };

            list.AddRange(collection);
            list.Add(HoverTipFactory.FromPower<ThornsPower>());

            return list.AsReadOnly();
        }
    }

    public void IgnoreNextInstance()
    {
        _shouldIgnoreNextInstance = true;
    }

    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (_shouldIgnoreNextInstance)
        {
            _shouldIgnoreNextInstance = false;
        }
        else
        {
            await PowerCmd.Apply<ThornsPower>(target, (decimal)Sign * amount, applier, cardSource, silent: true);
        }
    }

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (amount != (decimal)base.Amount && power == this)
        {
            if (_shouldIgnoreNextInstance)
            {
                _shouldIgnoreNextInstance = false;
            }
            else
            {
                await PowerCmd.Apply<ThornsPower>(base.Owner, (decimal)Sign * amount, applier, cardSource,
                    silent: true);
            }
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.None && side != base.Owner.Side)
        {
            Flash();
            await PowerCmd.Remove(this);
            await PowerCmd.Apply<ThornsPower>(base.Owner, -Sign * base.Amount, base.Owner, null);
        }
    }
}