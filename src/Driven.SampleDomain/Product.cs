namespace Driven.SampleDomain
{
    public class Product : Entity
    {
        private readonly TenantId _tenantId;
        private readonly ProductId _productId;
        private readonly ProductName _productName;

        public Product(TenantId tenantId, ProductId productId, ProductName productName)
        {
            _tenantId = tenantId;
            _productId = productId;
            _productName = productName;
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
