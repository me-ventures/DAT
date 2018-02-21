using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using DAT.Configuration;
using DAT.EventBus;
using DAT.EventBus.RabbitMQ;
using DAT.Metrics;
using Microsoft.Reactive.Testing;
using Optional;
using Xunit;

namespace DAT.Testing.EventBus.RabbitMQ.Integration
{
    public class RabbitMqIntegrationTest
    {
        [Fact]
        public void InstanceDefaultParametersCreationTest()
        {
            IMetricsClient client = NSubstitute.Substitute.For<IMetricsClient>();
            
            IEventBus bus = new RabbitMQEventBus(new DATConfiguration(), client);
        }

        [Fact]
        public void PublishTest()
        {
            IMetricsClient client = NSubstitute.Substitute.For<IMetricsClient>();
            
            IEventBus bus = new RabbitMQEventBus(new DATConfiguration(), client);
            
            MessageTest test = new MessageTest{ Name = "Maikel"};
            
            bus.Publish("exchange.topic", test);
        }
        
        [Fact]
        public void GetTest()
        {
            IMetricsClient client = NSubstitute.Substitute.For<IMetricsClient>();
            
            IEventBus bus = new RabbitMQEventBus(new DATConfiguration(), client);
            
            MessageTest test = new MessageTest{ Name = "Maikel"};
            
            bus.Publish("exchange.topic-get", test);

            Option<MessageTest> returnMessage = bus.Get<MessageTest>("exchange.topic-get");
            
            Assert.True(returnMessage.HasValue);
            Assert.True(returnMessage.Exists(x => x.Name == "Maikel"));
            
        }

        [Fact]
        public void ObservableSubscribeTest()
        {
            IMetricsClient client = NSubstitute.Substitute.For<IMetricsClient>();
            
            IEventBus bus = new RabbitMQEventBus(new DATConfiguration(), client);
            
            MessageTest test = new MessageTest{ Name = "Maikel"};
            
            bus.Publish("exchange.subscribe-test", test);
            bus.Publish("exchange.subscribe-test", test);
            bus.Publish("exchange.subscribe-test", test);

            
            IObservable<MessageTest> subscription = bus.Subscribe<MessageTest>("exchange.subscribe-test");
           

            subscription.Do(messageTest => Assert.Equal("Maikel", messageTest.Name)).Subscribe();
        }
        
        [Fact]
        public void HandlerSubscribeTest()
        {
            IMetricsClient client = NSubstitute.Substitute.For<IMetricsClient>();
            
            IEventBus bus = new RabbitMQEventBus(new DATConfiguration(), client);
            
            MessageTest test = new MessageTest{ Name = "Maikel"};
            
            bus.Publish("exchange.subscribe-test-handler", test);
            bus.Publish("exchange.subscribe-test-handler", test);
            bus.Publish("exchange.subscribe-test-handler", test);


            int index = 0;
            
            bus.Subscribe<MessageTest>("exchange.subscribe-test-handler", messageTest =>
                {
                    Assert.Equal("Maikel", messageTest.Name);

                    index++;
                    
                    return true;
                });
           
            Thread.Sleep(500);
            
            Assert.Equal(3, index);
        }
    }
    
    public class MessageTest
    {
        public string Name { get; set; }
    }
}