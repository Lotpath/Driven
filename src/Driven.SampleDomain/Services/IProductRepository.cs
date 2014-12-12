using System.Collections.Generic;
using System.Threading.Tasks;

namespace Driven.SampleDomain.Services
{
    /// <summary>
    /// Repositories will interact directly with domain aggregates/entities and so will declare
    /// methods that reference core domain types (aggregates, entities, value types). The are 
    /// declared in the Domain so they can be referenced by Managers and Engines. However they
    /// are not intended to be used outside of the Domain itself (even though implemented outside
    /// the domain as an Adapter).
    /// </summary>
    public interface IProductRepository
    {
        Task RemoveAsync(Product product);
        Task RemoveAllAsync(IEnumerable<Product> products);
        Task SaveAsync(Product product);
        Task SaveAllAsync(IEnumerable<Product> products);
        Task<IEnumerable<Product>> AllProductsOfTenantAsync(TenantId tenantId);
        Task<Product> ProductOfIdAsync(TenantId tenantId, ProductId productId);
        Task<IEnumerable<Product>> ProductsOfNameAsync(TenantId tenantId, ProductName name);
    }
}