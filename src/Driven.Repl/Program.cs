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
            var configuration = new PersistenceConfiguration(
                cfg =>
                    {
                        cfg.UseConnectionStringsFromAppConfig();
                        cfg.Serializer(new NewtonsoftJsonSerializer());
                        cfg.Map<Product>("tbl_products")
                           .Index<Product>("tbl_products_tenant_id_product_id_index",
                                           "((data->'_tenantId'->>'_id'), (data->'_productId'->>'_id'))");
                    });

            var bootstrapper = new DatabaseBootstrapper(configuration);

            if (!await bootstrapper.StoreExists())
            {
                await bootstrapper.InitializeStore();
            }

            await bootstrapper.TearDownStore();

            await bootstrapper.EnsureSchemaIsUpToDate();

            var repo = new PostgreSQLJsonRepository(configuration);
            var productRepo = new PostgreSQLJsonProductRepository(repo);
            var productManager = new ProductManager(productRepo);

            var ids = new List<string>();

            var itemCount = 1000;
            for (int i = 0; i < itemCount; i++)
            {
                var id = await productManager.CreateNewAsync(Guid.Empty.ToString(), "widgets");

                ids.Add(id);

                Console.WriteLine("{0:00000} - {1}", i, id);
            }

            var timer = new Timer();
            foreach (var id in ids)
            {
                await productManager.LoadAsync(Guid.Empty.ToString(), id);
            }
            var interval = timer.Interval;
            Console.WriteLine("Time to load each individually: {0}", interval);
            Console.WriteLine("Average load time: {0:0.0000} seconds", interval.TotalSeconds / itemCount);

            timer.Restart();
            await productManager.LoadAllAsync(Guid.Empty.ToString());
            Console.WriteLine("Time to load all: {0}", timer.Interval);

            timer.Restart();
            await productManager.FindAsync(Guid.Empty.ToString(), "widgets");
            Console.WriteLine("Time to find by name: {0}", timer.Interval);

            //await bootstrapper.TearDownStore();

            Console.WriteLine("Press a key to exit...");
            Console.ReadKey();
        }
    }

    public class Timer
    {
        public Timer()
        {
            Start = DateTime.UtcNow;
        }

        public DateTime Start { get; private set; }
        public TimeSpan Interval { get { return DateTime.UtcNow - Start; } }

        public void Restart()
        {
            Start = DateTime.UtcNow;
        }
    }
}
