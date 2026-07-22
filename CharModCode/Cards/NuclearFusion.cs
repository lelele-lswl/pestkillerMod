using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace CharMod.CharModCode.Cards;

public sealed class NuclearFusion : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DamageVar(80m, ValueProp.Move)
        };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromCard<Debris>() };

    public NuclearFusion()
        : base(4, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int cardsToDraw = CardPile.MaxCardsInHand - PileType.Hand.GetPile(base.Owner).Cards.Count;
        if (cardsToDraw > 0)
        {
            await CardPileCmd.Draw(choiceContext, cardsToDraw, base.Owner);
        }

        var handCards = PileType.Hand.GetPile(base.Owner).Cards.ToList();
        foreach (CardModel card in handCards)
        {
            if (card.IsTransformable)
            {
                await CardCmd.TransformTo<Debris>(card);
            }
        }

        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target!)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
