using System;

namespace AStar.Dev.Annotations;

/// <summary>
/// Represents an attribute that can be used to mark a struct as a strong identifier.
/// </summary>
/// <remarks>
/// This attribute is used to annotate structures with a specific identifier type, typically
/// to signify that the struct is a strongly typed id. The default identifier type is a GUID.
/// </remarks>
/// <param name="idType">
/// The type of the identifier associated with the struct. By default, it is "System.Guid".
/// </param>
/// <param name="guidV7">
/// Indicates whether the GUID used as the identifier for the struct conforms to the version 7 GUID format.
/// </param>
[AttributeUsage(AttributeTargets.Struct)]
public sealed class StrongIdAttribute(string idType = "System.Guid", bool guidV7 = true) : Attribute
{
    /// <summary>
    /// Gets the type of the identifier associated with a struct marked by the <see cref="StrongIdAttribute"/>.
    /// </summary>
    /// <remarks>
    /// This property returns the string name of the identifier type. By default, it is "System.Guid".
    /// The identifier type specifies the type used as an identifier for the struct.
    /// </remarks>
    public string IdType { get; } = idType;

    /// <summary>
    /// Indicates whether the GUID used as the identifier for a struct marked by the <see cref="StrongIdAttribute"/>
    /// conforms to the version 7 GUID format.
    /// </summary>
    /// <remarks>
    /// Version 7 GUIDs are designed to facilitate time-based sorting and are a newer standard
    /// compared to the traditional random GUIDs (version 4). When this property is set to true,
    /// the identifier adopts the version 7 format.
    /// </remarks>
    public bool GuidV7 { get; } = guidV7;
}
