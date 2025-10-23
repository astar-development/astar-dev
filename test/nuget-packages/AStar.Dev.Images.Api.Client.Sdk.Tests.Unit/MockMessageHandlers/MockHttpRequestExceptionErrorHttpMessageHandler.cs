namespace AStar.Dev.Images.Api.Client.Sdk.Tests.Unit.MockMessageHandlers;

public sealed class MockHttpRequestExceptionErrorHttpMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
        => throw new HttpRequestException();
}
