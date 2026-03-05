using System;
using AStar.Dev.Source.Generators.Attributes;

namespace AStar.Dev.Source.Generators.Sample;

/// <summary>
/// Sample entities demonstrating the use of the StrongId attribute to generate strongly-typed identifiers with various underlying types (int, string, Guid).
/// </summary>
[StrongId]
public partial record struct UserId;

/// <summary>
/// Sample entities demonstrating the use of the StrongId attribute to generate strongly-typed identifiers with various underlying types (int, string, Guid).
/// </summary>
[StrongId(typeof(int))]
public partial record struct UserId1;

/// <summary>
/// Sample entities demonstrating the use of the StrongId attribute to generate strongly-typed identifiers with various underlying types (int, string, Guid).
/// </summary>
[StrongId(typeof(string))]
public partial record struct UserId2;

/// <summary>
/// Sample entities demonstrating the use of the StrongId attribute to generate strongly-typed identifiers with various underlying types (int, string, Guid).
/// </summary>
[StrongId(typeof(Guid))]
public partial record struct UserId3;
