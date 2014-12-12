using System;
using System.Threading.Tasks;

namespace Driven.SampleDomain.Services
{
    /// <summary>
    /// Managers provide methods which can be used to drive a use case. These methods
    /// should not expose or reference core domain types (aggregates, entities, value types). As a rule
    /// of thumb, a Manager should only modify one aggregate per method called and should not perform
    /// any business logic, delegating instead to aggregates and value objects for the enforcement of 
    /// business rules and logic.
    /// 
    /// A Manager can be designed in one of two ways depending on whether you want to build an
    /// Anti Corruption Layer (ACL) directly into your core domain, or delegate this task to the client of
    /// your core domain.
    /// 
    /// If you define the ACL in your core domain project, the Managers should receive ACL objects
    /// as parameters and return ACL objects for methods which return values, following a Model-In, Model-Out 
    /// (MIMO) approach.
    /// 
    /// Alternatively, you can design your Managers to receive primitives as input parameters and return 
    /// primitives for methods which return values. This places the development of an Anti Corruption layer 
    /// in the hands of the client of the domain.
    /// 
    /// </summary>
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
            var product = new Product(new TenantId(tenantId), productId, new ProductName(name));
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
            var product = await _repository.ProductsOfNameAsync(new TenantId(tenantId), new ProductName(name));
        }
    }
}