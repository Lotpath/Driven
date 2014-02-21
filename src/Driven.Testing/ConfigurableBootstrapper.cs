using System;
using Driven.Bootstrapper;

namespace Driven.Testing
{
    public class ConfigurableBootstrapper : IDrivenBootstrapper
    {
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
    }
}