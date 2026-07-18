using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace CharMod.CharModCode.Cards;

public sealed class DefyDeath : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DynamicVar("MaxHpLoss", 1m),
            new DynamicVar("Energy", 3m)
        };

    public DefyDeath()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.LoseMaxHp(choiceContext, base.Owner.Creature, base.DynamicVars["MaxHpLoss"].BaseValue, true);
        await PlayerCmd.GainEnergy(base.DynamicVars["Energy"].BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Energy"].UpgradeValueBy(1m);
    }
}
