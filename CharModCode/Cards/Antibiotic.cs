using System.Collections.Generic;
using System.Threading.Tasks;
using CharMod.CharModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace CharMod.CharModCode.Cards;

public sealed class Antibiotic : CharModCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[]
        {
            HoverTipFactory.FromPower<AntibioticPower>(),
            HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        };

    public Antibiotic()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ApplyAntibioticPower(choiceContext);
    }

    // 被消耗时触发（变化不算消耗，见 AfterTransformedFrom）
    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card == this && base.CombatState != null)
        {
            await ApplyAntibioticPower(choiceContext);
        }
    }

    // 被变化时触发（变化不走消耗钩子，是独立的同步钩子）。
    // 仅战斗中生效；战斗外（事件变化、星盘等）跳过。
    // 注意：不能用 base.CombatState 判断——游戏在调用此钩子前已 RemoveFromCurrentPile()，
    // CombatState 依赖 Pile != null，此时恒为 null。改用 Owner.Creature.CombatState（与 CardCmd.Exhaust 同款回退）。
    public override void AfterTransformedFrom()
    {
        var combatState = base.Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }
        HookPlayerChoiceContext context = new(this, LocalContext.NetId ?? base.Owner.NetId, combatState, GameActionType.CombatPlayPhaseOnly);
        _ = context.AssignTaskAndWaitForPauseOrCompletion(ApplyAntibioticPower(context));
    }

    // 效果统一延迟到下回合开始时生效，见 AntibioticPower。
    private async Task ApplyAntibioticPower(PlayerChoiceContext choiceContext)
    {
        await PowerCmd.Apply<AntibioticPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
