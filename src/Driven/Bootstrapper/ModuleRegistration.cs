using System;

namespace Driven.Bootstrapper
{
    public sealed class ModuleRegistration
    {
        public ModuleRegistration(Type moduleType)
        {
            ModuleType = moduleType;
        }

        public Type ModuleType { get; private set; }
    }
}