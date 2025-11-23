using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;

namespace AStar.Dev.OneDrive.Client.Tests.Unit.Fakes
{
    // Minimal test-only IRequestAdapter implementation that returns canned responses
    internal class FakeRequestAdapter : IRequestAdapter
    {
        private readonly Func<RequestInformation, Type, CancellationToken, Task<object?>> _responder;

        public FakeRequestAdapter(Func<RequestInformation, Type, CancellationToken, Task<object?>> responder)
        {
            _responder = responder ?? throw new ArgumentNullException(nameof(responder));
            PathParameters = new Dictionary<string, object?>();
        }

        public IDictionary<string, object?> PathParameters { get; }

        public IRequestOption[] DefaultRequestOptions => Array.Empty<IRequestOption>();

        public ISerializationWriterFactory SerializationWriterFactory { get; set; } = new ParsableSerializerFactory();

        public IParseNodeFactory CollectionParserFactory { get; set; } = new ParsableFactory();

        public Task<T?> SendAsync<T>(RequestInformation requestInfo, ParsableFactory<T> factory, CancellationToken cancellationToken = default)
        {
            return SendInternalAsync<T>(requestInfo, cancellationToken);
        }

        public Task<object?> SendAsync(RequestInformation requestInfo, Type responseType, CancellationToken cancellationToken = default)
        {
            return _responder(requestInfo, responseType, cancellationToken);
        }

        public Task<T?> SendNoContentAsync<T>(RequestInformation requestInfo, CancellationToken cancellationToken = default)
        {
            return SendInternalAsync<T>(requestInfo, cancellationToken);
        }

        public Task<T?> SendPrimitiveAsync<T>(RequestInformation requestInfo, CancellationToken cancellationToken = default)
        {
            return SendInternalAsync<T>(requestInfo, cancellationToken);
        }

        private async Task<T?> SendInternalAsync<T>(RequestInformation requestInfo, CancellationToken cancellationToken)
        {
            var obj = await _responder(requestInfo, typeof(T), cancellationToken).ConfigureAwait(false);
            if (obj == null) return default;
            return (T?)obj;
        }

        public Task<string> GetSerializationContentTypeAsync() => Task.FromResult("application/json");

        public Task<IDictionary<string, ParsableFactory<IParsable>>> GetRootParseNodeFactory(CancellationToken cancellationToken = default)
            => Task.FromResult<IDictionary<string, ParsableFactory<IParsable>>>(new Dictionary<string, ParsableFactory<IParsable>>());

        // Other members not used in tests: throw if called to surface mistakes.
        public Task<RequestInformation> CreateRequestInformationAsync(RequestInformation requestInfo) => throw new NotImplementedException();
        public Task<Stream> SendStreamAsync(RequestInformation requestInfo, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<T?> SendAsync<T>(RequestInformation requestInfo, ParsableFactory<T> factory, IDictionary<string, ParsableFactory<IParsable>> additionalParsers, CancellationToken cancellationToken = default) => SendAsync(requestInfo, factory, cancellationToken);
        public Task<object?> SendAsync(RequestInformation requestInfo, Type responseType, IDictionary<string, ParsableFactory<IParsable>> additionalParsers, CancellationToken cancellationToken = default) => SendAsync(requestInfo, responseType, cancellationToken);
    }

    // Very small dummy factory types to satisfy interface properties in tests
    internal class ParsableFactory : IParseNodeFactory
    {
        public IParseNode GetRootParseNode(string contentType, Stream content) => throw new NotImplementedException();
        public IParseNode GetRootParseNode(string contentType, string content) => throw new NotImplementedException();
    }

    internal class ParsableSerializerFactory : ISerializationWriterFactory
    {
        public ISerializationWriter GetSerializationWriter(string contentType) => throw new NotImplementedException();
    }
}
