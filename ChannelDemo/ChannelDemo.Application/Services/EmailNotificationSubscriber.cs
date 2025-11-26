using ChannelDemo.Application.Interfaces;
using ChannelDemo.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ChannelDemo.Application.Services;

public class EmailNotificationSubscriber(ILogger<EmailNotificationSubscriber> logger)
    : IEventSubscriber<OrderCreatedEvent>
{
    public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken ct)
    {
        await Task.Delay(1500, ct);
        logger.LogInformation("Sending confirmation email to {Email} for order {OrderId}",
            @event.CustomerEmail, @event.OrderId);
    }
}