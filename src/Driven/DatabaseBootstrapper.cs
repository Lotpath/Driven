using System.Data;
using System.Threading.Tasks;
using Npgsql;

namespace Driven
{
    public class DatabaseBootstrapper
    {
        private readonly ConnectionStringProvider _connectionStringProvider;

        public DatabaseBootstrapper(ConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        public async Task<bool> StoreExists()
        {
            var storeDatabaseName = new NpgsqlConnectionStringBuilder(_connectionStringProvider.Store).Database;

            using (var conn = new NpgsqlConnection(_connectionStringProvider.Master))
            {
                await conn.OpenAsync();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select count(*) from pg_catalog.pg_database where datname=:dbname";
                    var p = cmd.CreateParameter();
                    p.ParameterName = "dbname";
                    p.DbType = DbType.AnsiString;
                    p.Value = storeDatabaseName;
                    cmd.Parameters.Add(p);
                    cmd.CommandType = CommandType.Text;
                    return (long)await cmd.ExecuteScalarAsync() != 0;
                }
            }
        }

        public async Task InitializeStore()
        {
            var storeDatabaseName = new NpgsqlConnectionStringBuilder(_connectionStringProvider.Store).Database;

            using (var conn = new NpgsqlConnection(_connectionStringProvider.Master))
            {
                await conn.OpenAsync();

                using (var comm = conn.CreateCommand())
                {
                    comm.CommandText = string.Format("create database \"{0}\"", storeDatabaseName);
                    await comm.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task TearDownStore()
        {
            using (var conn = new NpgsqlConnection(_connectionStringProvider.Store))
            {
                await conn.OpenAsync();

                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tran;

                        cmd.CommandText = "drop schema public cascade";

                        await cmd.ExecuteNonQueryAsync();
                    }

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tran;

                        cmd.CommandText = "create schema public authorization postgres";

                        await cmd.ExecuteNonQueryAsync();
                    }

                    tran.Commit();
                }
            }
        }

        public async Task SchemaUpdate(PersistenceConfiguration configuration)
        {
            using (var conn = new NpgsqlConnection(_connectionStringProvider.Store))
            {
                await conn.OpenAsync();

                using (var tran = conn.BeginTransaction())
                {
                    foreach (var tableName in configuration.GetAllTableNames())
                    {
                        await EnsureTableExists(conn, tran, tableName);
                    }

                    foreach (var index in configuration.GetIndexDefinitions())
                    {
                        await EnsureIndexExists(conn, tran, index.Key, index.Value);
                    }

                    tran.Commit();
                }
            }
        }

        public async Task ExecuteSql(string sql, params object[] args)
        {
            using (var conn = new NpgsqlConnection(_connectionStringProvider.Store))
            {
                await conn.OpenAsync();

                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = conn.CreateCommand(sql, args))
                    {
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
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

            using (var cmd = conn.CreateCommand(string.Format("create table {0} (id bigserial primary key, data json not null);", tableName)))
            {
                cmd.Transaction = tran;

                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task EnsureIndexExists(NpgsqlConnection conn, NpgsqlTransaction tran, string indexName, string index)
        {
            using (var cmd = conn.CreateCommand("select count(*) from pg_indexes where schemaname='public' and indexname=:0", indexName))
            {
                cmd.Transaction = tran;

                var exists = (long)await cmd.ExecuteScalarAsync() != 0;

                if (exists)
                {
                    return;
                }
            }

            using (var cmd = conn.CreateCommand(index))
            {
                cmd.Transaction = tran;

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}