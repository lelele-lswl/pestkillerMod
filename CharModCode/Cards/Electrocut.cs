using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using CharMod.CharModCode.Powers;

namespace CharMod.CharModCode.Cards;

public sealed class Electrocut : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DamageVar(12m, ValueProp.Move)
        };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromPower<PathwayPower>() };

    public Electrocut()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        if (cardPlay.Target.IsDead) return;

        PathwayPower? pathway = cardPlay.Target.GetPower<PathwayPower>();
        int stacks = pathway != null ? (int)pathway.Amount : 0;

        if (stacks > 0)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .WithHitCount(stacks)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_lightning")
                .Execute(choiceContext);

            await pathway!.Trigger(choiceContext, cardPlay.Target, base.Owner.Creature);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
