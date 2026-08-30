# How to use

## Before running

```bash
chmod +x configure-gh-repo.sh
```

Authenticate with GitHub CLI. The account must be able to administer the repository and grant the existing `AStar` organization team repository access:

```bash
gh auth login
```

The script requires `gh` and `jq` on your `PATH`. It does not create the `AStar` team or change its membership.

## How to run

Run either:

- Inside a cloned repo:

```bash
    ./configure-gh-repo.sh
```

- Or explicitly:

```bash
    ./configure-gh-repo.sh owner/repo
```

The script enables auto-merge and branch deletion after merge, grants `AStar` the `maintain` role, and creates or updates the active `AStar Main Branch Protection` Ruleset. The Ruleset protects the default branch, prevents deletion and force-pushes, requires signed commits, requires the `Full solution build & test` GitHub Actions check, requires one PR approval from the `AStar` team, requires reapproval after a push and an extra approval for unattributed changes, requires resolved threads, and permits squash merging only.

It refuses to apply a Ruleset when legacy classic branch protection exists on `main`; remove that protection first so GitHub does not enforce two overlapping policies.

### Dry Run

Whichever approach you decide to use for running this script, you can add:

```bash
./configure-gh-repo.sh --dry-run owner/repo
```

### Audit

Compare the repository's current Ruleset against this script's policy without changing GitHub settings:

```bash
./configure-gh-repo.sh --audit owner/repo
```
