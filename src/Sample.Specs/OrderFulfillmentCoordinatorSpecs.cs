using System.Collections.Generic;
using System.Linq;
using Driven;
using Driven.Testing;
using Moq;
using NEventStore;
using NEventStore.Dispatcher;
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
            var dispatcher = new Mock<IDispatchCommits>();

            "Given the order fulfillment domain"
                .Context(() =>
                    {
                        fixture = DomainFixture
                            .Init()
                            .WithDispatcher(dispatcher.Object)
                            .WithClaims(new[] { "Ordering" })                            
                            .WithUser("test");

                        dispatcher.Setup(x => x.Dispatch(It.IsAny<Commit>()))
                                  .Callback((Commit commit) =>
                                      {
                                          foreach (var e in commit.Events.Select(x => x.Body))
                                          {
                                              
                                          }
                                      });
                    });

            "when submitting an order"
                .Do(() => fixture.Execute(ctx => new OrderFulfillmentModule
                    {
                        Context = ctx
                    }, new SubmitOrderCommand
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
