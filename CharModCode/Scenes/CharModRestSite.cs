using System.Reflection;
using Godot;
using MegaCrit.Sts2.Core.Nodes.RestSite;

namespace CharMod.CharModCode.Scenes;

/// <summary>
/// 篝火场景角色形象（静态，无动画）。
/// 继承 NRestSiteCharacter（游戏通过 Instantiate&lt;NRestSiteCharacter&gt;() 加载）。
///
/// 注意：不调用 base._Ready()，因为基类需要 SpineSprite 和 NSelectionReticle 等外部资源。
/// 通过反射手动完成最小必要初始化，确保 Hitbox 等关键属性不为 null。
/// </summary>
public partial class CharModRestSite : NRestSiteCharacter
{
    private static readonly System.Type BaseType = typeof(NRestSiteCharacter);
    private static readonly FieldInfo? ControlRootField = BaseType.GetField("_controlRoot", BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly FieldInfo? LeftAnchorField = BaseType.GetField("_leftThoughtAnchor", BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly FieldInfo? RightAnchorField = BaseType.GetField("_rightThoughtAnchor", BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly PropertyInfo? HitboxProp = BaseType.GetProperty("Hitbox", BindingFlags.Public | BindingFlags.Instance);

    public override void _Ready()
    {
        // 不调用 base._Ready() — 基类需要 SpineSprite 和 NSelectionReticle（mod 里不存在）
        // 手动完成最小必要初始化，确保 Hitbox 不为 null（否则输入事件会 NRE 刷日志导致卡顿）

        var controlRoot = GetNode<Control>("ControlRoot");
        var hitbox = GetNode<Control>("%Hitbox");
        var leftAnchor = GetNodeOrNull<Control>("%ThoughtBubbleLeft");
        var rightAnchor = GetNodeOrNull<Control>("%ThoughtBubbleRight");

        ControlRootField?.SetValue(this, controlRoot);
        LeftAnchorField?.SetValue(this, leftAnchor);
        RightAnchorField?.SetValue(this, rightAnchor);
        HitboxProp?.SetValue(this, hitbox);

        // 连接 Hitbox 信号（不连到基类的 OnFocus/OnUnfocus，因为它们依赖 _selectionReticle）
        hitbox.Connect(Control.SignalName.FocusEntered, Callable.From(() => { }));
        hitbox.Connect(Control.SignalName.FocusExited, Callable.From(() => { }));
        hitbox.Connect(Control.SignalName.MouseEntered, Callable.From(() => { }));
        hitbox.Connect(Control.SignalName.MouseExited, Callable.From(() => { }));
    }
}