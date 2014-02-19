using System;

namespace Sample.Contract
{
    public interface ICommand
    {
        Guid CommandId { get; set; }
        Guid CorrelationId { get; set; }
    }
}
