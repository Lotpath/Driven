using System;

namespace Driven.Bootstrapper
{
    public sealed class ServiceRegistration
    {
        public ServiceRegistration(Type serviceType)
        {
            ServiceType = serviceType;
        }

        public Type ServiceType { get; private set; }
    }
}