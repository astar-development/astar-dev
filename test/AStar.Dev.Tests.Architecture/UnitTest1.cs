using AStar.Dev.Web;
using NetArchTest.Rules;

namespace AStar.Dev.Tests.Architecture;

public sealed class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var types = Types.InAssembly(typeof(IAssemblyMarker).Assembly);
        // Classes in the presentation should not directly reference repositories
        _ = types
            .That()
            .ResideInNamespace("NetArchTest.SampleLibrary.Presentation")
            .ShouldNot()
            .HaveDependencyOn("NetArchTest.SampleLibrary.Data")
            .GetResult()
            .IsSuccessful;

        // Classes in the "data" namespace should implement IRepository
        _ = types
            .That().HaveDependencyOn("System.Data")
            .And().ResideInNamespace("ArchTest")
            .Should().ResideInNamespace("NetArchTest.SampleLibrary.Data")
            .GetResult()
            .IsSuccessful;

        // All the service classes should be sealed
        _ = types
            //.That().ImplementInterface(typeof(IWidgetService))
            .Should().BeSealed()
            .GetResult()
            .IsSuccessful;
    }
}
