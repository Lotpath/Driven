using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Driven.SampleDomain.Services;

namespace Driven.Repl
{
    public class App
    {
        private PersistenceConfiguration _configuration;
        private DatabaseBootstrapper _bootstrapper;
        private ProductManager _manager;

        public App()
        {
            _configuration = new PersistenceConfiguration(new SampleDomainConfigurer().Configure);

            _bootstrapper = new DatabaseBootstrapper(_configuration);

            var repo = new PostgreSQLJsonRepository(_configuration);
            var productRepo = new PostgreSQLJsonProductRepository(repo);
            _manager = new ProductManager(productRepo);
        }

        public async Task FullBattery()
        {
            await InitializeDatabase();
            await TearDownDatabase();
            await BootstrapDatabase();
            //await CreateAndFetchOneThousandProducts();
            //await FetchAllProductsForTenant();
            //await FetchAllProductsByName();
            await ChangeProductNameAndRefetchByName();
        }

        public async Task InitializeDatabase()
        {
            if (!await _bootstrapper.StoreExists())
            {
                await _bootstrapper.InitializeStore();
            }
        }

        public async Task BootstrapDatabase()
        {
            var timer = new Timer();

            InitializeDatabase();

            await _bootstrapper.EnsureSchemaIsUpToDate();

            Console.WriteLine("Completed: " + timer.Interval);
        }

        public async Task TearDownDatabase()
        {
            var timer = new Timer();

            await _bootstrapper.TearDownStore();

            Console.WriteLine("Completed: " + timer.Interval);
        }

        public async Task CreateAndFetchOneThousandProducts()
        {
            var ids = new List<string>();

            var itemCount = 1000;
            for (int i = 0; i < itemCount; i++)
            {
                var id = await _manager.CreateNewAsync(Guid.Empty.ToString(), "widgets");

                ids.Add(id);

                Console.WriteLine("{0:00000} - {1}", i, id);
            }

            var timer = new Timer();
            foreach (var id in ids)
            {
                await _manager.LoadAsync(Guid.Empty.ToString(), id);
            }
            var interval = timer.Interval;
            Console.WriteLine("Time to load each individually: {0}", interval);
            Console.WriteLine("Average load time: {0:0.0000} seconds", interval.TotalSeconds / itemCount);
        }

        public async Task FetchAllProductsForTenant()
        {
            var timer = new Timer();
            await _manager.LoadAllAsync(Guid.Empty.ToString());
            Console.WriteLine("Time to load all: {0}", timer.Interval);            
        }

        public async Task FetchAllProductsByName()
        {
            var timer = new Timer();
            await _manager.FindAsync(Guid.Empty.ToString(), "widgets");
            Console.WriteLine("Time to find by name: {0}", timer.Interval);
        }

        public async Task ChangeProductNameAndRefetchByName()
        {
            var timer = new Timer();
            var id = await _manager.CreateNewAsync(Guid.Empty.ToString(), "woot");
            await _manager.ChangeName(Guid.Empty.ToString(), id, "woohoo");
            await _manager.FindAsync(Guid.Empty.ToString(), "woot");
            await _manager.FindAsync(Guid.Empty.ToString(), "woohoo");
            Console.WriteLine("Time to find by name: {0}", timer.Interval);
        }
    }
}