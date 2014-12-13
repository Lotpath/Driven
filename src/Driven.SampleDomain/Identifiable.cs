namespace Driven.SampleDomain
{
    /// <summary>
    /// An identifiable implementation provides a shared layer class so that persisted
    /// domain objects can have a surrogate identity assigned to them in the persistence
    /// service.
    /// </summary>
    public abstract class Identifiable
    {
        private long _surrogateIdentity;

        protected Identifiable()
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
            var typed = obj as Identifiable;
            if (typed == null || (GetType() != typed.GetType()))
            {
                return false;
            }

            return ReferenceEquals(this, obj) || _surrogateIdentity.Equals(typed._surrogateIdentity);
        }
    }
}