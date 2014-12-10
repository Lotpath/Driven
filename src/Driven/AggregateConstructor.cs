using System;
using Driven.Persistence;

namespace Driven
{
    public class AggregateConstructor : IConstructAggregates
    {
        public IAggregate Build(Type type, Guid id, IRootEntity rootEntity)
        {
            var rootEntityType = type.BaseType.GenericTypeArguments[0];
            if (rootEntity == null)
            {
                rootEntity = (IRootEntity)Activator.CreateInstance(rootEntityType, new object[0]);
                rootEntity.Id = id;
                rootEntity.Version = 0;
            }

            return (IAggregate)Activator.CreateInstance(type, new object[] { rootEntity });
        }
    }
}