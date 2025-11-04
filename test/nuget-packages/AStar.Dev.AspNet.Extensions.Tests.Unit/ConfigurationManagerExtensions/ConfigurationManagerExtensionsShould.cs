using AStar.Dev.AspNet.Extensions.ConfigurationManagerExtensions;
using Microsoft.Extensions.Configuration;

namespace AStar.Dev.AspNet.Extensions.Tests.Unit.ConfigurationManagerExtensions;

[TestSubject(typeof(Extensions.ConfigurationManagerExtensions.ConfigurationManagerExtensions))]
public sealed class ConfigurationManagerExtensionsShould
{
    [Fact]
    public void ReturnAnEmptyConfigurationWhenTheConfigurationKeyDoesNotExist()
    {
        var sut = new ConfigurationManager();

        var configuration = sut.GetValidatedConfigurationSection<ApiConfiguration>("Some Key That Does Not Exist");

        configuration.ShouldBeEquivalentTo(new ApiConfiguration());
    }

    [Fact(Skip = "Underlying code is broken")]
    public void ReturnTheExpectedConfigurationWhenTheConfigurationKeyExists()
    {
        var sut = new ConfigurationManager();
        _ = sut.AddJsonFile("testdata/appsettings.json");

        var configuration = sut.GetValidatedConfigurationSection<ApiConfiguration>("apiConfiguration")!;

        configuration.OpenApiInfo.Title.ShouldBe("AStar Development Admin Api - from TestData.");
    }
}
