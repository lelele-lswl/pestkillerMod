using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace CharMod.CharModCode.Cards;

public sealed class LethalRhythm : CharModCard, ISlashCard
{
    private const string _increaseKey = "Increase";

    private int _currentDamage = 1;

    private int _increasedDamage;

    [SavedProperty]
    public int CurrentDamage
    {
        get => _currentDamage;
        set
        {
            AssertMutable();
            _currentDamage = value;
            base.DynamicVars.Damage.BaseValue = _currentDamage;
        }
    }

    [SavedProperty]
    public int IncreasedDamage
    {
        get => _increasedDamage;
        set
        {
            AssertMutable();
            _increasedDamage = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DamageVar(CurrentDamage, ValueProp.Move),
            new RepeatVar(3),
            new IntVar(_increaseKey, 1m)
        };

    public LethalRhythm()
        : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        int increaseAmount = base.DynamicVars[_increaseKey].IntValue;
        BuffFromPlay(increaseAmount);
        (base.DeckVersion as LethalRhythm)?.BuffFromPlay(increaseAmount);
    }

    protected override PileType GetResultPileTypeForCardPlay()
    {
        PileType pileType = base.GetResultPileTypeForCardPlay();
        if (pileType == PileType.Discard)
        {
            return PileType.Hand;
        }
        return pileType;
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars[_increaseKey].UpgradeValueBy(1m);
    }

    protected override void AfterDowngraded()
    {
        UpdateDamage();
    }

    private void BuffFromPlay(int extraDamage)
    {
        IncreasedDamage += extraDamage;
        UpdateDamage();
    }

    private void UpdateDamage()
    {
        CurrentDamage = 1 + IncreasedDamage;
    }
}