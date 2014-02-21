using System;

namespace Driven.Bootstrapper
{
    public interface IServiceRegister
    {
        IServiceRegister Register<TService>(Func<IServiceProvider, TService> serviceCreator) where TService : class;
        IServiceRegister Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;
    }
}