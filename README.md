# Clean Architecture Notifier-Subscriber Pattern with .NET Channels

A comprehensive demonstration of the Publisher-Subscriber (Notifier-Subscriber) pattern implemented in ASP.NET Core using `System.Threading.Channels` for in-memory message queuing, following Clean Architecture principles.

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [How It Works](#how-it-works)
- [API Endpoints](#api-endpoints)
- [Configuration](#configuration)
- [Extending the Demo](#extending-the-demo)
- [Best Practices](#best-practices)
- [Troubleshooting](#troubleshooting)

## 🎯 Overview

This project demonstrates a production-ready implementation of an event-driven architecture using the Notifier-Subscriber pattern. It showcases how to decouple business logic using asynchronous message passing with .NET Channels, all within a clean architecture framework.

### What You'll Learn

- **Clean Architecture** - Proper separation of concerns across Domain, Application, Infrastructure, and API layers
- **.NET Channels** - High-performance in-memory message queuing with `System.Threading.Channels`
- **Background Services** - Long-running background tasks in ASP.NET Core
- **Event-Driven Design** - Decoupled event publishing and subscription patterns
- **Dependency Injection** - Proper DI configuration for scalable applications

## 🏗️ Architecture

This project follows Clean Architecture principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                      API Layer                          │
│  (Controllers, DTOs, HTTP Request/Response Handling)    │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│                  Application Layer                      │
│     (Business Logic, Use Cases, Interfaces)             │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│                Infrastructure Layer                     │
│  (Channel Implementation, Event Dispatcher Service)     │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│                    Domain Layer                         │
│         (Core Entities, Events, Abstractions)           │
└─────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

| Layer | Responsibilities | Dependencies |
|-------|-----------------|--------------|
| **Domain** | Core business entities, events, interfaces | None (pure C#) |
| **Application** | Use cases, business rules, subscriber implementations | Domain |
| **Infrastructure** | Channel implementation, background services, external concerns | Application, Domain |
| **API** | HTTP endpoints, request/response handling, routing | Application |

## ✨ Features

- ✅ **Type-safe event handling** with generic constraints
- ✅ **Multiple subscribers per event** for parallel processing
- ✅ **Unbounded channels** for high-throughput scenarios
- ✅ **Background service integration** for continuous event processing
- ✅ **Dependency injection** throughout the application
- ✅ **Structured logging** with ASP.NET Core's ILogger
- ✅ **Graceful shutdown** with proper cancellation token handling
- ✅ **Exception isolation** - subscriber failures don't affect others
- ✅ **Swagger/OpenAPI** documentation included

## 🔧 Prerequisites

- .NET 6.0 SDK or later
- Visual Studio 2022, VS Code, or Rider (optional)
- Basic understanding of async/await, dependency injection, and REST APIs

## 🚀 Getting Started

### 1. Create a New Project

```bash
# Create a new ASP.NET Core Web API project
dotnet new webapi -n EventDrivenDemo
cd EventDrivenDemo

# Remove the default WeatherForecast files
rm WeatherForecast.cs Controllers/WeatherForecastController.cs
```

### 2. Add the Code

Copy the entire code from the artifact into your `Program.cs` file.

### 3. Run the Application

```bash
dotnet run
```

The application will start on `https://localhost:5001` (or the port shown in your console).

### 4. Access Swagger UI

Open your browser and navigate to:
```
https://localhost:5001/swagger
```

### 5. Test the API

#### Create an Order
```bash
curl -X POST https://localhost:5001/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "orderId": "ORD-001",
    "amount": 299.99,
    "customerEmail": "customer@example.com"
  }'
```

#### Process a Payment
```bash
curl -X POST https://localhost:5001/api/payments \
  -H "Content-Type: application/json" \
  -d '{
    "paymentId": "PAY-001",
    "amount": 299.99,
    "orderId": "ORD-001"
  }'
```

#### Ship an Order
```bash
curl -X POST https://localhost:5001/api/orders/ORD-001/ship \
  -H "Content-Type: application/json" \
  -d '{
    "orderId": "ORD-001",
    "trackingNumber": "TRK-123456789"
  }'
```

## ⚙️ How It Works

### 1. Event Publishing Flow

```
HTTP Request → Controller → EventPublisher → ChannelMessageQueue
                                                     ↓
                                            Channel<TEvent>
```

When an API endpoint is called:
1. Controller receives HTTP request
2. Creates domain event with relevant data
3. Calls `IEventPublisher.PublishAsync(event)`
4. Event is written to the appropriate typed channel
5. HTTP 200 response returned immediately (async processing)

### 2. Event Processing Flow

```
Channel<TEvent> → EventDispatcherService → IEventSubscriber<TEvent>
                                                  ↓
                                          Multiple Handlers (Parallel)
```

The background service continuously:
1. Reads events from channels using `await foreach`
2. Resolves all registered subscribers for that event type
3. Executes all subscribers in parallel with `Task.WhenAll`
4. Handles exceptions per subscriber (isolation)
5. Continues processing next event

### 3. Channel Architecture

Each event type gets its own dedicated channel:

```csharp
// OrderCreatedEvent → Channel<OrderCreatedEvent>
// OrderShippedEvent → Channel<OrderShippedEvent>
// PaymentProcessedEvent → Channel<PaymentProcessedEvent>
```

**Benefits:**
- Type safety at compile time
- Independent processing rates per event type
- No event type mixing or casting required
- Easy to monitor and scale per event type

### 4. Dependency Injection Configuration

```csharp
// Singleton - shared across the application lifetime
services.AddSingleton<IMessageQueue, ChannelMessageQueue>();
services.AddSingleton<IEventPublisher, EventPublisher>();

// Scoped - new instance per event processing
services.AddScoped<IEventSubscriber<OrderCreatedEvent>, OrderNotificationSubscriber>();

// Hosted Service - starts with application
services.AddHostedService<EventDispatcherService>();
```

## 🌐 API Endpoints

### Orders API

#### POST `/api/orders`
Creates a new order and publishes `OrderCreatedEvent`.

**Request Body:**
```json
{
  "orderId": "ORD-001",
  "amount": 299.99,
  "customerEmail": "customer@example.com"
}
```

**Response:**
```json
{
  "message": "Order created successfully",
  "orderId": "ORD-001"
}
```

**Triggered Subscribers:**
- `OrderNotificationSubscriber` - Logs order creation
- `EmailNotificationSubscriber` - Sends confirmation email

---

#### POST `/api/orders/{orderId}/ship`
Ships an order and publishes `OrderShippedEvent`.

**Request Body:**
```json
{
  "orderId": "ORD-001",
  "trackingNumber": "TRK-123456789"
}
```

**Response:**
```json
{
  "message": "Order shipped successfully",
  "trackingNumber": "TRK-123456789"
}
```

**Triggered Subscribers:**
- `ShippingSubscriber` - Logs shipping information

---

### Payments API

#### POST `/api/payments`
Processes a payment and publishes `PaymentProcessedEvent`.

**Request Body:**
```json
{
  "paymentId": "PAY-001",
  "amount": 299.99,
  "orderId": "ORD-001"
}
```

**Response:**
```json
{
  "message": "Payment processed successfully",
  "paymentId": "PAY-001"
}
```

**Triggered Subscribers:**
- `PaymentSubscriber` - Logs payment processing

## 🔨 Configuration

### Changing Channel Behavior

To use bounded channels instead of unbounded:

```csharp
public class EventChannel<TEvent> where TEvent : IEvent
{
    public EventChannel(int capacity = 100)
    {
        _channel = Channel.CreateBounded<TEvent>(new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait // or DropWrite, DropOldest
        });
    }
}
```

### Adding Logging Configuration

In `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Application.Services": "Debug",
      "Infrastructure.Messaging": "Debug"
    }
  }
}
```

## 🔄 Extending the Demo

### Adding a New Event

**1. Define the event in Domain layer:**
```csharp
public record InventoryUpdatedEvent(string ProductId, int Quantity) : BaseEvent;
```

**2. Create a subscriber in Application layer:**
```csharp
public class InventorySubscriber : IEventSubscriber<InventoryUpdatedEvent>
{
    private readonly ILogger<InventorySubscriber> _logger;

    public InventorySubscriber(ILogger<InventorySubscriber> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(InventoryUpdatedEvent @event, CancellationToken ct)
    {
        _logger.LogInformation("Inventory updated for {ProductId}: {Quantity} units", 
            @event.ProductId, @event.Quantity);
        await Task.CompletedTask;
    }
}
```

**3. Register the subscriber in Program.cs:**
```csharp
builder.Services.AddScoped<IEventSubscriber<InventoryUpdatedEvent>, InventorySubscriber>();
```

**4. Add processing in EventDispatcherService:**
```csharp
_processingTasks.Add(ProcessEventsAsync<InventoryUpdatedEvent>(stoppingToken));
```

**5. Publish from your controller:**
```csharp
var @event = new InventoryUpdatedEvent("PROD-123", 50);
await _eventPublisher.PublishAsync(@event);
```

### Adding Multiple Subscribers to One Event

```csharp
// Register multiple subscribers for the same event
builder.Services.AddScoped<IEventSubscriber<OrderCreatedEvent>, OrderNotificationSubscriber>();
builder.Services.AddScoped<IEventSubscriber<OrderCreatedEvent>, EmailNotificationSubscriber>();
builder.Services.AddScoped<IEventSubscriber<OrderCreatedEvent>, AnalyticsSubscriber>();
builder.Services.AddScoped<IEventSubscriber<OrderCreatedEvent>, InventorySubscriber>();
```

All subscribers will execute in parallel when the event is published.

## 📚 Best Practices

### 1. Keep Subscribers Focused
Each subscriber should have a single responsibility:
```csharp
// ✅ Good - focused responsibility
public class EmailNotificationSubscriber : IEventSubscriber<OrderCreatedEvent>

// ❌ Bad - too many responsibilities
public class OrderProcessingSubscriber : IEventSubscriber<OrderCreatedEvent>
```

### 2. Handle Exceptions Gracefully
The `EventDispatcherService` already handles exceptions per subscriber, but you should also add specific error handling:

```csharp
public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken ct)
{
    try
    {
        await SendEmailAsync(@event.CustomerEmail, @event.OrderId);
    }
    catch (SmtpException ex)
    {
        _logger.LogError(ex, "Failed to send email for order {OrderId}", @event.OrderId);
        // Consider adding retry logic or dead-letter queue
    }
}
```

### 3. Use Scoped Services
Register subscribers as scoped to ensure clean state per event:
```csharp
builder.Services.AddScoped<IEventSubscriber<T>, TSubscriber>();
```

### 4. Monitor Channel Metrics
Add monitoring for channel health:
```csharp
public class EventChannel<TEvent> where TEvent : IEvent
{
    public int Count => _channel.Reader.Count; // Current items in channel
    
    public ChannelMetrics GetMetrics()
    {
        return new ChannelMetrics
        {
            EventType = typeof(TEvent).Name,
            QueuedItems = _channel.Reader.Count
        };
    }
}
```

### 5. Use Bounded Channels in Production
For production systems, consider bounded channels to prevent memory issues:
```csharp
_channel = Channel.CreateBounded<TEvent>(new BoundedChannelOptions(1000)
{
    FullMode = BoundedChannelFullMode.Wait
});
```

## 🐛 Troubleshooting

### Events Not Being Processed

**Problem:** Events are published but subscribers aren't executing.

**Solution:** Ensure `EventDispatcherService` is registered and event type processing is added:
```csharp
builder.Services.AddHostedService<EventDispatcherService>();
// In EventDispatcherService.ExecuteAsync:
_processingTasks.Add(ProcessEventsAsync<YourEventType>(stoppingToken));
```

### Application Hangs on Shutdown

**Problem:** Application doesn't shut down gracefully.

**Solution:** Ensure cancellation tokens are properly propagated:
```csharp
public async Task HandleAsync(TEvent @event, CancellationToken ct)
{
    // Pass ct to all async operations
    await SomeAsyncOperation(ct);
}
```

### Memory Keeps Growing

**Problem:** Memory usage increases over time.

**Solution:** 
1. Use bounded channels instead of unbounded
2. Ensure subscribers complete quickly
3. Add monitoring to detect slow subscribers
4. Consider implementing backpressure mechanisms

### Subscribers Not Receiving Events

**Problem:** Some subscribers work, others don't.

**Solution:** Check DI registration - ensure all subscribers are registered:
```csharp
// Missing registration will prevent subscriber from being resolved
builder.Services.AddScoped<IEventSubscriber<OrderCreatedEvent>, MySubscriber>();
```

## 📊 Performance Considerations

### Channel Performance
- **Unbounded channels:** No backpressure, faster writes, higher memory usage
- **Bounded channels:** Backpressure control, slower writes when full, bounded memory

### Subscriber Performance
- Subscribers run in parallel using `Task.WhenAll`
- Slow subscribers don't block others
- Consider timeout policies for long-running subscribers

### Scaling Considerations
- Add multiple instances of `EventDispatcherService` for parallel processing
- Use distributed message brokers (RabbitMQ, Azure Service Bus) for multi-instance deployments
- Implement partition keys for ordered event processing

## 🎓 Learning Resources

- [System.Threading.Channels Documentation](https://learn.microsoft.com/en-us/dotnet/api/system.threading.channels)
- [Clean Architecture in ASP.NET Core](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
- [Background Services in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services)
- [Dependency Injection in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

## 📝 License

This is a demo project for educational purposes. Feel free to use and modify as needed.