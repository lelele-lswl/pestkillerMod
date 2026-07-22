using Godot;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace CharMod.CharModCode.Scenes;

/// <summary>
/// 商店场景角色形象（静态，无动画）。
/// 继承 NMerchantCharacter（游戏通过 Instantiate&lt;NMerchantCharacter&gt; 加载）。
///
/// 注意：不调用 base._Ready()，因为基类假设第一个子节点是 SpineSprite
/// 并尝试 new MegaSprite(GetChild(0))，会崩溃。
/// </summary>
public partial class CharModMerchant : NMerchantCharacter
{
    public override void _Ready()
    {
        // 不调用 base._Ready() — 基类会 new MegaSprite(GetChild(0)) 在非 Spine 节点上崩溃
        // 不做任何动画，保持静态显示
    }
}