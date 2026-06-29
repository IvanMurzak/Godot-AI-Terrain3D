# Terrain3D Tools

AI MCP tools for the Godot Terrain3D addon.

A **source-only** MCP tool extension for [Godot-MCP / AI Game Developer](https://github.com/IvanMurzak/Godot-MCP)
that adds AI tools for the community [**Terrain3D**](https://github.com/TokisanGames/Terrain3D) addon
(large-scale, editable clipmap terrain). The package ships C# source (no compiled DLL, no bundled Godot)
that compiles inside your Godot project against your own GodotSharp, so it never locks you to a Godot
version. Terrain3D is a precompiled C++ GDExtension, so this extension drives it **by string name at
runtime** (through Godot's `ClassDB`) and never depends on the addon at compile time — every tool is
**presence-gated** (a missing addon returns a clean `installed: false` result).

## Required prerequisite — Terrain3D (install it yourself)

This extension **does NOT include** the Terrain3D addon. Install it separately into your Godot project
from the **Godot Asset Library** (search "Terrain3D") or **https://github.com/TokisanGames/Terrain3D**
(tested against **v1.0.2-stable** — it ships precompiled binaries, so no C++ build is needed). Unzip it
into `addons/terrain_3d/`, **restart Godot**, and enable it under **Project Settings → Plugins**.
Terrain3D is © its authors under the **MIT License**; this extension is **not affiliated with or endorsed
by** it.

## Install

Requires the core [`godot_mcp`](https://github.com/IvanMurzak/Godot-MCP) addon in your Godot C# project.

```bash
# via the godot-cli (resolves from the shared catalog, edits your .csproj, rebuilds)
godot-cli install-extension com.IvanMurzak.Godot.MCP.Terrain3D

# …or add the reference manually and rebuild:
#   <PackageReference Include="com.IvanMurzak.Godot.MCP.Terrain3D" Version="0.1.0" />
```

…or pick it from the **Extensions** dock inside the Godot editor.

After a rebuild, the extension's `[AiToolType]` tool families are auto-discovered — no registry edit.

## Tools

Every editor tool is **presence-gated**: when the Terrain3D addon is not installed it returns a
structured `installed: false` result with an install hint instead of crashing.

| Tool | Kind | Description |
| --- | --- | --- |
| `terrain3d-defaults` | pure-managed | Recommended region size / clipmap LOD / data-directory starter config (`small`/`medium`/`large` preset). No addon needed. |
| `terrain3d-create` | editor | Create a `Terrain3D` node (optional name / parent), optionally seed its region size + data directory. |
| `terrain3d-set-data-directory` | editor | Set the terrain's `res://` data directory (Terrain3D persists region data on disk — a writable dir is required). |
| `terrain3d-set-region-size` | editor | Set the terrain region size (snapped to a valid Terrain3D size: 64/128/256/512/1024/2048). |
| `terrain3d-set-material` | editor | Assign (or create) a `Terrain3DMaterial` on the terrain. |
| `terrain3d-get` | editor | Read a `Terrain3D` node's scalar config (region size, data directory, version, region count) — read-only. |

License: Apache-2.0.
