using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Driven.Persistence
{
    public interface IRepository
    {
        Task<TAggregate> GetAsync<TAggregate>(Guid id) where TAggregate : class, IAggregate;
        Task<TAggregate> GetAsync<TAggregate>(Guid id, int version) where TAggregate : class, IAggregate;
        Task Add(IAggregate aggregate, Action<IDictionary<string, object>> updateHeaders);
    }
}