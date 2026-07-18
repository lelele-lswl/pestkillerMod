# 害虫杀手 (Pestkiller) — Slay the Spire 2 角色 Mod

《杀戮尖塔2》的自定义角色 Mod，基于 Godot 4.5.1 + C# (.NET 9) + BaseLib 开发。

## 环境要求

| 工具 | 版本 | 说明 |
|------|------|------|
| Godot | **4.5.1 mono** | 必须 mono 版，用于导出 .pck |
| .NET SDK | 9.0 | 编译 C# 代码 |
| STS2 | 0.107.1+ | 游戏本体 |
| BaseLib | 3.3.5+ | Mod 框架依赖 |

## 快速开始

### 1. 克隆并配置路径

```bash
git clone <repo-url>
cd CharacterModTemplate
```

编辑 [Directory.Build.props](Directory.Build.props)，确保两个路径正确：

```xml
<GodotPath>D:/path/to/Godot_v4.5.1-stable_mono_win64/Godot_v4.5.1-stable_mono_win64.exe</GodotPath>
<Sts2Path>D:/path/to/Steam/steamapps/common/Slay the Spire 2</Sts2Path>
```

### 2. 编译（仅检查代码）

```bash
dotnet build pestkiller.csproj
```

### 3. 导出到游戏 Mods 目录

```bash
dotnet publish pestkiller.csproj -c Release
```

这个命令会自动完成三步：编译 C# → Godot 导入资源 → 导出 .pck，最终输出到：
```
<Slay the Spire 2>/mods/pestkiller/
  ├── pestkiller.dll
  ├── pestkiller.pck
  ├── pestkiller.pdb
  └── pestkiller.json
```

> **注意**：必须用 `dotnet publish` 而非 `dotnet build`，只有 publish 才会触发 Godot 的资源导入和 .pck 导出步骤。

### 4. 导出失败时的清理

```bash
dotnet clean pestkiller.csproj
rm -rf .godot/mono/temp
rm -rf data_pestkiller_windows_x86_64
# 然后重新 publish
```

## 项目结构

```
├── CharModCode/              # C# 代码
│   ├── MainFile.cs           # Mod 入口，Harmony 补丁
│   ├── Cards/                # 卡牌逻辑 (101 张)
│   ├── Powers/               # 能力逻辑 (22 个)
│   ├── Relics/               # 遗物逻辑 (3 个)
│   ├── Potions/              # 药水逻辑 (1 个)
│   ├── Character/            # 角色/卡池/遗物池/药水池定义
│   ├── Extensions/           # 工具扩展方法
│   └── Scenes/               # 自定义场景脚本绑定
├── pestkiller/               # Godot 资源包内容
│   ├── localization/
│   │   ├── zhs/              # 简体中文本地化
│   │   └── eng/              # 英文本地化
│   ├── images/
│   │   ├── card_portraits/   # 卡牌原画 (98+98 大小图)
│   │   ├── powers/           # 能力图标 (22+22)
│   │   ├── relics/           # 遗物图标
│   │   └── charui/           # 角色 UI 素材
│   └── mod_image.png         # Mod 封面图
├── scenes/                   # Godot 场景文件
│   ├── creature_visuals/     # 角色战斗形象
│   ├── combat/               # 能量计数器
│   ├── merchant/             # 商店动画
│   ├── rest_site/            # 休息场景
│   ├── screens/              # 角色选择界面背景
│   └── vfx/                  # 卡牌拖尾特效
├── pestkiller.csproj         # .NET 项目文件
├── pestkiller.json           # Mod 清单
├── project.godot             # Godot 项目配置
├── Directory.Build.props     # 构建路径配置（需自己创建）
├── Sts2PathDiscovery.props   # Steam/STS2 路径自动发现
└── export_presets.cfg        # Godot 导出预设
```

## 角色信息

| 属性 | 值 |
|------|-----|
| ID | `pestkiller` |
| 名称 | 害虫杀手 |
| 作者 | Lelelelswl |
| 版本 | v0.0.1 |
| 最低游戏版本 | 0.107.1 |

## 内容规模

| 类型 | 数量 |
|------|------|
| 卡牌 | 101 张 |
| 能力 | 22 个 |
| 遗物 | 3 个 |
| 药水 | 1 个 |

## 命名规范

### 卡牌 ID
- 格式：`CHARMOD-<SNAKE_CASE>`
- 类名：`PascalCase.cs`
- 示例：`CHARMOD-LETHAL_RHYTHM` → `LethalRhythm.cs`

### 能力 ID
- 格式：`CHARMOD-<SNAKE_CASE>_POWER`
- 示例：`CHARMOD-POISON_POISON_POWER` → `PoisonPoisonPower.cs`

### 图片命名
引擎通过 `Id.RemovePrefix().ToLowerInvariant()` 自动匹配：
- 卡牌：`<snake_case>.png` → `pestkiller/images/card_portraits/`
- 能力：`<snake_case>.png` → `pestkiller/images/powers/`
- 大图统一放在对应目录的 `big/` 子目录下

## 本地化

同时维护简体中文 (`zhs/`) 和英文 (`eng/`) 两个目录：

| 文件 | 内容 |
|------|------|
| `cards.json` | 卡牌名称和描述 |
| `powers.json` | 能力名称和描述 |
| `relics.json` | 遗物名称和描述 |
| `characters.json` | 角色文本和代词 |

动态变量语法：`{Damage:diff()}` / `{Block:diff()}` / `{Energy:diff()}` 等。

## Mod 清单说明

[pestkiller.json](pestkiller.json) 是 Mod 的身份证：

```json
{
  "id": "pestkiller",
  "name": "害虫杀手",
  "author": "Lelelelswl",
  "version": "v0.0.1",
  "min_game_version": "0.107.1",
  "has_pck": true,
  "has_dll": true,
  "dependencies": [
    {"id": "BaseLib", "min_version": "3.3.5"}
  ]
}
```

## 相关链接

- [ModTemplate-StS2 Wiki](https://github.com/Alchyr/ModTemplate-StS2/wiki)
- [BaseLib-STS2](https://github.com/Alchyr/BaseLib-STS2)
- [Godot-Spine 插件](https://github.com/sanja-sa/godot-spine)（如需导入 Spine 动画）
