using System.Text;
using System.Text.Json;
using AStar.Dev.Api.Usage.Sdk;
using AStar.Dev.Infrastructure.UsageDb.Data;
using AStar.Dev.Utilities;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AStar.Dev.Usage.Logger;

public sealed class ProcessUsageEventsService(
    IOptions<ApiUsageConfiguration>    usageConfiguration,
    IConnection                        connection,
    ApiUsageContext                    apiUsageContext,
    IPeriodicTimer                     timer,
    ILogger<ProcessUsageEventsService> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Entering the StartAsync method in ProcessUsageEventsService");

        await ProcessEventsAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Entering the StopAsync method in ProcessUsageEventsService");

        return Task.CompletedTask;
    }

    public async Task ProcessEventsAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogDebug("Entering the ProcessEvents method in ProcessUsageEventsService");
            ApiUsageConfiguration config  = usageConfiguration.Value;
            await using IChannel  channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

            _ = await channel.QueueDeclareAsync(config.QueueName, exclusive: false, durable: true,
                                            autoDelete: false, arguments: null, cancellationToken: stoppingToken);

            await channel.BasicQosAsync(0, 1, false, stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, eventArgs) => { await ProcessReceivedMessageAsync(eventArgs, stoppingToken); };

            _ = await channel.BasicConsumeAsync(config.QueueName, true, consumer, stoppingToken);

            while(!stoppingToken.IsCancellationRequested &&
                  await timer.WaitForNextTickAsync(stoppingToken))
            {
                // No action required here, this just prevents the method exiting
            }
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Error occured while processing usage events: {EventError}", ex.Message);
        }
    }

    private async Task ProcessReceivedMessageAsync(BasicDeliverEventArgs eventArgs, CancellationToken stoppingToken)
    {
        try
        {
            logger.LogDebug("Entering the ProcessReceivedMessageAsync method in ProcessUsageEventsService");

            var           body          = eventArgs.Body.ToArray();
            var           message       = Encoding.UTF8.GetString(body);
            ApiUsageEvent apiUsageEvent = JsonSerializer.Deserialize<ApiUsageEvent>(message, JsonSettings.Options)!;

            var eventData = apiUsageEvent.ToJson();
            var exists    = apiUsageContext.ApiUsage.Any(x => x.Id == apiUsageEvent.Id);

            if(!exists)
                await SaveTheEventAsync(apiUsageEvent, stoppingToken);
            else
                logger.LogDebug("NOT Saving {Event} as it is already in the database", eventData);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Error occured while processing usage events: {EventError}", ex.Message);
        }
    }

    private async Task SaveTheEventAsync(ApiUsageEvent apiUsageEvent, CancellationToken stoppingToken)
    {
        _ = apiUsageContext.ApiUsage.Add(apiUsageEvent);

        var result = await apiUsageContext.SaveChangesAsync(stoppingToken);

        if(result > 0)
            logger.LogDebug("Saved {Event}", apiUsageEvent.ToJson());
        else
            logger.LogWarning("Save {Event} did not update the database", apiUsageEvent.ToJson());
    }
}
