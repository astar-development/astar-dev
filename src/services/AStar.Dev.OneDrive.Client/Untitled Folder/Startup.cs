using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspnetCore_Changed_Files.Extensions;
using AspnetCore_Changed_Files.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Kiota.Abstractions.Authentication;

namespace AspnetCore_Changed_Files;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        _ = services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        // Define the scopes your app needs
        var initialScopes = new[] { "Files.ReadWrite.All" };

        // Update the authentication configuration
        services
            .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
            .EnableTokenAcquisitionToCallDownstreamApi(initialScopes) // KEEP this, it pre-populates the cache requirement
            .AddDistributedTokenCaches();

        _ = services.AddDistributedMemoryCache(); 
        // Manually register GraphServiceClient
        services.AddScoped<GraphServiceClient>(serviceProvider =>
        {
            ITokenAcquisition tokenAcquisition = serviceProvider.GetRequiredService<ITokenAcquisition>();
                
            // Use the same scopes here
            var authenticationProvider = new BaseBearerTokenAuthenticationProvider(
                new TokenAcquisitionTokenProvider(tokenAcquisition, initialScopes)
            );

            return new GraphServiceClient(authenticationProvider);
        });
        
        services.AddControllersWithViews().AddRazorRuntimeCompilation();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
#pragma warning disable CS0618 // Type or member is obsolete
    public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
#pragma warning restore CS0618 // Type or member is obsolete
    {
        if (env.IsDevelopment())
            _ = app.UseDeveloperExceptionPage();
        else
        {
            _ = app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            _ = app.UseHsts();
        }

        _ = app.UseHttpsRedirection();
        _ = app.UseStaticFiles();
        _ = app.UseCookiePolicy();
        _ = app.UseRouting();
        _ = app.UseAuthentication();
        _ = app.UseAuthorization();

        _ = app.UseEndpoints(endpoints =>
        {
            _ = endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}

public class TokenAcquisitionTokenProvider : IAccessTokenProvider
{
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly string[] _scopes;

    public TokenAcquisitionTokenProvider(ITokenAcquisition tokenAcquisition, string[] scopes)
    {
        _tokenAcquisition = tokenAcquisition;
        _scopes = scopes;
    }

    public async Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = default,
        CancellationToken cancellationToken = default)
        // Ensure we are just passing the scopes.
        // If this throws MsalUiRequiredException, AuthorizeForScopes in the controller should catch it.
        // The loop happens if AuthorizeForScopes redirects, but the redirect doesn't result in a valid token being cached.
        // Try adding the tenant ID if possible, but usually simpler is better.
        => await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);

    public AllowedHostsValidator AllowedHostsValidator { get; } = new AllowedHostsValidator();
}
