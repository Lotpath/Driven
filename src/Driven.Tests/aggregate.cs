using System.Collections.Generic;
using Xunit;

namespace Driven.Tests
{
    public class aggregate_with_non_event_sourced_root_entity
    {

    }

    public class aggregate_with_event_sourced_root_entity
    {
        private CartState _state;
        private CartAggregate _aggregate;

        public aggregate_with_event_sourced_root_entity()
        {
            _state = new CartState();
            _aggregate = new CartAggregate(_state);
        }

        [Fact]
        public void foo()
        {
            _aggregate.AddItemToCart("widget-01", 5);
            Assert.Equal(1, _state.Items.Count);
        }
    }

    public class CartState : RootEntityBase<CartState>
    {
        public CartState()
        {
            Items = new Dictionary<string, int>();
        }

        protected void When(ItemAddedToCart e)
        {
            if (Items.ContainsKey(e.Code))
            {
                Items[e.Code] += e.Quantity;
            }
            else
            {
                Items[e.Code] = e.Quantity;
            }
        }

        public Dictionary<string, int> Items { get; set; }
    }

    public class CartAggregate : AggregateBase<CartState>
    {
        public CartAggregate(CartState rootEntity)
            : base(rootEntity)
        {
            Register<ItemAddedToCart>();
        }

        public void AddItemToCart(string itemCode, int quantity)
        {
            RaiseEvent(new ItemAddedToCart { Code = itemCode, Quantity = quantity });
        }
    }

    public class ItemAddedToCart
    {
        public string Code { get; set; }
        public int Quantity { get; set; }
    }
}