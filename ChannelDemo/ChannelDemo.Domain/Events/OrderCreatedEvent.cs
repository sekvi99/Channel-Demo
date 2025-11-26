namespace ChannelDemo.Domain.Events;

public record OrderCreatedEvent(string OrderId, decimal Amount, string CustomerEmail) : BaseEvent;