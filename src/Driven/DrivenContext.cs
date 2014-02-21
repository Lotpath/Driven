using System;
using System.Collections.Generic;

namespace Driven
{
    public interface IDrivenEngine
    {
        DrivenContext HandleMessage(Message message, Func<DrivenContext, DrivenContext> preRequest);
    }

    public class DrivenEngine : IDrivenEngine
    {
        private readonly IDrivenContextFactory _contextFactory;

        public DrivenEngine(IDrivenContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public DrivenContext HandleMessage(Message message, Func<DrivenContext, DrivenContext> preRequest)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message", "The message parameter cannot be null.");
            }

            var context = _contextFactory.Create(message);

            // todo: build up context dependencies

            return context;
        }
    }

    public class DrivenContext
    {
        public Message Message { get; set; }

        public ISecurityContext SecurityContext { get; set; }
    }

    public class Message
    {
        public Message(object body, IDictionary<string,object> headers = null)
        {
            Body = body;
            Headers = headers ?? new Dictionary<string, object>();
        }

        public object Body { get; private set; }
        public IDictionary<string, object> Headers { get; private set; }
    }

    public class Result
    {
    }
}