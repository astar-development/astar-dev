namespace AStar.Dev.Source.Generators.Tests.TestHelpers;

internal static class Constants
{
    public const string AttributeSource = """
                                          namespace AStar.Dev.Annotations;
                                          public enum Lifetime { Singleton, Scoped, Transient }
                                          [System.AttributeUsage(System.AttributeTargets.Class)]
                                          public sealed class RegisterServiceAttribute : System.Attribute
                                          {
                                              public RegisterServiceAttribute(Lifetime lifetime = Lifetime.Scoped) => Lifetime = lifetime;
                                              public Lifetime Lifetime { get; }
                                              public System.Type? As { get; set; }
                                              public bool AsSelf { get; set; }
                                          }
                                          """;
}