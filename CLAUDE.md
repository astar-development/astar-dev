# CLAUDE.md

Guide for Claude Code (claude.ai/code) working this repo.

## Repository overview

`astar-dev` = .NET 10 mono repo. Several standalone desktop apps, web apps, reusable NuGet packages, one solution file. Early-stage — several projects still in dev.

- Solution file: `AStarDev.slnx` (newer XML-lite `.slnx` format, not `.sln`)
- Target framework: `net10.0` everywhere — don't bring net8.0/net9.0 or mixed targeting unless task explicitly needs it
- Test runner: `Microsoft.Testing.Platform` (configured in `global.json`), xUnit v3 (`xunit.v3.mtp-v2`), not older VSTest-based runner

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

Before first restore/build against GitHub Packages, add GitHub NuGet source locally — see `local-setup.md`.

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

Each app/package dir same shape: main project plus sibling test projects suffixed `.TestsUnit` and, where present, `.TestsIntegration` (e.g. `AStarDev.OneDriveSyncClient` has both; simpler apps/packages currently only `.TestsUnit`). Keep new app code under matching `apps/*` folder, new reusable/dependency-light code under `nuget-packages/*` — don't cross boundary casually.

### Desktop apps (Avalonia)

`AStarDev.Clock`, `AStarDev.OneDriveSyncClient`, `AStarDev.WallpaperScraper` = Avalonia 12 UI apps (`OutputType=WinExe`, `Avalonia.Themes.Fluent`). `AStarDev.OneDriveSyncClient` also uses EF Core with SQLite (`Microsoft.EntityFrameworkCore.Sqlite`), domain/persistence split:

- `Domain/` — plain records for app's model (e.g. `SearchConfiguration`, `SearchCategory`, strongly-typed ID records `SearchConfigurationId`/`SearchCategoryId`)
- `Persistence/` — EF Core-mapped entity types same names plus `AStarDevContext` (the `DbContext`, configured via `ApplyConfigurationsFromAssembly`) and `AStarDevContextFactory` (design-time factory for migrations)

Keep domain/persistence split when extending this app — don't map EF entities directly onto domain records.

### Web apps (Blazor)

`AStarDev.Web`, `Fab4Kids.Web` both `Microsoft.NET.Sdk.Web` Blazor apps, `BlazorDisableThrowNavigationException` set, standard ASP.NET Core component conventions (`Components/App.razor`, `Routes.razor`, `Program.cs`).

## Releasing

Release pipelines tag-triggered, each artifact type owns disjoint tag namespace — one tag fires only one workflow. See `how-to-publish.md` for full table + per-artifact steps (NuGet packages: `{PackageName}/v{version}`; desktop apps: own `{app}-v{version}` prefix each). Never reuse another artifact's tag format. Sanity-check before tagging: confirm commit (`git log -1
--oneline`) and tag not already exist (`git tag -l "<tag>"`).

## Working conventions

- Never commit directly to `main`. Before making any changes, create a feature branch (`git checkout -b <branch-name>`) off `main`; commit there, push it, and raise a PR — do not push `main` or commit straight to it.
- Prefer modern C#/.NET 10 idioms: nullable reference types, implicit usings, file-scoped namespaces, minimal/straightforward DI.
- Prefer small focused changes over broad refactors; keep public APIs stable unless task changes them; update matching `.TestsUnit`/`.TestsIntegration` project when behavior changes.
- Favor minimal dependencies; don't add new frameworks/patterns without clear need.
- Working with GitHub-hosted NuGet packages: follow `local-setup.md` before restore/build actions needing that source.
- Run only tests affected by change (new or existing, scoped to touched project(s)/method(s) via `--filter-method`), not whole solution — see Commands above. Full-suite coverage runs via `./code-coverage.sh` at session end instead.

## graphify

Project has knowledge graph at graphify-out/ — god nodes, community structure, cross-file relationships.

Rules:
- Codebase questions: run `graphify query "<question>"` first when graphify-out/graph.json exists. `graphify path "<A>" "<B>"` for relationships, `graphify explain "<concept>"` for focused concepts. Returns scoped subgraph, usually much smaller than GRAPH_REPORT.md or raw grep output.
- If graphify-out/wiki/index.md exists, use for broad navigation instead of raw source browsing.
- Read graphify-out/GRAPH_REPORT.md only for broad architecture review or when query/path/explain don't surface enough context.
- After modifying code, run `graphify update .` to keep graph current (AST-only, no API cost).