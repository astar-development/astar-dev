using AStar.Dev.Database.Updater.Core;
using Microsoft.Extensions.Options;

namespace AStar.Dev.Database.Updater;

/// <summary>
///     This class is the new way (source-generation) to validate the startup configuration
/// </summary>
[OptionsValidator]
public partial class DatabaseUpdaterConfigurationValidator : IValidateOptions<DatabaseUpdaterConfiguration>
{
}
