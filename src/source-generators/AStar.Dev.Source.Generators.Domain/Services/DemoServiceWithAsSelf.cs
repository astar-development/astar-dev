using AStar.Dev.Annotations;

namespace AStar.Dev.Source.Generators.Domain.Services;

[RegisterService(Lifetime.Transient, AsSelf = true)]
public class DemoServiceWithAsSelf : IDemoServiceWithAsSelf
{
}