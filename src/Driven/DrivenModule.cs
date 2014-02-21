using System;
using System.Collections.Generic;

namespace Driven
{
    public abstract class DrivenModule : IDrivenModule
    {
        private readonly List<MessageHandler> _handlers = new List<MessageHandler>();

        public DrivenContext Context { get; set; }

        protected void Handle<TMessage>(Action<TMessage> handler)
        {
            _handlers.Add(new MessageHandler(typeof (TMessage), o => handler((TMessage) o)));
        }

        public IEnumerable<MessageHandler> Handlers { get { return _handlers; } } 
    }
}