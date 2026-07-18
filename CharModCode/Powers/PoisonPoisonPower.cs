using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CharMod.CharModCode.Powers;

public sealed class PoisonPoisonPower : CharModPower
{
    private class Data
    {
        public int poisonApplyCount;
        public bool isApplying;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        var data = GetInternalData<Data>();
        if (data.isApplying) return;
        if (power is PoisonPower && power.Owner != null && power.Owner.IsEnemy && applier == base.Owner)
        {
            data.poisonApplyCount++;
            if (data.poisonApplyCount >= 2)
            {
                data.poisonApplyCount = 0;
                data.isApplying = true;
                try
                {
                    Flash();
                    await PowerCmd.Apply<PoisonPower>(choiceContext, power.Owner, base.Amount, base.Owner, null, silent: true);
                }
                finally
                {
                    data.isApplying = false;
                }
            }
        }
    }
}