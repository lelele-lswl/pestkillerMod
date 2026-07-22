using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace CharMod.CharModCode.Powers;

public sealed class AssemblyLinePower : CharModPower
{
    private class Data
    {
        public int exhaustCount;
        public int etherealCount;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromKeyword(CardKeyword.Exhaust) };

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner.Creature == base.Owner)
        {
            if (causedByEthereal)
            {
                GetInternalData<Data>().etherealCount++;
            }
            else
            {
                var data = GetInternalData<Data>();
                data.exhaustCount++;
                int threshold = (int)base.Amount;
                if (threshold > 0 && data.exhaustCount >= threshold)
                {
                    data.exhaustCount = 0;
                    await CardPileCmd.Draw(choiceContext, 1, base.Owner.Player!);
                }
            }
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(base.Owner))
        {
            Data data = GetInternalData<Data>();
            data.exhaustCount += data.etherealCount;
            int threshold = (int)base.Amount;
            while (threshold > 0 && data.exhaustCount >= threshold)
            {
                data.exhaustCount -= threshold;
                await CardPileCmd.Draw(choiceContext, 1, base.Owner.Player!);
            }
            data.etherealCount = 0;
        }
    }
}