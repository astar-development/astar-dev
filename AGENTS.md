# AGENTS.md

.NET 10 multi-project solution. Desktop + web apps, plus reusable NuGet packages.

## Project context

- Target framework: net10.0 solution-wide.
- Primary solution file: [AStarDev.slnx](AStarDev.slnx)
- Repo overview: [README.md](README.md)
- Local setup, package source notes: [local-setup.md](local-setup.md)

## Core coding conventions

- Keep solution on .NET 10. No net8.0, net9.0, mixed-targeting unless task explicitly needs it.
- Prefer modern C#/.NET 10 idioms: nullable reference types, implicit usings, file-scoped namespaces where consistent, straightforward dependency injection.
- Follow existing structure. App code under matching folder in [apps](apps); package/library code under [nuget-packages](nuget-packages).
- Prefer small, focused changes over broad refactors.
- Keep public APIs stable unless task specifically changes them; update related tests when behavior changes.
- Favor minimal dependencies, clear separation of responsibilities. No new frameworks/patterns without clear need.

## Testing and validation

- Restore, validate via .NET CLI.
- Typical commands:
  - `dotnet restore AStarDev.slnx`
  - `dotnet build AStarDev.slnx --nologo`
  - `dotnet test AStarDev.slnx --nologo`
- For focused validation, run smallest project-level test command exercising changed area, not whole solution.
- Tests use xUnit v3 + Microsoft.Testing.Platform. Keep unless explicit reason to change.
- Important: VS Code Test Explorer authoritative for test discovery here. Don't report "no tests discovered" from solution-level runner invocation unless known to match repo's MTP/xUnit config.
- Project/solution command reports "Zero tests ran" → treat as possible runner-config mismatch, not proof repo has no tests. Prefer test explorer/discovered test list when available.
- Don't say "tests not running" or "no tests found" unless checked actual discovery source in VS Code or exact project-level command used.

## Application-specific expectations

- Desktop apps: [apps/desktop](apps/desktop). Stay aligned with existing Avalonia structure.
- Web apps: [apps/web](apps/web). Follow existing ASP.NET Core conventions.
- Reusable libraries in [nuget-packages](nuget-packages): stay general-purpose, dependency-light.
- Working with GitHub-hosted NuGet packages: follow setup steps in [local-setup.md](local-setup.md) before restore/build needing package source.

## Working rules for AI agents

- Prefer direct, targeted edits to relevant project/file.
- No "temporary" debug code, broad logging, speculative abstractions.
- Match naming, conventions, structure to nearby code.
- Task crosses project boundaries → keep integration points clean, explicit.
- Before finishing, validate changed behavior with most relevant command available.

## Reference points

- [README.md](README.md)
- [local-setup.md](local-setup.md)
- [AStarDev.slnx](AStarDev.slnx)
- [apps](apps)
- [nuget-packages](nuget-packages)