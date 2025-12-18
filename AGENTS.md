# HMD-Air Unity Project

## Rules

- Verify your work after every change:
    1. Compile the project using dotnet
    2. Load the project using UnityMCP
- Write a test suite for every new feature
    - Run the test suite during verification
- Avoid GUIDs in asmdef files, use [Required]/[Autofill] attributes for Unity references
- No Unity cross-asset references except prefabs
- Do not ask the user to run the tests, check test results, or close Unity instance
- Do not close or quit Unity Editor

## Build/Test Commands

- **Build**: `dotnet build HMD-Air.sln`
- **Test**:
    - **Run tests Directly**: Use Unity MCP `run_tests` with mode="EditMode" or "PlayMode"
      ```bash
      mcp__UnityMCP__run_tests(mode="<EditMode|PlayMode>")
      ```
    - **Run tests through menu option**:  Use Unity MCP to search menu under
      `Tools/ MCP Tools` to find the right option, test results will be reported in the console.
    - Test results will be reported in the console with detailed pass/fail information
    - EditMode tests run in Unity Editor environment (faster, no scene loading required)
    - PlayMode tests run in actual gameplay mode (slower, but tests runtime behavior)

## Architecture

- **Unity 2022.3** AR/XR helmet-mounted display for aerial vehicles
- **Core Components**: Assets/HMD/ (main app), Assets/NRSDK/ (AR), Assets/VLCUnity/ (video streaming)
- **MAVLink API**: Packages/MAVLinkAPI/ (drone telemetry communication)
- **Key Namespaces**: HMD.Scripts (main), HMD.Scripts.Streaming (video), MAVLinkAPI (telemetry)
- **Assemblies**: HMD.Scripts.asmdef, ca.hpvdt.mavlinkapi.Runtime.asmdef

## Code Style (.editorconfig)

- **Formatting**: 4-space indents, LF line endings, UTF-8 encoding, trim whitespace on save
- **Testing**: NUnit framework with [Test] and [UnityTest] attributes
- **Imports**: Follow existing patterns in HMD.Scripts namespace
- **Error Handling**: Use MonoBehaviourWithLogging base class for enhanced logging

## Agent Script Rules

- All script files should be under "HMD.Editor.AgentScript" namespace
- If a Script file for the task exists, use it instead of writing a new one
- All Unity Editor menu entries added by script should be under "Tools/Agent Scripts" menu
- Each script should not require human interaction (e.g dialog box) to complete.