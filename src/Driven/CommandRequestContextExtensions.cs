using System;
using System.Collections.Generic;
using System.Linq;

namespace Driven
{
    public static class CommandRequestContextExtensions
    {
        public static void Transition<TSaga>(this ICommandRequestContext context, Guid sagaId, object message)
            where TSaga : class, ISaga, new()
        {
            var saga = GetSaga<TSaga>(context, sagaId);
            saga.Transition(message);
            context.SagaRepository.Save(saga, SequentialGuid.New(), headers =>
                {
                    headers.Add("ProcessedDateTimeUtc", SystemClock.UtcNow);
                    headers.Add("UserName", context.SecurityContext.UserName);
                });
        }

        private static TSaga GetSaga<TSaga>(ICommandRequestContext context, Guid sagaId)
            where TSaga : class, ISaga, new()
        {
            return context.SagaRepository.GetById<TSaga>(sagaId);
        }

        public static void Execute<TAggregate>(this ICommandRequestContext context, Guid aggregateId, Action<TAggregate> action)
            where TAggregate : class, IAggregate
        {
            var aggregate = Get<TAggregate>(context, aggregateId);
            action(aggregate);
            Save(context, aggregate);
        }

        private static TAggregate Get<TAggregate>(ICommandRequestContext context, Guid aggregateId)
            where TAggregate : class, IAggregate
        {
            return context.Repository.GetById<TAggregate>(aggregateId);
        }

        private static void Save<TAggregate>(ICommandRequestContext context, TAggregate aggregate)
            where TAggregate : class, IAggregate
        {
            context.Repository.Save(aggregate, SequentialGuid.New(), headers =>
                {
                    headers.Add("ProcessedDateTimeUtc", SystemClock.UtcNow);
                    headers.Add("UserName", context.SecurityContext.UserName);
                });
        }

        public static void Validate(this ICommandRequestContext requestContext, object command)
        {
            var errors = requestContext.CommandValidator.Validate(command).ToList();
            if (errors.Any())
                throw new CommandValidationException(errors);
        }

        public static void RequireClaim(this ICommandRequestContext requestContext, string requiredClaim)
        {
            if (!requestContext.CurrentClaims().Any(x => x == requiredClaim))
                throw new DomainSecurityException(requiredClaim);
        }

        public static void RequireAllClaims(this ICommandRequestContext requestContext, IEnumerable<string> requiredClaims)
        {
            var requiredClaimsList = requiredClaims.ToList();
            if (requestContext.CurrentClaims().Intersect(requiredClaimsList).Count() != requiredClaimsList.Count())
                throw new DomainSecurityException(requiredClaimsList);
        }

        public static void RequireAnyClaims(this ICommandRequestContext requestContext, IEnumerable<string> requiredClaims)
        {
            var claimsList = requiredClaims.ToList();
            if (!requestContext.CurrentClaims().Intersect(claimsList).Any())
                throw new DomainSecurityException(claimsList);
        }

        internal static IList<string> CurrentClaims(this ICommandRequestContext requestContext)
        {
            return (requestContext.SecurityContext.Claims ?? new string[0]).ToList();
        }
    }
}