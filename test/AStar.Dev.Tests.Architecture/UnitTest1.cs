using AStar.Dev.Web;
using NetArchTest.Rules;

namespace AStar.Dev.Tests.Architecture;

public sealed class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var uiLayerAssembly = typeof(Program).Assembly;
        var types = Types.InAssembly(uiLayerAssembly);
        // Act - pointless as it doesn't work!!
        var result = types
        .ShouldNot().HaveDependencyOnAll("AStar.Dev.Files.Api.Client.SDK", "Contracts", "Persistence", "Services", "Services.Abstractions")
        .GetResult();
        // Assert
        result.IsSuccessful.ShouldBeTrue();

        _ = Types.InAssembly(typeof(IAssemblyMarker).Assembly);

        // All the service classes should be sealed
        var results = types
            //.That().ImplementInterface(typeof(IWidgetService))
            .Should().BeSealed()
            .GetResult();
        results
            .IsSuccessful.ShouldBeTrue();
    }
}
