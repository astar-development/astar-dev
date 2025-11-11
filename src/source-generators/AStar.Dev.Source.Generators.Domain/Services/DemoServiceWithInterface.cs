using AStar.Dev.Annotations;

namespace AStar.Dev.Source.Generators.Domain.Services;

[RegisterService]
public class DemoServiceWithInterface : IDemoServiceWithInterface
{
}

public interface ISomeOtherInterface;