using System;
using System.Collections;

namespace Driven
{
    public interface IAggregate
    {
        object Id { get; }
        int Version { get; }

        void ApplyEvent(object @event);
        ICollection GetUncommittedEvents();
        void ClearUncommittedEvents();

        Type RootEntityType { get; }
        IRootEntity GetRootEntity();
    }
}