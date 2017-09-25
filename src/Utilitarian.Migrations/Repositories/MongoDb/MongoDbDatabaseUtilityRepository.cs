using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Utilitarian.Data.MongoDB;
using Utilitarian.Migrations.Interfaces;

namespace Utilitarian.Migrations.Repositories.MongoDb
{
    public class MongoDbDatabaseUtilityRepository : MongoDbRepositoryBase, IDatabaseUtilityService
    {
        public MongoDbDatabaseUtilityRepository(string databaseName, string connectionString) 
            : base(databaseName, connectionString)
        {
        }

        public async Task<bool> DatabaseExists()
        {
            var databases = (await MongoClient.ListDatabasesAsync()).ToList();

            return databases.Any(d => d.GetValue("name") == DatabaseName);
        }

        public async Task DropDatabase()
        {
            await MongoClient.DropDatabaseAsync(DatabaseName);
        }
    }
}
