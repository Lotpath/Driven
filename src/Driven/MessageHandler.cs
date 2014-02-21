using System;

namespace Driven
{
    public class MessageHandler
    {
        public MessageHandler(Type messageType, Action<object> handler)
        {
            MessageType = messageType;
            Handler = handler;
        }

        public Type MessageType { get; private set; }
        public Action<object> Handler { get; private set; } 
    }
}