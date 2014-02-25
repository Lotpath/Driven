using System;
using System.Collections.Generic;
using System.Linq;
using Driven.Bootstrapper;
using NEventStore;

namespace Driven.Testing
{
    public interface IFetchLastCommit
    {
        Commit GetLastCommit();
    }

    public class ConfigurableBootstrapper : DefaultDrivenBootstrapper, IFetchLastCommit
    {
        private List<ModuleRegistration> _modules;
        private bool _allDiscoveredModules;

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
                if ((_modules ?? new List<ModuleRegistration>()).Any())
                {
                    return _modules;
                }

                return _allDiscoveredModules ? base.Modules : new ModuleRegistration[] { };
            }
        }

        Commit IFetchLastCommit.GetLastCommit()
        {
            return LastCommit;
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

            public ConfigurableBootstrapperConfigurator AllDiscoveredModules()
            {
                _bootstrapper._allDiscoveredModules = true;
                return this;
            }
        }        
    }
}