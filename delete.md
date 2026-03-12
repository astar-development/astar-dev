gh api repos/astar-development/astar-dev-onedrive-sync-client/rules/branches/main

gh api repos/astar-development/astar-dev-onedrive-sync-client/rulesets --jq '.[] | {id,name,rules}'


# GitHub CLI api
# https://cli.github.com/manual/gh_api

gh api \
  --method POST \
  -H "Accept: application/vnd.github+json" \
  -H "X-GitHub-Api-Version: 2022-11-28" \
  /repos/OWNER/REPO/rulesets \
  --input - <<< '{
  "name": "super cool ruleset",
  "target": "branch",
  "enforcement": "active",
  "bypass_actors": [
    {
      "actor_id": 234,
      "actor_type": "Team",
      "bypass_mode": "always"
    }
  ],
  "conditions": {
    "ref_name": {
      "include": [
        "refs/heads/main"
      ],
      "exclude": [
      ]
    }
  },
  "rules": [
    {
      "type": "commit_author_email_pattern",
      "parameters": {
        "operator": "contains",
        "pattern": "github"
      }
    }
  ]
}'