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
            this.RequireClaim("Ordering");
            this.Validate(c);
            this.Execute<OrderFulfillmentCoordinator>(c.CorrelationId, a => a.Initialize(c));
        }
    }

    public class OrderFulfillmentCoordinator : AggregateBase<OrderFulfillmentState>
    {
        public OrderFulfillmentCoordinator(OrderFulfillmentState state)
            : base(state)
        {
            Register<OrderSubmittedEvent>();
        }

        public void Initialize(SubmitOrderCommand c)
        {
            RaiseEvent(new OrderSubmittedEvent(SequentialGuid.New(), c.CorrelationId, c.Items));
        }   
    }

    public class OrderFulfillmentState : RootEntityBase<OrderFulfillmentState>
    {
        private bool _orderSubmitted;

        private void When(OrderSubmittedEvent e)
        {
            _orderSubmitted = true;
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
