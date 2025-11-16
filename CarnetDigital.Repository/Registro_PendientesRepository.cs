using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarnetDigital.Entities;
using Dapper;

namespace CarnetDigital.Repository
{
    public class Registro_PendientesRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public Registro_PendientesRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Registros_Pendientes>> GetAllAsync()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                return await connection.QueryAsync<Registros_Pendientes>("SP_Mostrar_Registros_Pendientes", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<(string mensaje, int resultado)> EjecutarDecisionAsync(string identificacion, string decision)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@Identificacion", identificacion, DbType.String, ParameterDirection.Input, 22);
                parametros.Add("@Decision", decision, DbType.String, ParameterDirection.Input, 5);
                parametros.Add("@ResultadoSalida", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parametros.Add("@Mensaje", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("SP_Solicitudes_Registro_Pendientes", parametros, commandType: CommandType.StoredProcedure);

                var mensaje = parametros.Get<string>("@Mensaje");
                var resultado = parametros.Get<int>("@ResultadoSalida");

                return (mensaje, resultado);
            }
        }



    }
}
