using System;

namespace Driven.SampleDomain
{
    public class ProductId
    {
        private string _id;

        public ProductId(string id) : this()
        {
            SetId(id);
        }

        public ProductId(ProductId id) : this()
        {
            SetId(id._id);
        }

        private ProductId()
        {
        }

        public override string ToString()
        {
            return "ProductId [_id=" + _id + "]";
        }

        private void SetId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("The id must be provided.");
            }

            Guid parsed;
            if (!Guid.TryParse(id, out parsed))
            {
                throw new ArgumentException("The id must be a valid Guid.");
            }

            _id = id;
        }

        public string Id()
        {
            return _id;
        }
    }
}