using System;
using Autofac;
using DAT.Configuration;
using DAT.Context;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DAT.Testing.Context
{
    public class BootstrapTest
    {

        [Fact]
        public void BootstrapDefaultTest()
        {
            DATContext.Bootstrap();

            DATConfiguration datConfiguration = DATContext.Container.Resolve<DATConfiguration>();
            
            Assert.Equal("Default-Name", datConfiguration.Name);
        }
    }
}