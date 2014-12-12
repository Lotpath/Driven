namespace Driven.SampleDomain
{
    public abstract class Entity
    {
        private long _surrogateIdentity;

        protected Entity()
        {
            SetIdentity(0);
        }

        public long GetIdentity()
        {
            return _surrogateIdentity;
        }

        public void SetIdentity(long value)
        {
            _surrogateIdentity = value;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + _surrogateIdentity.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            var typed = obj as Entity;
            if (typed == null || (GetType() != typed.GetType()))
            {
                return false;
            }

            return ReferenceEquals(this, obj) || _surrogateIdentity.Equals(typed._surrogateIdentity);
        }
    }
}