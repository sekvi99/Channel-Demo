using ChannelDemo.Application.Interfaces;
using ChannelDemo.Domain.Events;
using ChannelDemo.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ChannelDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController(IEventPublisher eventPublisher, ILogger<PaymentsController> logger)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest request)
    {
        logger.LogInformation("Processing payment {PaymentId}", request.PaymentId);

        var @event = new PaymentProcessedEvent(request.PaymentId, request.Amount, request.OrderId);
        await eventPublisher.PublishAsync(@event);

        return Ok(new { message = "Payment processed successfully", paymentId = request.PaymentId });
    }
}