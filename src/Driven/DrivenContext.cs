using CommonDomain.Persistence;

namespace Driven
{
    public class DrivenContext
    {
        public DrivenContext() { }

        public DrivenContext(IRepository repository, ISagaRepository sagaRepository)
        {
            Repository = repository;
            SagaRepository = sagaRepository;
        }

        public IRepository Repository { get; set; }
        public ISagaRepository SagaRepository { get; set; }

        public Message Message { get; set; }
    }
}