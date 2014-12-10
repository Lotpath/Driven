using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Driven.Persistence.AtomEventStore
{
    public interface IPersister
    {
        Task PersistAsync<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate;
    }

    public interface IFetcher
    {
        TAggregate Fetch<TAggregate>(object id) where TAggregate : IAggregate;
    }

    public class AtomEventStoreRepository : IRepository
    {
        private readonly IPersister _persister;
        private readonly IFetcher _fetcher;
        private readonly IConstructAggregates _factory;
        private readonly IDetectConflicts _conflictDetector;

        public AtomEventStoreRepository(IPersister persister, IFetcher fetcher, IConstructAggregates factory, IDetectConflicts conflictDetector)
        {
            _persister = persister;
            _fetcher = fetcher;
            _factory = factory;
            _conflictDetector = conflictDetector;
        }

        public Task<TAggregate> GetAsync<TAggregate>(Guid id) where TAggregate : class, IAggregate
        {
            throw new NotImplementedException();
        }

        public Task<TAggregate> GetAsync<TAggregate>(Guid id, int version) where TAggregate : class, IAggregate
        {
            throw new NotImplementedException();
        }

        public Task Add(IAggregate aggregate, Action<IDictionary<string, object>> updateHeaders)
        {
            throw new NotImplementedException();
        }
    }
}
