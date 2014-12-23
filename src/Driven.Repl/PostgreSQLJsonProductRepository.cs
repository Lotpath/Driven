using System;
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
            var filter = new
            {
                tenantId = new { id = tenantId.Id() },
            };

            return await _repository.FindAllAsync<Product>(filter, "");
        }

        public async Task<Product> ProductOfIdAsync(TenantId tenantId, ProductId productId)
        {
            var filter = new
            {
                tenantId = new { id = tenantId.Id() },
                productId = new { id = productId.Id() }
            };

            return await _repository.FindOneAsync<Product>(filter);
        }

        public async Task<IEnumerable<Product>> ProductsOfNameAsync(TenantId tenantId, ProductName productName)
        {
            var filter = new
                {
                    tenantId = new { id = tenantId.Id() },
                    productName = new { name = productName.Name }
                };

            return await _repository.FindAllAsync<Product>(filter, "");
        }
    }
}