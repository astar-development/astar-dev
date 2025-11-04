✅ Blazor + Entra ID App Role Access Control Checklist

🔧 App Registration Setup

- App role defined in the App Registration manifest (e.g., "Admin").
- App Registration is treated as an API:
  - Application ID URI is set (e.g., api://{client-id}).
  - At least one scope is defined under “Expose an API”.
  - accessTokenAcceptedVersion is set to 2 in the manifest.



👥 Role Assignment

- Users or groups are assigned to the app role via Enterprise Applications → Users and groups.
- Role assignment is done on the service principal of the API App Registration.

🔐 Token Request Configuration

- Blazor app requests token for the API scope:

  - Example: api://{API-App-ClientId}/.default


- MSAL or Microsoft Identity Web is configured to request scopes correctly.

🧾 Token Validation

- Token contains the roles claim:

- Use https://jwt.ms to inspect the token.
- Confirm "roles": ["Admin"] is present.



🧰 Blazor Authorization Setup

- Authentication is configured using AddMicrosoftIdentityWebApp.
- Authorization policy is defined:

```C#
options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));Show more lines
```

 Razor components/pages are protected:

```C#
@attribute [Authorize(Roles = "Admin")]
```

🧪 Testing

- Test with a user assigned to the role.
- Confirm access is granted to protected pages.
- Test with a user not assigned to the role.
- Confirm access is denied or redirected.

