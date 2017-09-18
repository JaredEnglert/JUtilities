using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Utilitarian.Data.MongoDB
{
    public abstract class MongoDbRepositoryBase
    {
        private readonly string _connectionString;

        public string DatabaseName { get; }

        public MongoClient MongoClient => MongoDbClientFactory.GetMongoClient(DatabaseName, _connectionString);

        protected MongoDbRepositoryBase(string databaseName, string connectionString)
        {
            DatabaseName = databaseName;
            _connectionString = connectionString;
        }

        protected IMongoDatabase GetDatabase()
        {
            return MongoClient.GetDatabase(DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return GetDatabase().GetCollection<T>(collectionName);
        }
        
        public async Task<bool> DatabaseExists()
        {
            var databases = (await MongoClient.ListDatabasesAsync()).ToList();

            return databases.Any(d => d.GetValue("name") == DatabaseName);
        }

        //protected void RunScript(string script)
        //{
        //    GetDatabase().Eval("");
        //}
    }
}
