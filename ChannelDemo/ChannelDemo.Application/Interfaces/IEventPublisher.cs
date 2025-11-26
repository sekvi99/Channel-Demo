using ChannelDemo.Domain.Events;

namespace ChannelDemo.Application.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : IEvent;
}