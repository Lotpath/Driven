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

            var mappingConfiguration = new EntityToTableNameMappingConfiguration(
                cfg =>
                cfg.Map<Product>("tbl_products"));

            await bootstrapper.EnsureTablesExist(mappingConfiguration);

            var repo = new PostgreSQLJsonRepository(new NewtonsoftJsonSerializer(), new ConnectionStringProvider(), mappingConfiguration);
            var productRepo = new PostgreSQLJsonProductRepository(repo);
            var productManager = new ProductManager(productRepo);

            for (int i = 0; i < 10000; i++)
            {
                var id = await productManager.CreateNewAsync(new TenantId(Guid.Empty), "widgets");

                Console.WriteLine("{0:00000} - {1}", i, id);
            }

            //await productManager.LoadAsync(new TenantId(Guid.Empty), id);
            //await productManager.LoadAllAsync(new TenantId(Guid.Empty));
            //await productManager.FindAsync(new TenantId(Guid.Empty), "widgets");

            //await bootstrapper.TearDownStore();

            Console.WriteLine("Press a key to exit...");
            Console.ReadKey();
        }
    }


}
