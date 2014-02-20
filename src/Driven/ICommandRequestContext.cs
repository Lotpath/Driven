namespace Driven
{
    public interface ICommandRequestContext
    {
        IRepository Repository { get; }
        ISagaRepository SagaRepository { get; }
        ISecurityContext SecurityContext { get; }
        ICommandValidator CommandValidator { get; }
    }
}