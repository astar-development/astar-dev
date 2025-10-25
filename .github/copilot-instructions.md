## Purpose

Give coding agents the minimal, focused context they need to be immediately productive in the AStar.Dev monorepo.
Keep instructions short and concrete — point to exact files and examples the agent should reuse.

## High-level architecture (what to know fast)

- Monorepo of .NET 9 projects grouped under `src/` and tests under `test/`.
- The repo follows the Aspire patterns: shared helpers live in the `_aspire/` folder and are consumed by APIs, services and the UI.
- Important surface areas you should read before editing code:
  - App host / orchestration: `src/_aspire/AStar.Dev.AppHost/AppHost.cs` — builds a DistributedApplication and wires application projects.
  - Shared service helpers: `src/_aspire/AStar.Dev.ServiceDefaults/Extensions.cs` — AddServiceDefaults, OpenTelemetry wiring, health checks.
  - Constants (authoritative names): `src/_aspire/AStar.Dev.Aspire.Common/AspireConstants.cs` — DB and service name constants.
  - Example API wiring: `src/apis/AStar.Dev.Files.Api/Program.cs` shows the typical minimal Program.cs and extension usage.
  - Shared packages: `src/nuget-packages/*` contain code intended for reuse across services; prefer adding cross-cutting changes here.

## What to read & re-use (concrete files)

- Use `AspireConstants` for DB/service names (avoid string literals): `src/_aspire/AStar.Dev.Aspire.Common/AspireConstants.cs`.
- Reuse `AddServiceDefaults()` from `src/_aspire/AStar.Dev.ServiceDefaults/Extensions.cs` for telemetry, logging, health checks.
- Register EF contexts with `AddSqlServerDbContext<TContext>(name)` — pass names from `AspireConstants.Sql.*`.
- RabbitMQ clients use the constant `AspireConstants.Services.AstarMessaging` when registering/consuming.
- AppHost uses `applicationConfiguration:sqlServerMountDirectory` — check `src/_aspire/AStar.Dev.AppHost/appsettings.json` for local dev hints.

## Developer workflows & commands

- Build repository (CI): from repo root run
  - dotnet build --configuration Release
- Run unit tests (fast):
  - dotnet test --filter 'FullyQualifiedName!~Tests.EndToEnd&FullyQualifiedName!~Tests.Integration'
  - Tests primarily use xUnit V3 and Shouldly. NSubstitute is used for mocking when necessary.
- When adding shared runtime code, add to `src/nuget-packages/*` and reference the project instead of introducing new global packages.

## Project-specific conventions and patterns

- Tiny Program.cs: services and APIs are composed via extension methods. Prefer adding extension helpers in `_aspire` or `src/nuget-packages`.
- Telemetry & logging: Serilog + OpenTelemetry. Do not add duplicate OTLP exporters — use `AddOpenTelemetryExporters()` from service defaults.
- Configuration keys: use the existing `Parameters` entries (e.g., `Parameters:sql1-password`) and `applicationConfiguration` keys when present.
- Tests: put unit tests in `test/<area>/*.Tests.Unit` and follow existing patterns (Shouldly for assertions, test fixtures in Fixtures/ when needed).
- Whilst the AAA pattern is used, comments should not be added to a test unless the logic is complex. If the logic is complex, consider breaking it into multiple tests. Instead of comments, use blank lines to separate the Arrange, Act, and Assert sections.
- Using statements for Shouldly and Xunit are not required as they are included globally via the relevant `csproj`.
- Public methods in the production code should have XML doc comments. Test methods do not require XML doc comments.

## Integration & cross-component communication

- Database contexts: EF DbContexts registered with `AddSqlServerDbContext<T>(name)` use the named DBs from `AspireConstants.Sql.*`.
- Message bus: RabbitMQ client names come from `AspireConstants.Services.*` — follow that naming for publishers/subscribers.
- AppHost mounts local SQL folders via `applicationConfiguration:sqlServerMountDirectory` — used for local integration runs.

## Editing guidance (what NOT to change without approval)

- Do not change values in `AspireConstants.cs` without coordinating infra/deployments — these are authoritative.
- Avoid adding new top-level telemetry exporters or duplicate service discovery wiring; reuse `AddServiceDefaults` and `AddOpenTelemetryExporters()`.

## Quick examples (copy/paste patterns)

- Register files DB context (use constants):
  - builder.AddSqlServerDbContext<FilesContext>(AspireConstants.Sql.FilesDb);
- Add RabbitMQ client:
  - builder.AddRabbitMQClient(AspireConstants.Services.AstarMessaging);

## Where to add shared code

- If the change is cross-cutting (telemetry, DI helpers, logging) add an extension in `_aspire/` or create/update a project under `src/nuget-packages/` so it can be consumed across services.

## Tests & CI expectations

- New production code should include unit tests in the relevant `test/*` project. Follow local test project references (xUnit + Shouldly) and CI filters.
- CI workflow: `.github/workflows/main_astar-dev.yml` runs build + coverage; ensure changes don't dramatically increase test runtime.

## If you need more context

