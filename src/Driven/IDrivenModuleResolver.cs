using System;

namespace Driven
{
    public interface IDrivenModuleResolver
    {
        DrivenModule Resolve(Type messageType);
    }
}