# Repository Guidelines

## Project Structure & Module Organization
- Code: `Assets/BOH/Scripts` (e.g., `Core`, `DialogueS`), organized by feature.
- Data: `Assets/BOH/Scriptables` (`Events`, `Configs`, `Items`).
- Assets: `Assets/BOH/Fonts`, `Assets/TextMesh Pro`, shaders, materials, etc.
- Settings: `Assets/Settings` (URP and project-wide configuration).
- Scenes follow `SCN_*` naming (e.g., `SCN_Menu`, `SCN_Chawl`).

## Build, Test, and Development Commands
- Open locally: Open the folder in Unity Hub, then use the Unity Editor.
- Build: Editor → File → Build Settings… → Build. For CI, use Unity CLI (example):
  `unity -batchmode -quit -projectPath . -executeMethod BuildScripts.PerformBuild`
  Replace method with your build entry if different.
- Run tests (CLI examples):
  - EditMode: `unity -batchmode -quit -projectPath . -runTests -testPlatform EditMode -logFile -`
  - PlayMode: `unity -batchmode -quit -projectPath . -runTests -testPlatform PlayMode -logFile -`

## Coding Style & Naming Conventions
- C#: 4-space indent; one class per file; filename matches class.
- Namespaces: `BOH` root (e.g., `BOH.Core`).
- Classes/Properties: PascalCase. Private fields: camelCase; serialize with `[SerializeField]`.
- Events/Scriptables: prefer ScriptableObject events for decoupling; avoid hard scene references.

## Testing Guidelines
- Framework: Unity Test Framework (EditMode/PlayMode).
- Location: `Assets/Tests/EditMode` and `Assets/Tests/PlayMode`.
- Naming: suffix files with `Tests` (e.g., `TimeSystemTests.cs`).
- Scope: test core systems (`TimeSystem`, `GameStateManager`, `ResourceSystem`) with focused, deterministic cases.

## Commit & Pull Request Guidelines
- Commits: concise, imperative, Conventional Commits style when possible
  (e.g., `feat(core): add pause toggle in GameStateManager`).
- PRs: include summary, linked issues, impacted scenes/assets, and screenshots/GIFs of UI changes.
- Include `.meta` files for new assets; avoid committing `Library/` or large binaries unless required.

## Security & Configuration Tips
- Input System: configs in `Assets/BOH/Scriptables/Configs/MyControls.inputactions`. After editing, regenerate C# wrappers.
- Scenes/Addressing: use scene names via a central config/service; avoid string literals scattered in code.
- Rendering: URP settings live under `Assets/Settings`; changes affect all scenes—coordinate via PR.

