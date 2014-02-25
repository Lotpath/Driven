using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Driven
{
    public class Message
    {
        public Message(object payload, IDictionary<string, object> headers)
        {
            Payload = payload;
            Headers = new ReadOnlyDictionary<string, object>(headers);
        }

        public object Payload { get; private set; }
        public IDictionary<string, object> Headers { get; private set; }
    }
}