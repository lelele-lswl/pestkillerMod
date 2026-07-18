# CharacterModTemplate 工作经验

## 导出 Mod 流程

### 一键导出命令

```bash
dotnet publish pestkiller.csproj -c Release
```

这个命令会自动执行三步：
1. **编译 C# 代码** → `pestkiller.dll`
2. **Godot --headless --import** → 导入资源（纹理、场景等）
3. **Godot --headless --export-pack** → 生成 `pestkiller.pck`

输出到：`D:/Steam121/steamapps/common/Slay the Spire 2/mods/pestkiller/`

### 发布前清理（可选，但推荐）

当出现奇怪问题时执行：
```bash
dotnet clean pestkiller.csproj
rm -rf .godot/mono/temp
rm -rf data_CharMod_windows_x86_64
```

### 关键文件

| 文件 | 路径 | 用途 |
|------|------|------|
| 导出配置 | `export_presets.cfg` | Godot 导出预设，指定导出路径和过滤规则 |
| 构建配置 | `Directory.Build.props` | Godot 路径、StS2 路径 |
| 路径发现 | `Sts2PathDiscovery.props` | 自动发现 Steam/StS2 路径 |
| 项目文件 | `pestkiller.csproj` | 定义 Publish → GodotImport → GodotPublish 目标链 |
| Mod 清单 | `pestkiller.json` | Mod 元数据（id, name, version, dependencies） |

### 遇到的坑

1. **导出失败 / 资源不更新**：原因是 `.godot/mono/temp` 缓存残留 → 清理后重试
2. **不能直接用 `dotnet build` 导出**：`dotnet build` 只编译不触发 Godot 导出步骤，必须用 `dotnet publish`
3. **导出 = build + publish**：publish 会触发 `GodotImport`（headless import）和 `GodotPublish`（export-pack）两个 MSBuild Target
4. **Godot 路径必须存在**：`Directory.Build.props` 中的 `GodotPath` 指向 Godot 4.5.1 mono 版
