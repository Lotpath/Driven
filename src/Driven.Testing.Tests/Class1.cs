using Driven.Bootstrapper;

namespace Driven.Testing.Tests
{
    public class BrokerFixture
    {
        private readonly Broker _Broker;

        public BrokerFixture()
        {
            var bootstrapper = default(IDrivenBootstrapper);
            _Broker = new Broker(bootstrapper);
        }


    }
}
