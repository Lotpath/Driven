using Driven;
using Sample.Contract;

namespace Sample.Domain
{
    public class OrderFulfillmentModule : DrivenModule
    {
        public OrderFulfillmentModule()
        {
            Handle<SubmitOrderCommand>(When);
        }

        public void When(SubmitOrderCommand c)
        {
            this.Execute<OrderFulfillmentCoordinator>(c.CorrelationId, a => a.Initialize(c));
        }
    }

    public class OrderFulfillmentCoordinator : AggregateBase<OrderFulfillmentState>
    {
        public OrderFulfillmentCoordinator(OrderFulfillmentState state)
            : base(state)
        {
            Register<OrderSubmittedEvent>(RootEntity.When);
        }

        public void Initialize(SubmitOrderCommand c)
        {
            RaiseEvent(new OrderSubmittedEvent(SequentialGuid.New(), c.CorrelationId, c.Items));
        }   
    }

    public class OrderFulfillmentState : RootEntityBase
    {
        public bool OrderSubmitted { get; private set; }

        public void When(OrderSubmittedEvent e)
        {
            OrderSubmitted = true;
        }
    }

    public class ItemInventoryModule : DrivenModule
    {
        public void When(ReserveInventoryForOrderComand c)
        {
            // place inventory on reserve or throw if insufficient inventory available?
        }
    }
}
