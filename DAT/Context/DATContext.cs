using System.Linq;
using Autofac;
using DAT.Configuration;
using DAT.EventBus;
using DAT.EventBus.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DAT.Context
{
    public class DATContext
    {
        
        public static IContainer Container { get; private set; }

        /**
         * Initialize a new DAT context. This will try to use the appsettings.json in the local directory. If this file
         * is not found it will fall back to a default configuration.
         */
        public static void Bootstrap()
        {
            // Default location is appsettings.json in the local directory
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true);

            IConfiguration config = builder.Build();

            // If there are no childeren configuration load failed, so we manually load the default. 
            DATConfiguration configuration = config.GetSection("DAT").Get<DATConfiguration>();
            if (configuration == null)
            {
                configuration = new DATConfiguration();
            }
             
            ContainerBuilder containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterInstance(configuration).As<DATConfiguration>();
            
            BootstrapLogger(containerBuilder, configuration);
            BootstrapEventbus(containerBuilder, configuration);

            Container = containerBuilder.Build();
        }

        private static void BootstrapEventbus(ContainerBuilder builder, DATConfiguration configuration)
        {
            if (configuration.EventBus == null)
            {
                return;
            }

            switch (configuration.EventBus.Type)
            {
                    case "rabbitmq":
                        RabbitMQEventBus bus = new RabbitMQEventBus(configuration, configuration.EventBus);
                        builder.RegisterInstance(bus).As<IEventBus>();
                        break;
            }
        }

        private static void BootstrapLogger(ContainerBuilder builder, DATConfiguration configuration)
        {
            if (configuration.Logging == null)
            {
                return;
            }
            
            ILoggerFactory factory = new LoggerFactory()
                                             .AddConsole(configuration.Logging.LogLevel)
                                             .AddDebug(configuration.Logging.LogLevel);

            builder.Register((context, parameters) => factory.CreateLogger<DATContext>()).As<ILogger>();
        }
    }
}