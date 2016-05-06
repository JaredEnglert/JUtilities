using JUtilities.Settings;

namespace JUtilities.Status.Test.Unit.Mocks
{
    public class DatabaseStatusCheck : DatabaseStatusCheckBase
    {
        public DatabaseStatusCheck(IConnectionStringProvider connectionStringProvider) 
            : base(connectionStringProvider)
        {
        }

        protected override string ConnectionStringName
        {
            get { return "TestConnectionString"; }
        }
    }
}
