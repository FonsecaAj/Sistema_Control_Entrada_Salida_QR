using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace CarnetDigital.Repository
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuracion;

        public DbConnectionFactory(IConfiguration configuracion)
        {
            _configuracion = configuracion;
        }

        public IDbConnection CreateConnection()
        {
            var Str_Conexion = _configuracion.GetConnectionString("DefaultConnection");
            
            return new SqlConnection(Str_Conexion);
        }
    }
}