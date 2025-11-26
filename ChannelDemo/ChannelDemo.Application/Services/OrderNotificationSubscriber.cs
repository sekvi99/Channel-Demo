using ChannelDemo.Application.Interfaces;
using ChannelDemo.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ChannelDemo.Application.Services;

public class OrderNotificationSubscriber(ILogger<OrderNotificationSubscriber> logger)
    : IEventSubscriber<OrderCreatedEvent>
{
    public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken ct)
    {
        await Task.Delay(1000, ct);
        logger.LogInformation("Order {@OrderId} created with amount ${Amount}",
            @event.OrderId, @event.Amount);
    }
}