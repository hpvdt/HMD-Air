# AGENT.md - HMD-Air Unity Project

## Build/Test Commands

- **Build**: `dotnet build HMD-Air.sln` (compile frequently to verify work)
- **Test All**: `dotnet test ca.hpvdt.mavlinkapi.Tests.csproj` (runs NUnit tests)
- **Single Test**: `dotnet test ca.hpvdt.mavlinkapi.Tests.csproj --filter "TestName"` or use Unity Test Runner
- **Unity**: Open project in Unity 2022.3 LTS, use Unity Test Runner for Play Mode tests
- The path of Unity editor is OS-dependent:
    - `C:\Program Files\Unity\Hub\Editor\2022.3.62f1\Editor\Unity.exe` on Windows
    - `/home/peng/Unity/Hub/Editor/2022.3.62f1/Editor/Unity` on Linux

## Architecture

- **Unity 2022.3** AR/XR helmet-mounted display for aerial vehicles
- **Core Components**: Assets/HMD/ (main app), Assets/NRSDK/ (AR), Assets/VLCUnity/ (video streaming)
- **MAVLink API**: Packages/MAVLinkAPI/ (drone telemetry communication)
- **Key Namespaces**: HMD.Scripts (main), HMD.Scripts.Streaming (video), MAVLinkAPI (telemetry)
- **Assemblies**: HMD.Scripts.asmdef, ca.hpvdt.mavlinkapi.Runtime.asmdef

## Code Style (.editorconfig)

- **Formatting**: 4-space indents, LF line endings, UTF-8 encoding, trim whitespace on save
- **Testing**: NUnit framework with [Test] and [UnityTest] attributes
- **Unity Rules**: Avoid GUIDs in asmdef files, use [Required]/[Autofill] attributes for Unity references
- **Imports**: Follow existing patterns in HMD.Scripts namespace
- **Error Handling**: Use MonoBehaviourWithLogging base class for enhanced logging
- **No Unity cross-asset references** except prefabs (breaks on git merge/rebase)
