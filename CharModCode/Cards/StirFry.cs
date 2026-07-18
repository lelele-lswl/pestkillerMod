using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace CharMod.CharModCode.Cards;

public sealed class StirFry : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new CardsVar(1)
        };

    public StirFry()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);

        CardModel freshMeatInHand = base.Owner.PlayerCombatState.Hand.Cards.FirstOrDefault(c => c is FreshMeat);
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
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
