using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CharMod.CharModCode.Cards;

public sealed class OverlordEgg : CharModCard, BaseLib.Abstracts.ITomeCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new PowerVar<StrengthPower>(1m),
            new PowerVar<DexterityPower>(1m)
        };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[]
        {
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromPower<DexterityPower>(),
        };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Innate, CardKeyword.Exhaust };

    public OverlordEgg()
        : base(0, CardType.Skill, CardRarity.Ancient, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, 1, base.Owner);
        await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, base.DynamicVars.Strength.BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<DexterityPower>(choiceContext, base.Owner.Creature, base.DynamicVars.Dexterity.BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Strength.UpgradeValueBy(1m);
        base.DynamicVars.Dexterity.UpgradeValueBy(1m);
    }
}