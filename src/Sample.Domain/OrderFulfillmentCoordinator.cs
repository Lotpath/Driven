using Driven;
using Sample.Contract;

namespace Sample.Domain
{
    public class OrderFulfillmentService : IApplicationService
    {
        private readonly ICommandRequestContext _context;

        public OrderFulfillmentService(ICommandRequestContext context)
        {
            _context = context;
        }

        public void When(SubmitOrderCommand c)
        {
            _context.RequireClaim("Ordering");
            _context.Validate(c);
            _context.Execute<OrderFulfillmentCoordinator>(c.CorrelationId, a => a.Initialize(c));
        }

        public void Execute(object c)
        {
            RedirectToWhen.InvokeCommand(this, c);
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
        private void When(OrderSubmittedEvent e)
        {
            
        }
    }
}
