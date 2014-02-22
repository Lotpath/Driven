using System;
using System.Collections.Generic;
using System.Linq;

namespace Driven.Bootstrapper
{
    public abstract class DrivenBootstrapperBase<TContainer> : IDrivenBootstrapper, IDrivenModuleResolver
    {
        private bool _initialized;
        private ModuleRegistration[] _modules;

        public void Initialize()
        {
            _initialized = true;

            ApplicationContainer = GetApplicationContainer();

            RegisterBootstrapperTypes(ApplicationContainer);

            ConfigureApplicationContainer(ApplicationContainer);

            RegisterModules(ApplicationContainer, Modules);
        }

        public DrivenModule Resolve(Type messageType)
        {
            var modules = GetAllModules(ApplicationContainer).Cast<DrivenModule>();
            var module = modules.SingleOrDefault(x => x.CanHandle(messageType));
            if (module == null)
                throw new DrivenException("No registered module found for handling message {0}", messageType);
            return module;
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

        protected abstract void RegisterBootstrapperTypes(TContainer applicationContainer);

        protected abstract IEnumerable<IDrivenModule> GetAllModules(TContainer container);

        protected abstract void RegisterModules(TContainer container, IEnumerable<ModuleRegistration> moduleRegistrationTypes);

        protected virtual IEnumerable<ModuleRegistration> Modules
        {
            get
            {
                return
                    _modules ??
                    (_modules = AppDomainAssemblyTypeScanner
                                        .TypesOf<IDrivenModule>(ScanMode.ExcludeDriven)
                                        .Select(t => new ModuleRegistration(t))
                                        .ToArray());
            }
        }
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