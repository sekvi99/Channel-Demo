using ChannelDemo.Application.Interfaces;
using ChannelDemo.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ChannelDemo.Application.Services;

public class ShippingSubscriber(ILogger<ShippingSubscriber> logger) : IEventSubscriber<OrderShippedEvent>
{
    public async Task HandleAsync(OrderShippedEvent @event, CancellationToken ct)
    {
        await Task.Delay(1000, ct);
        logger.LogInformation("Order {OrderId} shipped. Tracking: {TrackingNumber}",
            @event.OrderId, @event.TrackingNumber);
    }
}