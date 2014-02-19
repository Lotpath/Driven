using System;

namespace Sample.Contract
{
    public interface IEvent
    {
        Guid EventId { get; }
        Guid CorrelationId { get; }
    }
}