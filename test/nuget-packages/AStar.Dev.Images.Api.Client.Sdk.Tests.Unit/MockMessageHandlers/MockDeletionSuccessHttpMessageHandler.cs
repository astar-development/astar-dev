using System.Net;

namespace AStar.Dev.Images.Api.Client.Sdk.MockMessageHandlers;

public sealed class MockDeletionSuccessHttpMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                           CancellationToken  cancellationToken) =>
        Task.FromResult(
                        new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Marked for deletion."), });
}
