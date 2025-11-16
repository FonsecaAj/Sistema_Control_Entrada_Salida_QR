using CarnetDigital.Entities;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CarnetDigital.Repository
{
    public class DependenciasRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DependenciasRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IDbConnection CreateConnection()
        {
            return _connectionFactory.CreateConnection();
        }

        public async Task<IEnumerable<Dependencias>> GetAllAsync()
        {
            using var conn = CreateConnection();

            var lista = await conn.QueryAsync<Dependencias>(
                "sp_ListarDependencias",
                commandType: CommandType.StoredProcedure
            );

            return lista;
        }
    }
}

