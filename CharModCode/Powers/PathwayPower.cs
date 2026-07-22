using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace CharMod.CharModCode.Powers;

public sealed class PathwayPower : CharModPower
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task Trigger(PlayerChoiceContext? choiceContext, Creature owner, Creature player)
    {
        int stacks = (int)base.Amount;
        if (stacks <= 0) return;
        if (owner.IsDead) return;

        PlayerChoiceContext ctx = choiceContext ?? new BlockingPlayerChoiceContext();

        Flash();
        await CardPileCmd.Draw(ctx, stacks, player.Player!);
        await PlayerCmd.GainEnergy(stacks, player.Player!);
        if (owner.IsDead) return;
        await CreatureCmd.Damage(ctx, owner, stacks * 8m, ValueProp.Unpowered, player);
    }
}