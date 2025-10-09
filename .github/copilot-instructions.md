## Purpose

These instructions give coding agents the minimal, focused context they need to be productive in the AStar.Dev repository. They purposely call out architecture, conventions, and common developer workflows discovered in the codebase so agents can write code and tests that fit the project.

## High-level architecture (what to know fast)

- This is a .NET 9 monorepo built around the Aspire patterns. Projects are grouped under `src/` and tests under `test/`.
- Key surface areas:
  - Host / orchestration: `src/_aspire/AStar.Dev.AppHost/AppHost.cs` — builds a DistributedApplication and calls `AddApplicationProjects(...)` using a `sqlServerMountDirectory` from configuration.
  - Service defaults and shared helpers: `src/_aspire/AStar.Dev.ServiceDefaults/Extensions.cs` — common AddServiceDefaults extension used by services to enable health checks, OpenTelemetry, and service discovery.
  - Per-API projects: `src/apis/*` — each API uses a minimal `Program.cs` with extension helpers (e.g., `AddServiceDefaults`, `AddSqlServerDbContext`, `AddRabbitMQClient`, `AddUsageServices`). Example: `src/apis/AStar.Dev.Files.Api/Program.cs`.
  - UI: `src/uis/AStar.Dev.Web` — Blazor/ASP.NET UI project using `AddApplicationServices()` and `UseApplicationServices()`.

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
  - Test (fast): `dotnet test --filter "FullyQualifiedName!~Tests.EndToEnd&FullyQualifiedName!~Tests.Integration"` to run unit tests only (the CI wraps this with coverage collector).

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

---
If any of these areas look incomplete or you'd like the instructions tuned toward a particular task (adding an API, adding a nuget-packages project, or running the whole solution locally), tell me which area to expand and I'll iterate.
