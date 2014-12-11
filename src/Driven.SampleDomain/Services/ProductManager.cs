using System;
using System.Threading.Tasks;

namespace Driven.SampleDomain.Services
{
    public class ProductManager
    {
        private readonly IProductRepository _repository;

        public ProductManager(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> CreateNewAsync(string tenantId, string name)
        {
            var productId = new ProductId(Guid.NewGuid().ToString().ToUpperInvariant());
            var product = new Product(new TenantId(tenantId), productId, name);
            await _repository.SaveAsync(product);
            return productId.Id();
        }

        public async Task LoadAllAsync(string tenantId)
        {
            var all = await _repository.AllProductsOfTenantAsync(new TenantId(tenantId));
        }

        public async Task LoadAsync(string tenantId, string productId)
        {
            var product = await _repository.ProductOfIdAsync(new TenantId(tenantId), new ProductId(productId));
        }

        public async Task FindAsync(string tenantId, string name)
        {
            var product = await _repository.ProductsOfNameAsync(new TenantId(tenantId), name);
        }
    }
}