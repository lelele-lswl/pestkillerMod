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
using MegaCrit.Sts2.Core.ValueProps;

namespace CharMod.CharModCode.Cards;

public sealed class DreamShield : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new BlockVar(16m, ValueProp.Move)
        };

    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Retain };

    public DreamShield()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        var allCards = base.Owner.Character.CardPool.GetUnlockedCards(
            base.Owner.UnlockState,
            base.Owner.RunState.CardMultiplayerConstraint);

        var rareCards = allCards.Where(c => c.Rarity == CardRarity.Rare).ToList();

        List<CardModel> choices = CardFactory.GetDistinctForCombat(
            base.Owner,
            rareCards,
            3,
            base.Owner.RunState.Rng.CombatCardGeneration).ToList();

        CardModel selected = await CardSelectCmd.FromChooseACardScreen(
            choiceContext, choices, base.Owner, canSkip: false);

        if (selected != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(selected, PileType.Hand, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(4m);
    }
}