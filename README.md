
# Monorepo

A unified, scalable monorepo for .NET applications, websites, tools, and reusable libraries.

This repository hosts:

- **Apps** (desktop, tools)
- **Websites / APIs**
- **Shared libraries** (NuGet packages)
- **Automated builds, versioning, and releases**
- **Full test coverage**

All projects use:
- **.NET 10**
- **Centralized versioning** (Nerdbank.GitVersioning)
- **Central package management**
- **VS Code workspace integration**
- **GitHub Actions CI/CD**

---

## 🏛 Repository Structure

```
repo/
  apps/        # Executable desktop/CLI apps
  web/         # Websites / APIs
  libs/        # NuGet-packaged libraries
  tests/       # Full test suites parallel to projects
  build/       # Versioning, build logic, CI scripts
```

---

## 🚀 Features

### ✔ Centralized Versioning
Using **Nerdbank.GitVersioning**:
- Tags (e.g., `v1.2.0`) control all package versions.
- Pre-release versions generated automatically for branches.

### ✔ Single‑source Package Version Management
Using `Directory.Packages.props`:
- No project contains a version number.
- No package drift across 50+ libraries.

### ✔ Modern Coding Standards
- C# 14  
- Strict analyzers  
- Nullable enabled  
- Automatic formatting & import cleanup  

### ✔ Fully Automated Release Flow
- `workflow_dispatch` → create tag + GitHub Release  
- Tag triggers NuGet publishing for changed libraries only  

### ✔ VS Code First-Class Workspace
- Preconfigured `.code-workspace` file  
- Collapsible structure  
- Clean navigation  

---

## 🔧 Build & Test

To build everything:

```bash
dotnet build
```

To install Nerdbank.GitVersioning:

```bash
dotnet tool install --global nbgv --version 3.9.50
```

To run all tests:

```bash
dotnet test
```

To inspect the monorepo version:

```bash
nbgv get-version
```

---

## 📦 Publishing (NuGet)

Publishing runs automatically when a version tag is pushed:

```bash
git tag v1.3.0
git push origin v1.3.0
```

Only changed libraries under `/libs` will be packed & published.

---

## 🤝 Contributing

See CONTRIBUTING.md

---

## 📜 License

MIT (see LICENSE)
