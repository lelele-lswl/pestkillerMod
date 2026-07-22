using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CharMod.CharModCode.Cards;

public sealed class Grindstone : CharModCard
{
    private const string _upgradeCountKey = "UpgradeCount";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DamageVar(5m, ValueProp.Move),
            new DynamicVar(_upgradeCountKey, 2m)
        };

    public Grindstone()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target!)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        int upgradeCount = (int)base.DynamicVars[_upgradeCountKey].BaseValue;
        var selected = await CardSelectCmd.FromCombatPile(
            prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, upgradeCount, upgradeCount),
            context: choiceContext,
            pile: PileType.Draw.GetPile(base.Owner),
            player: base.Owner,
            filter: c => c.IsUpgradable);

        foreach (CardModel card in selected)
        {
            CardCmd.Upgrade(card);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars[_upgradeCountKey].UpgradeValueBy(1m);
    }
}