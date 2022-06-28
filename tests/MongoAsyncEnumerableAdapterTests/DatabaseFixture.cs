using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Mongo2Go;
using MongoDB.Driver;

namespace MongoAsyncEnumerableAdapterTests
{
    public class DatabaseFixture : IDisposable
    {
        private static readonly MongoDbRunner Runner = MongoDbRunner.Start();
        private static readonly MongoClient Client = new(Runner.ConnectionString);

        public void Dispose()
        {
            ((IDisposable)Runner).Dispose();
        }

        public IMongoCollection<TDocument> GetCollection<TDocument>(string collectionName)
        {
            var database = Client.GetDatabase("IntegrationTest");
            return database.GetCollection<TDocument>(collectionName);
        }
    }
}
