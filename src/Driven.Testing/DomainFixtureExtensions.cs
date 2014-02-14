using System.Linq;
using Xunit;

namespace Driven.Testing
{
    public static class DomainFixtureExtensions
    {
        public static DomainFixture WithUser(this DomainFixture fixture, string userName)
        {
            fixture.ConfigureUser(userName);
            return fixture;
        }

        public static DomainFixture WithClaims(this DomainFixture fixture, string[] claims)
        {            
            fixture.ConfigureClaims(claims);
            return fixture;
        }

        public static DomainFixture WithCommandValidator(this DomainFixture fixture, ICommandValidator validator)
        {
            fixture.UseValidator(validator);
            return fixture;
        }

        public static void AssertDomainSecurityExceptionIsThrown(this DomainFixture fixture)
        {
            Assert.IsType<DomainSecurityException>(fixture.ThrownException);
        }

        public static void AssertDomainSecurityExceptionIsNotThrown(this DomainFixture fixture)
        {
            Assert.IsNotType<DomainSecurityException>(fixture.ThrownException);
        }

        public static void AssertCommandValidationExceptionIsThrown(this DomainFixture fixture)
        {
            Assert.IsType<CommandValidationException>(fixture.ThrownException);
        }

        public static void AssertCommandValidationExceptionIsNotThrown(this DomainFixture fixture)
        {
            var ex = fixture.ThrownException as CommandValidationException;
            if (ex != null)
            {
                var message = ex.ToString();
                Assert.True(false, message);
            }
            Assert.True(true);
        }

        public static TEvent AssertEventWasDispatched<TEvent>(this DomainFixture fixture)
        {
            var match = fixture.DispatchedEvents.FirstOrDefault(x => x is TEvent);
            Assert.NotNull(match);
            return (TEvent)match;
        }
    }
}
