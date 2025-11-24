using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.Kiota.Serialization;
using Microsoft.Kiota.Serialization.Json;
using Microsoft.Kiota.Serialization.Text;
using Microsoft.Kiota.Serialization.Multipart;
using Microsoft.Kiota.Serialization.Form;
using Polly;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly.Retry;
using Microsoft.Kiota.Abstractions.Serialization;
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

    private class PolicyHandler : DelegatingHandler
    {
        private readonly IAsyncPolicy<HttpResponseMessage> _policy;
        private readonly Action<string>? _logAction;

        public PolicyHandler(IAsyncPolicy<HttpResponseMessage> policy, Action<string>? logAction = null)
        {
            _policy = policy;
            _logAction = logAction;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logAction?.Invoke($"➡️ {request.Method} {request.RequestUri}");
            return _policy.ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
        }
    }
}
