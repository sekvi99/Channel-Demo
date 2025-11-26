namespace ChannelDemo.Domain.Events;

public interface IEvent
{
    Guid Id { get; }
    DateTime OccurredAt { get; }
}