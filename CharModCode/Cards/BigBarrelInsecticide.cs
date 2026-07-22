using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace CharMod.CharModCode.Cards;

public sealed class BigBarrelInsecticide : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DamageVar(2m, ValueProp.Move)
        };

    public BigBarrelInsecticide()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (Creature enemy in base.CombatState!.Enemies.ToList())
        {
            if (enemy.IsAlive)
            {
                await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                    .WithHitCount(6)
                    .FromCard(this)
                    .Targeting(enemy)
                    .WithHitFx("vfx/vfx_attack_blunt")
                    .Execute(choiceContext);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
