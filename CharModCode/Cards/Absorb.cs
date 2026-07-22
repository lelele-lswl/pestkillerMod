using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CharMod.CharModCode.Cards;

public sealed class Absorb : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new PowerVar<IntangiblePower>(1m),
            new PowerVar<FocusPower>(2m)
        };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[]
        {
            HoverTipFactory.FromPower<IntangiblePower>(),
            HoverTipFactory.FromPower<FocusPower>(),
        };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Exhaust };

    public Absorb()
        : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<IntangiblePower>(choiceContext, base.Owner.Creature, base.DynamicVars["IntangiblePower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<CharMod.CharModCode.Powers.FocusPower>(choiceContext, base.Owner.Creature, base.DynamicVars["FocusPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["FocusPower"].UpgradeValueBy(1m);
    }
}