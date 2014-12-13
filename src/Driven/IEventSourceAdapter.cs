using System.Collections.Generic;

namespace Driven
{
    public interface IEventSourceAdapter
    {
        IEnumerable<object> AppliedEvents(object aggregate);
    }
}