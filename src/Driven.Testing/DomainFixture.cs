using System;
using System.Collections.Generic;
using CommonDomain.Core;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using NEventStore;
using NEventStore.Dispatcher;

namespace Driven.Testing
{
    public class DomainFixture
    {
        private readonly IStoreEvents _eventStore;
        private readonly IRepository _repository;
        private readonly FakeSecurityContext _securityContext;
        public Exception ThrownException = new Exception("No exception thrown");
        public IList<object> DispatchedEvents { get; private set; }

        public static DomainFixture Init()
        {
            var fixture = new DomainFixture();
            fixture.DispatchedEvents = new List<object>();
            return fixture;
        }

        private DomainFixture()
        {
            _securityContext = new FakeSecurityContext();

            _eventStore = Wireup
                .Init()
                .UsingInMemoryPersistence()
                .InitializeStorageEngine()
                .UsingSynchronousDispatchScheduler(new DelegateMessageDispatcher(c => c.Events.ForEach(m => DispatchedEvents.Add(m.Body))))
                .Build();

            _repository = new EventStoreRepository(_eventStore, new AggregateConstructor(), new ConflictDetector());
        }

        public void Execute<TService>(Func<ICommandRequestContext, TService> serviceBuilder, object command)
            where TService : IApplicationService
        {
            try
            {
                var context = new CommandRequestContext(_repository, _securityContext);
                var service = serviceBuilder(context);
                service.Execute(command);
            }
            catch (DomainSecurityException ex)
            {
                ThrownException = ex;
            }
            catch (CommandValidationException ex)
            {
                ThrownException = ex;
            }
            catch (DomainException ex)
            {
                ThrownException = ex;
            }
        }

        public void ConfigureUser(string userName)
        {
            _securityContext.UserName = userName;
        }

        public void ConfigureClaims(string[] claims)
        {
            _securityContext.Claims = claims;
        }

        private class FakeSecurityContext : ISecurityContext
        {
            public string UserName { get; set; }
            public IEnumerable<string> Claims { get; set; }
        }
    }
}