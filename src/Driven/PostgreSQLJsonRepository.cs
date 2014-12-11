using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace Driven
{
    public class PostgreSQLJsonRepository
    {
        private readonly ISerializer _serializer;
        private readonly ConnectionStringProvider _connectionStringProvider;

        public PostgreSQLJsonRepository(ISerializer serializer, ConnectionStringProvider connectionStringProvider)
        {
            _serializer = serializer;
            _connectionStringProvider = connectionStringProvider;
        }

        public async Task DeleteAsync(string tableName, IIdentifiable<long> aggregate)
        {
            try
            {
                using (var uow = await BeginUnitOfWork())
                {
                    await DeleteAsync(uow, tableName, aggregate);

                    uow.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceException("Cannot delete: " + aggregate + " because: " + ex.Message);
            }
        }

        public async Task DeleteAsync(string tableName, IEnumerable<IIdentifiable<long>> aggregates)
        {
            try
            {
                using (var uow = await BeginUnitOfWork())
                {
                    foreach (var aggregate in aggregates)
                    {
                        await DeleteAsync(uow, tableName, aggregate);
                    }

                    uow.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceException("Cannot delete: " + aggregates + " because: " + ex.Message);
            }
        }

        public async Task SaveAsync(string tableName, IIdentifiable<long> aggregate)
        {
            try
            {
                using (var uow = await BeginUnitOfWork())
                {
                    if (aggregate.IsUnidentified())
                    {
                        await InsertAsync(uow, tableName, aggregate);
                    }
                    else
                    {
                        await UpdateAsync(uow, tableName, aggregate);
                    }

                    uow.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceException("Cannot save: " + aggregate + " because: " + ex.Message);
            }
        }

        public async Task SaveAsync(string tableName, IEnumerable<IIdentifiable<long>> aggregates)
        {
            try
            {
                using (var uow = await BeginUnitOfWork())
                {
                    foreach (var aggregate in aggregates)
                    {
                        if (aggregate.IsUnidentified())
                        {
                            await InsertAsync(uow, tableName, aggregate);
                        }
                        else
                        {
                            await UpdateAsync(uow, tableName, aggregate);
                        }
                    }

                    uow.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceException("Cannot save: " + aggregates + " because: " + ex.Message);
            }
        }

        private async Task DeleteAsync(UnitOfWork unitOfWork, string tableName, IIdentifiable<long> aggregate)
        {
            var commandText = string.Format("delete from {0} where id = @0", tableName);
            using (var command = unitOfWork.CreateCommand(commandText, aggregate.Identity()))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task InsertAsync(UnitOfWork unitOfWork, string tableName, IIdentifiable<long> aggregate)
        {
            var json = _serializer.Serialize(aggregate);

            var commandText = string.Format("insert into {0} (data) values (@0)", tableName);

            using (var command = unitOfWork.CreateCommand(commandText, json))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task UpdateAsync(UnitOfWork unitOfWork, string tableName, IIdentifiable<long> aggregate)
        {
            var json = _serializer.Serialize(aggregate);

            var commandText = string.Format("update {0} set data = @0 where id = @1", tableName);

            using (var command = unitOfWork.CreateCommand(commandText, json, aggregate.Identity()))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task<UnitOfWork> BeginUnitOfWork()
        {
            var uow = new UnitOfWork(_connectionStringProvider.Store);
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

        public async Task<IEnumerable<T>> FindAllAsync<T>(string tableName, string filter, string orderBy, params object[] args)
            where T : IIdentifiable<long>
        {
            var aggregates = new List<T>();

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

                            aggregate.Identity(identity);

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

        public async Task<T> FindOneAsync<T>(string tableName, string filter, params object[] args)
            where T : IIdentifiable<long>
        {
            var aggregates = await FindAllAsync<T>(tableName, filter, "", args);

            return aggregates.FirstOrDefault();
        }

        private async Task<NpgsqlConnection> OpenConnection()
        {
            var connection = new NpgsqlConnection(_connectionStringProvider.Store);
            await connection.OpenAsync();
            return connection;
        }
    }
}