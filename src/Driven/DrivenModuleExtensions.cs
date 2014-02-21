﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Driven
{
    public static class DrivenModuleExtensions
    {
        public static void Transition<TSaga>(this IDrivenModule module, Guid sagaId, object message)
            where TSaga : class, ISaga, new()
        {
            var context = module.Context;
            var saga = GetSaga<TSaga>(context, sagaId);
            saga.Transition(message);
            context.SagaRepository.Save(saga, SequentialGuid.New(), headers =>
                {
                    headers.Add("ProcessedDateTimeUtc", SystemClock.UtcNow);
                    headers.Add("UserName", context.SecurityContext.UserName);
                });
        }

        private static TSaga GetSaga<TSaga>(DrivenContext context, Guid sagaId)
            where TSaga : class, ISaga, new()
        {
            return context.SagaRepository.GetById<TSaga>(sagaId);
        }

        public static void Execute<TAggregate>(this IDrivenModule module, Guid aggregateId, Action<TAggregate> action)
            where TAggregate : class, IAggregate
        {
            var context = module.Context;
            var aggregate = Get<TAggregate>(context, aggregateId);
            action(aggregate);
            Save(context, aggregate);
        }

        private static TAggregate Get<TAggregate>(DrivenContext context, Guid aggregateId)
            where TAggregate : class, IAggregate
        {
            return context.Repository.GetById<TAggregate>(aggregateId);
        }

        private static void Save<TAggregate>(DrivenContext context, TAggregate aggregate)
            where TAggregate : class, IAggregate
        {
            context.Repository.Save(aggregate, SequentialGuid.New(), headers =>
                {
                    headers.Add("ProcessedDateTimeUtc", SystemClock.UtcNow);
                    headers.Add("UserName", context.SecurityContext.UserName);
                });
        }

        public static void Validate(this IDrivenModule module, object command)
        {
            var context = module.Context;
            var errors = context.CommandValidator.Validate(command).ToList();
            if (errors.Any())
                throw new CommandValidationException(errors);
        }

        public static void RequireClaim(this IDrivenModule module, string requiredClaim)
        {
            var context = module.Context;
            if (!context.CurrentClaims().Any(x => x == requiredClaim))
                throw new DomainSecurityException(requiredClaim);
        }

        public static void RequireAllClaims(this IDrivenModule module, IEnumerable<string> requiredClaims)
        {
            var context = module.Context;
            var requiredClaimsList = requiredClaims.ToList();
            if (context.CurrentClaims().Intersect(requiredClaimsList).Count() != requiredClaimsList.Count())
                throw new DomainSecurityException(requiredClaimsList);
        }

        public static void RequireAnyClaims(this IDrivenModule module, IEnumerable<string> requiredClaims)
        {
            var context = module.Context;
            var claimsList = requiredClaims.ToList();
            if (!context.CurrentClaims().Intersect(claimsList).Any())
                throw new DomainSecurityException(claimsList);
        }

        internal static IList<string> CurrentClaims(this DrivenContext context)
        {
            return (context.SecurityContext.Claims ?? new string[0]).ToList();
        }
    }
}