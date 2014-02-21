using System;
using System.Collections.Generic;

namespace Driven
{
    public interface IDrivenServiceCatalog
    {
        IEnumerable<IApplicationService> GetAllServices(DrivenContext context);
        IApplicationService GetService(Type serviceType, DrivenContext context);
    }
}