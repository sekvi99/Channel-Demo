namespace ChannelDemo.Domain.Events;

public record OrderShippedEvent(string OrderId, string TrackingNumber) : BaseEvent;