using MSPR_bloc_4_orders.Events;

namespace MSPR_bloc_4_orders.Services
{
    public class FakeRabbitMqPublisher : IRabbitMqPublisher
    {
        public Task PublishOrderCreated(OrderCreatedEvent orderEvent)
        {
            return Task.CompletedTask;
        }
    }
}
