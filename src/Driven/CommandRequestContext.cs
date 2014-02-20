using System.Collections.Generic;

namespace Driven
{
    public class CommandRequestContext : ICommandRequestContext
    {        
        public CommandRequestContext(IRepository repository, ISagaRepository sagaRepository, ISecurityContext securityContext = null, ICommandValidator commandValidator = null)
        {
            Repository = repository;
            SagaRepository = sagaRepository;
            SecurityContext = securityContext ?? new DefaultSecurityContext();
            CommandValidator = commandValidator ?? new NulloCommandValidator();
        }

        public IRepository Repository { get; private set; }
        public ISagaRepository SagaRepository { get; private set; }
        public ISecurityContext SecurityContext { get; private set; }
        public ICommandValidator CommandValidator { get; private set; }

        private class DefaultSecurityContext : ISecurityContext
        {
            public DefaultSecurityContext()
            {
                UserName = "";
                Claims = new string[0];
            }

            public string UserName { get; private set; }
            public IEnumerable<string> Claims { get; private set; }
        }
    }
}