using System;

namespace Driven.Persistence
{
    public interface IConstructAggregates
    {
        IAggregate Build(Type type, Guid id, IRootEntity rootEntity);
    }
}