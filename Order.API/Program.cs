using MassTransit;
using Order.API.Models.Contexts;
using Microsoft.EntityFrameworkCore;
using Order.API.Models.ViewModels;
using Order.API.Models;
using Shared.Events;
using Order.API.Consumers;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configuration =>
{
    configuration.AddConsumer<PaymentCompletedEventConsumer>();
    configuration.AddConsumer<PaymentFailedEventConsumer>();
    configuration.AddConsumer<StockNotReservedEventConsumer>();
    configuration.UsingRabbitMq((context, _configration) =>
    {
        _configration.Host(builder.Configuration["RabbitMQ"]);
        _configration.ReceiveEndpoint(RabbitMQSettings.Order_PaymentCompletedEventQueue, e => e.ConfigureConsumer<PaymentCompletedEventConsumer>(context));
        _configration.ReceiveEndpoint(RabbitMQSettings.Order_PaymentFaileddEventQueue, e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
        _configration.ReceiveEndpoint(RabbitMQSettings.Order_StockNotReservedEventQueue, e=> e.ConfigureConsumer<StockNotReservedEventConsumer>(context));
    });
});

builder.Services.AddDbContext<OrderAPIDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLServer")));

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/create-order", async (CreateOrderVM model, OrderAPIDbContext context, IPublishEndpoint publishEndpoint) =>
{
    Order.API.Models.Order order = new()
    {
        BuyerId = Guid.TryParse(model.BuyerId, out Guid _buyerId) ? _buyerId : Guid.NewGuid(),
        OrderItems = model.OrderItems.Select(oi => new OrderItem()
        {
            Quantity = oi.Quantity,
            Price = oi.Price,
            ProductId = Guid.Parse(oi.ProductId)
        }).ToList(),
        OrderStatus = Order.API.Enums.OrderState.Suspend,
        CreatedDate = DateTime.Now,
        TotalPrice = model.OrderItems.Sum(oi => oi.Price * oi.Quantity)
    };

    await context.Orders.AddAsync(order);
    await context.SaveChangesAsync();

    OrderCreatedEvent orderCreatedEvent = new()
    {
        BuyerId = order.BuyerId,
        OrderId = order.Id,
        TotalPrice = order.TotalPrice,
        OrderItems = order.OrderItems.Select(oi => new Shared.Messages.OrderItemMessage()
        {
            Quantity = oi.Quantity,
            Price = oi.Price,
            ProductId = oi.ProductId
        }).ToList()
    };

    await publishEndpoint.Publish(orderCreatedEvent);
});


app.Run();
