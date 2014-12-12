using System.Collections.Generic;
using System.Threading.Tasks;
using Driven.SampleDomain;
using Driven.SampleDomain.Services;

namespace Driven.Repl
{
    public class PostgreSQLJsonProductRepository : IProductRepository
    {
        private readonly PostgreSQLJsonRepository _repository;

        public PostgreSQLJsonProductRepository(PostgreSQLJsonRepository repository)
        {
            _repository = repository;
        }

        public async Task RemoveAsync(Product product)
        {
            await _repository.DeleteAsync(product);
        }

        public async Task RemoveAllAsync(IEnumerable<Product> products)
        {
            await _repository.DeleteAsync(products);
        }

        public async Task SaveAsync(Product product)
        {
            await _repository.SaveAsync(product);
        }

        public async Task SaveAllAsync(IEnumerable<Product> products)
        {
            await _repository.SaveAsync(products);
        }
  
        public async Task<IEnumerable<Product>> AllProductsOfTenantAsync(TenantId tenantId)
        {
            var filter = "data->'_tenantId'->>'_id' = @0";

            return await _repository.FindAllAsync<Product>(filter, "", tenantId.Id());
        }

        public async Task<Product> ProductOfIdAsync(TenantId tenantId, ProductId productId)
        {
            var filter = "data->'_tenantId'->>'_id' = @0 and data->'_productId'->>'_id' = @1";

            return await _repository.FindOneAsync<Product>(filter, tenantId.Id(), productId.Id());
        }

        public async Task<IEnumerable<Product>> ProductsOfNameAsync(TenantId tenantId, ProductName productName)
        {
            var filter = "data->'_tenantId'->>'_id' = @0 and data->'_productName'->>'_name' = @1";

            return await _repository.FindAllAsync<Product>(filter, "", tenantId.Id(), productName.Name);
        }
    }
}