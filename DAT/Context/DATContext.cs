using System;
using System.Linq;
using Autofac;
using DAT.Configuration;
using DAT.EventBus;
using DAT.EventBus.RabbitMQ;
using DAT.Metrics;
using DAT.Metrics.Mock;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DAT.Context
{
    public class DATContext
    {
        
        public static IContainer Container { get; private set; }

        /// <summary>
        /// This event will be fired before the final container build is started and be used to register custom services
        /// and components. 
        /// </summary>
        public static event EventHandler<ContainerBuilder> PreContainerBuild;

        /// <summary>
        /// Initialize a new DAT context. This will try to use the appsettings.json in the local directory. If this file
        /// is not found it will fall back to a default configuration. 
        /// </summary>
        public static void Bootstrap()
        {
            Bootstrap<Configuration.Configuration>();
        }

        /// <summary>
        /// Initialize a new DAT context. This will try to use the appsettings.json in the local directory. If this file
        /// is not found it will fall back to a default configuration.
        /// </summary>
        /// <typeparam name="T">Type of the configuration file, must inherit from Configuration.Configuration</typeparam>
        public static void Bootstrap<T>() where T : Configuration.Configuration
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();

            DATConfiguration configuration = BootstrapConfiguration<T>(containerBuilder);
            
            BootstrapMetrics(containerBuilder, configuration);
            BootstrapLogger(containerBuilder, configuration);
            BootstrapEventbus(containerBuilder, configuration);
            
            OnPreContainerBuild(containerBuilder);

            Container = containerBuilder.Build();
        }

        private static DATConfiguration BootstrapConfiguration<T>(ContainerBuilder containerBuilder)
        {
            // Default location is appsettings.json in the local directory
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true);

            IConfiguration config = builder.Build();

            // If there are no childeren configuration load failed, so we manually load the default.
            Configuration.Configuration fullConfig = config.Get<Configuration.Configuration>();
            DATConfiguration configuration = config.GetSection("DAT").Get<DATConfiguration>();
            if (configuration == null)
            {
                configuration = new DATConfiguration();
            }
            
            containerBuilder.RegisterInstance(configuration).As<DATConfiguration>();
            containerBuilder.RegisterInstance(fullConfig).As<T>();

            return configuration;
        }

        private static void BootstrapMetrics(ContainerBuilder containerBuilder, DATConfiguration configuration)
        {
            MockMetricsClient client = new MockMetricsClient();
            containerBuilder.RegisterInstance(client).As<IMetricsClient>();
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
                        
                        builder.Register(c => new RabbitMQEventBus(configuration, configuration.EventBus, c.Resolve<IMetricsClient>()))
                               .As<IEventBus>()
                               .SingleInstance();
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

        private static void OnPreContainerBuild(ContainerBuilder builder)
        {
            EventHandler<ContainerBuilder> eh = PreContainerBuild;

            eh?.Invoke(null, builder);
        }
    }
}