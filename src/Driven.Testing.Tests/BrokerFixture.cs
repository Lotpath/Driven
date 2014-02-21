using Xunit;

namespace Driven.Testing.Tests
{
    public class BrokerFixture
    {
        private readonly Broker _broker;

        public BrokerFixture()
        {
            var bootstrapper =
                new ConfigurableBootstrapper(config => config.Services(typeof(EchoModule)));

            _broker = new Broker(bootstrapper);
        }

        [Fact]
        public void Should_be_able_to_send_message()
        {
            var result = _broker.When(new MyMessage(), with =>
                {
                    with.Header("", "");                    
                });

            result.Context.
        }

        public class EchoModule : DrivenModule
        {
            public EchoModule()
            {
                this.RequiresAuthentication();
                this.RequiresClaims("");

                When<MyMessage>(m =>
                    {
                        return new Result();
                    });
            }
        }

        public class MyMessage
        {
            
        }
    }
}
