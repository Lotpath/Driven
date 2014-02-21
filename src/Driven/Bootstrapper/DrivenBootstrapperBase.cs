using System;

namespace Driven.Bootstrapper
{
    public abstract class DrivenBootstrapperBase<TContainer> : IDrivenBootstrapper
    {
        private bool _initialized;

        public void Initialize()
        {
            _initialized = true;

            ApplicationContainer = GetApplicationContainer();

            ConfigureApplicationContainer(ApplicationContainer);
        }

        public IDrivenEngine GetEngine()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("Bootstrapper is not initialised. Call Initialize before GetEngine");
            }

            var engine = SafeGetDrivenEngineInstance();

            return engine;
        }

        protected TContainer ApplicationContainer { get; private set; }

        protected abstract TContainer GetApplicationContainer();

        protected abstract void ConfigureApplicationContainer(TContainer container);

        protected abstract IDrivenEngine GetEngineInternal();

        private IDrivenEngine SafeGetDrivenEngineInstance()
        {
            try
            {
                return GetEngineInternal();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Something went wrong when trying to satisfy one of the dependencies during composition, make sure that you've registered all new dependencies in the container and inspect the innerexception for more details.",
                    ex);
            }
        }
    }
}