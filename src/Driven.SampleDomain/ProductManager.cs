using System;
using System.Threading.Tasks;

namespace Driven.SampleDomain
{
    public class ProductManager
    {
        private readonly IProductRepository _repository;

        public ProductManager(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductId> CreateNewAsync(TenantId tenantId, string name)
        {
            var productId = new ProductId(Guid.NewGuid());
            var product = new Product(tenantId, productId, name);
            await _repository.SaveAsync(product);
            return productId;
        }

        public async Task LoadAllAsync(TenantId tenantId)
        {
            var all = await _repository.AllProductsOfTenantAsync(tenantId);
        }

        public async Task LoadAsync(TenantId tenantId, ProductId productId)
        {
            var product = await _repository.ProductOfIdAsync(tenantId, productId);
        }

        public async Task FindAsync(TenantId tenantId, string name)
        {
            var product = await _repository.ProductsOfNameAsync(tenantId, name);
        }
    }
}