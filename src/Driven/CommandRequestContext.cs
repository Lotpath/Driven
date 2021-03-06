﻿using System.Collections.Generic;
using CommonDomain.Persistence;

namespace Driven
{
    public class CommandRequestContext : ICommandRequestContext
    {        
        public CommandRequestContext(IRepository repository, ISecurityContext securityContext = null, ICommandValidator commandValidator = null)
        {
            Repository = repository;
            SecurityContext = securityContext ?? new DefaultSecurityContext();
            CommandValidator = commandValidator ?? new DataAnnotationsCommandValidator();
        }

        public IRepository Repository { get; private set; }
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