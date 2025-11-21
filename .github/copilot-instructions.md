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
## Purpose

Give coding agents the minimal, focused context to be productive in this monorepo. Be concise, point to exact files, and prefer reuse of the `_aspire` helpers.

**High-level architecture**
- Monorepo of .NET services, APIs and a Blazor UI in `src/` with tests in `test/`.
- `_aspire/` contains shared DI/telemetry helpers; `src/nuget-packages/` holds reusable packages.
- AppHost (`src/_aspire/AStar.Dev.AppHost/AppHost.cs`) composes services for local integration.

**Key files to read first**
- `src/_aspire/AStar.Dev.ServiceDefaults/Extensions.cs` — `AddServiceDefaults()`, OTLP wiring, health checks.
- `src/_aspire/AStar.Dev.Aspire.Common/AspireConstants.cs` — authoritative DB and service names.
- `src/_aspire/AStar.Dev.AppHost/AppHost.cs` and its `appsettings.json` — local mount/SQL hints.
- Example wiring: `src/apis/AStar.Dev.Files.Api/Program.cs` and `uis/AStar.Dev.Web/Program.cs`.

**Developer workflows (commands)**
- Build (CI): `dotnet build --configuration Release` (run from repo root).
- Run unit tests (fast): `dotnet test --filter 'FullyQualifiedName!~Tests.EndToEnd&FullyQualifiedName!~Tests.Integration'`.
- Run a single project: open its folder and use `dotnet run` or the small `Program.cs` — many services use extension helpers.

**Project conventions & patterns**
- Tiny `Program.cs` and DI via extension methods: add cross-cutting helpers under `_aspire` or `src/nuget-packages/`.
- Use `AspireConstants` instead of string literals for DB/service names: e.g. `AspireConstants.Sql.FilesDb`.
- Register EF: `builder.AddSqlServerDbContext<Ctx>(AspireConstants.Sql.XxxDb)`.
- Messaging: use `AspireConstants.Services.*` names for RabbitMQ clients/publishers.
- Telemetry: Serilog + OpenTelemetry. Use `AddOpenTelemetryExporters()`; do not add duplicate exporters.

**Integration notes**
- AppHost can mount local SQL directories via `applicationConfiguration:sqlServerMountDirectory` in AppHost `appsettings.json` for integration runs.
- Secrets and parameters typically live under `Parameters:*` in each project config; supply via environment or `appsettings.Development.json`.

**Editing guidance (what not to change without approval)**
- Do not change values in `AspireConstants.cs` without coordinating infra; these are authoritative.
- Avoid adding new global telemetry exporters or duplicate service discovery wiring — reuse `AddServiceDefaults()`.

**Quick examples**
- Register files DB context: `builder.AddSqlServerDbContext<FilesContext>(AspireConstants.Sql.FilesDb)`
- Add RabbitMQ client: `builder.AddRabbitMQClient(AspireConstants.Services.AstarMessaging)`

**PR checklist**
- Prefer adding helpers in `_aspire` or `src/nuget-packages/` for cross-cutting changes.
- Update/consume `AspireConstants` when adding DBs/services.
- Ensure no duplicate telemetry exporters and keep CI test filters.

If anything here is unclear or you want more examples (tests, Program.cs patterns, AppHost run steps), tell me which area to expand.
