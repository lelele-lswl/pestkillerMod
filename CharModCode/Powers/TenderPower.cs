using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CharMod.CharModCode.Powers;

public sealed class TenderPower : CharModPower
{
    private class Data
    {
        public int turnStrengthGain;
        public int turnDexterityGain;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == base.Owner.Player)
        {
            Flash();
            int amount = (int)base.Amount;
            await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner, amount, base.Applier, null, silent: true);
            await PowerCmd.Apply<DexterityPower>(choiceContext, base.Owner, amount, base.Applier, null, silent: true);
            var data = GetInternalData<Data>();
            data.turnStrengthGain += amount;
            data.turnDexterityGain += amount;
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player && participants.Contains(base.Owner))
        {
            var data = GetInternalData<Data>();
            if (data.turnStrengthGain > 0)
            {
                await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner, -data.turnStrengthGain, base.Applier, null, silent: true);
            }
            if (data.turnDexterityGain > 0)
            {
                await PowerCmd.Apply<DexterityPower>(choiceContext, base.Owner, -data.turnDexterityGain, base.Applier, null, silent: true);
            }
            data.turnStrengthGain = 0;
            data.turnDexterityGain = 0;
        }
    }
}