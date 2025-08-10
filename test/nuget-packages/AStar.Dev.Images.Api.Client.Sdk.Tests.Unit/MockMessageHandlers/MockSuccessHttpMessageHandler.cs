using System.Net;
using System.Text.Json;
using AStar.Dev.Api.HealthChecks;

namespace AStar.Dev.Images.Api.Client.Sdk.MockMessageHandlers;

public sealed class MockSuccessHttpMessageHandler(string responseRequired) : HttpMessageHandler
{
    private const int Counter = 1234;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                           CancellationToken  cancellationToken)
    {
        HttpContent content;

#pragma warning disable IDE0045 // Convert to conditional expression
        if(responseRequired == "Count")
        {
            content = new StringContent(Counter.ToString());
        }
        else if(responseRequired == "CountDuplicates")
        {
            content = new StringContent(Counter.ToString());
        }
        else if(responseRequired == "Health")
        {
            content = new StringContent(JsonSerializer.Serialize(new HealthStatusResponse { Status = "OK" }));
        }
        else if(responseRequired == "Image")
        {
            var bytes = new byte[1024];
            content = new StreamContent(new MemoryStream(bytes));
        }
        else if(responseRequired == "ImageMissing")
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound) { Content = null! });
        }
        else
        {
            content = new StringContent(JsonSerializer.Serialize(new HealthStatusResponse { Status = "OK" }));
        }
#pragma warning restore IDE0045 // Convert to conditional expression

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content });
    }
}
