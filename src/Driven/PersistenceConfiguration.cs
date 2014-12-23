using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;

namespace Driven
{
    public class PersistenceConfiguration
    {
        private readonly Dictionary<Type, string> _entityTypeToTableNameMappings
            = new Dictionary<Type, string>();

        private readonly Dictionary<Type, IIdentityAdapter> _idAdapters
            = new Dictionary<Type, IIdentityAdapter>();

        private ISerializer _serializer;
        private ConnectionStringProvider _connectionStringProvider;

        private string _eventsTableName = "events";
        private IEventSourceAdapter _eventSourceAdapter = new ReflectionEventSourceAdapter();

        public PersistenceConfiguration(Action<PersistenceConfigurationConfigurer> cfg)
        {
            if (cfg != null)
            {
                cfg(new PersistenceConfigurationConfigurer(this));
            }
        }

        public IEnumerable<string> GetAllTableNames()
        {
            var tableNames = _entityTypeToTableNameMappings.Values.ToList();
            tableNames.Add(_eventsTableName);
            return tableNames;
        }

        public string GetTableName(Type type)
        {
            return _entityTypeToTableNameMappings[type];
        }

        public string GetTableName<T>()
        {
            return _entityTypeToTableNameMappings[typeof(T)];
        }

        public IIdentityAdapter GetIdAdapter(Type type)
        {
            return _idAdapters[type];
        }

        public string GetEventsTableName()
        {
            return _eventsTableName;
        }
        
        public IEventSourceAdapter GetEventSourceAdapter()
        {
            return _eventSourceAdapter;
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

            public PersistenceConfigurationConfigurer Events<T>(string tableName, Func<T,IEnumerable<object>> appliedEvents)
            {
                _configuration._eventsTableName = tableName;
                return this;
            }

            public PersistenceConfigurationConfigurer Map<T>(string tableName, Func<T, long> getId = null, Action<T, long> setId = null)
            {
                if (getId == null && setId == null)
                {
                    _configuration._idAdapters.Add(typeof(T), new ReflectionIdentityAdapter(typeof(T)));
                }
                else if (getId != null && setId != null)
                {
                    _configuration._idAdapters
                                  .Add(typeof (T),
                                       new DelegateIdentityAdapter(o => getId((T) o), (o, id) => setId((T) o, id)));
                }
                else
                {
                    throw new ArgumentException("getId and setId must both have a value, or both be null");
                }

                _configuration._entityTypeToTableNameMappings.Add(typeof(T), tableName);
                
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

            public PersistenceConfigurationConfigurer UseMasterConnectionString(string connectionString)
            {
                _configuration._connectionStringProvider = (_configuration._connectionStringProvider ??new ConnectionStringProvider());
                _configuration._connectionStringProvider.Master = connectionString;
                return this;
            }

            public PersistenceConfigurationConfigurer UseStoreConnectionString(string connectionString)
            {
                _configuration._connectionStringProvider = (_configuration._connectionStringProvider ?? new ConnectionStringProvider());
                _configuration._connectionStringProvider.Store = connectionString;
                return this;
            }

            public PersistenceConfigurationConfigurer EventsTableName(string eventTableName)
            {
                _configuration._eventsTableName = eventTableName;
                return this;
            }
        }

        private class ConnectionStringProvider
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