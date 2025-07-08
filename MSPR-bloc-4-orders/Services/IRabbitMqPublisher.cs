using System.Threading.Tasks;
using MSPR_bloc_4_orders.Events;

namespace MSPR_bloc_4_orders.Services
{
    public interface IRabbitMqPublisher
    {
        Task PublishOrderCreated(OrderCreatedEvent orderEvent);
    }
}
