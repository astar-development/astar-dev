
✅ MSAL Persistent Login Checklist (Desktop App with Graph SDK)
1. Azure AD App Registration

    [ ] Register app in Entra ID (App registrations → New registration).

    [ ] Supported account types: Choose Azure AD + personal Microsoft accounts if you need both.

    [ ] Redirect URI: Add http://localhost under Mobile and desktop applications.

    [ ] Authentication settings:

        ✔️ Enable Access tokens

        ✔️ Enable ID tokens

        ✔️ Allow public client flows

    [ ] API permissions:

        ✔️ User.Read

        ✔️ Files.ReadWrite.All

        ✔️ offline_access

    [ ] Grant admin consent for all permissions.

2. MSAL Client Setup

    [ ] Use PublicClientApplicationBuilder:
    csharp

_pca = PublicClientApplicationBuilder.Create(clientId)
    .WithTenantId(tenantId)
    .WithRedirectUri("http://localhost")
    .Build();

[ ] Define scopes:
csharp

    string[] scopes = { "User.Read", "Files.ReadWrite.All", "offline_access" };

3. Token Cache Persistence

    [ ] Implement cache serialization:
    csharp

    TokenCacheHelper.EnableSerialization(_pca.UserTokenCache, "AStarOneDriveTokenCache.bin");

    [ ] EnableSerialization wires SetBeforeAccess / SetAfterAccess to read/write the cache file.

    [ ] Keep cache file name stable (only rotate if forcing sign‑out).

4. Acquire Tokens

    [ ] On startup, try silent login:
    csharp

    var accounts = _pca.GetAccountsAsync().Result;
    var account = accounts.FirstOrDefault();

    AuthenticationResult result;
    if (account != null)
        result = _pca.AcquireTokenSilent(scopes, account).ExecuteAsync().Result;
    else
        result = _pca.AcquireTokenInteractive(scopes).ExecuteAsync().Result;

    [ ] On first run → interactive login.

    [ ] On subsequent runs → silent login succeeds using cached refresh token.

5. Graph Client Wiring

    [ ] Implement IAuthenticationProvider that injects the MSAL token:
    csharp

request.Headers.Add("Authorization", $"Bearer {result.AccessToken}");

[ ] Create Graph client:
csharp

    _client = new GraphServiceClient(provider);

6. Verification

    [ ] After first login, check cache file (e.g. ~/.local/share/.IdentityService/AStarOneDriveTokenCache.bin) is non‑zero size.

    [ ] On second run, app should reuse cached token silently — no prompt.

    [ ] Only a hard sign‑out (rotating cache name or deleting file) should force re‑login.

7. Troubleshooting

    [ ] Cache file stays 0 bytes → App registration not issuing refresh tokens. Re‑check offline_access, public client flows, and redirect URI.

    [ ] Always prompted → Silent acquisition failing. Ensure cache file is stable and not rotated.

    [ ] Linux Mint → Requires libsecret + gnome-keyring for secure store. If unavailable, MSAL file‑based cache works as fallback.

This checklist captures the full recipe: Azure setup, MSAL wiring, cache persistence, and verification.
