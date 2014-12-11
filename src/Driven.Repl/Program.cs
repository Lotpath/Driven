using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Driven.SampleDomain;

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

            var tables = new List<string>();
            tables.Add("tbl_products");

            await bootstrapper.EnsureDocumentTablesExist(tables);

            var repo = new PostgreSQLJsonRepository(new NewtonsoftJsonSerializer(), new ConnectionStringProvider());
            var productRepo = new PostgreSQLJsonProductRepository(repo);
            var productManager = new ProductManager(productRepo);

            var id = await productManager.CreateNewAsync(new TenantId(Guid.Empty), "widgets");

            await productManager.LoadAllAsync(new TenantId(Guid.Empty));
            await productManager.LoadAsync(new TenantId(Guid.Empty), id);
            await productManager.FindAsync(new TenantId(Guid.Empty), "widgets");

            //await bootstrapper.TearDownStore();
        }
    }
}
