using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace CharMod.CharModCode.Cards;

public sealed class UltimateForging : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Exhaust };

    public UltimateForging()
        : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> strikes = new List<CardModel>();

        foreach (CardModel card in PileType.Hand.GetPile(base.Owner).Cards)
        {
            if (card.Tags.Contains(CardTag.Strike) && card.IsTransformable)
            {
                strikes.Add(card);
            }
        }
        foreach (CardModel card in PileType.Draw.GetPile(base.Owner).Cards)
        {
            if (card.Tags.Contains(CardTag.Strike) && card.IsTransformable)
            {
                strikes.Add(card);
            }
        }
        foreach (CardModel card in PileType.Discard.GetPile(base.Owner).Cards)
        {
            if (card.Tags.Contains(CardTag.Strike) && card.IsTransformable)
            {
                strikes.Add(card);
            }
        }

        foreach (CardModel strike in strikes)
        {
            await CardCmd.TransformTo<UltimateStrike>(strike);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
