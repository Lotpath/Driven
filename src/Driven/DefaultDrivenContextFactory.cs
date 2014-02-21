using CommonDomain.Persistence;

namespace Driven
{
    public class DefaultDrivenContextFactory : IDrivenContextFactory
    {
        private readonly IRepository _repository;
        private readonly ISagaRepository _sagaRepository;
        private readonly ICommandValidator _commandValidator;

        public DefaultDrivenContextFactory(IRepository repository, ISagaRepository sagaRepository, ICommandValidator commandValidator)
        {
            _repository = repository;
            _sagaRepository = sagaRepository;
            _commandValidator = commandValidator;
        }

        public DrivenContext Create(ISecurityContext securityContext)
        {
            var context = new DrivenContext();

            context.SecurityContext = securityContext;

            context.Repository = _repository;

            context.SagaRepository = _sagaRepository;
            
            context.CommandValidator = _commandValidator;
                        
            return context;
        }
    }
}