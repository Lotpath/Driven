using System.Collections.Generic;
using Driven;
using Driven.Testing;
using NEventStore;
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
            var commit = default(Commit);

            "Given the order fulfillment domain"
                .Context(() =>
                    {
                        harness = new Harness(cfg =>
                            {
                                cfg.Modules(typeof (OrderFulfillmentModule));
                                cfg.Dispatcher(c => commit = c);
                                cfg.WithClaims(new[] { });
                            });

                        //fixture = DomainFixture
                        //    .Init()
                        //    .WithDispatcher(dispatcher.Object)
                        //    .WithClaims(new[] { "Ordering" })                            
                        //    .WithUser("test");

                        //dispatcher.Setup(x => x.Dispatch(It.IsAny<Commit>()))
                        //          .Callback((Commit commit) =>
                        //              {
                        //                  foreach (var e in commit.Events.Select(x => x.Body))
                        //                  {
                                              
                        //                  }
                        //              });
                    });

            "when submitting an order"
                .Do(() => harness.When(new SubmitOrderCommand
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
                        //fixture.AssertEventWasDispatched<OrderSubmittedEvent>();
                    });
        }
    }
}
