using System.Collections.Generic;

namespace Driven
{
    public interface IDrivenEngine
    {
        void HandleMessage<TMessage>(TMessage message, IDictionary<string, object> headers = null);
    }
}