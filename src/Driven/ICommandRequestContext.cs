namespace Driven
{
    public interface ICommandRequestContext
    {
        IRepository Repository { get; }
        ISecurityContext SecurityContext { get; }
        ICommandValidator CommandValidator { get; }
    }
}