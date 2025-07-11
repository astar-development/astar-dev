using System.Text;
using AStar.Dev.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace AStar.Dev.Api.Usage.Sdk;

/// <summary>
/// </summary>
public sealed class Send(IConnection connection, ILogger<Send> logger, IOptions<ApiUsageConfiguration> usageConfiguration)
{
    /// <summary>
    /// </summary>
    /// <param name="usageEvent"></param>
    /// <param name="cancellationToken"></param>
    public async Task SendUsageEventAsync(ApiUsageEvent usageEvent, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("In Send > SendUsageEventAsync");

            // Hack to keep the original async method signature
            await Task.CompletedTask;

            var config = usageConfiguration.Value;

            using var channel    = connection.CreateModel();

            channel.QueueDeclare(config.QueueName, true, false, false);

            var body = Encoding.UTF8.GetBytes(usageEvent.ToJson());

            channel.BasicPublish(string.Empty, config.QueueName, true, null, body);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An error occured while sending an event.");
        }
    }
}
