using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace CharMod.CharModCode.Cards;

public sealed class Printer : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new CardsVar(1)
        };

    public Printer()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel? selection = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 1),
            context: choiceContext,
            player: base.Owner,
            filter: c => c.Type == CardType.Skill || c.Type == CardType.Power,
            source: this)).FirstOrDefault();

        if (selection != null)
        {
            for (int i = 0; i < base.DynamicVars.Cards.BaseValue; i++)
            {
                CardModel card = selection.CreateClone();
                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, base.Owner);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}