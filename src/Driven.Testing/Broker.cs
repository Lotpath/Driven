using System;
using Driven.Bootstrapper;

namespace Driven.Testing
{
    public class Broker
    {
        private readonly IDrivenBootstrapper _bootstrapper;
        private readonly IDrivenEngine _engine;

        public Broker(IDrivenBootstrapper bootstrapper)
        {
            _bootstrapper = bootstrapper;
            _bootstrapper.Initialize();
            _engine = _bootstrapper.GetEngine();
        }

        public BrokerResponse HandleMessage(Action<BrokerContext> BrokerContext)
        {
            var message = CreateMessage(BrokerContext);

            var response = new BrokerResponse(_engine.HandleMessage(message, context => context), this);

            return response;
        }

        private Message CreateMessage(Action<BrokerContext> BrokerContext)
        {
            var context = new BrokerContext();

            BrokerContext.Invoke(context);

            var contextValues = (IBrokerContextValues) context;

            return new Message(contextValues.Message, contextValues.Headers);
        }
    }
}