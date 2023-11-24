using MassTransit;
using Payment.API.Consumers;
using Shared;
using Shared.Events;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMassTransit(configuration =>
{
    configuration.AddConsumer<StockReservedEventConsumer>();
    configuration.UsingRabbitMq((context, _configration) =>
    {
        _configration.Host(builder.Configuration["RabbitMQ"]);
        _configration.ReceiveEndpoint(RabbitMQSettings.Payment_StockReservedEventQueue, e => e.ConfigureConsumer<StockReservedEventConsumer>(context));
    });
});

var app = builder.Build();




app.Run();
