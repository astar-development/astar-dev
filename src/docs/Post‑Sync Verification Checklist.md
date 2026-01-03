
✅ Post‑Sync Verification Checklist
1. Database Integrity

    [ ] Confirm every LocalDriveItem row has:

        Id = Graph GUID (short opaque string, not path).

        PathId = full path string (/drives/.../root:/...).

        Name, IsFolder, ETag, LastModifiedUtc populated.

    [ ] Spot‑check a few rows: GUIDs should match Graph API responses if queried directly.

    [ ] Ensure no duplicate GUIDs exist in the DB.

2. File System Consistency

    [ ] Compare DB PathId hierarchy with actual local folder structure.

        Example: DB says /root:/Documents/Report.docx → check Documents/Report.docx exists locally.

    [ ] Verify folder creation: every IsFolder = true entry has a corresponding directory.

    [ ] Confirm file counts: number of non‑folder DB rows ≈ number of files on disk.

3. Metrics & Progress

    [ ] Review sync logs:

        Percentage complete increments smoothly.

        ETA reported with confidence (“stable” vs “volatile”).

        Errors count visible if any failures occurred.

    [ ] Ensure throttled updates (every N files or X ms) kept logs readable, not spammy.

4. Error Handling

    [ ] Check if any “Item not found” or “Stream is null” errors were logged.

    [ ] Confirm those errors incremented the Errors counter in metrics.

    [ ] If errors exist, note the GUIDs/paths for later retry.

5. Retry & Stability

    [ ] Verify retry logic kicked in for transient HTTP/2 resets.

    [ ] Ensure retries didn’t duplicate files (no double‑writes).

    [ ] Confirm concurrency (set to 2) kept the sync stable.

6. Observability

    [ ] At the end, confirm per‑folder breakdown was printed:

        Each folder shows file count + MB total.

    [ ] Spot‑check a large folder (e.g. Pictures, Videos) to ensure stats match disk usage.

🎯 Outcome

When this checklist passes:

    DB and filesystem are in sync.

    GUIDs are consistently used for Graph calls.

    Progress reporting is smooth, with ETA confidence and error counts.

    Any anomalies are logged and traceable.