using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace Driven
{
    public class PostgreSQLJsonRepository
    {
        private readonly PersistenceConfiguration _configuration;
        private readonly ISerializer _serializer;
        private readonly string _connectionString;

        public PostgreSQLJsonRepository(PersistenceConfiguration configuration)
        {
            _configuration = configuration;
            _serializer = _configuration.Serializer;
            _connectionString = _configuration.StoreConnectionString;
        }

        public async Task DeleteAsync(object aggregate)
        {
            await DeleteAsync(new[] {aggregate});
        }

        public async Task DeleteAsync(IEnumerable<object> aggregates)
        {
            try
            {
                using (var uow = await BeginUnitOfWork())
                {
                    foreach (var aggregate in aggregates)
                    {
                        await DeleteAsync(uow, aggregate);
                    }

                    uow.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceException("Cannot delete: " + ex.Message);
            }
        }

        public async Task SaveAsync(object aggregate)
        {
            await SaveAsync(new[] {aggregate});
        }

        public async Task SaveAsync(IEnumerable<object> aggregates)
        {
            try
            {
                using (var uow = await BeginUnitOfWork())
                {
                    foreach (var aggregate in aggregates)
                    {
                        if (IsUnidentified(aggregate))
                        {
                            await InsertAsync(uow, aggregate);
                        }
                        else
                        {
                            await UpdateAsync(uow, aggregate);
                        }
                    }

                    uow.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceException("Cannot save: " + ex.Message);
            }
        }

        private async Task DeleteAsync(UnitOfWork unitOfWork, object aggregate)
        {
            var tableName = GetTableName(aggregate.GetType());

            var commandText = string.Format("delete from {0} where id = @0", tableName);

            using (var command = unitOfWork.CreateCommand(commandText, GetIdentity(aggregate)))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task InsertAsync(UnitOfWork unitOfWork, object aggregate)
        {
            var tableName = GetTableName(aggregate.GetType());

            var json = _serializer.Serialize(aggregate);

            var commandText = string.Format("insert into {0} (data) values (@0)", tableName);

            using (var command = unitOfWork.CreateCommand(commandText, json))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task UpdateAsync(UnitOfWork unitOfWork, object aggregate)
        {
            var tableName = GetTableName(aggregate.GetType());

            var json = _serializer.Serialize(aggregate);

            var commandText = string.Format("update {0} set data = @0 where id = @1", tableName);

            using (var command = unitOfWork.CreateCommand(commandText, json, GetIdentity(aggregate)))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task<UnitOfWork> BeginUnitOfWork()
        {
            var uow = new UnitOfWork(_connectionString);
            await uow.Begin();
            return uow;
        }

        private class UnitOfWork : IDisposable
        {
            private readonly string _connectionString;
            private NpgsqlConnection _connection;
            private NpgsqlTransaction _transaction;

            public UnitOfWork(string connectionString)
            {
                _connectionString = connectionString;
            }

            public async Task Begin()
            {
                _connection = new NpgsqlConnection(_connectionString);
                await _connection.OpenAsync();
                _transaction = _connection.BeginTransaction();
            }

            public NpgsqlCommand CreateCommand(string commandText, params object[] args)
            {
                var command = _connection.CreateCommand(commandText, args);

                command.Transaction = _transaction;

                command.Prepare();

                return command;
            }

            public void Commit()
            {
                _transaction.Commit();
            }

            public void Dispose()
            {
                _transaction.Dispose();
                _connection.Dispose();
            }
        }

        public async Task<IEnumerable<T>> FindAllAsync<T>(string filter, string orderBy, params object[] args)
        {
            var aggregates = new List<T>();

            var tableName = GetTableName(typeof(T));

            var query = string.Format("select id, data from {0} where {1} {2}", tableName, filter, orderBy);

            try
            {
                using (var conn = await OpenConnection())
                using (var cmd = conn.CreateCommand(query, args))
                {
                    cmd.Prepare();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var identity = reader.GetInt64(0);
                            var json = reader.GetValue(1).ToString();

                            var aggregate = _serializer.Deserialize<T>(json);

                            SetIdentity(aggregate, identity);

                            aggregates.Add(aggregate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceException("Cannot find: " + query + " because: " + ex.Message);
            }

            return aggregates;
        }

        public async Task<T> FindOneAsync<T>(string filter, params object[] args)
        {
            var aggregates = await FindAllAsync<T>(filter, "", args);

            return aggregates.FirstOrDefault();
        }

        private async Task<NpgsqlConnection> OpenConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
        
        private string GetTableName(Type type)
        {
            return _configuration.GetTableName(type);
        }

        private long GetIdentity(object aggregate)
        {
            var adapter = _configuration.GetIdAdapter(aggregate.GetType());
            return adapter.GetIdentity(aggregate);
        }

        private void SetIdentity(object aggregate, long identity)
        {
            var adapter = _configuration.GetIdAdapter(aggregate.GetType());
            adapter.SetIdentity(aggregate, identity);
        }

        private bool IsUnidentified(object aggregate)
        {
            var adapter = _configuration.GetIdAdapter(aggregate.GetType());
            return adapter.IsUnidentified(aggregate);
        }
    }
}