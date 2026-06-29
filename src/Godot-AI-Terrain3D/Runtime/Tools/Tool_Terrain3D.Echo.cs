/*
┌──────────────────────────────────────────────────────────────────┐
│  Author: Ivan Murzak (https://github.com/IvanMurzak)             │
│  Copyright (c) 2026 Ivan Murzak                                  │
│  Licensed under the Apache License, Version 2.0.                 │
│  See the LICENSE file in the project root for more information.  │
└──────────────────────────────────────────────────────────────────┘
*/
#nullable enable
using System.ComponentModel;
using com.IvanMurzak.McpPlugin;

namespace com.IvanMurzak.Godot.MCP.Terrain3D
{
    public partial class Tool_Terrain3D
    {
        /// <summary>The tool id, exposed as a const so tests / E2E reference the exact string.</summary>
        public const string EchoToolId = "terrain3d-echo";

        /// <summary>
        /// Pure-managed sample tool — no Godot native API, so it lives OUTSIDE <c>#if TOOLS</c> and is
        /// fully CI-unit-testable (see <c>Tool_Terrain3D_EchoTests</c>) and E2E-verifiable via
        /// <c>godot-cli run-tool terrain3d-echo</c>. Replace it with your real tool(s); keep
        /// pure-managed tools here and editor-driving tools under <c>../../Editor/Tools/</c>.
        /// </summary>
        [AiTool
        (
            EchoToolId,
            Title = "Terrain3D Tools / Echo",
            ReadOnlyHint = true,
            IdempotentHint = true,
            OpenWorldHint = false
        )]
        [Description("Sample readiness probe for the Terrain3D Tools extension. Returns the input " +
            "'message' echoed back, or 'terrain3d-ready' when omitted. Proves the extension's " +
            "tool family is discovered and callable end-to-end after the package compiles into the " +
            "consumer's Godot project.")]
        public string Echo
        (
            [Description("Optional message to echo back. When null or empty, returns 'terrain3d-ready'.")]
            string? message = null
        )
        {
            return string.IsNullOrEmpty(message) ? "terrain3d-ready" : message;
        }
    }
}
