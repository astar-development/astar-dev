#!/usr/bin/env bash
set -euo pipefail

RULESET_NAME="AStar Main Branch Protection"
REVIEW_TEAM_SLUG="astar"
REVIEW_TEAM_PERMISSION="maintain"
CI_CHECK_CONTEXT="Full solution build & test"
GITHUB_ACTIONS_INTEGRATION_ID=15368

ENABLE_AUTO_MERGE=true
DELETE_BRANCH_ON_MERGE=true

DRY_RUN=false
AUDIT=false
ERRORS=()

log() { printf '[INFO ] %s\n' "$*" >&2; }
warn() { printf '[WARN ] %s\n' "$*" >&2; }
error() { printf '[ERROR] %s\n' "$*" >&2; }

record_error() {
  ERRORS+=("$1")
  error "$1"
}

require_cmd() {
  command -v "$1" >/dev/null 2>&1 || {
    error "Required command '$1' not found"
    exit 1
  }
}

usage() {
  cat <<'EOF'
Usage: ./configure-gh-repo.sh [--dry-run | --audit] [owner/repo]

Configures repository settings, AStar team access, and the default-branch Ruleset.
  --dry-run  Print intended changes without updating repository settings.
  --audit    Compare the current Ruleset with the configured policy without changing it.
EOF
}

