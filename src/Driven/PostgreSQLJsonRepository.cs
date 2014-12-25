using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
                throw new PersistenceException("Cannot delete: " + ex.Message, ex);
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

                        await InsertEventsAsync(uow, aggregate);
                    }

                    uow.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceException("Cannot save: " + ex.Message, ex);
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

            var headerJson = _serializer.Serialize(new Dictionary<string, object>
                {
                    {"createdOn", DateTimeOffset.UtcNow},
                    {"createdBy", Thread.CurrentPrincipal.Identity.Name ?? ""}
                });

            var dataJson = _serializer.Serialize(aggregate);

            var commandText = string.Format("insert into {0} (header, data) values (@0, @1)", tableName);

            using (var command = unitOfWork.CreateCommand(commandText, headerJson, dataJson))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task UpdateAsync(UnitOfWork unitOfWork, object aggregate)
        {
            var tableName = GetTableName(aggregate.GetType());

            var headerJson = _serializer.Serialize(new Dictionary<string, object>
                {
                    {"updatedOn", DateTimeOffset.UtcNow},
                    {"updatedBy", Thread.CurrentPrincipal.Identity.Name ?? ""}
                });

            var dataJson = _serializer.Serialize(aggregate);

            var commandText = string.Format("update {0} set header = @0, data = @1 where id = @2", tableName);

            using (var command = unitOfWork.CreateCommand(commandText, headerJson, dataJson, GetIdentity(aggregate)))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task InsertEventsAsync(UnitOfWork unitOfWork, object aggregate)
        {
            var tableName = GetEventsTableName();

            var appliedEvents = GetAppliedEvents(aggregate);

            var commandText = string.Format("insert into {0} (header, data) values (@0, @1)", tableName);

            foreach (var appliedEvent in appliedEvents)
            {
                var headerJson = _serializer.Serialize(new Dictionary<string, object>
                {
                    {"createdOn", DateTimeOffset.UtcNow},
                    {"createdBy", Thread.CurrentPrincipal.Identity.Name ?? ""}
                });

                var dataJson = _serializer.Serialize(appliedEvent);

                using (var command = unitOfWork.CreateCommand(commandText, headerJson, dataJson))
                {
                    await command.ExecuteNonQueryAsync();
                }
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

        public async Task<IEnumerable<T>> FindAllAsync<T>(string where, string orderBy, params object[] args)
        {
            // TODO: allow plain old where clause
            throw new NotImplementedException("");
        }

        public async Task<IEnumerable<T>> FindAllAsync<T>(object filter, string orderBy)
        {
            var aggregates = new List<T>();

            var tableName = GetTableName(typeof(T));

            var filterJson = JsonConvert.SerializeObject(filter);

            var query = string.Format("select id, data from {0} where data @>'{1}'::jsonb {2}", tableName, filterJson, orderBy);

            try
            {
                using (var conn = await OpenConnection())
                using (var cmd = conn.CreateCommand(query))
                {
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
                throw new PersistenceException("Cannot find: " + query + " because: " + ex.Message, ex);
            }

            return aggregates;
        }

        public async Task<T> FindOneAsync<T>(object filter)
        {
            var aggregates = await FindAllAsync<T>(filter, "");

            return aggregates.FirstOrDefault();
        }

        private async Task<NpgsqlConnection> OpenConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
        
        private IEnumerable<object> GetAppliedEvents(object aggregate)
        {
            var adapter = _configuration.GetEventSourceAdapter();
            return adapter.AppliedEvents(aggregate);
        }

        private string GetTableName(Type type)
        {
            return _configuration.GetTableName(type);
        }

        private string GetEventsTableName()
        {
            return _configuration.GetEventsTableName();
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

        private IEnumerable<object> AppliedEvents(object aggregate)
        {
            return new object[0];
        }
    }
}