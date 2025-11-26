using ChannelDemo.Application.Interfaces;
using ChannelDemo.Domain.Events;

namespace ChannelDemo.Infrastructure.Messaging;

public class ChannelMessageQueue : IMessageQueue
{
    private readonly Dictionary<Type, object> _channels = new();
    private readonly Lock _lock = new();

    public async Task EnqueueAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : IEvent
    {
        var channel = GetOrCreateChannel<TEvent>();
        await channel.WriteAsync(@event, ct);
    }

    public IAsyncEnumerable<TEvent> DequeueAsync<TEvent>(CancellationToken ct = default) where TEvent : IEvent
    {
        var channel = GetOrCreateChannel<TEvent>();
        return channel.ReadAllAsync(ct);
    }

    private EventChannel<TEvent> GetOrCreateChannel<TEvent>() where TEvent : IEvent
    {
        var type = typeof(TEvent);

        if (_channels.TryGetValue(type, out var channelObj)) return (EventChannel<TEvent>)channelObj;

        lock (_lock)
        {
            if (_channels.TryGetValue(type, out channelObj)) return (EventChannel<TEvent>)channelObj;

            var channel = new EventChannel<TEvent>();
            _channels[type] = channel;
            return channel;
        }
    }
}