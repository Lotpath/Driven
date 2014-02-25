using System;
using Xunit;

namespace Driven.Testing.Tests
{
    public class HarnessFixture
    {
        private readonly Harness _harness;

        public HarnessFixture()
        {
            _harness = new Harness(config => config.Modules(typeof(EchoModule)));
        }

        [Fact]
        public void Should_be_able_to_send_message()
        {
            var result = _harness.When(
                new MyCommand
                    {
                        CommandId = SequentialGuid.New(),
                        CorrelationId = SequentialGuid.New()
                    });            
        }

        public class EchoModule : DrivenModule
        {
            public EchoModule()
            {
                Handle<MyCommand>(When);
            }

            public void When(MyCommand c)
            {
                this.Execute<MyAggregate>(c.CorrelationId, a => a.DoIt(c));
            }
        }

        public class MyCommand
        {
            public Guid CommandId { get; set; }
            public Guid CorrelationId { get; set; }
        }

        public class MyEvent
        {
            public Guid EventId { get; set; }
            public Guid CorrelationId { get; set; }
        }

        public class MyAggregate : AggregateBase<MyRootEntity>
        {
            public MyAggregate(MyRootEntity rootEntity)
                : base(rootEntity)
            {
                Register<MyEvent>(rootEntity.When);
            }

            public void DoIt(MyCommand c)
            {
                RaiseEvent(new MyEvent
                    {
                        EventId = SequentialGuid.New(),
                        CorrelationId = c.CorrelationId
                    });
            }
        }

        public class MyRootEntity : RootEntityBase
        {
            public bool WasReceived { get; private set; }

            public void When(MyEvent e)
            {
                WasReceived = true;
            }
        }
    }
}
