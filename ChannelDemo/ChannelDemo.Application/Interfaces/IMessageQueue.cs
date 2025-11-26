using ChannelDemo.Domain.Events;

namespace ChannelDemo.Application.Interfaces;

public interface IMessageQueue
{
    Task EnqueueAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : IEvent;
    IAsyncEnumerable<TEvent> DequeueAsync<TEvent>(CancellationToken ct = default) where TEvent : IEvent;
}