- Look at these files first: `src/_aspire/AStar.Dev.ServiceDefaults/Extensions.cs`, `src/_aspire/AStar.Dev.Aspire.Common/AspireConstants.cs`, `src/_aspire/AStar.Dev.AppHost/AppHost.cs`, `src/apis/AStar.Dev.Files.Api/Program.cs`.
- If a service fails to start locally, check `appsettings.Development.json` in the service project and `applicationConfiguration:sqlServerMountDirectory` in AppHost appsettings.

## Naming & configuration conventions

- AspireConstants are authoritative for service and DB names: `src/_aspire/AStar.Dev.Aspire.Common/AspireConstants.cs`. Use these constants when referencing DB names, API names, or service names (e.g., `AspireConstants.Sql.FilesDb`, `AspireConstants.Services.AstarMessaging`).
- Connection strings and parameters are read via extension helpers. Example config keys:
  - `applicationConfiguration:sqlServerMountDirectory` (used by AppHost)
  - `Parameters:sql1-password`, `rabbitmq-username`, `rabbitmq-password` (optional parameter placeholders in appsettings.json)

## Common extension helpers & patterns agents should reuse

- AddServiceDefaults (from `AStar.Dev.ServiceDefaults`) is the place for telemetry, health checks, and HTTP client defaults. New services should call `builder.AddServiceDefaults()` early in `Program.cs`.
- AddSqlServerDbContext<TContext>(name) is used to register EF contexts against named DBs. Look for usages in `src/apis/*` and `src/services/*`. Use `AspireConstants.Sql.*` for the name parameter.
- OpenTelemetry exporters / Application Insights are enabled by environment variables or configuration keys (see `ConfigureOpenTelemetry` in `Extensions.cs`). Don't duplicate exporters; reuse `AddOpenTelemetryExporters()`.
- RabbitMQ client names are referenced via `AspireConstants.Services.AstarMessaging` — use the constant.

## Testing & CI signals

- CI uses the repository-level GitHub workflow at `.github/workflows/main_astar-dev.yml` which runs `dotnet build --configuration Release` and a dotnet coverage command. Follow the same `dotnet` commands locally:
  - Build: `dotnet build --configuration Release` from repository root.
  - Test (fast): `dotnet test --filter 'FullyQualifiedName!~Tests.EndToEnd&FullyQualifiedName!~Tests.Integration'` to run unit tests only (the CI wraps this with coverage collector).
  - Tests: existing tests use xUnit. Shouldly is used for assertions. NSubstitute is used for mocking when necessary.
  - When creating new production code, add unit tests in the corresponding `test/*` project. Follow existing test patterns.

## How services start (local dev hints)

- Each service has a `Program.cs` that uses builder extension helpers. For local runs you usually need:
  - Environment configuration (appsettings.Development.json) is present in many projects. Adjust `Parameters` or environment variables to provide secrets (sql password, rabbitmq password) if required.
  - Many services call `builder.AddSqlServerDbContext<...>(AspireConstants.Sql.FilesDb)` — supplying a real SQL connection or a local SQL mount directory (see `applicationConfiguration:sqlServerMountDirectory` in `src/_aspire/AStar.Dev.AppHost/appsettings.json`) will make EF start.

## Project-specific patterns agents should follow

- Prefer extension methods and small Program.cs files. Most functionality is surfaced via extension packages in `src/_aspire` or `src/nuget-packages`.
- When adding dependencies, consider adding them to the appropriate `src/nuget-packages/*` project if they're intended to be shared across services. Those projects are built and packaged from source inside this repo.
- Logging and telemetry: services use Serilog and OpenTelemetry by default. Use the existing logging patterns (Log.Information / Log.Error) rather than adding new logging frameworks.

## Files and locations to reference when changing behavior

- Aspire constants: `src/_aspire/AStar.Dev.Aspire.Common/AspireConstants.cs`
- Service defaults and OpenTelemetry: `src/_aspire/AStar.Dev.ServiceDefaults/Extensions.cs`
- Example API wiring and usage of helpers: `src/apis/AStar.Dev.Files.Api/Program.cs`
- App host entrypoint and mount directory config: `src/_aspire/AStar.Dev.AppHost/AppHost.cs` and `appsettings.json`

## Examples (concrete snippets agents can follow)

- Register DB context for files DB (use constants):
  - builder.AddSqlServerDbContext<FilesContext>(AspireConstants.Sql.FilesDb);
- Add RabbitMQ client with consistent service name:
  - builder.AddRabbitMQClient(AspireConstants.Services.AstarMessaging);

## What not to change without checking humans

- Do not change the constants in `AspireConstants.cs` unless you're also updating deployment and orchestration configuration.
- Avoid enabling health endpoints in non-development environments without checking the security implications (see comment in `MapDefaultEndpoints`).

## Quick checklist for PRs

1. Follow existing extension patterns — prefer adding an extension in `_aspire` or `src/nuget-packages` for cross-cutting changes.
2. Update/consume `AspireConstants` when adding new services or DBs.
3. Ensure telemetry and exporters aren't duplicated (reuse `AddOpenTelemetryExporters`).
4. Run unit tests (see Test section) and ensure CI-friendly filters are respected.
