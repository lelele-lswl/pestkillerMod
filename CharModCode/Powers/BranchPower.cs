using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CharMod.CharModCode.Powers;

public sealed class BranchPower : CharModPower
{
    private class Data
    {
        public int exhaustCount;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner.Creature != base.Owner)
        {
            return;
        }

        var data = GetInternalData<Data>();
        data.exhaustCount++;
        int threshold = base.Amount >= 2 ? 1 : 2;
        if (data.exhaustCount >= threshold)
        {
            data.exhaustCount -= threshold;
            Flash();

            var cards = CardFactory.GetForCombat(
                base.Owner.Player,
                base.Owner.Player.Character.CardPool.GetUnlockedCards(
                    base.Owner.Player.UnlockState,
                    base.Owner.Player.RunState.CardMultiplayerConstraint),
                1,
                base.Owner.Player.RunState.Rng.CombatCardGeneration).ToList();

            await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, base.Owner.Player);
        }
    }
}