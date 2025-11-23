using CarnetDigital.Entities;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CarnetDigital.Repository
{
    public class TiposIdentificacionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TiposIdentificacionRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IDbConnection CreateConnection()
        {
            return _connectionFactory.CreateConnection();
        }

        public async Task<IEnumerable<Tipos_Identificacion>> GetAllAsync()
        {
            using var conn = CreateConnection();

            var lista = await conn.QueryAsync<Tipos_Identificacion>(
                "sp_ListarTiposIdentificacion",
                commandType: CommandType.StoredProcedure
            );

            return lista;
        }
    }
}

