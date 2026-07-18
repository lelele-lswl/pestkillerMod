using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace CharMod.CharModCode.Powers;

public sealed class PoisonCoatPower : CharModPower
{
    private class Data
    {
        public int unblockedAttackCount;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer == base.Owner
            && props.IsPoweredAttack()
            && target.IsEnemy
            && result.UnblockedDamage > 0)
        {
            var data = GetInternalData<Data>();
            data.unblockedAttackCount++;
            int threshold = (int)base.Amount;
            if (data.unblockedAttackCount >= threshold)
            {
                data.unblockedAttackCount = 0;
                Flash();
                await PowerCmd.Apply<PoisonPower>(choiceContext, target, 1m, base.Owner, null, silent: true);
            }
        }
    }
}