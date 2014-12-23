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
        }
    }
}