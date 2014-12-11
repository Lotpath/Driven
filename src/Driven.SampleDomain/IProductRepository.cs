using System.Collections.Generic;
using System.Threading.Tasks;

namespace Driven.SampleDomain
{
    public interface IProductRepository
    {
        Task RemoveAsync(Product product);
        Task RemoveAllAsync(IEnumerable<Product> products);
        Task SaveAsync(Product product);
        Task SaveAllAsync(IEnumerable<Product> products);
        Task<IEnumerable<Product>> AllProductsOfTenantAsync(TenantId tenantId);
        Task<Product> ProductOfIdAsync(TenantId tenantId, ProductId productId);
        Task<IEnumerable<Product>> ProductsOfNameAsync(TenantId tenantId, string name);
    }
}