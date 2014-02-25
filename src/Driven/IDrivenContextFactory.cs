using System.Collections.Generic;

namespace Driven
{
    public interface IDrivenContextFactory
    {
        DrivenContext Create(object payload, IDictionary<string, object> headers);
    }
}