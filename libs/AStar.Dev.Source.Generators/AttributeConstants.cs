namespace AStar.Dev.Source.Generators;

/// <summary>
/// Defines constant values related to attributes used in the source generators, such as the namespace for the attributes and the name of the StrongId attribute. These constants can be used throughout the source generator code to ensure consistency when referencing attribute types and namespaces, and to avoid hardcoding string literals in multiple places, which can help prevent errors and make maintenance easier if the attribute names or namespaces ever need to change.
/// </summary>
public static class AttributeConstants
{
    /// <summary>
    /// Defines the namespace where the source generator attributes are located. This constant can be used when referencing attribute types in the source generator code to ensure that the correct namespace is used consistently, which is important for correctly identifying and processing attributes during source generation. The value "AStar.Dev.Source.Generators.Attributes" indicates that the attributes are defined in a sub-namespace specifically for attributes within the AStar.Dev.Source.Generators assembly.
    /// </summary>
    public const string AttributeNamespace = "AStar.Dev.Source.Generators.Attributes";
    /// <summary>
    /// Defines the name of the StrongId attribute, which is used to mark record structs for which strongly-typed identifier code should be generated. This constant can be used when the source generator is scanning for attributes to identify which record structs are decorated with the StrongId attribute, allowing the generator to process those types accordingly. By using a constant for the attribute name, it ensures that the correct attribute is referenced consistently throughout the source generator code, and it also makes it easier to update the attribute name in the future if needed without having to search for string literals across the codebase.
    /// </summary>
    public const string StrongIdAttributeName = "StrongIdAttribute";
}
