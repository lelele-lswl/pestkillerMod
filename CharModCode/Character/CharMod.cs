// ============================================================
// 角色模型 - CharMod
// 继承 PlaceholderCharacterModel：未被 override 的属性
// 会自动使用游戏自带的"占位素材"（即战士/铁甲战士的图）
// ============================================================

// 引入依赖命名空间
using BaseLib.Abstracts;          // BaseLib 角色基类 PlaceholderCharacterModel
using BaseLib.Utils.NodeFactories; // BaseLib 提供的 UI 节点工厂
using CharMod.CharModCode.Extensions; // 路径工具（CharacterUiPath、ImagePath 等）
using Godot;                      // Godot 基础类型（Color、Control、Texture2D 等）
using MegaCrit.Sts2.Core.Entities.Characters; // CharacterGender 等
using MegaCrit.Sts2.Core.Models;  // ModelDb（游戏模型注册表）
using MegaCrit.Sts2.Core.Models.Cards;   // Squash、Exterminate 等
using CharMod.CharModCode.Cards;         // CharModStrike、CharModDefend 等
using CharMod.CharModCode.Relics; // ModifiedHive 等自定义遗物

namespace CharMod.CharModCode.Character;

/// <summary>
/// 自定义角色类。继承 PlaceholderCharacterModel，
/// 未被 override 的属性会使用游戏默认素材（即战士的图）。
/// </summary>
public class CharMod : PlaceholderCharacterModel
{
    // ===== 基本信息 =====

    /// <summary>角色内部 ID（不显示给玩家，用于注册表和资源查找）</summary>
    public const string CharacterId = "pestkiller";

    /// <summary>角色主颜色（影响角色名、卡牌边框等元素的着色）</summary>
    public static readonly Color Color = new("FFD700"); // 黄金色


    /// <summary>角色名在 UI 中显示的颜色</summary>
    public override Color NameColor => Color;

    /// <summary>角色性别（Male/Female/Neutral，影响某些对话/动画）</summary>
    public override CharacterGender Gender => CharacterGender.Neutral;

    /// <summary>解锁前置角色（null 表示没有前置条件）</summary>
    protected override CharacterModel? UnlocksAfterRunAs => null;

    /// <summary>初始最大生命值</summary>
    public override int StartingHp => 70;

    /// <summary>初始金币</summary>
    public override int StartingGold => 149;

    /// <summary>角色播放攻击动作后，到真正出手/命中前的延迟时间（关键：打牌后等待动画完成）</summary>
    public override float AttackAnimDelay => 0.15f;

    /// <summary>角色播放施法动作后，到效果实际触发前的延迟时间（关键：打牌后等待动画完成）</summary>
    public override float CastAnimDelay => 0.25f;


    // ===== 初始牌组 =====
    /// <summary>新游戏开始时拥有的初始牌组（10 张：5 攻击 + 5 防御）</summary>
    public override IEnumerable<CardModel> StartingDeck => [
   // 打击
        ModelDb.Card<CharModStrike>(),
        ModelDb.Card<CharModStrike>(),
        ModelDb.Card<CharModStrike>(),
        ModelDb.Card<CharModStrike>(),
        ModelDb.Card<CharModStrike>(),
   // 防御
        ModelDb.Card<CharModDefend>(),
        ModelDb.Card<CharModDefend>(),
        ModelDb.Card<CharModDefend>(),
        ModelDb.Card<CharModDefend>(),
        ModelDb.Card<CharModDefend>(),
        
        ModelDb.Card<Squash>(),          // 压扁：事件卡
        ModelDb.Card<Exterminate>()      // 杀灭：事件卡（对所有敌人造成伤害多次）
    ];

    // ===== 初始遗物 =====
    /// <summary>新游戏开始时自动获得的遗物</summary>
    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<ModifiedHive>()    // 改造蜂巢：每受到一段攻击，抽 1 张牌
    ];


    // ===== 池注册 =====
    // 游戏从以下池子里抽取战斗奖励、商店商品、精英奖励等
    /// <summary>卡牌池（决定该角色能抽到哪些卡）</summary>
    public override CardPoolModel CardPool => ModelDb.CardPool<CharModCardPool>();
    /// <summary>遗物池（决定该角色能抽到哪些遗物）</summary>
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<CharModRelicPool>();
    /// <summary>药水池（决定该角色能抽到哪些药水）</summary>
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<CharModPotionPool>();


    // ===== 图片资源路径 =====
    // 所有路径最终解析为：res://pestkiller/images/charui/[文件名]
    // 需要把对应的 PNG 图片放在 CharMod/images/charui/ 目录下

    /* ⬇️ 下面 4 个路径已在 BaseLib 中定义，直接覆盖就能替换角色图片 */

    /// <summary>角色自定义小图标（被游戏 UI 直接使用的 Control 节点）</summary>
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }
    // public override string CharacterName => "害虫杀手";  // 这个属性名不正确

    // ===== 角色选择背景 =====
    /// <summary>角色选择界面的背景场景路径（调试用：确认 Id.Entry 是什么值）</summary>
    // BaseLib 可能提供了可以 override 的属性，先尝试直接 override 原生的背景路径
    // 如果这个属性可以 override，就能直接指向我们自己的背景场景
    // public override string CharacterSelectBg => SceneHelper.GetScenePath("screens/char_select/char_select_bg_charmod"); // 原生非 virtual，无法直接 override

    /// <summary>小图标图片路径（角色名字旁的头像徽章）</summary>
    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();

    /// <summary>角色选择界面的小头像图片路径</summary>
    public override string CustomCharacterSelectIconPath => "char_select_char_name.jpg".CharacterUiPath();

    /// <summary>角色锁定状态的小头像图片路径（灰色、带锁图标版本）</summary>
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();

    /// <summary>地图上角色移动标记的图片路径</summary>
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();

    /// <summary>角色攻击建筑师时使用的攻击特效列表</summary>
    public override List<string> GetArchitectAttackVfx()
    {
        return new List<string>();
    }
}