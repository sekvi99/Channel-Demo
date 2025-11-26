namespace ChannelDemo.Requests;

public class ShipOrderRequest
{
    public string OrderId { get; set; } = string.Empty;
    public string TrackingNumber { get; set; } = string.Empty;
}