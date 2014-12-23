using Driven.SampleDomain;

namespace Driven.Repl
{
    public class SampleDomainConfigurer
    {
        public void Configure(PersistenceConfiguration.PersistenceConfigurationConfigurer cfg)
        {            
            cfg.UseConnectionStringsFromAppConfig();
            cfg.Serializer(new NewtonsoftJsonSerializer());
            cfg.Map<Product>("products");
            
            // this sort of indexing is not as performant for exact match indexing.
            // gin indexing with jsonb and exact matching is the most performant choice
            // however we may still need to define indexes like this for other operations such as "like"s
            //.Index<Product>("tbl_products_tenant_id_product_id_index",
            //                "((data->'_tenantId'->>'_id'), (data->'_productId'->>'_id'))");
        }
    }
}