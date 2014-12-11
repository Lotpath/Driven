namespace Driven
{
    public interface IIdentifiable<T>
    {
        T Identity();
        void Identity(T value);
        bool IsIdentified();
        bool IsUnidentified();
    }
}