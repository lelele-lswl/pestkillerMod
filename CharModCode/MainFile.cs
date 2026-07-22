using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using System.Collections.Generic;
using System.Reflection;
using BindingFlags = System.Reflection.BindingFlags;
using CharMod.CharModCode.Relics;
using CharMod.CharModCode.Scenes;
using CharacterCharMod = CharMod.CharModCode.Character.CharMod;

namespace CharMod.CharModCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "pestkiller"; //Used for resource filepath
    public const string ResPath = "res://pestkiller";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Logger.Info("=== pestkiller initializing ===");

        // 让 Godot 自动检索当前程序集中的自定义脚本
        // （教程第 594 行：没有这行，自定义 C# 脚本绑定的 .tscn 场景无法实例化）
        Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());

        // Force instantiation of character model to trigger registration
        try
        {
            var character = new CharacterCharMod();
            Logger.Info($"Character model created: Id.Entry={character.Id.Entry}, CharacterSelectBg={character.CharacterSelectBg}");
        }
        catch (System.Exception e)
        {
            Logger.Error($"Failed to create character model: {e.Message}");
            Logger.Error(e.StackTrace ?? string.Empty);
        }

        Harmony harmony = new(ModId);

        harmony.PatchAll();

        Logger.Info("=== pestkiller initialized successfully ===");
    }
}

// ============================================================
// Harmony patches: 替换角色场景路径
// CharacterModel 的 VisualsPath / EnergyCounterPath 等属性不是 virtual，
// 无法直接 override，用 Harmony patch 拦截 getter。
// ============================================================

// ==== 1. 角色选择界面背景 ====
[HarmonyPatch(typeof(CharacterModel), "CharacterSelectBg", MethodType.Getter)]
public static class CharacterSelectBgPatch
{
    static void Postfix(CharacterModel __instance, ref string __result)
    {
        if (__instance is CharacterCharMod)
        {
            __result = "res://scenes/screens/char_select/char_select_bg_charmod.tscn";
        }
    }
}

// ==== 2. 战斗中角色形象（核心：creature_visuals/charmod.tscn） ====
[HarmonyPatch(typeof(CharacterModel), "VisualsPath", MethodType.Getter)]
public static class VisualsPathPatch
{
    static void Postfix(CharacterModel __instance, ref string __result)
    {
        if (__instance is CharacterCharMod)
        {
            __result = "res://scenes/creature_visuals/charmod.tscn";
            MainFile.Logger.Info($"VisualsPath patched for CharMod -> {__result}");
        }
    }
}

// ==== 3. 能量计数器场景（战斗左上角的能量数字）====
[HarmonyPatch(typeof(CharacterModel), "EnergyCounterPath", MethodType.Getter)]
public static class EnergyCounterPathPatch
{
    static void Postfix(CharacterModel __instance, ref string __result)
    {
        if (__instance is CharacterCharMod)
        {
            __result = "res://scenes/combat/energy_counters/ironclad_energy_counter.tscn";
        }
    }
}

// ==== 4. 商店场景 ====
[HarmonyPatch(typeof(CharacterModel), "MerchantAnimPath", MethodType.Getter)]
public static class MerchantAnimPathPatch
{
    static void Postfix(CharacterModel __instance, ref string __result)
    {
        if (__instance is CharacterCharMod)
        {
            __result = "res://scenes/merchant/characters/charmod_merchant.tscn";
        }
    }
}

// ==== 5. 休息场景 ====
[HarmonyPatch(typeof(CharacterModel), "RestSiteAnimPath", MethodType.Getter)]
public static class RestSiteAnimPathPatch
{
    static void Postfix(CharacterModel __instance, ref string __result)
    {
        if (__instance is CharacterCharMod)
        {
            __result = "res://scenes/rest_site/characters/charmod_rest_site.tscn";
        }
    }
}

// ==== 6. 卡牌拖尾特效场景 ====
[HarmonyPatch(typeof(CharacterModel), "TrailPath", MethodType.Getter)]
public static class TrailPathPatch
{
    static void Postfix(CharacterModel __instance, ref string __result)
    {
        if (__instance is CharacterCharMod)
        {
            __result = "res://scenes/vfx/card_trail_ironclad.tscn";
        }
    }
}

