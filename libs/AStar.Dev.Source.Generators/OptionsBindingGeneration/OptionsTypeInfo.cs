using System;
using Microsoft.CodeAnalysis;

namespace AStar.Dev.Source.Generators.OptionsBindingGeneration;

/// <summary>
/// Represents information about a configuration options type, including its name, full type name, associated configuration section name, and source code location. This class is used to store metadata about options types that are processed by the source generator for generating binding code to configuration sections.
/// </summary>
public sealed class OptionsTypeInfo : IEquatable<OptionsTypeInfo>
{
    /// <summary>
    /// Gets the simple name of the options type (without namespace).
    /// </summary>
    public string TypeName { get; }
    /// <summary>
    /// Gets the fully qualified name of the options type, including its namespace, for use in generated code to ensure correct type references.
    /// </summary>
    public string FullTypeName { get; }
    /// <summary>
    /// Gets the name of the configuration section associated with this options type. This is used to determine which section of the configuration should be bound to this options class when generating code for configuration binding.
    /// </summary>
    public string SectionName { get; }
    /// <summary>
    /// Gets the source code location where this options type is defined. This information can be used for diagnostics, such as reporting errors or warnings related to the options type during source generation, by pointing back to the exact location in the user's code.
    /// </summary>
    public Location Location { get; }

/// <summary>
/// Initializes a new instance of the OptionsTypeInfo class with the specified type name, full type name, configuration section name, and source code location. This constructor is typically called by the source generator when it identifies a class or struct that should be treated as an options type based on the presence of the AutoRegisterOptionsAttribute, allowing the generator to capture all relevant metadata about the options type for later use in code generation and diagnostics.
/// </summary>
/// <param name="typeName">The simple name of the options type (without namespace).</param>
/// <param name="fullTypeName">The fully qualified name of the options type, including its namespace.</param>
/// <param name="sectionName">The name of the configuration section associated with this options type.</param>
/// <param name="location">The source code location where this options type is defined. This information can be used for diagnostics, such as reporting errors or warnings related to the options type during source generation, by pointing back to the exact location in the user's code.</param>
    public OptionsTypeInfo(string typeName, string fullTypeName, string sectionName, Location location)
    {
        TypeName = typeName ?? string.Empty;
        FullTypeName = fullTypeName ?? string.Empty;
        SectionName = sectionName;
        Location = location;
    }

/// <summary>
/// Determines whether the specified object is equal to the current OptionsTypeInfo instance by comparing their type names, full type names, section names, and source code locations. This method is used to ensure that two OptionsTypeInfo instances are considered equal if they represent the same options type with the same metadata, which can be important for avoiding duplicate code generation or for correctly identifying options types during the source generation process.
/// </summary>
/// <param name="obj">The object to compare with the current OptionsTypeInfo instance.</param>
/// <returns>true if the specified object is equal to the current OptionsTypeInfo instance; otherwise, false.</returns>
    public override bool Equals(object obj) => Equals((OptionsTypeInfo)obj);

/// <summary>
/// Determines whether the specified OptionsTypeInfo instance is equal to the current instance by comparing their type names, full type names, section names, and source code locations. This method is used to ensure that two OptionsTypeInfo instances are considered equal if they represent the same options type with the same metadata, which can be important for avoiding duplicate code generation or for correctly identifying options types during the source generation process.
/// </summary>
/// <param name="other">The OptionsTypeInfo instance to compare with the current instance.</param>
/// <returns>true if the specified OptionsTypeInfo instance is equal to the current instance; otherwise, false.</returns>
    public bool Equals(OptionsTypeInfo other) => ReferenceEquals(this, other) || (other is not null && string.Equals(TypeName, other.TypeName, System.StringComparison.Ordinal)
            && string.Equals(FullTypeName, other.FullTypeName, System.StringComparison.Ordinal)
            && string.Equals(SectionName, other.SectionName, System.StringComparison.Ordinal)
            && Equals(Location, other.Location));

    /// <summary>
    /// Returns a hash code for the current OptionsTypeInfo instance based on its type name, full type name, section name, and source code location. This method is used to provide a hash code that is consistent with the equality comparison defined in the Equals method, which can be important for using OptionsTypeInfo instances in hash-based collections or for ensuring that duplicate options types are correctly identified during the source generation process.
    /// </summary>
    /// <returns>A hash code for the current OptionsTypeInfo instance.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = (hash * 23) + (TypeName != null ? TypeName.GetHashCode() : 0);
            hash = (hash * 23) + (FullTypeName != null ? FullTypeName.GetHashCode() : 0);
            hash = (hash * 23) + (SectionName != null ? SectionName.GetHashCode() : 0);
            hash = (hash * 23) + (Location != null ? Location.GetHashCode() : 0);
            return hash;
        }
    }

    /// <summary>
    /// Provides a string representation of the OptionsTypeInfo instance, including the full type name and associated configuration section name for debugging purposes.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"{FullTypeName} (Section: {SectionName})";
}
