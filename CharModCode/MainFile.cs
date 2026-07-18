using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using System.Reflection;
using CharMod.CharModCode.Relics;
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
            Logger.Error(e.StackTrace);
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
            __result = "res://scenes/rest_site/characters/ironclad_rest_site.tscn";
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