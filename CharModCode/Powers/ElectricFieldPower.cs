using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using CharMod.CharModCode.Powers;

namespace CharMod.CharModCode.Powers;

public sealed class ElectricFieldPower : CharModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player) return;

        Flash();
        foreach (var enemy in combatState.Enemies.ToList())
        {
            if (enemy.IsDead) continue;
            PathwayPower pathway = enemy.GetPower<PathwayPower>();
            if (pathway != null)
            {
                await pathway.Trigger(null, enemy, base.Owner);
            }
        }
    }
}