namespace DAT.EventBus.RabbitMQ
{
    public class RabbitMQConnectionOptions : ConnectionOptions
    {
        public string VirtualHost { get; set; }
    }
}