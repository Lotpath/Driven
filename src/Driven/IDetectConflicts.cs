using System.Collections.Generic;

namespace Driven
{
    public delegate bool ConflictDelegate(object uncommitted, object committed);

    public interface IDetectConflicts
    {
        void Register<TUncommitted, TCommitted>(ConflictDelegate handler)
            where TUncommitted : class
            where TCommitted : class;

        bool ConflictsWith(IEnumerable<object> uncommittedEvents, IEnumerable<object> committedEvents);
    }
}