namespace Driven.SampleDomain
{
    public abstract class Entity : IIdentifiable<long>
    {
        private long _surrogateIdentity;

        protected Entity()
        {
            Identity(0);
        }

        public void Identity(long value)
        {
            _surrogateIdentity = value;
        }

        public long Identity()
        {
            return _surrogateIdentity;
        }

        public bool IsIdentified()
        {
            return Identity() > 0;
        }

        public bool IsUnidentified()
        {
            return Identity() <= 0;
        }
    }
}