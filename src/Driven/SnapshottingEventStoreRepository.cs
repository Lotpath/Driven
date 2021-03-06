﻿using System;
using System.Collections.Generic;
using CommonDomain;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using NEventStore;

namespace Driven
{
    public class SnapshottingEventStoreRepository : IRepository
    {
        private readonly IRepository _repository;
        private readonly IStoreEvents _eventStore;

        public SnapshottingEventStoreRepository(IStoreEvents eventStore, IConstructAggregates aggregateConstructor, IDetectConflicts conflictDetector)
        {
            _repository = new EventStoreRepository(eventStore, aggregateConstructor, conflictDetector);
            _eventStore = eventStore;
        }

        public TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IAggregate
        {
            return _repository.GetById<TAggregate>(id);
        }

        public TAggregate GetById<TAggregate>(Guid id, int version) where TAggregate : class, IAggregate
        {
            return _repository.GetById<TAggregate>(id, version);
        }

        public void Save(IAggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders)
        {
            var newEventsCount = aggregate.GetUncommittedEvents().Count;
            _repository.Save(aggregate, commitId, updateHeaders);
            if (ShouldSnapshot(aggregate, newEventsCount))
                _eventStore.Advanced.AddSnapshot(new Snapshot(aggregate.Id, aggregate.Version, aggregate.GetSnapshot()));
        }

        private bool ShouldSnapshot(IAggregate aggregate, int newEventsCount)
        {
            var calculator = new SnapshotCalculator(50);
            return calculator.ShouldSnapshot(aggregate.Version - newEventsCount, aggregate.Version);
        }
    }
}
