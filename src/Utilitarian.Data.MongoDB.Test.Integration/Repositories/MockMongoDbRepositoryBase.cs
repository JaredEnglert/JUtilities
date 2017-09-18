using Utilitarian.Settings;

namespace Utilitarian.Data.MongoDB.Test.Integration.Repositories
{
    public class MockMongoDbRepositoryBase : MongoDbRepositoryBase
    {
        public MockMongoDbRepositoryBase() 
            : base("MockMongoDbRepositoryBase", new AppSettingsConnectionStringProvider().Get("MockMongoDbRepositoryBase"))
        {
        }
    }
}
