using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Stock.API.Consumers;
using Stock.API.Models.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StockAPIDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLServer")));

builder.Services.AddMassTransit(configuration =>
{
    configuration.AddConsumer<OrderCreatedEventConsumer>();
    configuration.AddConsumer<PaymentFailedEventConsumer>();
    configuration.UsingRabbitMq((context, _configration) =>
    {
        _configration.Host(builder.Configuration["RabbitMQ"]);
        _configration.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue, e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
        _configration.ReceiveEndpoint(RabbitMQSettings.Stock_PaymentFaileddEventQueue, e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
    });
});


var app = builder.Build();


app.Run();
