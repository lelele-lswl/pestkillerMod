using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace CharMod.CharModCode.Cards;

public sealed class FreeHand : CharModCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Exhaust };

    public FreeHand()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var drawPile = PileType.Draw.GetPile(base.Owner);
        if (drawPile.Cards.Count > 0)
        {
            var selectedFromDraw = (await CardSelectCmd.FromCombatPile(
                prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 1),
                context: choiceContext,
                pile: drawPile,
                player: base.Owner)).FirstOrDefault();
            if (selectedFromDraw != null)
            {
                await CardPileCmd.Add(selectedFromDraw, PileType.Hand);
            }
        }

        var discardPile = PileType.Discard.GetPile(base.Owner);
        if (discardPile.Cards.Count > 0)
        {
            var selectedFromDiscard = (await CardSelectCmd.FromCombatPile(
                prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 1),
                context: choiceContext,
                pile: discardPile,
                player: base.Owner)).FirstOrDefault();
            if (selectedFromDiscard != null)
            {
                await CardPileCmd.Add(selectedFromDiscard, PileType.Hand);
            }
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}