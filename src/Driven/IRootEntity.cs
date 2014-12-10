namespace Driven
{
    public interface IRootEntity
    {
        object Id { get; set; }
        int Version { get; set; }
        void Mutate(object e);
    }
}