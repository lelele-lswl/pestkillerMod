using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace CharMod.CharModCode.Cards;

public sealed class FullCourseMeal : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => Array.Empty<DynamicVar>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromCard<FreshMeat>() };

    public FullCourseMeal()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int handSize = base.Owner.PlayerCombatState!.Hand.Cards.Count;
        int maxHand = 10;
        int toAdd = maxHand - handSize;

        for (int i = 0; i < toAdd; i++)
        {
            await CardPileCmd.AddToCombatAndPreview<FreshMeat>(base.Owner.Creature, PileType.Hand, 1, null);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}