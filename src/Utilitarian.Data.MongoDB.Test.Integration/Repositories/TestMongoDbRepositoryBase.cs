using Utilitarian.Settings;

namespace Utilitarian.Data.MongoDB.Test.Integration.Repositories
{
    public class TestMongoDbRepositoryBase : MongoDbRepositoryBase
    {
        public TestMongoDbRepositoryBase() 
            : base("TestMongoDbRepositoryBase", new AppSettingsConnectionStringProvider().Get("TestMongoDbRepositoryBase"))
        {
        }
    }
}
