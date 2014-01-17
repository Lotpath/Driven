using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;

namespace Driven
{
    public static class CommandRequestContextExtensions
    {
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

        public static void RequireClaim(this ICommandRequestContext requestContext, string claim)
        {
            if (!requestContext.SecurityContext.Claims.Any(x => x == claim))
                throw new DomainSecurityException(claim);
        }

        public static void RequireAllClaims(this ICommandRequestContext requestContext, IEnumerable<string> claims)
        {
            var claimsList = claims.ToList();
            if (requestContext.SecurityContext.Claims.Intersect(claimsList).Count() != claimsList.Count())
                throw new DomainSecurityException(claimsList);
        }

        public static void RequireAnyClaims(this ICommandRequestContext requestContext, IEnumerable<string> claims)
        {
            var claimsList = claims.ToList();
            if (!requestContext.SecurityContext.Claims.Intersect(claimsList).Any())
                throw new DomainSecurityException(claimsList);
        }
    }
}