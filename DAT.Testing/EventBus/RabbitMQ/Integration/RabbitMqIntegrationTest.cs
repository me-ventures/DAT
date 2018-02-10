using DAT.EventBus;
using DAT.EventBus.RabbitMQ;
using Xunit;

namespace DAT.Testing.EventBus.RabbitMQ.Integration
{
    public class RabbitMqIntegrationTest
    {
        [Fact]
        public void InstanceCreationTest()
        {
            IEventBus bus = new RabbitMQEventBus();
            
            MessageTest test = new MessageTest{ Name = "Maikel"};
            
            bus.Publish("test1.test2", test);
        }
    }
    
    public class MessageTest
    {
        public string Name { get; set; }
    }
}