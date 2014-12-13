using System;

namespace Driven
{
    public class DelegateIdentityAdapter : IIdentityAdapter
    {
        private Action<object, long> _setIdentity;
        private Func<object, long> _getIdentity;

        public DelegateIdentityAdapter(Func<object, long> getIdentity, Action<object, long> setIdentity)
        {
            _getIdentity = getIdentity;
            _setIdentity = setIdentity;
        }

        public void SetIdentity(object target, long value)
        {
            _setIdentity(target, value);
        }

        public long GetIdentity(object target)
        {
            return _getIdentity(target);
        }

        public bool IsUnidentified(object target)
        {
            return GetIdentity(target) <= 0;
        }
    }
}