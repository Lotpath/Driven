using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Driven.SampleDomain;
using Driven.SampleDomain.Services;

namespace Driven.Repl
{
    class Program
    {
        static void Main(string[] args)
        {
            var task = Task.Run(async () => await Run());
            Task.WaitAll(task);
        }

        private static async Task Run()
        {
            var bootstrapper = new DatabaseBootstrapper(new ConnectionStringProvider());

            if (!await bootstrapper.StoreExists())
            {
                await bootstrapper.InitializeStore();
            }
            
            await bootstrapper.TearDownStore();

            var mappingConfiguration = new EntityToTableNameMappingConfiguration(
                cfg =>
                cfg.Map<Product>("tbl_products"));

            await bootstrapper.EnsureTablesExist(mappingConfiguration);
            await bootstrapper.ExecuteSql("CREATE INDEX tbl_products_tenant_id_product_id_index ON tbl_products ((data->'_tenantId'->>'_id'), (data->'_productId'->>'_id'));");

            var repo = new PostgreSQLJsonRepository(new NewtonsoftJsonSerializer(), new ConnectionStringProvider(), mappingConfiguration);
            var productRepo = new PostgreSQLJsonProductRepository(repo);
            var productManager = new ProductManager(productRepo);

            var ids = new List<string>();

            for (int i = 0; i < 1000; i++)
            {
                var id = await productManager.CreateNewAsync(Guid.Empty.ToString(), "widgets");

                ids.Add(id);

                Console.WriteLine("{0:00000} - {1}", i, id);
            }

            var start = DateTime.UtcNow;

            foreach (var id in ids)
            {
                await productManager.LoadAsync(Guid.Empty.ToString(), id);
            }

            var interval = DateTime.UtcNow - start;
            Console.WriteLine("Time to load: {0}", interval);

            //await productManager.LoadAllAsync(new TenantId(Guid.Empty));
            //await productManager.FindAsync(new TenantId(Guid.Empty), "widgets");

            //await bootstrapper.TearDownStore();

            Console.WriteLine("Press a key to exit...");
            Console.ReadKey();
        }
    }


}
