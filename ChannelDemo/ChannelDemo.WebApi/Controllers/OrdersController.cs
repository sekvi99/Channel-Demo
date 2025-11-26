using ChannelDemo.Application.Interfaces;
using ChannelDemo.Domain.Events;
using ChannelDemo.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ChannelDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IEventPublisher eventPublisher, ILogger<OrdersController> logger)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        logger.LogInformation("Creating order {OrderId}", request.OrderId);

        var @event = new OrderCreatedEvent(request.OrderId, request.Amount, request.CustomerEmail);
        await eventPublisher.PublishAsync(@event);

        return Ok(new { message = "Order created successfully", orderId = request.OrderId });
    }

    [HttpPost("{orderId}/ship")]
    public async Task<IActionResult> ShipOrder(string orderId, [FromBody] ShipOrderRequest request)
    {
        logger.LogInformation("Shipping order {OrderId}", orderId);

        var @event = new OrderShippedEvent(request.OrderId, request.TrackingNumber);
        await eventPublisher.PublishAsync(@event);

        return Ok(new { message = "Order shipped successfully", trackingNumber = request.TrackingNumber });
    }
}