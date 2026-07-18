using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace CharMod.CharModCode.Relics;

public sealed class HolyHive : CharModRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override string PackedIconPath => "res://pestkiller/images/relics/modified_hive.png";

    protected override string PackedIconOutlinePath => "res://pestkiller/images/relics/relic_outline.png";

    protected override string BigIconPath => "res://pestkiller/images/relics/big/modified_hive.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new IntVar("Draw", 2m)
    };

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner.Creature && result.UnblockedDamage > 0)
        {
            TaskHelper.RunSafely(DoActivate(choiceContext));
        }
        return Task.CompletedTask;
    }

    private async Task DoActivate(PlayerChoiceContext choiceContext)
    {
        Flash();
        await CardPileCmd.Draw(choiceContext, 2m, base.Owner);
    }
}