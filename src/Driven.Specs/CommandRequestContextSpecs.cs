using System;
using CommonDomain.Persistence;
using Moq;
using SubSpec;
using Xunit;

namespace Driven.Specs
{
    public class CommandRequestContextSpecs
    {
        [Specification]
        public void NullSecurityClaimsTest()
        {
            var context = default(CommandRequestContext);
            var securityContext = new Mock<ISecurityContext>();
            var thrownException = default(Exception);

            "Given a command request context with null security claims"
                .Context(() =>
                    {
                        context = new CommandRequestContext(new Mock<IRepository>().Object, securityContext.Object);
                        securityContext.Setup(x => x.Claims).Returns(() => null);
                    });

            "when checking claims security"
                .Do(() =>
                    {
                        context.RequireClaim("foo");
                    }, (DomainSecurityException ex) => thrownException = ex);

            "then a domain security exception is thrown"
                .Assert(() =>
                    {
                        Assert.IsType<DomainSecurityException>(thrownException);
                    });
        }
    }
}