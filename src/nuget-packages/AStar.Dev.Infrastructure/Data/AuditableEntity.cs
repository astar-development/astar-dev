namespace AStar.Dev.Infrastructure.Data;

/// <summary>
///     The <see cref="AuditableEntity" /> class defines the audit properties to be set on all relevant entities
/// </summary>
public abstract class AuditableEntity
{
    /// <summary>
    ///     Indicates the user or system that performed the last update.
    /// </summary>
    public string UpdatedBy { get; set; } = "Jay Barden";

    /// <summary>
    ///     Represents the date and time when the entity was last updated (defaults to the current UTC time).
    /// </summary>
    public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
}
