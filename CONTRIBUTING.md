
# Contributing Guidelines

Thank you for contributing to the monorepo!  
This repository is optimized for clarity, consistency, and automation.

## 🧱 Coding Standards

All code must comply with:

- `.editorconfig`
- Central analyzers enabled in `Directory.Build.props`
- Nullable reference types (`nullable enable`)
- File-scoped namespaces
- Consistent naming conventions

Formatting and imports are automatically handled by VS Code on save.

---

## 🧪 Tests

Every library should have a matching test project:

```
libs/MyLibrary/
tests/MyLibrary.Tests/
```

Tests must:

- Use **xUnit**
- Use **FluentAssertions**
- Have meaningful assertion messages
- Cover edge cases

---

## 📦 NuGet Packaging

All packable libraries live under:

```
/libs
```

Do **not** manually specify version numbers in `.csproj`.  
These are provided automatically by **Nerdbank.GitVersioning**.

---

## 🌳 Branching Model

Simple and predictable:

- `main` — stable, always releasable
- Feature branches — anywhere else

All PRs must pass:

- Build
- Tests
- Analyzers

---

## 🚀 Releases

Use the Release workflow:

1. Navigate to **Actions → Create Release**
2. Enter version (e.g., `1.4.0`)
3. Workflow:
   - creates tag  
   - pushes it  
   - generates release notes  
   - creates release  
   - triggers NuGet publishing  

---

## 🛠 Adding New Projects

Use the CLI script in `/tools/new-project.ps1`.  
It automatically:

- Creates folder structure  
- Generates `.csproj`, `Program.cs` (if app), and test scaffolding  
- Registers everything in the repo  
