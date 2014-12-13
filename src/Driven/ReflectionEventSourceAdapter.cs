using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Driven
{
    public class ReflectionEventSourceAdapter : IEventSourceAdapter
    {
        private static readonly Dictionary<Type, Func<object, IEnumerable<object>>> EventSources
            = new Dictionary<Type, Func<object, IEnumerable<object>>>();

        public IEnumerable<object> AppliedEvents(object aggregate)
        {
            var eventSource = GetOrAddSource(aggregate.GetType());
            return eventSource(aggregate);
        }

        private Func<object, IEnumerable<object>> GetOrAddSource(Type type)
        {
            if (!EventSources.ContainsKey(type))
            {
                var prop = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                               .SingleOrDefault(x => x.Name == "AppliedEvents"
                                                     && x.PropertyType == typeof(IEnumerable<object>));

                if (prop == null)
                {
                    throw new ArgumentException("No property named 'AppliedEvents' found on object of type " + type);
                }

                EventSources.Add(type, o => (IEnumerable<object>)prop.GetValue(o));
            }

            return EventSources[type];
        }
    }
}