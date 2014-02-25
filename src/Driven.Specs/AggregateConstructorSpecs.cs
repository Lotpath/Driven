using System;
using CommonDomain;
using CommonDomain.Core;
using SubSpec;
using Xunit;

namespace Driven.Specs
{
    public class AggregateConstructorSpecs
    {
        [Specification]
        public void ValidAggregateWithNullRootEntitySpecs()
        {
            var sut = default(AggregateConstructor);
            var id = Guid.NewGuid();
            var aggregate = default(IAggregate);

            "Given an aggregate constructor"
                .Context(() => { sut = new AggregateConstructor(); });

            "when constructing an aggregate with no memento"
                .Do(() => aggregate = sut.Build(typeof (ValidAggregate), id, null));

            "then aggregate is constructed"
                .Assert(() => Assert.NotNull(aggregate));

            "then aggregate id is set"
                .Assert(() => Assert.Equal(id, aggregate.Id));

            "then aggregate version is zero"
                 .Assert(() => Assert.Equal(0, aggregate.Version));
        }

        [Specification]
        public void ValidAggregateWithExistingRootEntitySpecs()
        {
            var sut = default(AggregateConstructor);
            var id = Guid.NewGuid();
            var aggregate = default(IAggregate);

            "Given an aggregate constructor"
                .Context(() => { sut = new AggregateConstructor(); });

            "when constructing an aggregate with an existing memento"
                .Do(() =>
                    {
                        var rootEntity = new ValidRootEntity();
                        var memento = ((IMemento) rootEntity);
                        memento.Id = id;
                        memento.Version = 5;
                        aggregate = sut.Build(typeof (ValidAggregate), id, rootEntity);
                    });

            "then aggregate is constructed"
                .Assert(() => Assert.NotNull(aggregate));

            "then aggregate id is set"
                .Assert(() => Assert.Equal(id, aggregate.Id));

            "then aggregate version is set to value from memento"
                 .Assert(() => Assert.Equal(5, aggregate.Version));
        }

        [Specification]
        public void AggregateMustInheritFromAggregateBaseSpecs()
        {
            var sut = default(AggregateConstructor);
            var id = Guid.NewGuid();
            var aggregate = default(IAggregate);

            "Given an aggregate constructor"
                .Context(() => { sut = new AggregateConstructor(); });

            "when constructing an aggregate with a type not inheriting from AggregateBase<T>"
                .Do(() =>
                {
                    aggregate = sut.Build(typeof(InvalidAggregate), id, null);
                });

            "then aggregate is not constructed"
                .Assert(() => Assert.Null(aggregate));
        }

        public class ValidAggregate : AggregateBase<ValidRootEntity>
        {
            public ValidAggregate(ValidRootEntity rootEntity)
                : base(rootEntity)
            {
            }
        }

        public class ValidRootEntity : RootEntityBase
        {            
        }

        public class InvalidAggregate : AggregateBase
        {            
        }
    }
}