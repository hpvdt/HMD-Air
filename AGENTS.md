# HMD-Air Unity Project

## Rules

- Compile and run tests frequently to verify work.
- Avoid GUIDs in asmdef files, use [Required]/[Autofill] attributes for Unity references
- No Unity cross-asset references except prefabs
- Do not ask the user to run the tests, check test results, or close Unity instance
- Do not close or quit Unity Editor

## Build/Test Commands

- **Build**: `dotnet build HMD-Air.sln`
- **Test**:  Use Unity MCP to run PlayMode or EditMode tests, search menu under `Tools/ MCP Tools` to find the right
  tools, test results will be reported in the console.

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
