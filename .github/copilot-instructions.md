# Copilot instructions for astar-dev

## Primary repo rules

- This repository targets .NET 10 across all projects; do not introduce net8.0, net9.0, or mixed-targeting unless the task explicitly requires it.
- Keep app code under [apps](../apps) and reusable/package code under [nuget-packages](../nuget-packages).
- Prefer minimal, focused changes over broad refactors.
- Keep public APIs stable unless the task specifically changes them.
- Prefer modern C#/.NET 10 idioms such as nullable reference types, implicit usings, file-scoped namespaces, and straightforward DI.

## Testing rules

- VS Code Test Explorer is the authoritative source of truth for whether tests are discovered in this repo.
- Do not say “no tests discovered,” “tests are not running,” or “no tests were found” based only on a solution-level `dotnet test` invocation unless that exact command is confirmed to match the repo’s test runner configuration.
- If a solution-level or project-level test command reports `Zero tests ran`, treat that as a possible runner/configuration mismatch rather than proof the repo has no tests.
- Prefer the project’s actual VS Code test discovery or the exact project-level command when validating a change.
- Keep xUnit v3 and Microsoft.Testing.Platform usage consistent unless the task explicitly requires otherwise.

## Validation workflow

- Restore/build with the .NET CLI when needed.
- For focused validation, run the smallest project-level test command that checks the changed area.
- When changing behavior, update the relevant tests in the same project area.

## Repo references

- [AStarDev.slnx](../AStarDev.slnx)
- [README.md](../README.md)
- [local-setup.md](../local-setup.md)
- [global.json](../global.json)
