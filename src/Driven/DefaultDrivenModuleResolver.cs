using System;
using System.Collections.Generic;
using System.Linq;
using Driven.Bootstrapper;

namespace Driven
{
    public class DefaultDrivenModuleResolver : IDrivenModuleResolver
    {
        private IDictionary<Type, DrivenModule> _modules 
            = new Dictionary<Type, DrivenModule>();

        private IDictionary<Type, Type> _handlerToModuleMap 
            = new Dictionary<Type, Type>();

        public DefaultDrivenModuleResolver()
        {
            var moduleTypes = AppDomainAssemblyTypeScanner.TypesOf<IDrivenModule>()
                .Where(x=> !x.IsAbstract && !x.IsInterface);

            //todo: need a way to create modules from a container
            //so that domain services can be injected into modules

            foreach (var moduleType in moduleTypes)
            {
                var module = (DrivenModule)Activator.CreateInstance(moduleType, new object[0]);
                foreach (var handler in module.Handlers)
                {
                    _handlerToModuleMap.Add(handler.MessageType, moduleType);
                    _modules.Add(moduleType, module);
                }
            }
        }

        public DrivenModule Resolve(Type messageType)
        {
            var moduleType = _handlerToModuleMap[messageType];
            return _modules[moduleType];
        }
    }
}