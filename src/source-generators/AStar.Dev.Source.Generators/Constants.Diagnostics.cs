namespace AStar.Dev.Source.Generators;

/// <summary>
///     Provides a centralized location for constant values used in the AStar.Dev.Source.Generators library.
/// </summary>
public static partial class Constants
{
    /// <summary>
    ///     Contains unique diagnostic IDs used to report issues or warnings during the execution
    ///     of the source generators within the AStar.Dev.Source.Generators library.
    /// </summary>
    public static class DiagnosticIds
    {
        /// <summary>
        ///     Represents the diagnostic ID used for reporting issues specifically related to invalid
        ///     "RegisterService As" arguments within the AStar.Dev.Source.Generators library.
        /// </summary>
        public const string AStarGen001 = "AStarGen001";
    }
}