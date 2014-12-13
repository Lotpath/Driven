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
            await CreateAndFetchOneThousandProducts();
            await FetchAllProductsForTenant();
            await FetchAllProductsByName();
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
            await InitializeDatabase();

            await _bootstrapper.EnsureSchemaIsUpToDate();
        }

        public async Task TearDownDatabase()
        {
            await _bootstrapper.TearDownStore();
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

            foreach (var id in ids)
            {
                await _manager.LoadAsync(Guid.Empty.ToString(), id);
            }
        }

        public async Task FetchAllProductsForTenant()
        {
            await _manager.LoadAllAsync(Guid.Empty.ToString());
        }

        public async Task FetchAllProductsByName()
        {
            await _manager.FindAsync(Guid.Empty.ToString(), "widgets");
        }

        public async Task ChangeProductNameAndRefetchByName()
        {
            var id = await _manager.CreateNewAsync(Guid.Empty.ToString(), "woot");
            await _manager.ChangeName(Guid.Empty.ToString(), id, "woohoo");
            await _manager.FindAsync(Guid.Empty.ToString(), "woot");
            await _manager.FindAsync(Guid.Empty.ToString(), "woohoo");
        }
    }
}