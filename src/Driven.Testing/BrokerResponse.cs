using System;

namespace Driven.Testing
{
    public class BrokerResponse
    {
        private readonly Broker _hostBroker;

        public BrokerResponse(DrivenContext context, Broker hostBroker)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "The value of the context parameter cannot be null.");
            }

            if (hostBroker == null)
            {
                throw new ArgumentNullException("hostBroker", "The value of the hostBroker parameter cannot be null.");
            }

            _hostBroker = hostBroker;

            Context = context;
        }

        public DrivenContext Context { get; private set; }

        public Broker Then
        {
            get { return _hostBroker; }
        }
    }
}