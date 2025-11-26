using ChannelDemo.Application.Interfaces;
using ChannelDemo.Application.Services;
using ChannelDemo.Domain.Events;
using ChannelDemo.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register message queue as singleton
builder.Services.AddSingleton<IMessageQueue, ChannelMessageQueue>();

// Register event publisher
builder.Services.AddSingleton<IEventPublisher, EventPublisher>();

// Register subscribers as scoped (new instance per event)
builder.Services.AddScoped<IEventSubscriber<OrderCreatedEvent>, OrderNotificationSubscriber>();
builder.Services.AddScoped<IEventSubscriber<OrderCreatedEvent>, EmailNotificationSubscriber>();
builder.Services.AddScoped<IEventSubscriber<OrderShippedEvent>, ShippingSubscriber>();
builder.Services.AddScoped<IEventSubscriber<PaymentProcessedEvent>, PaymentSubscriber>();

// Register background service
builder.Services.AddHostedService<EventDispatcherService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Logger.LogInformation("Application started. Event dispatcher is running in the background.");

app.Run();