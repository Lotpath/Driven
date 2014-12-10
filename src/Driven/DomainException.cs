using System;
using System.Runtime.Serialization;

namespace Driven
{
    [Serializable]
    public class DomainException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DomainException() { }
        public DomainException(string message) : base(message) { }
        public DomainException(string format, params object[] args) : base(string.Format(format, args)) { }

        /// <summary>
        /// Creates domain error exception with a string name, that is easily identifiable in the tests
        /// </summary>
        /// <param name="name">The name to be used to identify this exception in tests.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static DomainException Named(string name, string format = null, params object[] args)
        {
            var message = "[" + name + "] " + string.Format(format ?? "", args);
            return new DomainException(message)
                {
                    Name = name
                };
        }

        public string Name { get; private set; }

        public DomainException(string message, Exception inner) : base(message, inner) { }

        protected DomainException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }
    }
}