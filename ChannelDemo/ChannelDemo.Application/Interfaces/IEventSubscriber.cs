using ChannelDemo.Domain.Events;

namespace ChannelDemo.Application.Interfaces;

public interface IEventSubscriber<TEvent> where TEvent : IEvent
{
    Task HandleAsync(TEvent @event, CancellationToken ct = default);
}