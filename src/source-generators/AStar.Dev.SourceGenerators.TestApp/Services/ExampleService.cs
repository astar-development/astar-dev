using AStar.Dev.Annotations;

namespace AStar.Dev.SourceGenerators.TestApp.Services;

[RegisterService(Lifetime.Scoped, AsSelf = false)]
public class ExampleService
{
    /// <summary>
    /// Dummy service method
    /// </summary>
    /// <returns></returns>
#pragma warning disable CA1822
    public int DoSomething() => 42;
#pragma warning restore CA1822
}
