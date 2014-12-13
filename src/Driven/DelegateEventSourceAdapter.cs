using System;
using System.Collections.Generic;

namespace Driven
{
    public class DelegateEventSourceAdapter : IEventSourceAdapter
    {
        private readonly Func<object, IEnumerable<object>> _appliedEvents;

        public DelegateEventSourceAdapter(Func<object, IEnumerable<object>> appliedEvents)
        {
            _appliedEvents = appliedEvents;
        }

        public IEnumerable<object> AppliedEvents(object aggregate)
        {
            return _appliedEvents(aggregate);
        }
    }
}