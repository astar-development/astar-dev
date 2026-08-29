# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository overview

`astar-dev` is a .NET 10 mono repo containing several independent desktop apps, web apps, and reusable NuGet
packages, all sharing one solution file. The repo is early-stage: several projects are still template scaffolding
(`Class1.cs`, `UnitTest1.cs`).

- Solution file: `AStarDev.slnx` (the newer XML-lite `.slnx` format, not `.sln`)
- Target framework: `net10.0` everywhere — do not introduce net8.0/net9.0 or mixed targeting unless a task
  explicitly requires it
- Test runner: `Microsoft.Testing.Platform` (configured in `global.json`), using xUnit v3
  (`xunit.v3.mtp-v2`), not the older VSTest-based runner

## Commands

```bash
# Restore / build / test the whole solution
dotnet restore AStarDev.slnx
dotnet build AStarDev.slnx --nologo
dotnet test AStarDev.slnx --nologo

# Build/test a single project (preferred for focused work — faster than the whole solution)
dotnet test apps/desktop/AStarDev.Clock/AStarDev.Clock.TestsUnit/AStarDev.Clock.TestsUnit.csproj

# Run a single test by name (Microsoft.Testing.Platform filter syntax)
dotnet test <path-to-test-csproj> --filter-method "*MethodName*"

# Code coverage across the whole solution (cleans, builds, collects cobertura coverage, generates HTML report)
./code-coverage.sh
```

Before restoring/building against GitHub Packages for the first time, add the GitHub NuGet source locally — see
`local-setup.md`.

## Repo layout

```
apps/
  desktop/    Avalonia desktop apps (AStarDev.Clock, AStarDev.OneDriveSyncClient, AStarDev.WallpaperScraper)
  web/        ASP.NET Core Blazor web apps (AStarDev.Web, Fab4Kids)
nuget-packages/
  functional/  AStarDev.FunctionalParadigm — general-purpose functional-style helpers
  logging/     AStarDev.LoggingOTel — OpenTelemetry-based logging package
  utilities/   AStarDev.Utilities — general-purpose utilities
  source-generators/  reserved, currently empty
```

Each app/package directory follows the same shape: a main project plus sibling test projects suffixed
`.TestsUnit` and, where present, `.TestsIntegration` (e.g. `AStarDev.OneDriveSyncClient` has both; the simpler
apps/packages currently only have `.TestsUnit`). Keep new app code under the matching `apps/*` folder and new
reusable/dependency-light code under `nuget-packages/*` — don't cross that boundary casually.

### Desktop apps (Avalonia)

`AStarDev.Clock`, `AStarDev.OneDriveSyncClient`, `AStarDev.WallpaperScraper` are Avalonia 12 UI apps
(`OutputType=WinExe`, `Avalonia.Themes.Fluent`). `AStarDev.OneDriveSyncClient` additionally uses EF Core with
SQLite (`Microsoft.EntityFrameworkCore.Sqlite`) and follows a domain/persistence split:

- `Domain/` — plain records representing the app's model (e.g. `SearchConfiguration`, `SearchCategory`,
  and their strongly-typed ID records `SearchConfigurationId`/`SearchCategoryId`)
- `Persistence/` — EF Core-mapped entity types of the same names plus `AStarDevContext` (the `DbContext`,
  configured via `ApplyConfigurationsFromAssembly`) and `AStarDevContextFactory` (design-time factory for
  migrations)

Keep this domain/persistence separation when extending this app rather than mapping EF entities directly onto
domain records.

### Web apps (Blazor)

`AStarDev.Web` and `Fab4Kids.Web` are both `Microsoft.NET.Sdk.Web` Blazor apps with
`BlazorDisableThrowNavigationException` set, following standard ASP.NET Core component conventions
(`Components/App.razor`, `Routes.razor`, `Program.cs`).

## Releasing

Release pipelines are tag-triggered and each type of artifact owns its own disjoint tag namespace, so pushing
one tag only ever fires one workflow — see `how-to-publish.md` for the full table and per-artifact steps
(NuGet packages use `{PackageName}/v{version}`; desktop apps each use their own `{app}-v{version}` prefix).
Never reuse another artifact's tag format. Sanity-check before tagging: confirm the commit (`git log -1
--oneline`) and that the tag doesn't already exist (`git tag -l "<tag>"`).

## Working conventions

- Prefer modern C#/.NET 10 idioms: nullable reference types, implicit usings, file-scoped namespaces, minimal/
  straightforward DI.
- Prefer small, focused changes over broad refactors; keep public APIs stable unless the task changes them, and
  update the matching `.TestsUnit`/`.TestsIntegration` project when behavior changes.
- Favor minimal dependencies; don't introduce new frameworks or patterns without clear need.
- When working with GitHub-hosted NuGet packages, follow `local-setup.md` before restore/build actions that need
  that package source.
- Run only the tests affected by a change (new or existing, scoped to the touched project(s)/method(s) via
  `--filter-method`), not the whole solution — see Commands above. Full-suite coverage runs via `./code-coverage.sh`
  at session end instead.

## graphify

This project has a knowledge graph at graphify-out/ with god nodes, community structure, and cross-file relationships.

Rules:
- For codebase questions, first run `graphify query "<question>"` when graphify-out/graph.json exists. Use `graphify path "<A>" "<B>"` for relationships and `graphify explain "<concept>"` for focused concepts. These return a scoped subgraph, usually much smaller than GRAPH_REPORT.md or raw grep output.
- If graphify-out/wiki/index.md exists, use it for broad navigation instead of raw source browsing.
- Read graphify-out/GRAPH_REPORT.md only for broad architecture review or when query/path/explain do not surface enough context.
- After modifying code, run `graphify update .` to keep the graph current (AST-only, no API cost).
