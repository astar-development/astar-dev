namespace AStarDev.Utilities;

/// <summary>
///
/// </summary>
public static class IntParser
{
    extension(string value)
    {
        /// <summary>
        ///     The ToInt method will parse the supplied string and return the matching int value
        /// </summary>
        /// <returns>The parsed value as the matching int value</returns>
        /// <exception cref="FormatException">Thrown when the string is not a valid int value</exception>
        public int ToInt() =>
            int.Parse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);

        /// <summary>
        ///    The ToIntSafe method will parse the supplied string and return the matching int value, or 0 if the string is not a valid int value
        /// </summary>
        /// <returns>The parsed value as the matching int value, or 0 if the string is not a valid int value</returns>
        public int ToIntSafe() =>
            int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int result) ? result : 0;
    }
}
