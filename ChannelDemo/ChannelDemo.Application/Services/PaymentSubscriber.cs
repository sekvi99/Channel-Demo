using ChannelDemo.Application.Interfaces;
using ChannelDemo.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ChannelDemo.Application.Services;

public class PaymentSubscriber(ILogger<PaymentSubscriber> logger) : IEventSubscriber<PaymentProcessedEvent>
{
    public async Task HandleAsync(PaymentProcessedEvent @event, CancellationToken ct)
    {
        await Task.Delay(1200, ct);
        logger.LogInformation("Payment {PaymentId} processed: ${Amount} for order {OrderId}",
            @event.PaymentId, @event.Amount, @event.OrderId);
    }
}