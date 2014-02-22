using System;
using System.Collections.Generic;
using System.Linq;
using Driven.Bootstrapper;
using NEventStore;
using NEventStore.Dispatcher;

namespace Driven.Testing
{
    public class ConfigurableBootstrapper : DefaultDrivenBootstrapper
    {
        private List<ModuleRegistration> _modules;
        private Action<Commit> _dispatcher = c => { };

        public ConfigurableBootstrapper()
            : this(null)
        {
        }

        public ConfigurableBootstrapper(Action<ConfigurableBootstrapperConfigurator> configuration)
        {
            if (configuration != null)
            {
                var configurator = new ConfigurableBootstrapperConfigurator(this);

                configuration.Invoke(configurator);
            }
        }

        protected override IEnumerable<ModuleRegistration> Modules
        {
            get
            {
                return _modules ?? base.Modules;
            }
        }

        protected override IDispatchCommits ConfigureDispatcher()
        {
            return new DelegateMessageDispatcher(_dispatcher);
        }

        public class ConfigurableBootstrapperConfigurator
        {
            private readonly ConfigurableBootstrapper _bootstrapper;

            public ConfigurableBootstrapperConfigurator(ConfigurableBootstrapper bootstrapper)
            {
                _bootstrapper = bootstrapper;
            }

            public ConfigurableBootstrapperConfigurator Modules(params Type[] modules)
            {
                _bootstrapper._modules = modules.Select(x => new ModuleRegistration(x)).ToList();
                return this;
            }

            public ConfigurableBootstrapperConfigurator Dispatcher(Action<Commit> dispatcher)
            {
                _bootstrapper._dispatcher = dispatcher;
                return this;
            }
        }
    }
}