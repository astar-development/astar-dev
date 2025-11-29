using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Store;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Serialization.Json;

namespace AStar.Dev.OneDrive.Client.Tests.Unit.Fakes
{
    // Minimal test-only IRequestAdapter implementation that returns canned responses
    internal sealed class FakeRequestAdapter : IRequestAdapter
    {
        private readonly Func<RequestInformation, Type, CancellationToken, Task<object?>> _fallbackResponder;
        private readonly List<(Func<RequestInformation, bool> Matcher, Func<RequestInformation, Type, CancellationToken, Task<object?>> Responder)> _responders = new();
        public string? BaseUrl { get; set; } = "https://graph.microsoft.com";

        public FakeRequestAdapter(Func<RequestInformation, Type, CancellationToken, Task<object?>> fallbackResponder)
        {
            _fallbackResponder = fallbackResponder ?? throw new ArgumentNullException(nameof(fallbackResponder));
            PathParameters = new Dictionary<string, object?>();
        }

        public FakeRequestAdapter()
        {
            PathParameters = new Dictionary<string, object?>();
            _fallbackResponder = (req, t, ct) => Task.FromResult<object?>(null);
        }

        public IDictionary<string, object?> PathParameters { get; }

        public IRequestOption[] DefaultRequestOptions => Array.Empty<IRequestOption>();

        public ISerializationWriterFactory SerializationWriterFactory { get; set; } = new JsonSerializationWriterFactory();

        public IParseNodeFactory CollectionParserFactory { get; set; } = new JsonParseNodeFactory();

        public Task<T?> SendAsync<T>(RequestInformation requestInfo, ParsableFactory<T> factory, CancellationToken cancellationToken = default)
            where T : IParsable
        {
            return SendInternalAsync<T>(requestInfo, cancellationToken);
        }

        public Task<object?> SendAsync(RequestInformation requestInfo, Type responseType, CancellationToken cancellationToken = default)
        {
            // Try endpoint-specific responders first
            foreach ((Func<RequestInformation, bool> matcher, Func<RequestInformation, Type, CancellationToken, Task<object?>> responder) in _responders)
            {
                try
                {
                    if (matcher(requestInfo))
                        return responder(requestInfo, responseType, cancellationToken);
                }
                catch
                {
                    // if a matcher/responder throws, bubble up the exception to the caller
                    throw;
                }
            }
            return _fallbackResponder(requestInfo, responseType, cancellationToken);
        }

        // Register a responder targeting requests that match the provided predicate
        public void RegisterResponder(Func<RequestInformation, bool> matcher, Func<RequestInformation, Type, CancellationToken, Task<object?>> responder)
        {
            if (matcher == null) throw new ArgumentNullException(nameof(matcher));
            if (responder == null) throw new ArgumentNullException(nameof(responder));
            _responders.Add((matcher, responder));
        }

        public Task<T?> SendNoContentAsync<T>(RequestInformation requestInfo, CancellationToken cancellationToken = default)
            where T : IParsable
        {
            return SendInternalAsync<T>(requestInfo, cancellationToken);
        }

        public Task<T?> SendPrimitiveAsync<T>(RequestInformation requestInfo, CancellationToken cancellationToken = default)
        {
            return SendInternalAsync<T>(requestInfo, cancellationToken);
        }

        public Task<IEnumerable<T>?> SendCollectionAsync<T>(RequestInformation requestInfo, ParsableFactory<T> factory, CancellationToken cancellationToken = default)
            where T : IParsable
        {
            return SendInternalCollectionAsync<T>(requestInfo, cancellationToken);
        }

        public Task<IEnumerable<T>?> SendPrimitiveCollectionAsync<T>(RequestInformation requestInfo, CancellationToken cancellationToken = default)
        {
            return SendInternalCollectionAsync<T>(requestInfo, cancellationToken);
        }

        private async Task<T?> SendInternalAsync<T>(RequestInformation requestInfo, CancellationToken cancellationToken)
        {
            var obj = await InvokeResponder(requestInfo, typeof(T), cancellationToken).ConfigureAwait(false);
            if (obj == null) return default;
            return (T?)obj;
        }

        private async Task<IEnumerable<T>?> SendInternalCollectionAsync<T>(RequestInformation requestInfo, CancellationToken cancellationToken)
        {
            var obj = await InvokeResponder(requestInfo, typeof(IEnumerable<T>), cancellationToken).ConfigureAwait(false);
            if (obj == null) return default;
            return (IEnumerable<T>?)obj;
        }

