using System.Collections.Generic;

namespace Driven
{
    public class DrivenEngine : IDrivenEngine
    {
        private readonly IDrivenModuleResolver _moduleResolver;
        private readonly IDrivenContextFactory _contextFactory;

        public DrivenEngine(IDrivenModuleResolver moduleResolver, IDrivenContextFactory contextFactory)
        {
            _moduleResolver = moduleResolver;
            _contextFactory = contextFactory;
        }

        public void HandleMessage<TMessage>(TMessage message)
        {
            // todo: need a way to determine the security context per message

            var module = _moduleResolver.Resolve(message.GetType());
            var context = _contextFactory.Create(new EmptySecurityContext());

            module.Context = context;

            var handler = ResolveHandler<TMessage>(module.Handlers);
            handler.Handler.Invoke(message);
        }

        private MessageHandler ResolveHandler<TMessage>(IEnumerable<MessageHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                if (handler.MessageType == typeof (TMessage))
                    return handler;
            }
            throw new DrivenException("No handler found for message {0}", typeof(TMessage));
        }
    }
}