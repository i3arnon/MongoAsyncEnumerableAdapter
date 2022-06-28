using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoAsyncEnumerableAdapterTests
{
    public class TestModel
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string TestProperty { get; set; }
    }
}
