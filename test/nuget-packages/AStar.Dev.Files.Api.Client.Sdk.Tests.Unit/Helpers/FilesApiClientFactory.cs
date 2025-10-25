using AStar.Dev.Files.Api.Client.SDK.FilesApi;
using AStar.Dev.Files.Api.Client.Sdk.Tests.Unit.MockMessageHandlers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Identity.Web;
using NSubstitute;

namespace AStar.Dev.Files.Api.Client.Sdk.Tests.Unit.Helpers;

internal static class FilesApiClientFactory
{
    private static readonly ILogger<FilesApiClient> DummyLogger = NullLogger<FilesApiClient>.Instance;
    private static readonly string IrrelevantUrl = "https://doesnot.matter.com";

    public static FilesApiClient Create(HttpMessageHandler mockHttpMessageHandler)
    {
        _ = Substitute.For<ITokenAcquisition>();
        var httpClient = new HttpClient(mockHttpMessageHandler) { BaseAddress = new(IrrelevantUrl) };

        return new(httpClient, /* tokenAcquisitionServiceMock, */ DummyLogger);
    }

    public static FilesApiClient CreateInternalServerErrorClient(string errorMessage)
    {
        _ = Substitute.For<ITokenAcquisition>();
        var handler = new MockInternalServerErrorHttpMessageHandler(errorMessage);
        var httpClient = new HttpClient(handler) { BaseAddress = new(IrrelevantUrl) };

        return new(httpClient, /* tokenAcquisitionServiceMock, */ DummyLogger);
    }
}
