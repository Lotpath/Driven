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
    }
}