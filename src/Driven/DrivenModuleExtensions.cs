using System;
using System.Collections.Generic;
using System.Linq;

namespace Driven
{
    public static class DrivenModuleExtensions
    {
        public static void Transition<TSaga>(this IDrivenModule module, Guid sagaId)
            where TSaga : class, ISaga, new()
        {
            var context = module.Context;
            var saga = context.SagaRepository.GetById<TSaga>(sagaId);
            saga.Transition(context.Message.Payload);
            context.SagaRepository.Save(saga, SequentialGuid.New(), headers => ConfigureHeaders(context.Message.Headers, headers));
        }

        public static void Execute<TAggregate>(this IDrivenModule module, Guid aggregateId, Action<TAggregate> action)
            where TAggregate : class, IAggregate
        {
            var context = module.Context;
            var aggregate = context.Repository.GetById<TAggregate>(aggregateId);
            action(aggregate);
            context.Repository.Add(aggregate, SequentialGuid.New(), headers => ConfigureHeaders(context.Message.Headers, headers));
        }

        private static void ConfigureHeaders(IDictionary<string, object> source, IDictionary<string, object> target)
        {
            target.Add("ProcessedDateTime", SystemClock.UtcNow);

            foreach (var kv in source)
                target.Add(kv);
        }
    }
}