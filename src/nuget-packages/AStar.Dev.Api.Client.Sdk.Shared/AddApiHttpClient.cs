using System.Net;
using System.Net.Mime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Polly;

namespace AStar.Dev.Api.Client.Sdk.Shared;

/// <summary>
/// </summary>
public static class AddApiHttpClient
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="scopes"></param>
    /// <typeparam name="TApiClient"></typeparam>
    /// <typeparam name="TApiConfiguration"></typeparam>
    public static void AddApiClient<TApiClient, TApiConfiguration>(this IServiceCollection services, string[] scopes)
        where TApiClient : class
        where TApiConfiguration : class, IApiConfiguration
    {
        var optionsScopes = string.Join(" ", scopes);

        _ = services.AddHttpClient<TApiClient>()
                    .AddMicrosoftIdentityUserAuthenticationHandler(
                                                                   nameof(TApiClient),
                                                                   options => { options.Scopes = optionsScopes; })
                    .ConfigureHttpClient((serviceProvider, client) =>
                                         {
                                             client.BaseAddress = serviceProvider.GetRequiredService<IOptions<TApiConfiguration>>().Value
                                                                                 .BaseUrl;

                                             client.DefaultRequestHeaders.Accept.Add(new(MediaTypeNames.Application.Json));
                                         })
                    .AddResilienceHandler($"{nameof(TApiClient)}Handler",
                                          b => b.AddFallback(new()
                                                             {
                                                                 FallbackAction = _ => Outcome.FromResultAsValueTask(
                                                                                                                     new
                                                                                                                         HttpResponseMessage(HttpStatusCode
                                                                                                                                                .ServiceUnavailable)),
                                                             })
                                                .AddConcurrencyLimiter(100)
                                                .AddRetry(new HttpRetryStrategyOptions())
                                                .AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions())
                                                .AddTimeout(new HttpTimeoutStrategyOptions()));
    }
}