// ==== 7. 欧洛巴斯之触 - 注册 ModifiedHive → HolyHive 映射 ====
[HarmonyPatch(typeof(TouchOfOrobas), "GetUpgradedStarterRelic")]
public static class TouchOfOrobasRefinementPatch
{
    static bool Prefix(RelicModel starterRelic, ref RelicModel __result)
    {
        if (starterRelic is ModifiedHive)
        {
            __result = ModelDb.Relic<HolyHive>();
            return false;
        }
        return true;
    }
}

// ==== 8. 动画触发器拦截（方案A：Tween 动画）====
// 当 NCreature.SetAnimationTrigger 被调用时，如果 Visuals 是 CustomCreatureVisuals，
// 把触发器转发过去，用 Godot Tween 代替 Spine 骨骼动画。
[HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
public static class AnimationTriggerPatch
{
    static void Postfix(NCreature __instance, string trigger)
    {
        if (__instance.Visuals is CustomCreatureVisuals customVisuals)
        {
            customVisuals.PlayAnimation(trigger);
        }
    }
}

// ==== 9. 死亡动画触发 + 时长修正 ====
// StartDeathAnim 中 SetAnimationTrigger("Dead") 包在 if (_spineAnimator != null) 里，
// 没有 Spine 时永远不会触发死亡动画。补丁直接调用 PlayAnimation("Dead")。
[HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
public static class StartDeathAnimPatch
{
    static void Postfix(NCreature __instance, ref float __result)
    {
        if (__instance.Visuals is CustomCreatureVisuals customVisuals)
        {
            customVisuals.PlayAnimation("Dead");
            __result = Mathf.Max(__result, CustomCreatureVisuals.DeathAnimLength);
        }
    }
}

// ==== 10. 修复 NInputManager._UnhandledInput 控制器摇杆错误刷屏 ====
// 问题：来自 ControllerConfig 的默认映射中包含 controller_r_stick_up/down/left/right 四个动作，
// 但它们在 InputMap 中可能不存在（游戏 bug），导致每个输入事件都产生 Godot 引擎错误。
// 这些错误刷屏到控制台，每次滚轮事件产生 6-8 条错误日志，严重卡顿。
// 修复：Prefix 完全接管原方法，跳过不在 InputMap 中的动作。
[HarmonyPatch(typeof(NInputManager), "_UnhandledInput")]
public static class FixMissingInputMapActionsPatch
{
    private static readonly FieldInfo? ControllerInputMapField =
        typeof(NInputManager).GetField("_controllerInputMap",
            BindingFlags.NonPublic | BindingFlags.Instance);

    private static bool Prefix(NInputManager __instance, InputEvent inputEvent)
    {
        // 原方法的前置检查逻辑
        if ((NGame.Instance?.Transition.InTransition ?? true) || !NGame.IsGameFocusedWindow())
        {
            return false;
        }

        var controllerInputMap = ControllerInputMapField?.GetValue(__instance) as Dictionary<StringName, StringName>;
        if (controllerInputMap == null)
        {
            return false;
        }

        foreach (KeyValuePair<StringName, StringName> item in controllerInputMap)
        {
            // 关键修复：跳过 InputMap 中不存在的动作，避免 Godot 引擎错误
            if (!InputMap.HasAction(item.Value))
            {
                continue;
            }

            if (inputEvent.IsActionPressed(item.Value))
            {
                InputEventAction inputEventAction = new InputEventAction
                {
                    Action = item.Key,
                    Pressed = true
                };
                Input.ParseInputEvent(inputEventAction);
            }
            else if (inputEvent.IsActionReleased(item.Value))
            {
                InputEventAction inputEventAction2 = new InputEventAction
                {
                    Action = item.Key,
                    Pressed = false
                };
                Input.ParseInputEvent(inputEventAction2);
            }
        }
        return false; // 跳过原方法
    }
}