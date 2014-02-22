using System;
using Driven.Bootstrapper;

namespace Driven.Testing
{
    public class Harness
    {
        private readonly IDrivenBootstrapper _bootstrapper;
        private readonly IDrivenEngine _engine;

        public Harness(Action<ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator> configure)
            : this(new ConfigurableBootstrapper(configure))
        {            
        }

        public Harness(IDrivenBootstrapper bootstrapper)
        {
            _bootstrapper = bootstrapper;
            _bootstrapper.Initialize();
            _engine = _bootstrapper.GetEngine();
        }

        public void When<TMessage>(TMessage message)
        {
            _engine.HandleMessage(message);
        }
    }
}