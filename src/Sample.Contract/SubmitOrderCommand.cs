using System;
using System.Collections.Generic;

namespace Sample.Contract
{
    public class SubmitOrderCommand : ICommand
    {
        public SubmitOrderCommand()
        {
            Items = new List<OrderLineItem>();
        }

        public Guid CommandId { get; set; }
        public Guid CorrelationId { get; set; }

        public IList<OrderLineItem> Items { get; set; }
    }

    public class OrderLineItem
    {
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
    }

    public class OrderSubmittedEvent : IEvent
    {
        protected OrderSubmittedEvent()
        {
            Items = new List<OrderLineItem>();
        }

        public OrderSubmittedEvent(Guid eventId, Guid correlationId, IEnumerable<OrderLineItem> items)
            : this()
        {
            EventId = eventId;
            CorrelationId = correlationId;
            foreach(var i in items) Items.Add(i);
        }

        public Guid EventId { get; private set; }
        public Guid CorrelationId { get; private set; }

        public IList<OrderLineItem> Items { get; private set; }
    }

    public class ReserveInventoryForOrderComand : ICommand
    {
        public Guid CommandId { get; set; }
        public Guid CorrelationId { get; set; }
    }

    public class OrderFulfillmentRoutingSlip
    {
        // order submitted

        // future: (0) check pricing for ordered items
        // future: (1) apply customer discounts (if any/if available)
        // future: (2) check customer credit for order
        
        // (3) check available inventory for each ordered item/quantity
        // (4) reserve inventory for each ordered item/quantity
        
        // future: if order is cancelled, unreserve inventory
        public List<OrderLineItem> Items { get; set; } 
    }
}