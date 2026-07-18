using MegaCrit.Sts2.Core.Nodes.Combat;

namespace CharMod.CharModCode.Scenes;

/// <summary>
/// 战斗角色形象场景的根节点脚本。
/// 必须继承 NCreatureVisuals，游戏会把场景实例化成这个类型。
/// 不添加/重写任何成员，只是给 Godot 场景用的"占位脚本"。
/// </summary>
public partial class CustomCreatureVisuals : NCreatureVisuals { }