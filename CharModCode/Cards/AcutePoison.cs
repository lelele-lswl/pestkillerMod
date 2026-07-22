using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CharMod.CharModCode.Cards;

public sealed class AcutePoison : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new PowerVar<PoisonPower>(3m),
            new RepeatVar(3)
        };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromPower<PoisonPower>() };

    public AcutePoison()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        for (int i = 0; i < base.DynamicVars.Repeat.IntValue; i++)
        {
            await PowerCmd.Apply<PoisonPower>(choiceContext, cardPlay.Target, base.DynamicVars.Poison.BaseValue, base.Owner.Creature, this);
        }

        base.EnergyCost.UpgradeBy(1);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Poison.UpgradeValueBy(1m);
    }
}