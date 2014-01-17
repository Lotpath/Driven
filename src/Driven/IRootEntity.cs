using CommonDomain;

namespace Driven
{
    public interface IRootEntity : IMemento
    {
        void Mutate(object e);
    }
}