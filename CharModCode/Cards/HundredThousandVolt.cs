using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CharMod.CharModCode.Powers;

namespace CharMod.CharModCode.Cards;

public sealed class HundredThousandVolt : CharModCard
{
    private const string _triggerCountKey = "TriggerCount";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DynamicVar(_triggerCountKey, 2m)
        };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromPower<PathwayPower>() };

    public HundredThousandVolt()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int triggerCount = (int)base.DynamicVars[_triggerCountKey].BaseValue;

        for (int i = 0; i < triggerCount; i++)
        {
            foreach (var enemy in base.CombatState!.Enemies.ToList())
            {
                PathwayPower? pathway = enemy.GetPower<PathwayPower>();
                if (pathway != null)
                {
                    await pathway.Trigger(choiceContext, enemy, base.Owner.Creature);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars[_triggerCountKey].UpgradeValueBy(1m);
    }
}