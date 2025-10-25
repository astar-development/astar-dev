using System.Net;
using System.Text.Json;
using AStar.Dev.Api.HealthChecks;
using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Files.Api.Client.Sdk.Tests.Unit.MockMessageHandlers;

public sealed class MockSuccessHttpMessageHandler(string responseRequired) : HttpMessageHandler
{
    public int Counter { get; set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        HttpContent content;

#pragma warning disable IDE0045 // Convert to conditional expression
        if(responseRequired == "ListDuplicates")
        {
            content = new StringContent(JsonSerializer.Serialize(new List<DuplicateGroup> { new(), new(), new() }));
        }
        else if(responseRequired == "ListFiles")
        {
            content = new StringContent(
                JsonSerializer.Serialize(new List<FileDetail> { new() { FileName = "does.not.matter.txt" }, new() }));
        }
        else if(responseRequired == "Count")
        {
            content = new StringContent(Counter.ToString());
        }
        else if(responseRequired == "CountDuplicates")
        {
            content = new StringContent(Counter.ToString());
        }
        else if(responseRequired == "FileAccessDetail")
        {
            content = new StringContent(JsonSerializer.Serialize(new FileAccessDetail { Id = 4 }));
        }
        else if(responseRequired == "FileDetail")
        {
            content = new StringContent(JsonSerializer.Serialize(new FileDetail { FileName = "Test File FileClassification" }));
        }
        else if(responseRequired == "Health")
        {
            content = new StringContent(JsonSerializer.Serialize(new HealthStatusResponse { Status = "OK" }));
        }
        else
        {
            content = new StringContent(JsonSerializer.Serialize(new HealthStatusResponse { Status = "OK" }));
        }
#pragma warning restore IDE0045 // Convert to conditional expression

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content });
    }
}
