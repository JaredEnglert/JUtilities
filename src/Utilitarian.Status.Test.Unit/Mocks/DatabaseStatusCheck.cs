using Utilitarian.Settings;

namespace Utilitarian.Status.Test.Unit.Mocks
{
    public class DatabaseStatusCheck : DatabaseStatusCheckBase
    {
        public DatabaseStatusCheck(IConnectionStringProvider connectionStringProvider) 
            : base(connectionStringProvider)
        {
        }

        protected override string ConnectionStringName => "TestConnectionString";
    }
}
