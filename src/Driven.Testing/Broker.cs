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

        public BrokerResponse When<TMessage>(TMessage messageBody, Action<BrokerContext> brokerContext)
        {
            var message = CreateMessage(messageBody, brokerContext);

            var response = new BrokerResponse(_engine.HandleMessage(message, context => context), this);

            return response;
        }

        private Message CreateMessage(object message, Action<BrokerContext> brokerContext)
        {
            var context = new BrokerContext();

            brokerContext.Invoke(context);

            var contextValues = (IBrokerContextValues) context;

            return new Message(contextValues.Message, contextValues.Headers);
        }
    }
}