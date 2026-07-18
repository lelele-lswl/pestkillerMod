using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;

namespace CharMod.CharModCode.Cards;

public sealed class AnthonyRapture : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { };

    public AnthonyRapture()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int cardsToAdd = CardPile.MaxCardsInHand - PileType.Hand.GetPile(base.Owner).Cards.Count;

        if (cardsToAdd <= 0)
            return;

        List<CardModel> cards = CardFactory.GetForCombat(
            base.Owner,
            base.Owner.Character.CardPool.GetUnlockedCards(
                base.Owner.UnlockState,
                base.Owner.RunState.CardMultiplayerConstraint),
            cardsToAdd,
            base.Owner.RunState.Rng.CombatCardGeneration).ToList();

        if (base.IsUpgraded)
        {
            CardCmd.Upgrade(cards, CardPreviewStyle.None);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, base.Owner);
    }
}
