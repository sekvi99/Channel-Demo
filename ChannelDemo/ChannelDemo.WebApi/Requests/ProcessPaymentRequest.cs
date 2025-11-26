namespace ChannelDemo.Requests;

public class ProcessPaymentRequest
{
    public string PaymentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string OrderId { get; set; } = string.Empty;
}