using MSPR_bloc_4_orders.Events;
using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MSPR_bloc_4_orders.Services
{
    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly Producer _producer;

        public RabbitMqPublisher(IConfiguration configuration)
        {
            var config = new StreamSystemConfig
            {
                UserName = "guest",
                Password = "guest",
                Endpoints = { new System.Net.IPEndPoint(System.Net.Dns.GetHostAddresses("rabbitmq")[0], 5552) }
            };

            var system = StreamSystem.Create(config).GetAwaiter().GetResult();

            var streamName = "order_stream";
            if (!system.StreamExists(streamName).GetAwaiter().GetResult())
            {
                system.CreateStream(new StreamSpec(streamName)).GetAwaiter().GetResult();
            }

            _producer = Producer.Create(new ProducerConfig(system, streamName)).GetAwaiter().GetResult();
        }

        public virtual async Task PublishOrderCreated(OrderCreatedEvent orderEvent)
        {
            var json = JsonSerializer.Serialize(orderEvent);
            var message = new Message(Encoding.UTF8.GetBytes(json));
            await _producer.Send(message);
        }
    }
}
