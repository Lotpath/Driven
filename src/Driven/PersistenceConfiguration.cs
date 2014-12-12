using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Driven
{
    public interface IIdentityAdapter
    {
        void SetIdentity(object target, long value);
        long GetIdentity(object target);
        bool IsUnidentified(object target);
    }

    public class ReflectionIdentityAdapter : IIdentityAdapter
    {
        private readonly IIdentityAdapter _adapter;

        public ReflectionIdentityAdapter(Type type)
        {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var get = methods.SingleOrDefault(x => x.Name == "GetIdentity");
            var set = methods.SingleOrDefault(x => x.Name == "SetIdentity");

            if (get == null || get.ReturnType != typeof(long))
            {
                throw new ArgumentException("Method with signature 'long GetIdentity()' not found");
            }

            if (set == null || set.GetParameters().ToList()[0].ParameterType != typeof(long))
            {
                throw new ArgumentException("Method with signature 'SetIdentity(long)' not found");
            }

            _adapter = new DelegateIdentityAdapter(
                o => (long) get.Invoke(o, new object[0]),
                (o, id) => set.Invoke(o, new object[] {id}));
        }

        public void SetIdentity(object target, long value)
        {
            _adapter.SetIdentity(target, value);
        }

        public long GetIdentity(object target)
        {
            return _adapter.GetIdentity(target);
        }

        public bool IsUnidentified(object target)
        {
            return _adapter.IsUnidentified(target);
        }
    }

    public class DelegateIdentityAdapter : IIdentityAdapter
    {
        private Action<object, long> _setIdentity;
        private Func<object, long> _getIdentity;

        public DelegateIdentityAdapter(Func<object, long> getIdentity, Action<object, long> setIdentity)
        {
            _getIdentity = getIdentity;
            _setIdentity = setIdentity;
        }

        public void SetIdentity(object target, long value)
        {
            _setIdentity(target, value);
        }

        public long GetIdentity(object target)
        {
            return _getIdentity(target);
        }

        public bool IsUnidentified(object target)
        {
            return GetIdentity(target) <= 0;
        }
    }

    public class PersistenceConfiguration
    {
        private readonly Dictionary<Type, string> _entityTypeToTableNameMappings
            = new Dictionary<Type, string>();

        private readonly List<IndexDefinition> _indexDefinitions
            = new List<IndexDefinition>();

        private readonly Dictionary<Type, IIdentityAdapter> _idAdapters
            = new Dictionary<Type, IIdentityAdapter>();

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

        public IIdentityAdapter GetIdAdapter(Type type)
        {
            return _idAdapters[type];
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
        }

        private class IndexDefinition
        {
            public string TableName { get; set; }
            public string IndexName { get; set; }
            public string Index { get; set; }
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