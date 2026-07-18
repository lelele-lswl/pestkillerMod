using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CharMod.CharModCode.Powers;

public sealed class PoisonPoison2Power : CharModPower
{
    private class Data
    {
        public int cardDrawCount;
        public int cardExhaustCount;
        public int cardPlayCount;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        var data = GetInternalData<Data>();
        data.cardDrawCount++;
        if (data.cardDrawCount >= 1)
        {
            data.cardDrawCount = 0;
            await ApplyPoisonToAll(choiceContext);
        }
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        var data = GetInternalData<Data>();
        data.cardExhaustCount++;
        if (data.cardExhaustCount >= 1)
        {
            data.cardExhaustCount = 0;
            await ApplyPoisonToAll(choiceContext);
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var data = GetInternalData<Data>();
        data.cardPlayCount++;
        if (data.cardPlayCount >= 1)
        {
            data.cardPlayCount = 0;
            await ApplyPoisonToAll(choiceContext);
        }
    }

    private async Task ApplyPoisonToAll(PlayerChoiceContext choiceContext)
    {
        Flash();
        foreach (var enemy in base.CombatState.Enemies.Where(e => e.IsAlive).ToList())
        {
            await PowerCmd.Apply<PoisonPower>(choiceContext, enemy, base.Amount, base.Owner, null, silent: true);
        }
    }
}