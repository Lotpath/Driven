using System;
using System.Collections.Generic;

namespace Driven
{
    public class EntityToTableNameMappingConfiguration
    {
        private readonly Dictionary<Type, string> _entityTypeToTableNameMappings
            = new Dictionary<Type, string>();

        public EntityToTableNameMappingConfiguration(Action<EntityToTableNameMappingConfigurationConfigurer> cfg)
        {
            if (cfg != null)
            {
                cfg(new EntityToTableNameMappingConfigurationConfigurer(this));
            }
        }

        public IEnumerable<string> GetAllTableNames()
        {
            return _entityTypeToTableNameMappings.Values;
        }

        public string GetTableName(Type type)
        {
            return _entityTypeToTableNameMappings[type];
        }

        public string GetTableName<T>()
        {
            return _entityTypeToTableNameMappings[typeof(T)];
        }

        public class EntityToTableNameMappingConfigurationConfigurer
        {
            private readonly EntityToTableNameMappingConfiguration _configuration;

            protected internal EntityToTableNameMappingConfigurationConfigurer(EntityToTableNameMappingConfiguration configuration)
            {
                _configuration = configuration;
            }

            public EntityToTableNameMappingConfigurationConfigurer Map(Type type, string tableName)
            {
                _configuration._entityTypeToTableNameMappings.Add(type, tableName);
                return this;
            }

            public EntityToTableNameMappingConfigurationConfigurer Map<T>(string tableName)
            {
                _configuration._entityTypeToTableNameMappings.Add(typeof(T), tableName);
                return this;
            }
        }
    }
}