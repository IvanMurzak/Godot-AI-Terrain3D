<h1 align="center">Godot AI Terrain3D</h1>

<p align="center">
  AI <b>MCP tools</b> for the <b>Terrain3D</b> Godot addon (large-scale clipmap terrain) —
  an extension for
  <a href="https://github.com/IvanMurzak/Godot-MCP">Godot-MCP / AI Game Developer</a>.
</p>

`Godot-AI-Terrain3D` adds a focused MCP tool family for the community
[**Terrain3D**](https://github.com/TokisanGames/Terrain3D) addon — a high-performance, editable
clipmap terrain system for Godot. The tools are authored in C# with `[AiToolType]` / `[AiTool]` (the
same model as Unity-MCP and the core Godot-MCP addon) and shipped as a **source-only NuGet package**
that compiles inside any consumer's Godot project against the consumer's own GodotSharp — no bundled
Godot, no version lock. Created from
[`Godot-AI-Tools-Template`](https://github.com/IvanMurzak/Godot-AI-Tools-Template).

This is an **addon-dependent** extension. Terrain3D is a **precompiled C++ GDExtension**, so its classes
register in Godot's `ClassDB`; this extension drives them **by name at runtime** (resolved through
`ClassDB`) and never takes a compile-time dependency on the addon. Every editor tool is
**presence-gated** — if Terrain3D is not installed, the tool returns a clean, structured
`installed: false` result (with an install hint) instead of crashing.

## Required prerequisite — Terrain3D (install it yourself)

This extension **does NOT include** the Terrain3D addon and does not download or vendor it. You must
install Terrain3D into your own Godot project separately:

- Install **Terrain3D** from the **Godot Asset Library** (search "Terrain3D"), or from
  **https://github.com/TokisanGames/Terrain3D** (tested against **v1.0.2-stable**). Terrain3D ships
  **precompiled binaries** in its release — no C++ build is required.
- Unzip it into **`addons/terrain_3d/`**, **restart Godot** (a GDExtension only loads on editor
  startup), then enable it under **Project Settings → Plugins**.

Without Terrain3D installed and enabled, the editor tools below report `installed: false` and take no
action (by design).

> Terrain3D is © its authors and distributed under the **MIT License**. This extension is
> **not affiliated with, endorsed by, or sponsored by** the Terrain3D project — it merely provides AI
> tools that drive it. See the addon's own repository for its licence and terms.

## Tools

| Tool | Kind | Description |
| --- | --- | --- |
| `terrain3d-defaults` | pure-managed | Return the recommended region size / clipmap / LOD starter config (no addon needed). |
| `terrain3d-create` | editor (`#if TOOLS`) | Instantiate a `Terrain3D` node in the edited scene; set its data directory + region size. |
| `terrain3d-set-height` | editor (`#if TOOLS`) | Sculpt/set terrain height at a world position via the terrain's data API. |
| `terrain3d-get-info` | editor (`#if TOOLS`) | Read a `Terrain3D` node's region count / size / extents (read-only). |

The exact editor tool set is finalized against the installed Terrain3D **v1.0.2-stable** API. Pure-managed
tools (no Godot native API — the `*-defaults` tool plus the addon class/member name + enum-int constants)
live under `src/Godot-AI-Terrain3D/Runtime/` and are CI-unit-tested; editor-driving tools live under
`Editor/` behind `#if TOOLS`, marshal every Godot call onto the editor main thread via
`MainThread.Instance.Run(...)`, and resolve Terrain3D's classes dynamically through
`Runtime/Interop/AddonInterop.cs` (the `ClassDB` path for GDExtension addons).

## Install (in a consumer Godot project)

Requires the core [`godot_mcp`](https://github.com/IvanMurzak/Godot-MCP) addon **and** the Terrain3D
addon (see the prerequisite above). Then either:

- **Extensions dock** — pick it inside the Godot editor (Install → adds the `<PackageReference>` → rebuild).
- **CLI** — `godot-cli install-extension com.IvanMurzak.Godot.MCP.Terrain3D`.
- **By hand** — add `<PackageReference Include="com.IvanMurzak.Godot.MCP.Terrain3D" Version="x.y.z" />`
  to the consumer `.csproj` and rebuild.

After a rebuild the `[AiToolType]` tool family is auto-discovered — no registry edit.

## Build & test (no Godot binary, addon absent)

`Godot.NET.Sdk` pulls GodotSharp from NuGet, so the package builds and unit-tests headless. Because the
package references Terrain3D **only by string name**, it compiles cleanly with the addon **absent**:

```bash
dotnet build src/Godot-AI-Terrain3D/Godot-AI-Terrain3D.csproj            # compiles tools (Godot API resolves; addon NOT needed)
dotnet test  tests/Godot-AI-Terrain3D.Tests/Godot-AI-Terrain3D.Tests.csproj   # pure-managed unit tests
dotnet pack  src/Godot-AI-Terrain3D/Godot-AI-Terrain3D.csproj -p:Version=0.0.0-ci -o local-nuget
dotnet build testbed/Terrain3D-Testbed.csproj                            # consumer build = source-injection proof
```

The testbed build proves the source-injection recipe: the package's `.cs` are injected as `<Compile>`
items into the consumer and compile against the consumer's own GodotSharp. CI runs this across a
multi-Godot-version matrix; an end-to-end leg additionally boots real headless Godot, installs the core
addon **and the pinned Terrain3D GDExtension** (with its linux-x64 binary), then drives each tool and
asserts the presence-gated results.

## Docs

- `docs/source-only-nuget-recipe.md` — the packaging recipe (the centerpiece).
- `docs/ci.md` — workflows, the version gate, the multi-Godot matrix, required secrets.
- `CLAUDE.md` — maintainer notes (incl. the addon-dependent / presence-gate model).

## Publish

Source-only, version-gated release (see `docs/ci.md`): configure NuGet Trusted Publishing (OIDC) + the
`NUGET_USER` variable, bump `<Version>` (`commands/bump-version.ps1 -NewVersion x.y.z`), merge to `main`;
`release.yml` runs the full matrix, publishes the package to NuGet, and cuts an atomic GitHub Release.

License: **Apache-2.0** (this extension). The Terrain3D addon it drives is MIT, © its authors —
install it yourself (see the prerequisite above); it is never bundled here.
