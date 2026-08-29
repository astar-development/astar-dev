# AGENTS.md

This repository is a .NET 10 multi-project solution for desktop and web applications, plus reusable NuGet packages.

## Project context

- Target framework: net10.0 across the solution.
- Primary solution file: [AStarDev.slnx](AStarDev.slnx)
- Repo overview: [README.md](README.md)
- Local setup and package source notes: [local-setup.md](local-setup.md)

## Core coding conventions

- Keep the solution on .NET 10 and do not introduce net8.0, net9.0, or mixed-targeting unless the task explicitly requires it.
- Prefer modern C# and .NET 10 idioms: nullable reference types, implicit usings, file-scoped namespaces where consistent, and straightforward dependency injection patterns.
- Follow the existing project structure. Keep app-specific code under the matching folder in [apps](apps), and package/library code under [nuget-packages](nuget-packages).
- Prefer small, focused changes over broad refactors.
- Keep public APIs stable unless the task specifically changes them, and update related tests when behavior changes.
- Favor minimal dependencies and clear separation of responsibilities; do not introduce new frameworks or patterns without a clear need.

## Testing and validation

- Restore and validate with the .NET CLI.
- Typical commands:
  - `dotnet restore AStarDev.slnx`
  - `dotnet build AStarDev.slnx --nologo`
  - `dotnet test AStarDev.slnx --nologo`
- For focused validation, run the smallest project-level test command that exercises the changed area instead of the entire solution.
- Tests already use xUnit v3 and Microsoft.Testing.Platform in the repo; keep that approach unless there is an explicit reason to change it.
- Important: in this repo, VS Code Test Explorer is authoritative for whether tests are discovered. Do not report “no tests discovered” based on a solution-level runner invocation unless that invocation is known to match the repo’s MTP/xUnit configuration.
- When a project or solution-level command reports “Zero tests ran,” treat that as a possible runner-configuration mismatch rather than evidence that the repository has no tests. Prefer the project’s test explorer / discovered test list when available.
- Do not say “tests are not running” or “no tests were found” unless you have checked the actual test discovery source in VS Code or the exact project-level command being used.

## Application-specific expectations

- Desktop apps live under [apps/desktop](apps/desktop) and should remain aligned with the existing Avalonia-based structure.
- Web apps live under [apps/web](apps/web) and should follow ASP.NET Core conventions already used in the repository.
- Reusable libraries in [nuget-packages](nuget-packages) should remain general-purpose and dependency-light.
- When working with GitHub-hosted NuGet packages, follow the setup steps in [local-setup.md](local-setup.md) before restore/build actions that require the package source.

## Working rules for AI agents

- Prefer direct, targeted edits to the relevant project or file.
- Do not add “temporary” debug code, broad logging, or speculative abstractions.
- Keep naming, conventions, and structure consistent with nearby code.
- If a task crosses project boundaries, keep the integration points clean and explicit.
- Before finishing, validate the changed behavior with the most relevant command available.

## Reference points

- [README.md](README.md)
- [local-setup.md](local-setup.md)
- [AStarDev.slnx](AStarDev.slnx)
- [apps](apps)
- [nuget-packages](nuget-packages)
