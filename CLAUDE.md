# CLAUDE.md — Godot-AI-Terrain3D

A **Godot-MCP extension** that wraps the third-party [**Terrain3D**](https://github.com/TokisanGames/Terrain3D)
addon (large-scale, editable clipmap terrain), shipped as a **source-only NuGet package**
(`com.IvanMurzak.Godot.MCP.Terrain3D`) that compiles inside a consumer's Godot project against the
consumer's own GodotSharp. Created from
[`Godot-AI-Tools-Template`](https://github.com/IvanMurzak/Godot-AI-Tools-Template). The packaging recipe is
the load-bearing detail — read `docs/source-only-nuget-recipe.md`.

This is an **addon-dependent ("Class B") GDExtension** extension: Terrain3D is a **precompiled C++
GDExtension**, so its classes live in Godot's `ClassDB` (NOT in GodotSharp, and NOT in the GDScript
global-class list), and the package **must not depend on the addon**. So the tools reference Terrain3D's
classes **only by string name**, resolved + instantiated at runtime via `ClassDB.ClassExists` /
`ClassDB.Instantiate`, and **presence-gate** every editor tool so a missing addon returns a clean
structured `installed: false` result instead of crashing. The addon is **never** vendored, submoduled, or
downloaded by this repo — installing Terrain3D is the consumer's own responsibility (CI downloads a pinned
copy with its linux-x64 binary only to exercise the e2e leg).

## Layout

- `src/Godot-AI-Terrain3D/` — the source-only package (`Godot.NET.Sdk`).
  - `Runtime/Tools/Tool_Terrain3D.cs` — the `[AiToolType]` family (one partial class).
  - `Runtime/Tools/Tool_Terrain3D.Ids.cs` — all tool-id consts (pure-managed; pinned by tests).
  - `Runtime/Tools/Tool_Terrain3D.Defaults.cs` — `terrain3d-defaults` (pure-managed tool).
  - `Runtime/Interop/AddonInterop.cs` — dynamic name-resolution helper (the `ClassDB` path:
    `ClassDB.ClassExists` → `ClassDB.Instantiate`); the `Node`-constructing calls stay in `#if TOOLS`
    editor tools (constructing a `Node` P/Invokes and crashes a no-Godot xUnit host).
  - `Runtime/Interop/AddonGate.cs` — the shared `AddonGateResult` shape + `NotInstalled(...)` factory
    (pure-managed, unit-tested).
  - `Runtime/Terrain3D/Terrain3DEnums.cs` — the addon's class/member **snake_case** name constants +
    enum-int values (no compile-time enum types exist, so the constants ARE the contract — unit-tested).
  - `Editor/Tools/Tool_Terrain3D.{Create,SetHeight,GetInfo}.cs` — editor tools behind `#if TOOLS` (touch
    `EditorInterface`/live nodes; main-thread-marshalled; presence-gated FIRST line; E2E-verified).
  - `build/com.IvanMurzak.Godot.MCP.Terrain3D.props` — the source-injection props (auto-imported by NuGet
    in the consumer; MUST stay named `<PackageId>.props`).
- `tests/Godot-AI-Terrain3D.Tests/` — xUnit specs for the pure-managed sources only (no Godot binary):
  the tool-id consts, the `AddonGateResult` shape + hint text, the snake_case name + enum-int constants.
- `testbed/Terrain3D-Testbed.csproj` — a consumer `Godot.NET.Sdk` project that restores the local-packed
  package; `dotnet build` of it is the source-injection proof.

## Tools

| Tool | Kind | File |
| --- | --- | --- |
| `terrain3d-defaults` | pure-managed | `Runtime/Tools/Tool_Terrain3D.Defaults.cs` |
| `terrain3d-create` | editor | `Editor/Tools/Tool_Terrain3D.Create.cs` |
| `terrain3d-set-height` | editor | `Editor/Tools/Tool_Terrain3D.SetHeight.cs` |
| `terrain3d-get-info` | editor | `Editor/Tools/Tool_Terrain3D.GetInfo.cs` |

