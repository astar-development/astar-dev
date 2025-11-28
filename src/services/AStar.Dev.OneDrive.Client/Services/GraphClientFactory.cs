using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Serialization.Json;
using Microsoft.Kiota.Serialization.Text;
using Microsoft.Kiota.Serialization.Multipart;
using Microsoft.Kiota.Serialization.Form;
using Microsoft.Kiota.Serialization;
using Microsoft.Kiota.Http.HttpClientLibrary;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions.Serialization;

public static class GraphClientFactory
{
    public static GraphServiceClient CreateGraphClient(
        IAuthenticationProvider authProvider,
        TimeSpan? timeout = null,
        Action<string>? logAction = null)
    {
        // Build HttpClient with custom DelegatingHandler
        var loggingHandler = new LoggingHandler(logAction)
        {
            InnerHandler = new HttpClientHandler()
        };

        var httpClient = new HttpClient(loggingHandler)
        {
            Timeout = timeout ?? TimeSpan.FromMinutes(5)
        };

        // Use default Kiota factories
        var parseNodeFactory = ParseNodeFactoryRegistry.DefaultInstance;
        var serializationWriterFactory = SerializationWriterFactoryRegistry.DefaultInstance;

        // Construct adapter with auth + factories + custom HttpClient
        var adapter = new HttpClientRequestAdapter(
            authProvider,
            parseNodeFactory,
            serializationWriterFactory,
            httpClient);

        return new GraphServiceClient(adapter);
    }

    private class LoggingHandler : DelegatingHandler
    {
        private readonly Action<string>? _logAction;

        public LoggingHandler(Action<string>? logAction = null)
        {
            _logAction = logAction;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            _logAction?.Invoke($"➡️ {request.Method} {request.RequestUri}");

            var response = await base.SendAsync(request, cancellationToken);

            _logAction?.Invoke($"⬅️ {(int)response.StatusCode} {response.ReasonPhrase}");

            if (response.Headers.RetryAfter != null)
            {
                _logAction?.Invoke($"⏳ Retry-After: {response.Headers.RetryAfter.Delta?.TotalSeconds}s");
            }

            return response;
        }
    }
}
