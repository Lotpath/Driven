using System.Collections.Generic;
using System.Threading.Tasks;
using Driven.SampleDomain;

namespace Driven.Repl
{
    public class PostgreSQLJsonProductRepository : IProductRepository
    {
        private readonly PostgreSQLJsonRepository _repository;
        private const string TableName = "tbl_products";

        public PostgreSQLJsonProductRepository(PostgreSQLJsonRepository repository)
        {
            _repository = repository;
        }

        public async Task RemoveAsync(Product product)
        {
            await _repository.DeleteAsync(TableName, product);
        }

        public async Task RemoveAllAsync(IEnumerable<Product> products)
        {
            await _repository.DeleteAsync(TableName, products);
        }

        public async Task SaveAsync(Product product)
        {
            await _repository.SaveAsync(TableName, product);
        }

        public async Task SaveAllAsync(IEnumerable<Product> products)
        {
            await _repository.SaveAsync(TableName, products);
        }
  
        public async Task<IEnumerable<Product>> AllProductsOfTenantAsync(TenantId tenantId)
        {
            var filter = "data->'_tenantId'->>'_identifier' = @0";

            return await _repository.FindAllAsync<Product>(TableName, filter, "", tenantId.Identifier);
        }

        public async Task<Product> ProductOfIdAsync(TenantId tenantId, ProductId productId)
        {
            var filter = "data->'_tenantId'->>'_identifier' = @0 and data->'_productId'->>'_identifier' = @1";

            return await _repository.FindOneAsync<Product>(TableName, filter, tenantId.Identifier, productId.Identifier);
        }

        public async Task<IEnumerable<Product>> ProductsOfNameAsync(TenantId tenantId, string name)
        {
            var filter = "data->'_tenantId'->>'_identifier' = @0 and data->>'_name' = @1";

            return await _repository.FindAllAsync<Product>(TableName, filter, "", tenantId.Identifier, name);
        }
    }
}