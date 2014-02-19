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
            var fixture = default(DomainFixture);
         
            "Given the order fulfillment domain"
                .Context(() =>
                    {
                        fixture = DomainFixture
                            .Init()
                            .WithClaims(new[] { "Ordering" })
                            .WithUser("test");
                    });

            "when submitting an order"
                .Do(() => fixture.Execute(ctx => new OrderFulfillmentService(ctx), new SubmitOrderCommand
                    {
                        CommandId = SequentialGuid.New(),
                        CorrelationId = SequentialGuid.New(),
                        Items = new List<OrderLineItem>
                            {
                                new OrderLineItem {ItemCode = "Widget", Quantity = 5},
                                new OrderLineItem {ItemCode = "Fidget", Quantity = 2},
                            }
                    }));

            "then an order submitted event is dispatched"
                .Assert(() =>
                    {
                        fixture.AssertEventWasDispatched<OrderSubmittedEvent>();
                    });
        }
    }
}
