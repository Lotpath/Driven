using System.Collections.Generic;
using Driven;
using Driven.Testing;
using Sample.Contract;
using Sample.Domain;
using SubSpec;

namespace Sample.Specs
{
    public class OrderFulfillmentCoordinatorSpecs
    {
        [Specification]
        public void test()
        {
            var harness = default(Harness);
            var result = default(Result);

            "Given the order fulfillment domain"
                .Context(() =>
                    {
                        harness = new Harness(cfg => cfg.Modules(typeof (OrderFulfillmentModule)));
                    });

            "when submitting an order"
                .Do(() => result = harness.When(new SubmitOrderCommand
                    {
                        CommandId = SequentialGuid.New(),
                        CorrelationId = SequentialGuid.New(),
                        Items = new List<OrderLineItem>
                            {
                                new OrderLineItem {ItemCode = "Widget", Quantity = 5},
                                new OrderLineItem {ItemCode = "Fidget", Quantity = 2},
                            }
                    }));

            "then an order submitted event is committed"
                .Assert(() =>
                    {
                        result.ShouldHaveEventOf<OrderSubmittedEvent>();
                    });
        }
    }
}
