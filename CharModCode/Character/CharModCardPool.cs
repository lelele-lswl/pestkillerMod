using BaseLib.Abstracts;
using CharMod.CharModCode.Cards;
using CharMod.CharModCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace CharMod.CharModCode.Character;

public class CharModCardPool : CustomCardPoolModel
{
    public override string Title => CharMod.CharacterId;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    public override float H => 0.35f;
    public override float S => 1f;
    public override float V => 1f;

    public override Color DeckEntryCardColor => new("FFFFFF");

    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return new CardModel[]
        {
            // ===== 基础攻击 =====
            ModelDb.Card<CharModStrike>(),    // 打击
            ModelDb.Card<CharModDefend>(),    // 防御

            // ===== 普通攻击 =====
            ModelDb.Card<SeriousPunch>(),     // 认真一拳
            ModelDb.Card<BattleFrenzy>(),     // 愈战愈勇
            ModelDb.Card<Steal>(),            // 偷窃
            ModelDb.Card<Chainsaw>(),         // 电锯
            ModelDb.Card<BeeVenom>(),         // 蜂毒
            ModelDb.Card<Grindstone>(),       // 磨刀石
            ModelDb.Card<Printer>(),          // 打印机
            ModelDb.Card<PhysicsSword>(),     // 物理学圣剑
            ModelDb.Card<CuttingArt>(),       // 扦插之术
            ModelDb.Card<ExternalForce>(),    // 外力
            ModelDb.Card<Ignite>(),           // 激发

            // ===== 普通技能 =====
            ModelDb.Card<BowlbugShell>(),     // 盛碗虫外壳
            ModelDb.Card<Bandage>(),          // 包扎
            ModelDb.Card<AnthonyRapture>(),   // 安东尼狂喜
            ModelDb.Card<CreateSomething>(),  // 无中生有
            ModelDb.Card<BurnCards>(),       // 烧牌
            ModelDb.Card<InsectCarapace>(),   // 昆虫覆甲
            ModelDb.Card<UltimateForging>(),  // 究极锻造
            ModelDb.Card<ChronicPoison>(),    // 慢性毒药
            ModelDb.Card<CounterArmor>(),     // 反甲
            ModelDb.Card<PoisonShield>(),     // 毒盾
            ModelDb.Card<FreshMeat>(),        // 新鲜肉块
            ModelDb.Card<Endurance>(),        // 忍耐
            ModelDb.Card<HardStand>(),        // 硬撑
            ModelDb.Card<NiohShield>(),       // 仁王盾
            ModelDb.Card<Backpack>(),         // 背包
            ModelDb.Card<WaitForOpening>(),   // 等待破绽
            ModelDb.Card<AnthonyRage>(),      // 安东尼之怒（白卡）
            ModelDb.Card<BlackFlash>(),       // 黑闪
            ModelDb.Card<Slippery>(),         // 滑溜

            // ===== 罕见攻击 =====
            ModelDb.Card<TurnWasteIntoTreasure>(), // 变废为宝
            ModelDb.Card<BigBarrelInsecticide>(), // 大桶杀虫剂
            ModelDb.Card<PoisonGasBomb>(),     // 毒气弹
            ModelDb.Card<Anesthetic>(),       // 麻醉剂
            ModelDb.Card<Rhapsody>(),         // 狂想曲
            ModelDb.Card<EndlessLife>(),      // 生生不息
            ModelDb.Card<NumericalSlash>(),   // 数值斩
            ModelDb.Card<Pierce>(),           // 刺穿
            ModelDb.Card<Stapler>(),          // 钉书机
            ModelDb.Card<MeatKnife>(),        // 削肉刀
            ModelDb.Card<Electrocut>(),       // 电刑
            ModelDb.Card<InsertAnyone>(),     // 电棍
            ModelDb.Card<SlashStrike>(),      // 斩击

            // ===== 罕见技能 =====
            ModelDb.Card<Panacea>(),          // 万灵药
            ModelDb.Card<AcutePoison>(),      // 急性猛毒
            ModelDb.Card<Fry>(),              // 油炸
            ModelDb.Card<Braise>(),           // 红烧
            ModelDb.Card<StirFry>(),          // 爆炒
            ModelDb.Card<RebirthContract>(),  // 重生契约
            ModelDb.Card<Panic>(),            // 恐慌
            ModelDb.Card<PowerSource>(),      // 动力源
            ModelDb.Card<DeathRoll>(),        // 死亡之掷
            ModelDb.Card<FathersPocketWatch>(), // 父亲的怀表
            ModelDb.Card<DefyDeath>(),        // 视死如归
            ModelDb.Card<Printer>(),          // 打印机
            ModelDb.Card<BattleTalent>(),     // 战斗天赋
            ModelDb.Card<UltimateForging2>(), // 究极锻造2
            ModelDb.Card<GeneMutation>(),     // 基因突变
            ModelDb.Card<Antibiotic>(),       // 抗生素

            // ===== 罕见能力 =====
            ModelDb.Card<PoisonPoison>(),     // 毒毒！！
            ModelDb.Card<PoisonCoat>(),       // 涂毒plus
            ModelDb.Card<PoisonPoison2>(),    // 毒毒？？
            ModelDb.Card<AshCloak>(),         // 灰烬披风
            ModelDb.Card<Branch>(),           // 分支
            ModelDb.Card<Refine>(),           // 精炼
            ModelDb.Card<Mechanization>(),    // 机械化
            ModelDb.Card<Evolve>(),           // 进化
            ModelDb.Card<SleepOnThorns>(),    // 卧薪尝胆
            ModelDb.Card<BurnOut>(),          // 燃尽
            ModelDb.Card<AssemblyLine>(),     // 流水线
            ModelDb.Card<Tender>(),           // 嫩化

            // ===== 稀有攻击 =====
            ModelDb.Card<Gluttony>(),         // 饕餮
            ModelDb.Card<Violence>(),         // 暴力
            ModelDb.Card<ExcuseMe>(),         // 借过一下
            ModelDb.Card<NuclearFusion>(),    // 核聚变
            ModelDb.Card<UltimateInsecticide>(), // 究极杀虫剂
            ModelDb.Card<Flawless>(),         // 无懈可击
            ModelDb.Card<Execution>(),        // 斩杀
            ModelDb.Card<LethalRhythm>(),     // 致命节奏

            // ===== 稀有技能 =====
            ModelDb.Card<FlyingThunderGod>(), // 飞雷神
            ModelDb.Card<DreamShield>(),      // 梦之盾
            ModelDb.Card<Spark>(),            // 火花
            ModelDb.Card<FreeHand>(),         // 自由之手
            ModelDb.Card<MirrorFlowerWaterMoon>(), // 镜花水月
            ModelDb.Card<HereItComes>(),      // 要来喽
            ModelDb.Card<MysteryBox>(),       // 神秘礼盒
            ModelDb.Card<HundredThousandVolt>(), // 十万福特
            ModelDb.Card<FullCourseMeal>(),   // 满汉全席
            ModelDb.Card<Alchemy>(),          // 炼金术

            // ===== 稀有能力 =====
            ModelDb.Card<Barren>(),           // 片草不生
            ModelDb.Card<ElectricField>(),    // 电场
            ModelDb.Card<NuclearFission>(),   // 核裂变
            ModelDb.Card<Plunder3>(),         // 掠夺3
            ModelDb.Card<Melt>(),             // 熔化
            ModelDb.Card<Torture>(),          // 酷刑
            ModelDb.Card<Fool>(),             // 愚者

            // ===== 先古技能（DustyTome） =====
            ModelDb.Card<OverlordEgg>(),      // 霸王之卵
        };
    }
}