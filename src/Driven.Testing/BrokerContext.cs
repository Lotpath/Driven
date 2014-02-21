using System.Collections.Generic;

namespace Driven.Testing
{
    public class BrokerContext : IBrokerContextValues
    {
        public BrokerContext()
        {
            Values.Headers = new Dictionary<string, object>();
        }

        object IBrokerContextValues.Message { get; set; }

        IDictionary<string, object> IBrokerContextValues.Headers { get; set; }

        public void Message(object message)
        {
            Values.Message = message;
        }

        public void Header(string name, object value)
        {
            Values.Headers[name] = value;
        }

        private IBrokerContextValues Values
        {
            get { return this; }
        }
    }
}