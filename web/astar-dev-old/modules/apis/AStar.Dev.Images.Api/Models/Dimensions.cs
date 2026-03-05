namespace AStar.Dev.Images.Api.Models;

/// <summary>
///     The dimensions class contains the height and width of the object it is attached to.
/// </summary>
public sealed class Dimensions
{
    private int height;
    private int width;

    /// <summary>
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     The height of the image (where applicable). The height cannot be set to null, less-than 0 or greater-than 9,999.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the above conditions are not met.
    /// </exception>
    public int Height
    {
        get => height;
        set
            => height = value switch
                        {
                            <= 0 => throw new ArgumentOutOfRangeException(nameof(Height), $"{nameof(Height)} cannot be negative."),
                            > 99999 => throw new ArgumentOutOfRangeException(nameof(Height),
                                                                             $"{nameof(Height)} cannot be greater than 99999. Actual: {value}."),
                            _ => value,
                        };
    }

    /// <summary>
    ///     The width of the image (where applicable). The width cannot be set to null, less-than 0 or greater-than 9,999.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the above conditions are not met.
    /// </exception>
    public int Width
    {
        get => width;
        set
            => width = value switch
                       {
                           <= 0 => throw new ArgumentOutOfRangeException(nameof(Width), $"{nameof(Width)} cannot be negative."),
                           > 99999 => throw new ArgumentOutOfRangeException(nameof(Width),
                                                                            $"{nameof(Width)} cannot be greater than 99999. Actual: {value}."),
                           _ => value,
                       };
    }
}
