using System;
using System.Collections.Generic;
using System.Linq;
using Driven.Bootstrapper;

namespace Driven.Testing
{
    public class ConfigurableBootstrapper : IDrivenBootstrapper
    {
        private readonly List<object> registeredTypes;

        public ConfigurableBootstrapper()
            : this(null)
        {
        }

        public ConfigurableBootstrapper(Action<ConfigurableBootstrapperConfigurator> configuration)
        {
            if (configuration != null)
            {
                var configurator =
                    new ConfigurableBootstrapperConfigurator(this);

                configuration.Invoke(configurator);
            }

        }

        public void Dispose()
        {
        }

        public void Initialize()
        {
        }

        public IDrivenEngine GetEngine()
        {
            var factory = new DefaultDrivenContextFactory();
            return new DrivenEngine(factory);
        }

        public class ConfigurableBootstrapperConfigurator
        {
            private readonly ConfigurableBootstrapper _bootstrapper;

            public ConfigurableBootstrapperConfigurator(ConfigurableBootstrapper bootstrapper)
            {
                _bootstrapper = bootstrapper;
            }

            public ConfigurableBootstrapperConfigurator Services(params Type[] services)
            {
                var serviceRegistrations =
                    from service in services
                    select new ServiceRegistration(service);

                _bootstrapper.registeredTypes.AddRange(serviceRegistrations);

                return this;
            }
        }
    }
}