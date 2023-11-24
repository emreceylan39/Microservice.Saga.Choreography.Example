using MassTransit;
using Shared.Events;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {

            //ödeme başarılı yada başarısızmış gibi yapıyoruz.

            //aşağıdaki if i false a çekerek compansable transaction u test edebiliriz.
            if (true)
            {
                //ödeme başarılı..
                PaymentCompletedEvent paymentCompletedEvent = new()
                {
                    OrderId = context.Message.OrderId
                };
                await _publishEndpoint.Publish(paymentCompletedEvent);
            }
            else
            {
                //ödeme başarısız
                PaymentFailedEvent paymentFailedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    Message="Ödeme başarısız oldu..",
                    OrderItems = context.Message.OrderItems
                };

                await _publishEndpoint.Publish(paymentFailedEvent);
            }
        }
    }
}
