using System.Threading.Tasks;
using Npgsql;

namespace Driven
{
    public class DatabaseBootstrapper
    {
        private readonly PersistenceConfiguration _configuration;

        public DatabaseBootstrapper(PersistenceConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> StoreExists()
        {
            var storeDatabaseName = new NpgsqlConnectionStringBuilder(_configuration.StoreConnectionString).Database;

            using (var conn = new NpgsqlConnection(_configuration.MasterConnectionString))
            {
                await conn.OpenAsync();

                using (var cmd = conn.CreateCommand("select count(*) from pg_catalog.pg_database where datname=@0", storeDatabaseName))
                {
                    return (long)await cmd.ExecuteScalarAsync() != 0;
                }
            }
        }

        public async Task InitializeStore()
        {
            var storeDatabaseName = new NpgsqlConnectionStringBuilder(_configuration.StoreConnectionString).Database;

            using (var conn = new NpgsqlConnection(_configuration.MasterConnectionString))
            {
                await conn.OpenAsync();

                using (var comm = conn.CreateCommand(string.Format("create database \"{0}\"", storeDatabaseName)))
                {
                    await comm.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task TearDownStore()
        {
            using (var conn = new NpgsqlConnection(_configuration.StoreConnectionString))
            {
                await conn.OpenAsync();

                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = conn.CreateCommand("drop schema public cascade"))
                    {
                        cmd.Transaction = tran;

                        await cmd.ExecuteNonQueryAsync();
                    }

                    using (var cmd = conn.CreateCommand("create schema public authorization postgres"))
                    {
                        cmd.Transaction = tran;

                        await cmd.ExecuteNonQueryAsync();
                    }

                    tran.Commit();
                }
            }
        }

        public async Task EnsureSchemaIsUpToDate()
        {
            using (var conn = new NpgsqlConnection(_configuration.StoreConnectionString))
            {
                await conn.OpenAsync();

                using (var tran = conn.BeginTransaction())
                {
                    foreach (var tableName in _configuration.GetAllTableNames())
                    {
                        await EnsureTableExists(conn, tran, tableName);
                    }

                    tran.Commit();
                }
            }
        }

        private async Task EnsureTableExists(NpgsqlConnection conn, NpgsqlTransaction tran, string tableName)
        {
            using (var cmd = conn.CreateCommand("select count(*) from information_schema.tables where table_schema='public' and table_name=:0", tableName))
            {
                cmd.Transaction = tran;
                
                var exists = (long)await cmd.ExecuteScalarAsync() != 0;

                if (exists)
                {
                    return;
                }
            }

            using (var cmd = conn.CreateCommand(string.Format("create table {0} (id bigserial primary key, data jsonb not null);", tableName)))
            {
                cmd.Transaction = tran;

                await cmd.ExecuteNonQueryAsync();
            }

            // we only indend to use the '@>' jsonb operator
            // see: http://www.postgresql.org/docs/9.4/static/datatype-json.html#JSON-INDEXING
            // for explanation of why 'jsonb_path_ops' parameter is specified when creating the gin index on the jsonb data column

            using (var cmd = conn.CreateCommand(string.Format("CREATE INDEX {0}_data_gin_index ON {0} USING gin (data jsonb_path_ops);", tableName)))
            {
                cmd.Transaction = tran;

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}