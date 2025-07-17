using AStar.Dev.Images.Api.Client.Sdk.ImagesApi;
using AStar.Dev.Utilities;

namespace AStar.Dev.Images.Api.Client.Sdk;

public sealed class ImagesApiConfigurationShould
{
    [Fact]
    public void ReturnTheExpectedDefaultValue() =>
        new ImagesApiConfiguration { Scopes = [] }
            .ToJson()
            .ShouldMatchApproved();

    [Fact]
    public void ContainTheExpectedProperties() =>
        new ImagesApiConfiguration { Scopes = ["Mock Scope"], BaseUrl = new("https://doesnt.matter.com") }
            .ToJson()
            .ShouldMatchApproved();

    [Fact]
    public void ReturnTheExpectedSectionLocationValue() =>
        ImagesApiConfiguration.SectionLocation.ShouldBe("ApiConfiguration:ImagesApiConfiguration");
}
