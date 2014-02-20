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
        private readonly FakeSecurityContext _defaultSecurityContext;
        private IDispatchCommits _dispatcher = new NullDispatcher();
        private ICommandValidator _commandValidator;
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
            _defaultSecurityContext = new FakeSecurityContext();
            _commandValidator = new DataAnnotationsCommandValidator();

            _eventStore = Wireup
                .Init()
                .UsingInMemoryPersistence()
                .InitializeStorageEngine()
                .UsingSynchronousDispatchScheduler(new DelegateMessageDispatcher(c =>
                    {
                        _dispatcher.Dispatch(c);
                        c.Events.ForEach(m => DispatchedEvents.Add(m.Body));
                    }))
                .Build();

            _repository = new EventStoreRepository(_eventStore, new AggregateConstructor(), new ConflictDetector());
        }

        public TService ConstructService<TService>(Func<ICommandRequestContext, TService> serviceBuilder) 
            where TService : IApplicationService
        {
            var context = new CommandRequestContext(_repository, _defaultSecurityContext, _commandValidator);
            var service = serviceBuilder(context);
            return service;
        }

        public void Execute<TService>(Func<ICommandRequestContext, TService> serviceBuilder, object command)
            where TService : IApplicationService
        {
            try
            {
                var service = ConstructService(serviceBuilder);
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
            _defaultSecurityContext.UserName = userName;
        }

        public void ConfigureClaims(string[] claims)
        {
            _defaultSecurityContext.Claims = claims;
        }

        public void UseDispatcher(IDispatchCommits dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void UseValidator(ICommandValidator validator)
        {
            _commandValidator = validator;
        }

        private class FakeSecurityContext : ISecurityContext
        {
            public FakeSecurityContext()
            {
                UserName = "test";
                Claims = new string[0];
            }

            public string UserName { get; set; }
            public IEnumerable<string> Claims { get; set; }
        }
    }
}