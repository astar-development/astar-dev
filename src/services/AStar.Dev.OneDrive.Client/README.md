# Avalonia Graph MVVM Sample

This sample is an Avalonia UI app using MVVM that demonstrates signing in with Azure (InteractiveBrowserCredential)
and basic OneDrive operations (list, upload, download) using Microsoft.Graph v5.x.

## Setup

1. Register an app in Azure AD. Add a **Mobile & desktop** redirect URI with value:
   `https://login.microsoftonline.com/common/oauth2/nativeclient`
2. Grant API permissions: `User.Read`, `Files.ReadWrite.All`, `offline_access` (delegated).
3. Copy your **ClientId** and **TenantId** into `appsettings.json`.
4. `dotnet restore` and `dotnet run` (requires .NET 7+ or 8).

## Notes

- This is a minimal sample to get you started. It uses `InteractiveBrowserCredential` and caches tokens via Azure.Identity defaults.
- Adjust scopes in `appsettings.json` as needed.
