namespace ChannelDemo.Domain.Events;

public record PaymentProcessedEvent(string PaymentId, decimal Amount, string OrderId) : BaseEvent;