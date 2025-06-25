using System.Net;
using System.Text.Json;
using AStar.Dev.Api.HealthChecks;

namespace AStar.Dev.Images.Api.Client.Sdk.MockMessageHandlers;

public sealed class MockSuccessHttpMessageHandler(string responseRequired) : HttpMessageHandler
{
    public int Counter { get; set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                           CancellationToken  cancellationToken)
    {
        HttpContent content;

        #pragma warning disable IDE0045 // Convert to conditional expression
        if (responseRequired == "Count")
        {
            content = new StringContent(Counter.ToString());
        }
        else if (responseRequired == "CountDuplicates")
        {
            content = new StringContent(Counter.ToString());
        }
        else if (responseRequired == "Health")
        {
            content = new StringContent(JsonSerializer.Serialize(new HealthStatusResponse { Status = "OK", }));
        }
        else
        {
            content = new StringContent(JsonSerializer.Serialize(new HealthStatusResponse { Status = "OK", }));
        }
        #pragma warning restore IDE0045 // Convert to conditional expression

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content, });
    }
}
