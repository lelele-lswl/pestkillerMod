using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace CharMod.CharModCode.Cards;

public sealed class Spark : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new CardsVar(1),
            new DynamicVar("Energy", 1m),
            new DynamicVar("CardsToGenerate", 1m),
            new DynamicVar("CardsToExhaust", 1m)
        };

    public Spark()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int drawCount = (int)base.DynamicVars.Cards.BaseValue;
        int energyGain = (int)base.DynamicVars["Energy"].BaseValue;
        int genCount = (int)base.DynamicVars["CardsToGenerate"].BaseValue;
        int exhaustCount = (int)base.DynamicVars["CardsToExhaust"].BaseValue;

        await CardPileCmd.Draw(choiceContext, drawCount, base.Owner);
        await PlayerCmd.GainEnergy(energyGain, base.Owner);

        List<CardModel> generatedCards = CardFactory.GetForCombat(
            base.Owner,
            base.Owner.Character.CardPool.GetUnlockedCards(
                base.Owner.UnlockState,
                base.Owner.RunState.CardMultiplayerConstraint),
            genCount,
            base.Owner.RunState.Rng.CombatCardGeneration).ToList();

        foreach (CardModel card in generatedCards)
        {
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, base.Owner);
        }

        IEnumerable<CardModel> cardsToExhaust = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, exhaustCount),
            context: choiceContext,
            player: base.Owner,
            filter: null,
            source: this);

        foreach (CardModel card in cardsToExhaust)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
        base.DynamicVars["Energy"].UpgradeValueBy(1m);
        base.DynamicVars["CardsToGenerate"].UpgradeValueBy(1m);
        base.DynamicVars["CardsToExhaust"].UpgradeValueBy(1m);
    }
}