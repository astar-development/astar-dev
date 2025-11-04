using Microsoft.OpenApi.Models;

namespace AStar.Dev.AspNet.Extensions.Tests.Unit;

[TestSubject(typeof(ApiConfiguration))]
public sealed class ApiConfigurationTest
{
    [Fact]
    public void ContainTheConfigurationSectionNameWithTheExpectedValue()
        => ApiConfiguration.ConfigurationSectionName.ShouldBe("ApiConfiguration");

    [Fact]
    public void ContainTheOpenApiInfoPropertyWithTheExpectedDefaultValue()
    {
        var sut = new ApiConfiguration();

        var actual = sut.OpenApiInfo;

        _ = actual.ShouldBeOfType<OpenApiInfo>();
    }
}
