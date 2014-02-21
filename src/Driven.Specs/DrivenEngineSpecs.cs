using SubSpec;
using Xunit;

namespace Driven.Specs
{
    public class DrivenEngineSpecs
    {
        [Specification]
        public void can_dispatch_message_to_module_using_engine()
        {
            var engine = default(IDrivenEngine);

            "Given a driven engine"
                .Context(() =>
                    {
                        var bootstrapper = new DefaultDrivenBootstrapper();
                        bootstrapper.Initialize();
                        engine = bootstrapper.GetEngine();
                    });

            "when handling a message for a configured module"
                .Do(() =>
                    {
                        engine.HandleMessage(new EngineTestMessage());
                    });

            "then the module receives the message for processing"
                .Assert(() =>
                    {
                        Assert.Equal(1, EngineTestModule.CallTimes);
                    });
        }

        public class EngineTestModule : DrivenModule
        {
            public EngineTestModule()
            {
                Handle<EngineTestMessage>(When);
            }

            public void When(EngineTestMessage message)
            {
                CallTimes = CallTimes + 1;
            }

            public static int CallTimes = 0;
        }

        public class EngineTestMessage
        {            
        }
    }
}