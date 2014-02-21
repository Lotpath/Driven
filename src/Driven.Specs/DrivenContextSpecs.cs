using System;
using CommonDomain.Persistence;
using Moq;
using SubSpec;
using Xunit;

namespace Driven.Specs
{
    public class DrivenContextSpecs
    {
        [Specification]
        public void NullSecurityClaimsTest()
        {
            var module = default(SecureModule);
            var securityContext = new Mock<ISecurityContext>();
            var thrownException = default(Exception);

            "Given a command request context with null security claims"
                .Context(() =>
                    {
                        module = new SecureModule();
                        module.Context = new DrivenContext(new Mock<IRepository>().Object, new Mock<ISagaRepository>().Object, securityContext.Object, new DataAnnotationsCommandValidator());
                        securityContext.Setup(x => x.Claims).Returns(() => null);
                    });

            "when checking claims security"
                .Do(() =>
                    {
                        module.When(new object());
                    }, (DomainSecurityException ex) => thrownException = ex);

            "then a domain security exception is thrown"
                .Assert(() =>
                    {
                        Assert.IsType<DomainSecurityException>(thrownException);
                    });
        }

        public class SecureModule : DrivenModule
        {
            public void When(object message)
            {
                this.RequireClaim("foo");
            }
        }
    }
}