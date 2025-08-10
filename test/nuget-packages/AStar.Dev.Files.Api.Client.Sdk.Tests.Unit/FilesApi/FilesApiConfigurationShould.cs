using AStar.Dev.Utilities;

namespace AStar.Dev.Files.Api.Client.Sdk.FilesApi;

public sealed class FilesApiConfigurationShould
{
    [Fact]
    public void ReturnTheExpectedDefaultValue() =>
        new FilesApiConfiguration { Scopes = [] }.ToJson().ShouldMatchApproved();

    [Fact]
    public void ContainTheExpectedProperties() =>
        new FilesApiConfiguration { Scopes = ["MockScope"], BaseUrl = new("https://mock.url.com") }.ToJson().ShouldMatchApproved();

    [Fact]
    public void ReturnTheExpectedSectionLocationValue() =>
        FilesApiConfiguration.SectionLocation.ShouldBe("ApiConfiguration:FilesApiConfiguration");
}
