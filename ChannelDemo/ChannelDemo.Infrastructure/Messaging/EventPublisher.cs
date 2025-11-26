using ChannelDemo.Application.Interfaces;
using ChannelDemo.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ChannelDemo.Infrastructure.Messaging;

public class EventPublisher(IMessageQueue messageQueue, ILogger<EventPublisher> logger) : IEventPublisher
{
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : IEvent
    {
        logger.LogInformation("Publishing event: {EventType} (ID: {EventId})",
            @event.GetType().Name, @event.Id);
        await messageQueue.EnqueueAsync(@event, ct);
    }
}