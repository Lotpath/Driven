using System.Collections.Generic;

namespace Driven
{
    public interface ISecurityContext
    {
        string UserName { get; }
        IEnumerable<string> Claims { get; }
    }
}