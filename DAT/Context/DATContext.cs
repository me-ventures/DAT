using System.Linq;
using Autofac;
using DAT.Configuration;
using Microsoft.Extensions.Configuration;

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

            IConfigurationRoot config = builder.Build();

            // If there are no childeren configuration load failed, so we manually load the default. 
            DATConfiguration configuration;
            if (! config.GetChildren().Any())
            {
                configuration = new DATConfiguration();
            }
            else
            {
                configuration = config.GetValue<DATConfiguration>("DAT");
            }
             
            ContainerBuilder containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterInstance(configuration).As<DATConfiguration>();

            Container = containerBuilder.Build();
        }

        private static void BootstrapEventbus(ContainerBuilder builder, DATConfiguration configuration)
        {
            if (configuration.EventBus == null)
            {
                return;
            }    
            
            
        } 
    }
}