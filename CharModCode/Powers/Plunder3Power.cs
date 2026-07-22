using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace CharMod.CharModCode.Powers;

public sealed class Plunder3Power : CharModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        for (int i = 0; i < base.Amount; i++)
        {
            RelicModel relic = RelicFactory.PullNextRelicFromFront(base.Owner.Player!);
            await RelicCmd.Obtain(relic.ToMutable(), base.Owner.Player!);
        }
    }
}