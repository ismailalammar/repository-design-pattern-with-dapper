using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL.DbConnection
{
    public class Connection : IConnection
    {
        private IConfiguration _config;
        private string Connectionstring = "localhost";
        private IDbConnection lContext;

        public Connection(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection LContext
        {
            get
            {
                if (lContext == null)
                {
                    lContext = new SqlConnection(_config.GetConnectionString(Connectionstring));
                }
                return lContext;
            }
        }
    }
}
