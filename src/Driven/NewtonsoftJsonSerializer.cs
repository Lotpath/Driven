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
            : this(ApplyDefaultConfiguration)
        {
        }

        public NewtonsoftJsonSerializer(Action<JsonSerializerSettings> cfg)
        {
            _settings = new JsonSerializerSettings();

            if (cfg != null)
            {
                cfg(_settings);
            }
        }

        private static void ApplyDefaultConfiguration(JsonSerializerSettings settings)
        {
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.Formatting = Formatting.None;
            settings.ContractResolver = new PrivateFieldsContractResolver();
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
        }
    }
}