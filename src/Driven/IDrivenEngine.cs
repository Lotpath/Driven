namespace Driven
{
    public interface IDrivenEngine
    {
        void HandleMessage<TMessage>(TMessage message);
    }
}