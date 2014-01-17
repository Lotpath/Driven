using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Driven
{
    [Serializable]
    public class DomainSecurityException : Exception
    {
        public DomainSecurityException(IEnumerable<string> requiredClaims) :
            this(string.Join(",", requiredClaims))
        {
        }

        public DomainSecurityException(string requiredClaim)
            : base(string.Format("The following claims are required for this operation: {0}", requiredClaim))
        {
        }

        protected DomainSecurityException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }
    }
}