The editor tool set is confirmed/adjusted against the installed Terrain3D **v1.0.2-stable** API in the
implement step. Terrain3D persists regions to a **writable data directory** (not just the scene), so the
`create` tool sets a `res://` data dir and the e2e fixture points at a committed/created one.

## Build / test (no Godot binary, addon absent)

```bash
dotnet build src/Godot-AI-Terrain3D/Godot-AI-Terrain3D.csproj   # source-only package compiles tools (addon NOT needed)
dotnet test  tests/Godot-AI-Terrain3D.Tests/Godot-AI-Terrain3D.Tests.csproj
dotnet pack  src/Godot-AI-Terrain3D/Godot-AI-Terrain3D.csproj -p:Version=0.0.0-ci -o local-nuget
dotnet build testbed/Terrain3D-Testbed.csproj                  # consumes the local package (injection proof)
```

`Godot.NET.Sdk` supplies GodotSharp from NuGet, so no Godot install is needed to build/test/pack or to
prove the source-injection recipe (the testbed build is a faithful proxy for `godot --build-solutions`).
**`dotnet build -c Debug` MUST exit 0 with the Terrain3D addon ABSENT** — the Class-B no-dependency gate:
the package compiles on a machine that never installed the addon, because it never names an addon type
(only string names). When proving locally, note `dotnet pack` re-uses the **global NuGet cache** for an
already-cached version: if you re-pack the same `Version`, clear
`~/.nuget/packages/com.ivanmurzak.godot.mcp.terrain3d/<ver>` (or pack a unique version) before re-restoring
the testbed, or you'll silently build the stale cached source.

## Conventions

- Root namespace `com.IvanMurzak.Godot.MCP.Terrain3D`. Every `.cs` starts with the Apache-2.0 header.
- Pure-managed tools + the `AddonGate` result shape + the name/enum-int constants → `Runtime/` (outside
  `#if TOOLS`, unit-testable); editor-driving tools → `Editor/` (behind `#if TOOLS`, every Godot call via
  `MainThread.Instance.Run(...)`, the presence gate as the FIRST line, E2E-verified).
- **No GodotSharp and no addon dependency** — the package declares ONLY the `com.IvanMurzak.McpPlugin` /
  `com.IvanMurzak.ReflectorNet` min-version pins; Terrain3D is referenced **by string name only** (CI
  asserts the nuspec). Keep the MCP pins in lockstep with the core Godot-MCP addon; bump with
  `commands/update-core.ps1`.
- Terrain3D is a **GDExtension** → resolve via `ClassDB` (`ClassDB.ClassExists("Terrain3D")` /
  `ClassDB.Instantiate("Terrain3D")`), **not** the GDScript global-class list. Member names are GDScript
  **`snake_case`** (`region_size`, `data`), **not** C# PascalCase; enums are plain ints. Centralize +
  unit-test them (there are no compile-time enum types to lean on).
- **Namespace-shadow:** the root namespace contains `com.IvanMurzak.Godot`, so any inline
  `Godot.`-qualified type name binds to that ancestor, not the engine — prefix such inline names with
  `global::Godot.` (e.g. `global::Godot.Collections.Dictionary`) or import via `using Godot.Collections;`.
  Plain `using Godot;` + unqualified `Node`/`ClassDB` are unaffected.
- One `[AiToolType] partial class Tool_Terrain3D`; one `[AiTool]` method per partial-class file. New
  pure-managed sources must be added to the test csproj `<Compile Include>` list to be unit-tested.

## Find detail in

- `docs/source-only-nuget-recipe.md` — the packaging recipe (the centerpiece) + the consumer story.
- `docs/ci.md` — workflows, the version gate, multi-Godot matrix, the publish secrets.
