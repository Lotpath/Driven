using System.Collections.Generic;

namespace Driven.Testing
{
    public interface IBrokerContextValues : IHideObjectMembers
    {
        object Message { get; set; }
        IDictionary<string, object> Headers { get; set; }
    }
}