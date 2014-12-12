using Xunit;

namespace Driven.Tests
{
    public class NewtonsoftJsonSerializerTests
    {
        [Fact]
        public void can_serialize_and_deserialize_object_with_private_members()
        {
            var serializer = new NewtonsoftJsonSerializer();
            var json = serializer.Serialize(new ObjectWithPrivateMembers(1, "test", "description"));
            var o = serializer.Deserialize<ObjectWithPrivateMembers>(json);

            Assert.Equal(1, o.GetId());
            Assert.Equal("test", o.GetName());
            Assert.Equal("description", o.GetDescription());
        }

        /// <summary>
        /// this is to prevent serialization and deserialization of 
        /// surrogate properties on entity base classes
        /// </summary>
        [Fact]
        public void deserialized_object_base_class_values_are_not_deserialized()
        {
            var serializer = new NewtonsoftJsonSerializer();
            var target = new ObjectWithPrivateMembersAndBaseClass();
            target.SetDeclaredNumber(10);
            target.SetNumber(50);
            var json = serializer.Serialize(target);
            var o = serializer.Deserialize<ObjectWithPrivateMembersAndBaseClass>(json);

            Assert.Equal(10, o.DeclaredNumber);
            Assert.Equal(0, o.BaseClassNumber);
        }

        [Fact]
        public void serializer_configured_to_use_default_newtonsoft_json_contract_resolver_does_not_deserialize_private_field_values()
        {
            var serializer = new NewtonsoftJsonSerializer(cfg => cfg.UseDefaultNewtonsoftJsonContractResolver());
            var json = serializer.Serialize(new ObjectWithPrivateMembers(1, "test", "description"));
            var o = serializer.Deserialize<ObjectWithPrivateMembers>(json);

            Assert.Equal(0, o.GetId());
            Assert.Equal(null, o.GetName());
            Assert.Equal(null, o.GetDescription());
        }

        public class ObjectWithPrivateMembers
        {
            private readonly int _id;
            private readonly string _name;
            private readonly string _desc;

            public ObjectWithPrivateMembers(int id, string name, string desc)
            {
                _id = id;
                _name = name;
                _desc = desc;
            }

            public int GetId()
            {
                return _id;
            }

            public string GetName()
            {
                return _name;
            }

            public string GetDescription()
            {
                return _desc;
            }
        }

        public abstract class BaseClass
        {
            private int _baseClassNumber;

            public int BaseClassNumber
            {
                get { return _baseClassNumber; }
            }

            public void SetNumber(int number)
            {
                _baseClassNumber = number;
            }
        }

        public class ObjectWithPrivateMembersAndBaseClass : BaseClass
        {
            private int _declaredNumber;

            public int DeclaredNumber
            {
                get { return _declaredNumber; }
            }

            public void SetDeclaredNumber(int number)
            {
                _declaredNumber = number;
            }
        }
    }
}