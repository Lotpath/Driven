using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using Driven.Bootstrapper;
using NEventStore;
using NEventStore.Dispatcher;
using TinyIoC;

namespace Driven
{
    public class DefaultDrivenBootstrapper : DrivenBootstrapperBase<TinyIoCContainer>
    {
        protected override TinyIoCContainer GetApplicationContainer()
        {
            return new TinyIoCContainer();
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register<IDispatchCommits>((c, o) => new DelegateMessageDispatcher(commit => { }));
            container.Register((c, o) => ConfigureEventStore());
            container.Register(typeof (IConstructAggregates), typeof (AggregateConstructor));
            container.Register(typeof (IDetectConflicts), typeof (ConflictDetector));
            container.Register(typeof (IRepository), typeof (EventStoreRepository));
            container.Register(typeof(ISagaRepository), typeof(SagaEventStoreRepository));
            container.Register(typeof(ICommandValidator), typeof(DataAnnotationsCommandValidator));
            container.Register(typeof(ISecurityContext), typeof(EmptySecurityContext));
            container.Register(typeof(IDrivenModuleResolver), typeof(DefaultDrivenModuleResolver));
            container.Register(typeof(IDrivenContextFactory), typeof(DefaultDrivenContextFactory));
            container.Register(typeof(IDrivenEngine), typeof(DrivenEngine));
        }

        protected override IDrivenEngine GetEngineInternal()
        {
            return ApplicationContainer.Resolve<IDrivenEngine>();
        }

        protected virtual IStoreEvents ConfigureEventStore()
        {
            return Wireup
                .Init()
                .UsingInMemoryPersistence()
                .InitializeStorageEngine()
                .UsingAsynchronousDispatchScheduler(ApplicationContainer.Resolve<IDispatchCommits>())
                .Build();
        }
    }
}