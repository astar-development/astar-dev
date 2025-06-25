using AStar.Dev.Images.Api.Client.SDK.ImagesApi;

namespace AStar.Dev.Images.Api.Client.Sdk;

public sealed class ImagesApiConfigurationShould
{
    [Fact]
    public void ReturnTheExpectedDefaultValue() =>
        new ImagesApiConfiguration { Scopes = [], }.BaseUrl.ShouldBe(new("https://not.set.com/"));

    [Fact]
    public void ReturnTheExpectedSectionLocationValue() =>
        ImagesApiConfiguration.SectionLocation.ShouldBe("ApiConfiguration:ImagesApiConfiguration");
}
