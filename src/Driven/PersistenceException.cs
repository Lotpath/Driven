using System;

namespace Driven
{
    public class PersistenceException : Exception
    {
        public PersistenceException(string message)
            : base(message)
        {

        }
    }
}