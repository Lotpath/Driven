using System;
using CommonDomain;
using CommonDomain.Persistence;

namespace Driven
{
    public class AggregateConstructor : IConstructAggregates
    {
        public IAggregate Build(Type type, Guid id, IMemento snapshot)
        {
            var rootEntityType = type.BaseType.GenericTypeArguments[0];
            if (snapshot == null)
            {
                snapshot = (IMemento)Activator.CreateInstance(rootEntityType, new object[0]);
                snapshot.Id = id;
                snapshot.Version = 0;
            }

            return (IAggregate)Activator.CreateInstance(type, new object[] { snapshot });
        }
    }
}