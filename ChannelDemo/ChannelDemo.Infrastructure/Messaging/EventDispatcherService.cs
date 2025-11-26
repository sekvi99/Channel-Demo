using ChannelDemo.Application.Interfaces;
using ChannelDemo.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChannelDemo.Infrastructure.Messaging;

public class EventDispatcherService(
    IMessageQueue messageQueue,
    IServiceProvider serviceProvider,
    ILogger<EventDispatcherService> logger)
    : BackgroundService
{
    private readonly List<Task> _processingTasks = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Event Dispatcher Service started");

        _processingTasks.Add(ProcessEventsAsync<OrderCreatedEvent>(stoppingToken));
        _processingTasks.Add(ProcessEventsAsync<OrderShippedEvent>(stoppingToken));
        _processingTasks.Add(ProcessEventsAsync<PaymentProcessedEvent>(stoppingToken));

        await Task.WhenAll(_processingTasks);
    }

    private async Task ProcessEventsAsync<TEvent>(CancellationToken ct) where TEvent : IEvent
    {
        await foreach (var @event in messageQueue.DequeueAsync<TEvent>(ct))
        {
            using var scope = serviceProvider.CreateScope();
            var subscribers = scope.ServiceProvider.GetServices<IEventSubscriber<TEvent>>();

            var tasks = subscribers.Select(s => SafeHandleAsync(s, @event, ct));
            await Task.WhenAll(tasks);
        }
    }

    private async Task SafeHandleAsync<TEvent>(IEventSubscriber<TEvent> subscriber, TEvent @event, CancellationToken ct)
        where TEvent : IEvent
    {
        try
        {
            await subscriber.HandleAsync(@event, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling event {EventType} in {SubscriberType}",
                typeof(TEvent).Name, subscriber.GetType().Name);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Event Dispatcher Service stopping");
        await base.StopAsync(cancellationToken);
    }
}