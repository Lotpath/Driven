using System;
using System.Linq;
using System.Reflection;

namespace Driven
{
    public class ReflectionIdentityAdapter : IIdentityAdapter
    {
        private readonly IIdentityAdapter _adapter;

        public ReflectionIdentityAdapter(Type type)
        {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var get = methods.SingleOrDefault(x => x.Name == "GetIdentity");
            var set = methods.SingleOrDefault(x => x.Name == "SetIdentity");

            if (get == null || get.ReturnType != typeof(long))
            {
                throw new ArgumentException("Method with signature 'long GetIdentity()' not found");
            }

            if (set == null || set.GetParameters().ToList()[0].ParameterType != typeof(long))
            {
                throw new ArgumentException("Method with signature 'SetIdentity(long)' not found");
            }

            _adapter = new DelegateIdentityAdapter(
                o => (long)get.Invoke(o, new object[0]),
                (o, id) => set.Invoke(o, new object[] { id }));
        }

        public void SetIdentity(object target, long value)
        {
            _adapter.SetIdentity(target, value);
        }

        public long GetIdentity(object target)
        {
            return _adapter.GetIdentity(target);
        }

        public bool IsUnidentified(object target)
        {
            return _adapter.IsUnidentified(target);
        }
    }
}