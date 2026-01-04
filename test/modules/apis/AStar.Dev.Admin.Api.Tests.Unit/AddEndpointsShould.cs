using JetBrains.Annotations;

namespace AStar.Dev.Admin.Api;

[TestSubject(typeof(AddEndpoints))]
public sealed class AddEndpointsShould
{
    [Fact]
    public void AddTheEndpointsWithoutError()
    {
        var application = WebApplication.Create();

        Action sut = () => application.AddApplicationEndpoints();

        sut.ShouldNotThrow();
    }
}
