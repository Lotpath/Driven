using CommonDomain;
using CommonDomain.Core;

namespace Driven
{
    public abstract class AggregateBase<TRootEntity> : AggregateBase
        where TRootEntity : IRootEntity
    {
        protected TRootEntity RootEntity { get; private set; }

        protected AggregateBase(TRootEntity rootEntity)
        {
            RootEntity = rootEntity;
            Id = rootEntity.Id;
            Version = rootEntity.Version;
        }

        protected override IMemento GetSnapshot()
        {
            return RootEntity;
        }

        protected void Register<TEvent>()
        {
            Register<TEvent>(e => ((IRootEntity)RootEntity).Mutate(e));
        }
    }
}