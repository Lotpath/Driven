using System;
using System.Collections.Generic;

namespace Driven
{
    public interface IRepository
    {
        TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IAggregate;
        TAggregate GetById<TAggregate>(Guid id, int version) where TAggregate : class, IAggregate;
        void Add(IAggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders);
    }
}