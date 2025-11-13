namespace AStar.Dev.Annotations;

/// <summary>
///     Represents an attribute to indicate a service registration configuration within a dependency injection container.
/// </summary>
/// <remarks>
///     This attribute is used to define the lifetime and interface type(s) for which the target class
///     should be registered during dependency injection setup. By applying this attribute, the target
///     class will be registered with the specified lifetime scope. There are additional options to
///     specify the type/interface it should be resolved as or if it should be resolved as itself (in addition to the
///     interface/base type if applicable)
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public sealed class RegisterServiceAttribute : Attribute
{
    // ReSharper disable once ConvertToPrimaryConstructor
    /// <summary>
    ///     Indicates the service registration configuration for a class within a dependency injection container.
    /// </summary>
    /// <remarks>
    ///     This attribute specifies the lifetime scope of the service and determines how it should be registered.
    ///     It can optionally allow the service to be resolved as itself.
    /// </remarks>
    public RegisterServiceAttribute(Lifetime lifetime = Lifetime.Scoped) => Lifetime = lifetime;

    /// <summary>
    ///     Specifies the lifetime of a service in dependency injection.
    /// </summary>
    /// <remarks>
    ///     The Lifetime enumeration is used to define the lifespan of a service instance.
    ///     It consists of the following options:
    ///     - Singleton: A single instance of the service is created and shared throughout the application's lifecycle.
    ///     - Scoped: A new instance of the service is created for each scope, typically per request in a web application.
    ///     - Transient: A new instance of the service is created each time it is requested.
    /// </remarks>
    public Lifetime Lifetime { get; }

    /// <summary>
    ///     Indicates whether the service should be registered as its own type in the dependency injection container.
    /// </summary>
    /// <remarks>
    ///     When set to true, the service will be registered such that it can be resolved by its own type.
    ///     This is useful for scenarios where a class should be self-resolvable in addition to being
    ///     registered to an interface or base type, if applicable.
    /// </remarks>
    public bool AsSelf { get; set; }
}
