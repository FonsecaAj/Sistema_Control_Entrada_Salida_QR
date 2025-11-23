using CarnetDigital.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Repository
{
    public class Carreras_ProgramasRepository
    {

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public Carreras_ProgramasRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Carreras_Programas>> ObtenerCarreras_ProgramasAsync()
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            return await connection.QueryAsync<Carreras_Programas>("SP_Obtener_Carreras_Programas", commandType: CommandType.StoredProcedure);
        }

    }
}
