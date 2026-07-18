using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace CharMod.CharModCode.Cards;

public sealed class Alchemy : CharModCard
{
    private const string _goldKey = "Gold";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DynamicVar(_goldKey, 120m)
        };

    public Alchemy()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.LoseMaxHp(choiceContext, base.Owner.Creature, 3m, isFromCard: true);
        await PlayerCmd.GainGold(base.DynamicVars[_goldKey].BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars[_goldKey].UpgradeValueBy(30m);
    }
}