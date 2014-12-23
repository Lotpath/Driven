using System;

namespace Driven
{
    public class PersistenceException : Exception
    {
        public PersistenceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}