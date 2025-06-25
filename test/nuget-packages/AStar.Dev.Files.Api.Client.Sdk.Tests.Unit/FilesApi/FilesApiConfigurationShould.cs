using AStar.Dev.Files.Api.Client.SDK.FilesApi;

namespace AStar.Dev.Files.Api.Client.Sdk.FilesApi;

public sealed class FilesApiConfigurationShould
{
    [Fact]
    public void ReturnTheExpectedDefaultValue() =>
        new FilesApiConfiguration { Scopes = [], }.BaseUrl.ShouldBe(new("https://not.set.com/"));

    [Fact]
    public void ReturnTheExpectedSectionLocationValue() =>
        FilesApiConfiguration.SectionLocation.ShouldBe("ApiConfiguration:FilesApiConfiguration");
}
