namespace ChannelDemo.Domain.Events;

public abstract record BaseEvent : IEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}