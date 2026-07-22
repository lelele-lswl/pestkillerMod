using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using CharMod.CharModCode.Powers;

namespace CharMod.CharModCode.Cards;

public sealed class MysteryBox : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[]
        {
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromPower<DexterityPower>(),
            HoverTipFactory.FromPower<CharMod.CharModCode.Powers.FocusPower>(),
            HoverTipFactory.FromPower<PathwayPower>(),
        };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Exhaust };

    public MysteryBox()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // NextInt 上界为开区间：0-5 共 6 种结果（原 NextInt(0, 5) 导致 case 5 永远不触发）
        int roll = base.Owner.RunState.Rng.Niche.NextInt(0, 6);

        switch (roll)
        {
            case 0:
                await PlayerCmd.GainGold(50m, base.Owner);
                break;
            case 1:
                var relic = RelicFactory.PullNextRelicFromFront(base.Owner).ToMutable();
                await RelicCmd.Obtain(relic, base.Owner);
                break;
            case 2:
                await CreatureCmd.GainBlock(base.Owner.Creature, 8m, ValueProp.Unpowered, cardPlay);
                break;
            case 3:
                await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, 5m, base.Owner.Creature, this);
                break;
            case 4:
                await PowerCmd.Apply<DexterityPower>(choiceContext, base.Owner.Creature, 4m, base.Owner.Creature, this);
                break;
            case 5:
                await PowerCmd.Apply<CharMod.CharModCode.Powers.FocusPower>(choiceContext, base.Owner.Creature, 3m, base.Owner.Creature, this);
                foreach (var enemy in base.CombatState!.Enemies.ToList())
                {
                    await PowerCmd.Apply<PathwayPower>(choiceContext, enemy, 2m, base.Owner.Creature, this);
                }
                break;
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}