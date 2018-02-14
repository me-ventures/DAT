using DAT.Configuration;
using DAT.EventBus;
using DAT.EventBus.RabbitMQ;
using Optional;
using Xunit;

namespace DAT.Testing.EventBus.RabbitMQ.Integration
{
    public class RabbitMqIntegrationTest
    {
        [Fact]
        public void InstanceDefaultParametersCreationTest()
        {
            IEventBus bus = new RabbitMQEventBus(new DATConfiguration());
        }

        [Fact]
        public void PublishTest()
        {
            IEventBus bus = new RabbitMQEventBus(new DATConfiguration());
            
            MessageTest test = new MessageTest{ Name = "Maikel"};
            
            bus.Publish("exchange.topic", test);
        }
        
        [Fact]
        public void GetTest()
        {
            IEventBus bus = new RabbitMQEventBus(new DATConfiguration());
            
            MessageTest test = new MessageTest{ Name = "Maikel"};
            
            bus.Publish("exchange.topic-get", test);

            Option<MessageTest> returnMessage = bus.Get<MessageTest>("exchange.topic-get");
            
            Assert.True(returnMessage.HasValue);
            Assert.True(returnMessage.Exists(x => x.Name == "Maikel"));
            
        }
    }
    
    public class MessageTest
    {
        public string Name { get; set; }
    }
}