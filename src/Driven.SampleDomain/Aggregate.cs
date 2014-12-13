using System.Collections.Generic;

namespace Driven.SampleDomain
{
    public abstract class Aggregate : Identifiable
    {
        private readonly IList<object> _appliedEvents;

        protected Aggregate()
        {
            _appliedEvents = new List<object>();
        }

        public IEnumerable<object> AppliedEvents { get { return _appliedEvents; } }

        protected void ApplyEvent(object e)
        {
            _appliedEvents.Add(e);
        }
    }
}