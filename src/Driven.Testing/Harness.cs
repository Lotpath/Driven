using System;
using System.Collections.Generic;

namespace Driven.Testing
{
    public class Harness
    {
        private readonly ConfigurableBootstrapper _bootstrapper;
        private readonly IDrivenEngine _engine;

        public Harness(Action<ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator> configure)
            : this(new ConfigurableBootstrapper(configure))
        {
        }

        private Harness(ConfigurableBootstrapper bootstrapper)
        {
            _bootstrapper = bootstrapper;
            _bootstrapper.Initialize();
            _engine = _bootstrapper.GetEngine();
        }

        public Result When<TMessage>(TMessage message, Action<IDictionary<string, object>> headers = null)
        {
            try
            {
                if (headers == null)
                {
                    _engine.HandleMessage(message);
                }
                else
                {
                    var dictionary = new Dictionary<string, object>();
                    headers(dictionary);
                    _engine.HandleMessage(message, dictionary);
                }
                return new Result { Commit = _bootstrapper.GetLastCommit() };
            }
            catch (DomainException ex)
            {
                return new Result { ThrownException = ex };
            }
        }
    }
}