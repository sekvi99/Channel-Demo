using System.Threading.Channels;
using ChannelDemo.Domain.Events;

namespace ChannelDemo.Infrastructure.Messaging;

public class EventChannel<TEvent> where TEvent : IEvent
{
    private readonly Channel<TEvent> _channel = Channel.CreateUnbounded<TEvent>(new UnboundedChannelOptions
    {
        SingleReader = false,
        SingleWriter = false
    });

    public ChannelWriter<TEvent> Writer => _channel.Writer;
    public ChannelReader<TEvent> Reader => _channel.Reader;

    public async Task WriteAsync(TEvent @event, CancellationToken ct = default)
    {
        await _channel.Writer.WriteAsync(@event, ct);
    }

    public IAsyncEnumerable<TEvent> ReadAllAsync(CancellationToken ct = default)
    {
        return _channel.Reader.ReadAllAsync(ct);
    }
}