using System;

namespace Driven.SampleDomain
{
    public class TenantId
    {
        public TenantId()
            : this(Guid.Empty)
        {
        }

        public TenantId(Guid identifier)
        {
            _identifier = "Tenant-" + identifier;
        }

        private readonly string _identifier;
        public string Identifier { get { return _identifier; } }

        public override string ToString()
        {
            return _identifier;
        }
    }
}