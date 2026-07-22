using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CharMod.CharModCode.Cards;

public sealed class Braise : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new CardsVar(1),
            new PowerVar<StrengthPower>(1m)
        };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[]
        {
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromCard<FreshMeat>(),
        };

    public Braise()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
        await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, base.DynamicVars.Strength.BaseValue, base.Owner.Creature, this);

        CardModel? freshMeatInHand = base.Owner.PlayerCombatState!.Hand.Cards.FirstOrDefault(c => c is FreshMeat);
        if (freshMeatInHand == null)
        {
            CardModel freshMeat = CardFactory.GetForCombat(
                base.Owner,
                new[] { ModelDb.Card<FreshMeat>() },
                1,
                base.Owner.RunState.Rng.CombatCardGeneration).First();

            await CardPileCmd.AddGeneratedCardToCombat(freshMeat, PileType.Hand, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：抽牌 +1，同时更新 DynamicVar 以便 {Cards:diff()} 文本高亮变化
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
