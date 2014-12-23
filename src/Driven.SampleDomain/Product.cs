using System;

namespace Driven.SampleDomain
{
    public class Product : Aggregate
    {
        private readonly TenantId _tenantId;
        private readonly ProductId _productId;
        private readonly ProductName _productName;
        private readonly DateTimeOffset _createdAt;

        protected Product() { }

        private Product(TenantId tenantId, ProductId productId, ProductName productName, DateTimeOffset createdAt)
        {
            _tenantId = tenantId;
            _productId = productId;
            _productName = productName;
            _createdAt = createdAt;
        }

        public static Product Create(TenantId tenantId, ProductId productId, ProductName productName)
        {
            var createdAt = DateTimeOffset.Now;
            var product = new Product(tenantId, productId, productName, createdAt);
            product.ApplyProductCreatedEvent(tenantId.Id(), productId.Id(), productName.Name, createdAt);
            return product;
        }

        public Product ChangeName(ProductName newName)
        {
            var currentName = _productName.Name;
            var updated = new Product(_tenantId, _productId, newName, _createdAt);
            updated.SetIdentity(GetIdentity());
            updated.ApplyProductNameChangedEvent(_tenantId.Id(), _productId.Id(), currentName, newName.Name);
            return updated;
        }

        private void ApplyProductCreatedEvent(string tenantId, string productId, string productName, DateTimeOffset createdAt)
        {
            var e = new ProductCreatedEvent(tenantId, productId, productName, createdAt);
            ApplyEvent(e);
        }

        private void ApplyProductNameChangedEvent(string tenantId, string productId, string originalName, string newName)
        {
            var e = new ProductNameChangedEvent(tenantId, productId, originalName, newName);
            ApplyEvent(e);
        }
    }

    public class ProductCreatedEvent : Identifiable
    {
        private readonly string _tenantId;
        private readonly string _productId;
        private readonly string _productName;
        private readonly DateTimeOffset _createdAt;

        protected ProductCreatedEvent() { }

        public ProductCreatedEvent(string tenantId, string productId, string productName, DateTimeOffset createdAt)
        {
            _tenantId = tenantId;
            _productId = productId;
            _productName = productName;
            _createdAt = createdAt;
        }
    }

    public class ProductNameChangedEvent : Identifiable
    {
        private readonly string _tenantId;
        private readonly string _productId;
        private readonly string _originalProductName;
        private readonly string _newProductName;

        protected ProductNameChangedEvent() { }

        public ProductNameChangedEvent(string tenantId, string productId, string originalProductName, string newProductName)
        {
            _tenantId = tenantId;
            _productId = productId;
            _originalProductName = originalProductName;
            _newProductName = newProductName;
        }
    }

    public class ProductName
    {
        private readonly string _name;

        public ProductName(string name)
        {
            // rule enforcement for product naming convention goes here
            _name = name;
        }

        public string Name { get { return _name; } }
    }
}
