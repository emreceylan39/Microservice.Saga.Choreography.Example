using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Events;
using Stock.API.Models.Contexts;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        StockAPIDbContext _context;
        ISendEndpointProvider _sendEndpointProvider;
        IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(StockAPIDbContext context, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();
            List<Models.Stock> stocks = await _context.Stocks.ToListAsync();

            foreach (var orderItems in context.Message.OrderItems)
            {
                Models.Stock? stock = stocks.Find(s => s.ProductId == orderItems.ProductId && s.Quantity >= orderItems.Quantity);
                if (stock is not null)
                    stockResult.Add(true);
                else
                    stockResult.Add(false);

            }

            if (stockResult.TrueForAll(s => s.Equals(true)))
            {
                // aranan tüm ürünler için stok uygun stok güncelle
                foreach (var orderItems in context.Message.OrderItems)
                {
                    Models.Stock stock = await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == orderItems.ProductId);

                    stock.Quantity -= orderItems.Quantity;

                    await _context.SaveChangesAsync();
                }
                // payment a event fırlat
                // kuyruk ad belirtelerek doğrudan kuyruğa ekleme
                //var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Payment_StockReservedEventQueue}"));


                StockReservedEvent stockReservedEvent = new()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    TotalPrice = context.Message.TotalPrice,
                    OrderItems = context.Message.OrderItems
                };

                await _publishEndpoint.Publish(stockReservedEvent);

            }
            else
            {
                //stokta olmayan ürünler var siparişi iptal et
                // order api ı uyaracak event fırlat
                StockNotReservedEvent stockNotReservedEvent = new()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    Message = "Stok miktarı yetersiz."
                };

                await _publishEndpoint.Publish(stockNotReservedEvent);
            }
        }
    }
}
