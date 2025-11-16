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
    public class Tipos_IdentificacionRepository
    {

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public Tipos_IdentificacionRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Tipos_Identificacion>> ObtenerTipos_IdentificacionAsync()
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            return await connection.QueryAsync<Tipos_Identificacion>("SP_Obtener_Tipos_Identificacion", commandType: CommandType.StoredProcedure);
        }

    }
}
