namespace Driven
{
    public interface IIdentityAdapter
    {
        void SetIdentity(object target, long value);
        long GetIdentity(object target);
        bool IsUnidentified(object target);
    }
}