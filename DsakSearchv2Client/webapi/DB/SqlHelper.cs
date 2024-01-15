using System.Data;
using System.Data.SqlClient;
using webapi.Configuration;
using webapi.Interfaces;

namespace webapi.DB
{
    public class SqlHelper : ISqlHelper
    {
        readonly IDsakConfiguration dsakConfig;

        public SqlHelper(IDsakConfiguration dsakConfig)
        {
            this.dsakConfig = dsakConfig;
        }

        public IDbConnection CreateConnection()
        {
            var connection = new SqlConnection(dsakConfig.GetConnectionString());
            connection.Open();
            return connection;
        }
    }
}
