using Driven.SampleDomain;

namespace Driven.Repl
{
    public class SampleDomainConfigurer
    {
        public void Configure(PersistenceConfiguration.PersistenceConfigurationConfigurer cfg)
        {
            cfg.UseConnectionStringsFromAppConfig();
            cfg.Serializer(new NewtonsoftJsonSerializer());
            cfg.Map<Product>("tbl_products")
               .Index<Product>("tbl_products_tenant_id_product_id_index",
                               "((data->'_tenantId'->>'_id'), (data->'_productId'->>'_id'))");
        }
    }
}