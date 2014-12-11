namespace Driven
{
    public interface ISerializer
    {
        T Deserialize<T>(string json);
        string Serialize(object target);
    }
}