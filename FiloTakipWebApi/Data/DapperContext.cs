using Microsoft.Data.SqlClient;
using System.Data;

namespace FiloTakipWebApi.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("FiloTakipVeritabani") 
                ?? throw new InvalidOperationException("Connection string 'FiloTakipVeritabani' not found.");
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
