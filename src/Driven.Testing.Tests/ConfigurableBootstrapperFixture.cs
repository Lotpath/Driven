using System;
using System.Collections.Generic;
using System.Reflection;
using Driven.Bootstrapper;
using Xunit;

namespace Driven.Testing.Tests
{
    public class ConfigurableBootstrapperFixture
    {
        private readonly ConfigurableBootstrapper _bootstrapper;
        private readonly IDrivenEngine _engine;

        public ConfigurableBootstrapperFixture()
        {
            AppDomainAssemblyTypeScanner.AssembliesToScan = new List<Func<Assembly, bool>>
                {
                    a => a.GetName().Name == "Driven.Testing.Tests"
                };

            _bootstrapper = new ConfigurableBootstrapper(cfg =>
                {
                    cfg.AllDiscoveredModules();
                });
            _bootstrapper.Initialize();
            _engine = _bootstrapper.GetEngine();
        }

        [Fact]
        public void Should_be_able_to_auto_resolve_modules_in_the_testing_assembly()
        {
            _engine.HandleMessage(new FooMessage());
        }

        public class FooMessage
        {            
        }

        public class FooModule : DrivenModule
        {
            public FooModule()
            {
                Handle<FooMessage>(When);
            }   

            public void When(FooMessage m)
            {
                
            }
        }
    }
}