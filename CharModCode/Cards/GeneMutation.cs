using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace CharMod.CharModCode.Cards;

public sealed class GeneMutation : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DynamicVar("CardsToTransform", 3m),
            new DynamicVar("CardsToExhaust", 1m)
        };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromKeyword(CardKeyword.Exhaust) };

    public GeneMutation()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 自选至多 3 张手牌，变化成随机牌
        int transformCount = base.DynamicVars["CardsToTransform"].IntValue;
        IEnumerable<CardModel> toTransform = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 0, transformCount),
            context: choiceContext,
            player: base.Owner,
            filter: null,
            source: this);
        foreach (CardModel card in toTransform)
        {
            await CardCmd.TransformToRandom(card, base.Owner.RunState.Rng.CombatCardGeneration);
        }

        // 再自选 1 张手牌消耗
        int exhaustCount = base.DynamicVars["CardsToExhaust"].IntValue;
        IEnumerable<CardModel> toExhaust = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, exhaustCount),
            context: choiceContext,
            player: base.Owner,
            filter: null,
            source: this);
        foreach (CardModel card in toExhaust)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
