using Microsoft.Extensions.Options;
using DatabaseUpdaterConfiguration = AStar.Dev.Database.Updater.Api.FileKeywordProcessor.DatabaseUpdaterConfiguration;

namespace AStar.Dev.Database.Updater.Api;

/// <summary>
///     This class is the new way (source-generation) to validate the startup configuration
/// </summary>
[OptionsValidator]
public partial class DatabaseUpdaterConfigurationValidator : IValidateOptions<DatabaseUpdaterConfiguration>
{
}
