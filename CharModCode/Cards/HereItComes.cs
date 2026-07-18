using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CharMod.CharModCode.Powers;

namespace CharMod.CharModCode.Cards;

public sealed class HereItComes : CharModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new PowerVar<FocusPower>(3m)
        };

    public HereItComes()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        FocusPower? focus = base.Owner.Creature.GetPower<FocusPower>();
        if (focus != null && focus.Amount > 0)
        {
            await PowerCmd.Apply<FocusPower>(choiceContext, base.Owner.Creature, focus.Amount, base.Owner.Creature, this);
        }
        else
        {
            await PowerCmd.Apply<FocusPower>(choiceContext, base.Owner.Creature, base.DynamicVars["FocusPower"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["FocusPower"].UpgradeValueBy(2m);
    }
}