using CommonDomain.Persistence;

namespace Driven
{
    public class DrivenContext
    {
        public DrivenContext() { }

        public DrivenContext(IRepository repository, ISagaRepository sagaRepository, ISecurityContext securityContext, ICommandValidator commandValidator)
        {
            Repository = repository;
            SagaRepository = sagaRepository;
            SecurityContext = securityContext;
            CommandValidator = commandValidator;
        }

        public IRepository Repository { get; set; }
        public ISagaRepository SagaRepository { get; set; }
        public ISecurityContext SecurityContext { get; set; }
        public ICommandValidator CommandValidator { get; set; }
    }
}