resolve_repo() {
  if [[ $# -eq 1 ]]; then
    printf '%s\n' "$1"
    return
  fi

  gh repo view --json nameWithOwner --jq .nameWithOwner 2>/dev/null || {
    error "Could not determine repository. Pass owner/repo or run inside a repository."
    exit 1
  }
}

send_request() {
  local method=$1
  local endpoint=$2
  local body=$3

  if $DRY_RUN; then
    log "DRY RUN: $method $endpoint"
    printf '%s\n\n' "$body"
    return 0
  fi

  gh api --method "$method" "$endpoint" \
    -H "Accept: application/vnd.github+json" \
    -H "X-GitHub-Api-Version: 2022-11-28" \
    --input - <<<"$body" >/dev/null
}

get_team_id() {
  local organization=$1

  gh api "orgs/$organization/teams/$REVIEW_TEAM_SLUG" \
    -H "X-GitHub-Api-Version: 2022-11-28" \
    --jq .id
}

get_ruleset_id() {
  local repo=$1
  local ruleset_ids

  ruleset_ids=$(gh api "repos/$repo/rulesets" \
    -H "X-GitHub-Api-Version: 2022-11-28" \
    --paginate \
    --jq ".[] | select(.name == \"$RULESET_NAME\") | .id") || return 1

  if [[ $(printf '%s\n' "$ruleset_ids" | sed '/^$/d' | wc -l) -gt 1 ]]; then
    error "More than one Ruleset is named '$RULESET_NAME'"
    return 1
  fi

  printf '%s\n' "$ruleset_ids"
}

build_repo_settings_payload() {
  cat <<EOF
{
  "allow_auto_merge": $ENABLE_AUTO_MERGE,
  "delete_branch_on_merge": $DELETE_BRANCH_ON_MERGE
}
EOF
}

build_ruleset_payload() {
  local team_id=$1

  cat <<EOF
{
  "name": "$RULESET_NAME",
  "target": "branch",
  "enforcement": "active",
  "conditions": {
    "ref_name": {
      "include": ["~DEFAULT_BRANCH"],
      "exclude": []
    }
  },
  "rules": [
    {
      "type": "deletion"
    },
    {
      "type": "non_fast_forward"
    },
    {
      "type": "required_signatures"
    },
    {
      "type": "required_status_checks",
      "parameters": {
        "do_not_enforce_on_create": false,
        "required_status_checks": [
          {
            "context": "$CI_CHECK_CONTEXT",
            "integration_id": $GITHUB_ACTIONS_INTEGRATION_ID
          }
        ],
        "strict_required_status_checks_policy": true
      }
    },
    {
      "type": "pull_request",
      "parameters": {
        "required_approving_review_count": 1,
        "dismiss_stale_reviews_on_push": true,
        "dismissal_restriction": {
          "enabled": false,
          "allowed_actors": []
        },
        "require_code_owner_review": false,
        "require_extra_approval_for_unattributed_changes": true,
        "require_last_push_approval": true,
        "required_review_thread_resolution": true,
        "required_reviewers": [
          {
            "minimum_approvals": 0,
            "file_patterns": ["*"],
            "reviewer": {
              "id": $team_id,
              "type": "Team"
            }
          }
        ],
        "allowed_merge_methods": ["squash"]
      }
    }
  ],
  "bypass_actors": []
}
EOF
}

ensure_no_legacy_branch_protection() {
  local repo=$1

  if gh api "repos/$repo/branches/main/protection" >/dev/null 2>&1; then
    record_error "Legacy branch protection exists on $repo/main. Remove it before applying the Ruleset."
  fi
}

apply_repo_settings() {
  local repo=$1
  local payload
  payload=$(build_repo_settings_payload)

  log "Applying repository settings to $repo"
  send_request PATCH "repos/$repo" "$payload" || record_error "Failed to update repository settings for $repo"
}

apply_team_access() {
  local repo=$1
  local organization=${repo%%/*}
  local payload
  payload=$(printf '{"permission":"%s"}' "$REVIEW_TEAM_PERMISSION")

  log "Granting team '$REVIEW_TEAM_SLUG' $REVIEW_TEAM_PERMISSION access to $repo"
  send_request PUT "orgs/$organization/teams/$REVIEW_TEAM_SLUG/repos/$repo" "$payload" || record_error "Failed to grant team access for $repo"
}

apply_ruleset() {
  local repo=$1
  local team_id=$2
  local ruleset_id
  local payload

  payload=$(build_ruleset_payload "$team_id")
  ruleset_id=$(get_ruleset_id "$repo") || {
    record_error "Failed to query Rulesets for $repo"
    return
  }

  if [[ -n $ruleset_id ]]; then
    log "Updating Ruleset '$RULESET_NAME' on $repo"
    send_request PUT "repos/$repo/rulesets/$ruleset_id" "$payload" || record_error "Failed to update Ruleset for $repo"
  else
    log "Creating Ruleset '$RULESET_NAME' on $repo"
    send_request POST "repos/$repo/rulesets" "$payload" || record_error "Failed to create Ruleset for $repo"
  fi
}

audit_ruleset() {
  local repo=$1
  local team_id=$2
  local ruleset_id
  local expected
  local actual

  ruleset_id=$(get_ruleset_id "$repo") || {
    record_error "Failed to query Rulesets for $repo"
    return
  }

  if [[ -z $ruleset_id ]]; then
    record_error "Ruleset '$RULESET_NAME' is missing from $repo"
    return
  fi

  expected=$(build_ruleset_payload "$team_id" | jq -S .)
  actual=$(gh api "repos/$repo/rulesets/$ruleset_id" \
    -H "X-GitHub-Api-Version: 2022-11-28" | jq -S '{name, target, enforcement, conditions, rules, bypass_actors}') || {
    record_error "Failed to retrieve Ruleset '$RULESET_NAME' from $repo"
    return
  }

  if diff -u <(printf '%s\n' "$expected") <(printf '%s\n' "$actual"); then
    log "Ruleset '$RULESET_NAME' matches the configured policy"
  else
    record_error "Ruleset '$RULESET_NAME' differs from the configured policy"
  fi
}

main() {
  require_cmd gh
  require_cmd jq

  local repo_argument=""
  while (($# > 0)); do
    case $1 in
      --dry-run)
        DRY_RUN=true
        ;;
      --audit)
        AUDIT=true
        ;;
      --help|-h)
        usage
        return
        ;;
      -*)
        error "Unknown option: $1"
        usage
        exit 1
        ;;
      *)
        if [[ -n $repo_argument ]]; then
          error "Only one owner/repo argument is allowed"
          exit 1
        fi
        repo_argument=$1
        ;;
    esac
    shift
  done

  if $DRY_RUN && $AUDIT; then
    error "Choose either --dry-run or --audit"
    exit 1
  fi

  local repo
  local organization
  local team_id
  repo=$(resolve_repo ${repo_argument:+"$repo_argument"})
  organization=${repo%%/*}
  team_id=$(get_team_id "$organization") || {
    error "Could not resolve the '$REVIEW_TEAM_SLUG' team in $organization"
    exit 1
  }

  log "Using repository: $repo"

  if $AUDIT; then
    audit_ruleset "$repo" "$team_id"
  else
    ensure_no_legacy_branch_protection "$repo"
    apply_repo_settings "$repo"
    apply_team_access "$repo"
    apply_ruleset "$repo" "$team_id"
  fi

  if ((${#ERRORS[@]} > 0)); then
    warn "Completed with errors:"
    printf '[WARN ]  - %s\n' "${ERRORS[@]}" >&2
    exit 1
  fi

  log "All operations completed successfully"
}

main "$@"
