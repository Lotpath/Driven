using System;
using System.Runtime.Serialization;

namespace Driven
{
    [Serializable]
    public class DrivenException : Exception
    {
        public DrivenException(string format, params object[] args) : base(string.Format(format, args)) { }

        protected DrivenException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }
    }
}