namespace Driven
{
    public class DefaultDrivenContextFactory : IDrivenContextFactory
    {
        public DrivenContext Create(Message message)
        {
            var drivenContext = new DrivenContext();

            drivenContext.Message = message;

            return drivenContext;
        }
    }
}