using System.Net;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Polly;
using Polly.Retry;
namespace AStar.Dev.OneDrive.Client;

public static class GraphClientFactory
{
    public static GraphServiceClient CreateGraphClient(
        IAuthenticationProvider authProvider,
        TimeSpan? timeout = null,
        Action<string>? logAction = null)
    {
        // Build exponential backoff with jitter manually
        AsyncRetryPolicy<HttpResponseMessage> retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(msg => (int)msg.StatusCode >= 500 || msg.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: attempt =>
                {
                    // Exponential backoff: 2^attempt seconds
                    var baseDelay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    // Add jitter up to 250ms
                    var jitter = TimeSpan.FromMilliseconds(new Random().Next(0, 250));
                    return baseDelay + jitter;
                },
                onRetry: (result, ts, retryCount, ctx) => logAction?.Invoke($"🔄 Retry {retryCount} after {ts.TotalSeconds:F1}s due to {result.Exception?.Message ?? result.Result.StatusCode.ToString()}"));

        // Wrap policy into DelegatingHandler
        var pollyHandler = new PolicyHandler(retryPolicy, logAction)
        {
            InnerHandler = new HttpClientHandler()
        };

        var httpClient = new HttpClient(pollyHandler)
        {
            Timeout = timeout ?? TimeSpan.FromMinutes(5)
        };

        // Use default Kiota factories
        ParseNodeFactoryRegistry parseNodeFactory = ParseNodeFactoryRegistry.DefaultInstance;
        SerializationWriterFactoryRegistry serializationWriterFactory = SerializationWriterFactoryRegistry.DefaultInstance;

        // Construct adapter with auth + factories + custom HttpClient
        var adapter = new HttpClientRequestAdapter(
            authProvider,
            parseNodeFactory,
            serializationWriterFactory,
            httpClient);

        return new GraphServiceClient(adapter);
    }

    private sealed class PolicyHandler(IAsyncPolicy<HttpResponseMessage> policy, Action<string>? logAction = null) : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            logAction?.Invoke($"➡️ {request.Method} {request.RequestUri}");
            return policy.ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
        }
    }
}
