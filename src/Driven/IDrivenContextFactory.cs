namespace Driven
{
    public interface IDrivenContextFactory
    {
        DrivenContext Create(Message message);
    }
}