using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace CharMod.CharModCode.Cards;

public sealed class UltimateInsecticide : CharModCard
{
    private const string _hitsKey = "Hits";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DamageVar(1m, ValueProp.Move),
            new DynamicVar(_hitsKey, 12m)
        };

    public UltimateInsecticide()
        : base(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int hitCount = (int)base.DynamicVars[_hitsKey].BaseValue;
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .WithHitCount(hitCount)
            .FromCard(this)
            .TargetingAllOpponents(base.CombatState!)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars[_hitsKey].UpgradeValueBy(4m);
    }
}
