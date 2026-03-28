using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Drumfish.DrumfishCode.History.Entries;



public class FeedfireEntry(decimal amount, Player player, int roundNumber, CombatSide currentSide, CombatHistory history)
    : CombatHistoryEntry(player.Creature, roundNumber, currentSide, history)
{
  public decimal Amount { get; } = amount;

  public override string Description => $"{base.Actor.Player.Character.Id.Entry} gained {Amount} Feedfire(s)";
}