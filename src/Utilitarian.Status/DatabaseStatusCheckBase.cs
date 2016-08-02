using System.Data;
using System.Data.SqlClient;
using Utilitarian.Settings;

namespace Utilitarian.Status
{
    public abstract class DatabaseStatusCheckBase : IStatusCheck
    {
        protected abstract string ConnectionStringName { get; }

        private readonly IConnectionStringProvider _connectionStringProvider;

        protected DatabaseStatusCheckBase(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        public bool IsActive()
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(_connectionStringProvider.Get(ConnectionStringName)) { ConnectTimeout = 3 };

            using (var connection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandTimeout = 3;
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT db_name()";
                var dbName = (string)command.ExecuteScalar();

                connection.Close();

                return dbName == sqlConnectionStringBuilder.InitialCatalog;
            }
        }
    }
}
