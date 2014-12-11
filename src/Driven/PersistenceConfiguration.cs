using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using Npgsql;

namespace Driven
{
    public class PersistenceConfiguration
    {
        private readonly Dictionary<Type, string> _entityTypeToTableNameMappings
            = new Dictionary<Type, string>();

        private readonly List<IndexDefinition> _indexDefinitions
            = new List<IndexDefinition>();

        private ISerializer _serializer;
        private ConnectionStringProvider _connectionStringProvider;

        public PersistenceConfiguration(Action<PersistenceConfigurationConfigurer> cfg)
        {
            if (cfg != null)
            {
                cfg(new PersistenceConfigurationConfigurer(this));
            }
        }

        public IEnumerable<string> GetAllTableNames()
        {
            return _entityTypeToTableNameMappings.Values;
        }

        public IDictionary<string, string> GetIndexDefinitions()
        {
            return new ReadOnlyDictionary<string, string>(
                _indexDefinitions
                    .ToDictionary(
                        x => x.IndexName,
                        x => string.Format("CREATE INDEX {0} ON {1} {2}",
                                           x.IndexName, x.TableName, x.Index)));
        }

        public string GetTableName(Type type)
        {
            return _entityTypeToTableNameMappings[type];
        }

        public string GetTableName<T>()
        {
            return _entityTypeToTableNameMappings[typeof(T)];
        }

        public string MasterConnectionString
        {
            get { return _connectionStringProvider.Master; }
        }

        public string StoreConnectionString
        {
            get { return _connectionStringProvider.Store; }
        }

        public ISerializer Serializer { get { return _serializer; } }

        public class PersistenceConfigurationConfigurer
        {
            private readonly PersistenceConfiguration _configuration;

            protected internal PersistenceConfigurationConfigurer(PersistenceConfiguration configuration)
            {
                _configuration = configuration;
            }

            public PersistenceConfigurationConfigurer Map(Type type, string tableName)
            {
                _configuration._entityTypeToTableNameMappings.Add(type, tableName);
                return this;
            }

            public PersistenceConfigurationConfigurer Map<T>(string tableName)
            {
                _configuration._entityTypeToTableNameMappings.Add(typeof(T), tableName);
                return this;
            }

            public PersistenceConfigurationConfigurer Index<T>(string indexName, string index)
            {
                var tableName = _configuration.GetTableName<T>();
                var definition = new IndexDefinition
                    {
                        TableName = tableName,
                        IndexName = indexName,
                        Index = index
                    };
                _configuration._indexDefinitions.Add(definition);
                return this;
            }

            public PersistenceConfigurationConfigurer Serializer(ISerializer serializer)
            {
                _configuration._serializer = serializer;
                return this;
            }

            public PersistenceConfigurationConfigurer UseConnectionStringsFromAppConfig()
            {
                _configuration._connectionStringProvider = new ConnectionStringProvider();
                return this;
            }
        }

        private class IndexDefinition
        {
            public string TableName { get; set; }
            public string IndexName { get; set; }
            public string Index { get; set; }
        }

        public class ConnectionStringProvider
        {
            public ConnectionStringProvider()
            {
                Master = ConfigurationManager.ConnectionStrings["Master"].ConnectionString;
                Store = ConfigurationManager.ConnectionStrings["Store"].ConnectionString;
            }

            public string Master { get; set; }
            public string Store { get; set; }
        }
    }
}