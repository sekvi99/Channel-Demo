namespace ChannelDemo.Requests;

public class CreateOrderRequest
{
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
}