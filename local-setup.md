# Local NuGet Updates

Run this locally - once per setup:

```text
dotnet nuget add source \
  --username YOUR_GITHUB_USERNAME \
  --password YOUR_PAT_TOKEN \
  --store-password-in-clear-text \
  --name github \
  "https://nuget.pkg.github.com/your-org/index.json"
```

The PAT needs only read:packages scope for restore. write:packages is only needed if you're pushing manually rather than via CI.

# Git Hooks

Enable the repository's tracked Git hooks after cloning:

```bash
git config core.hooksPath .githooks
```

The pre-commit hook runs `graphify update .` so the local knowledge graph stays current with source changes. The `graphify` command must be installed and available on your `PATH`; a generation failure blocks the commit.

For an exceptional commit where Graphify cannot run, bypass that update explicitly:

```bash
SKIP_GRAPHIFY_UPDATE=1 git commit -m "Your commit message"
```

