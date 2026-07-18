using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace CharMod.CharModCode.Cards;

/// <summary>
/// 究极锻造2 - 罕见技能牌，3费，获得12点格挡，将所有防御牌变成究极防御
/// 升级后减为2费
/// </summary>
public sealed class UltimateForging2 : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new BlockVar(12m, ValueProp.Move)
        };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Exhaust };

    public override bool GainsBlock => true;

    public UltimateForging2()
        : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        List<CardModel> defends = new List<CardModel>();

        foreach (CardModel card in PileType.Hand.GetPile(base.Owner).Cards)
        {
            if (card.Tags.Contains(CardTag.Defend) && card.IsTransformable)
            {
                defends.Add(card);
            }
        }
        foreach (CardModel card in PileType.Draw.GetPile(base.Owner).Cards)
        {
            if (card.Tags.Contains(CardTag.Defend) && card.IsTransformable)
            {
                defends.Add(card);
            }
        }
        foreach (CardModel card in PileType.Discard.GetPile(base.Owner).Cards)
        {
            if (card.Tags.Contains(CardTag.Defend) && card.IsTransformable)
            {
                defends.Add(card);
            }
        }

        foreach (CardModel defend in defends)
        {
            await CardCmd.TransformTo<UltimateDefend>(defend);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
