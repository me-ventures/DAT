using System;
using Autofac;
using DAT.Configuration;
using DAT.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DAT.Testing.Context
{
    public class BootstrapTest
    {

        [Fact]
        public void BootstrapDefaultTest()
        {
            bool eventHandlerCalled = false;

            DATContext.PreContainerBuild += (sender, builder) => { eventHandlerCalled = true; };
            
            DATContext.Bootstrap();

            DATConfiguration datConfiguration = DATContext.Container.Resolve<DATConfiguration>();
            
            Assert.Equal("test-service", datConfiguration.Name);

            ILogger logger = DATContext.Container.Resolve<ILogger>();
            
            Assert.NotNull(logger);
            Assert.True(eventHandlerCalled);
        }
    }
}