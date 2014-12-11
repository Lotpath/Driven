using System;

namespace Driven.SampleDomain
{
    public class ProductId
    {
        public ProductId()
            : this(Guid.Empty)
        {
        }

        public ProductId(Guid identifier)
        {
            _identifier = "Product-" + identifier;
        }

        private readonly string _identifier;
        public string Identifier { get { return _identifier; } }

        public override string ToString()
        {
            return _identifier;
        }
    }
}