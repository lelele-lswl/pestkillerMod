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

public sealed class Ignite : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DamageVar(20m, ValueProp.Move)
        };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromPower<PathwayPower>() };

    public Ignite()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
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
                .WithHitFx("vfx/vfx_molten_fist")
                .Execute(choiceContext);

            if (!cardPlay.Target.IsDead && pathway != null)
            {
                await PowerCmd.Remove(pathway);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}