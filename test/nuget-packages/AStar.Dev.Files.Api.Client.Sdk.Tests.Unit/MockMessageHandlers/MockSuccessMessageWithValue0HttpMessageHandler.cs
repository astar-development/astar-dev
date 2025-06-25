using System.Net;

namespace AStar.Dev.Files.Api.Client.Sdk.MockMessageHandlers;

public sealed class MockSuccessMessageWithValue0HttpMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                           CancellationToken  cancellationToken) =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("0"), });
}
