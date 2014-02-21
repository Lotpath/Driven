using System.Collections.Generic;

namespace Driven
{
    public class EmptySecurityContext : ISecurityContext
    {
        public EmptySecurityContext()
        {
            UserName = "";
            Claims = new string[0];
        }

        public string UserName { get; private set; }
        public IEnumerable<string> Claims { get; private set; }
    }
}