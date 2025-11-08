namespace AStar.Dev.Annotations;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ConfigSectionAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}