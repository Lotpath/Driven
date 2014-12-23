using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Driven
{
    public class NewtonsoftJsonSerializer : ISerializer
    {
        private readonly JsonSerializerSettings _settings;

        public NewtonsoftJsonSerializer()
            : this(null)
        {
        }

        public NewtonsoftJsonSerializer(Action<NewtonsoftJsonSerializerConfigurer> cfg)
        {
            _settings = new JsonSerializerSettings();
            _settings.TypeNameHandling = TypeNameHandling.None;
            _settings.Formatting = Formatting.None;
            _settings.ContractResolver = new PrivateFieldsContractResolver();

            var configurer = new NewtonsoftJsonSerializerConfigurer(this);

            if (cfg != null)
            {
                cfg(configurer);
            }
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }

        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, _settings);
        }

        private class PrivateFieldsContractResolver : DefaultContractResolver
        {
            protected override List<MemberInfo> GetSerializableMembers(Type objectType)
            {
                return objectType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Cast<MemberInfo>().ToList();
            }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var properties = base.CreateProperties(type, memberSerialization);

                foreach (var property in properties)
                {
                    property.Readable = true;
                    property.Writable = true;
                }

                return properties;
            }

            protected override string ResolvePropertyName(string propertyName)
            {
                if (propertyName.StartsWith("_"))
                {
                    return propertyName.Substring(1);
                }
                return base.ResolvePropertyName(propertyName);
            }
        }

        public class NewtonsoftJsonSerializerConfigurer
        {
            private readonly NewtonsoftJsonSerializer _serializer;

            protected internal NewtonsoftJsonSerializerConfigurer(NewtonsoftJsonSerializer serializer)
            {
                _serializer = serializer;
            }

            public NewtonsoftJsonSerializerConfigurer UseDefaultDrivenPrivateFieldsContractResolver()
            {
                _serializer._settings.ContractResolver = new PrivateFieldsContractResolver();
                return this;
            }

            public NewtonsoftJsonSerializerConfigurer UseDefaultNewtonsoftJsonContractResolver()
            {
                _serializer._settings.ContractResolver = new DefaultContractResolver();
                return this;
            }    
            
            public NewtonsoftJsonSerializerConfigurer UseCamelCasePropertyNamesContractResolver()
            {
                _serializer._settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                return this;
            }

            public NewtonsoftJsonSerializerConfigurer UseContractResolver(IContractResolver resolver)
            {
                _serializer._settings.ContractResolver = resolver;
                return this;
            }
        }
    }
}