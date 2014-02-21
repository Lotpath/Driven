namespace Driven
{
    public interface IDrivenContextFactory
    {
        DrivenContext Create(ISecurityContext securityContext);
    }
}