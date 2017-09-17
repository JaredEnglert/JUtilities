using System.Collections.Concurrent;
using MongoDB.Driver;
using Utilitarian.Settings;

namespace Utilitarian.Data.MongoDB
{
    public static class MongoDbClientFactory
    {
        public static ConcurrentDictionary<string, MongoClient> MongoClients;

        static MongoDbClientFactory()
        {
            MongoClients = new ConcurrentDictionary<string, MongoClient>();
        }

        public static MongoClient GetMongoClient(string databaseName, IConnectionStringProvider connectionStringProvider)
        {
            MongoClient mongoClient;
            MongoClients.TryGetValue(databaseName, out mongoClient);

            if (mongoClient != null) return mongoClient;

            mongoClient = new MongoClient(connectionStringProvider.Get(databaseName));

            MongoClients.TryAdd(databaseName, mongoClient);

            return mongoClient;
        }
    }
}
