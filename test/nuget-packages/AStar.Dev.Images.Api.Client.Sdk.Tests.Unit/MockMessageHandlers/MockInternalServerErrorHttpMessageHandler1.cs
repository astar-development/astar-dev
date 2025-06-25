using System.Net;

namespace AStar.Dev.Images.Api.Client.Sdk.MockMessageHandlers;

public sealed class MockInternalServerErrorHttpMessageHandler(string errorMessage) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                           CancellationToken  cancellationToken) =>
        Task.FromResult(
                        new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(errorMessage), });
}
