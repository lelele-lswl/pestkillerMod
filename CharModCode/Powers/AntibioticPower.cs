using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CharMod.CharModCode.Powers;

/// <summary>
/// 抗生素的延迟生效载体：挂上后，你的下回合开始时消耗所有牌堆中的状态牌，然后移除自身。
/// 参考 SummonNextTurnPower。
/// </summary>
public sealed class AntibioticPower : CharModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player || base.AmountOnTurnStart == 0)
        {
            return;
        }

        Flash();
        List<CardModel> statuses = base.Owner.Player!.PlayerCombatState!.AllCards
            .Where(c => c.Type == CardType.Status && c.Pile != null && c.Pile.Type != PileType.Exhaust)
            .ToList();
        foreach (CardModel status in statuses)
        {
            await CardCmd.Exhaust(choiceContext, status);
            // 每消耗一张状态牌，失去 1 点生命
            await CreatureCmd.Damage(choiceContext, base.Owner, 1m, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, base.Owner);
        }
        await PowerCmd.Remove(this);
    }
}
