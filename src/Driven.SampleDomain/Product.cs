namespace Driven.SampleDomain
{
    public class Product : Entity
    {
        private readonly TenantId _tenantId;
        private readonly ProductId _productId;
        private readonly string _name;

        public Product(TenantId tenantId, ProductId productId, string name)
        {
            _tenantId = tenantId;
            _productId = productId;
            _name = name;
        }
    }
}
