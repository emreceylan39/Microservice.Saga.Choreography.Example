using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Stock.API.Migrations;
using Stock.API.Models.Contexts;

namespace Stock.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        StockAPIDbContext _context;

        public PaymentFailedEventConsumer(StockAPIDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            //payment fail olduğunda compansable transaction
            var stocks = await _context.Stocks.ToListAsync();
            foreach(var orderItem in context.Message.OrderItems)
            {
                var stock = stocks.Find(s => s.ProductId == orderItem.ProductId);
                if (stock != null)
                {
                    stock.Quantity += orderItem.Quantity;
                    await _context.SaveChangesAsync();
                }

            }
        }
    }
}