        private Task<object?> InvokeResponder(RequestInformation requestInfo, Type responseType, CancellationToken cancellationToken)
        {
            // Try endpoint-specific responders first
            foreach ((Func<RequestInformation, bool> matcher, Func<RequestInformation, Type, CancellationToken, Task<object?>> responder) in _responders)
                if (matcher(requestInfo)) return responder(requestInfo, responseType, cancellationToken);

            return _fallbackResponder(requestInfo, responseType, cancellationToken);
        }

        public Task<string> GetSerializationContentTypeAsync() => Task.FromResult("application/json");

        public Task<IDictionary<string, ParsableFactory<IParsable>>> GetRootParseNodeFactory(CancellationToken cancellationToken = default)
            => Task.FromResult<IDictionary<string, ParsableFactory<IParsable>>>(new Dictionary<string, ParsableFactory<IParsable>>());

        public void EnableBackingStore(IBackingStoreFactory backingStoreFactory)
        {
            // No-op for tests: GraphServiceClient calls this during construction.
        }

        public Task<T?> ConvertToNativeRequestAsync<T>(RequestInformation requestInfo, CancellationToken cancellationToken = default)
            => Task.FromResult<T?>(default);

        // Other members not used in tests: throw if called to surface mistakes.
        public Task<RequestInformation> CreateRequestInformationAsync(RequestInformation requestInfo) => throw new NotImplementedException();
        public Task<Stream> SendStreamAsync(RequestInformation requestInfo, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<T?> SendAsync<T>(RequestInformation requestInfo, ParsableFactory<T> factory, IDictionary<string, ParsableFactory<IParsable>>? additionalParsers, CancellationToken cancellationToken = default)
            where T : IParsable
            => SendAsync(requestInfo, factory, cancellationToken);

        public Task<T?> SendAsync<T>(RequestInformation requestInfo, ParsableFactory<T> factory, Dictionary<string, ParsableFactory<IParsable>>? additionalParsers, CancellationToken cancellationToken = default)
            where T : IParsable
            => SendAsync(requestInfo, factory, cancellationToken);
        public Task<object?> SendAsync(RequestInformation requestInfo, Type responseType, IDictionary<string, ParsableFactory<IParsable>>? additionalParsers, CancellationToken cancellationToken = default) => SendAsync(requestInfo, responseType, cancellationToken);

        public Task SendNoContentAsync(RequestInformation requestInfo, IDictionary<string, ParsableFactory<IParsable>>? additionalParsers = default, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task SendNoContentAsync(RequestInformation requestInfo, Dictionary<string, ParsableFactory<IParsable>>? additionalParsers, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<T?> SendPrimitiveAsync<T>(RequestInformation requestInfo, IDictionary<string, ParsableFactory<IParsable>>? additionalParsers, CancellationToken cancellationToken = default)
            => SendPrimitiveAsync<T>(requestInfo, cancellationToken);

        public Task<T?> SendPrimitiveAsync<T>(RequestInformation requestInfo, Dictionary<string, ParsableFactory<IParsable>>? additionalParsers, CancellationToken cancellationToken = default)
            => SendPrimitiveAsync<T>(requestInfo, cancellationToken);

        public Task<IEnumerable<T>?> SendPrimitiveCollectionAsync<T>(RequestInformation requestInfo, IDictionary<string, ParsableFactory<IParsable>>? additionalParsers, CancellationToken cancellationToken = default)
            => SendPrimitiveCollectionAsync<T>(requestInfo, cancellationToken);

        public Task<IEnumerable<T>?> SendPrimitiveCollectionAsync<T>(RequestInformation requestInfo, Dictionary<string, ParsableFactory<IParsable>>? additionalParsers, CancellationToken cancellationToken = default)
            => SendPrimitiveCollectionAsync<T>(requestInfo, cancellationToken);

        public Task<IEnumerable<T>?> SendCollectionAsync<T>(RequestInformation requestInfo, ParsableFactory<T> factory, IDictionary<string, ParsableFactory<IParsable>>? additionalParsers, CancellationToken cancellationToken = default)
            where T : IParsable
            => SendCollectionAsync<T>(requestInfo, factory, cancellationToken);

        public Task<IEnumerable<T>?> SendCollectionAsync<T>(RequestInformation requestInfo, ParsableFactory<T> factory, Dictionary<string, ParsableFactory<IParsable>>? additionalParsers, CancellationToken cancellationToken = default)
            where T : IParsable
            => SendCollectionAsync<T>(requestInfo, factory, cancellationToken);
    }

    // Use Kiota's JSON parser/writer factories (from Microsoft.Kiota.Serialization.Json)
}
