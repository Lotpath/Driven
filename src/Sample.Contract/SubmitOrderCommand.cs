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

        public List<OrderLineItem> Items { get; set; } 
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
}