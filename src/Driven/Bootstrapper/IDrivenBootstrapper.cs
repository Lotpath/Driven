using System;

namespace Driven.Bootstrapper
{
    public interface IDrivenBootstrapper : IDisposable
    {
        void Initialize();
        IDrivenEngine GetEngine();
    }
}