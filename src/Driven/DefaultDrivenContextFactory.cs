using System.Collections.Generic;
using CommonDomain.Persistence;

namespace Driven
{
    public class DefaultDrivenContextFactory : IDrivenContextFactory
    {
        private readonly IRepository _repository;
        private readonly ISagaRepository _sagaRepository;

        public DefaultDrivenContextFactory(IRepository repository, ISagaRepository sagaRepository)
        {
            _repository = repository;
            _sagaRepository = sagaRepository;
        }

        public DrivenContext Create(object payload, IDictionary<string, object> headers)
        {
            var context = new DrivenContext();

            context.Repository = _repository;

            context.SagaRepository = _sagaRepository;

            context.Message = new Message(payload, headers);
                        
            return context;
        }
    }
}