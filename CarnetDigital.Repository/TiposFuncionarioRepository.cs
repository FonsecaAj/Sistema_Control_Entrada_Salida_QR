using CarnetDigital.Entities;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CarnetDigital.Repository
{
    public class TiposFuncionarioRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TiposFuncionarioRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IDbConnection CreateConnection()
        {
            return _connectionFactory.CreateConnection();
        }

        public async Task<IEnumerable<Tipos_Funcionarios>> GetAllAsync()
        {
            using var conn = CreateConnection();

            var lista = await conn.QueryAsync<Tipos_Funcionarios>(
                "sp_ListarTiposFuncionarios",
                commandType: CommandType.StoredProcedure
            );

            return lista;
        }
    }
}

