using JetBrains.Annotations;
using Microsoft.OpenApi;

namespace AStar.Dev.AspNet.Extensions;

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

        OpenApiInfo actual = sut.OpenApiInfo;

        actual.ShouldBeOfType<OpenApiInfo>();
    }
}
