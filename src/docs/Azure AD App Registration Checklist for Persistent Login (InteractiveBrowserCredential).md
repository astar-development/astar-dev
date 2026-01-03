
✅ Azure AD App Registration Checklist for Persistent Login (InteractiveBrowserCredential)
App Registration (Azure Portal)

    [ ] Register the app in Azure AD (App registrations → New registration).

    [ ] Redirect URI: Add http://localhost under Authentication → Platform configurations → Mobile and desktop applications.

    [ ] Tokens issued: Under Authentication → Advanced settings, check:

        ✔️ Access tokens

        ✔️ ID tokens

    [ ] Public client flows: Enable Allow public client flows (needed for InteractiveBrowserCredential).

    [ ] API permissions:

        ✔️ User.Read

        ✔️ Files.ReadWrite.All

        ✔️ offline_access (must be admin‑consented).

    [ ] Grant admin consent for all permissions.

Client Code (Azure.Identity)

    [ ] Use InteractiveBrowserCredentialOptions with:

        ClientId = your app’s ClientId

        TenantId = your tenant

        RedirectUri = http://localhost

    [ ] Enable persistent caching:
    csharp

    options.TokenCachePersistenceOptions = new TokenCachePersistenceOptions
    {
        Name = "AStarOneDriveToken", // stable name
        UnsafeAllowUnencryptedStorage = false // true only for debugging
    };

    [ ] Request scopes including offline_access when building the Graph client.

    [ ] Keep cache name stable — don’t rotate unless you want to force sign‑out.

Environment (Linux Mint)

    [ ] Ensure libsecret and gnome‑keyring are installed:
    bash

    sudo apt-get install libsecret-1-0 gnome-keyring

    [ ] Verify the keyring service is running (ps aux | grep gnome-keyring).

    [ ] If cache file remains 0 bytes:

        Temporarily set UnsafeAllowUnencryptedStorage = true to confirm persistence works.

        If that succeeds, the issue is with secure store integration.

Verification

    [ ] After first login, check cache file (e.g. ~/.local/share/.IdentityService/AStarOneDriveToken.cae) is non‑zero size.

    [ ] On subsequent runs, app should reuse cached token silently — no interactive prompt.

    [ ] Only a hard sign‑out (rotating cache name or clearing file) should force re‑login.

This checklist covers both the Azure portal setup and the client‑side credential configuration. If all boxes are ticked, “Remember Me” should work reliably across sessions.

## Troubleshooting

troubleshooting section:

    If you see a .nocae file instead of .cae, Azure.Identity has fallen back to unencrypted storage. Check that the file grows beyond 0 bytes after login. If it doesn’t, verify app registration settings (redirect URI, tokens issued, public client flows, offline_access permission).
