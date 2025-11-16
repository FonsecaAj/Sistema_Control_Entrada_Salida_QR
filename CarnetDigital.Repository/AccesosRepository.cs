using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CarnetDigital.Entities;
using Dapper;

namespace CarnetDigital.Repository
{
    public class AccesosRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public AccesosRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<TipoUsuarioCombo>> ObtenerTiposUsuarioAsync()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var lista = await connection.QueryAsync<TipoUsuarioCombo>(
                    "SP_Combo_Tipo",
                    commandType: CommandType.StoredProcedure);

                return lista;
            }
        }

        public async Task<IEnumerable<Accesos>> ObtenerReporteAsync(string identificacion,DateTime? fechaInicio,DateTime? fechaFin,string idEstado,string tipoUsuario)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var parametros = new DynamicParameters();

                parametros.Add("@Identificacion",value: identificacion,dbType: DbType.String,direction: ParameterDirection.Input,size: 22);
                parametros.Add("@FechaInicio",value: fechaInicio,dbType: DbType.DateTime2,direction: ParameterDirection.Input);
                parametros.Add("@FechaFin",value: fechaFin,dbType: DbType.DateTime2,direction: ParameterDirection.Input);
                parametros.Add("@IdEstado",value: idEstado,dbType: DbType.String,direction: ParameterDirection.Input,size: 3);
                parametros.Add("@TipoUsuario",value: tipoUsuario,dbType: DbType.String,direction: ParameterDirection.Input,size: 5);

                var resultado = await connection.QueryAsync<Accesos>(
                    "SP_Reporte_Accesos",
                    parametros,
                    commandType: CommandType.StoredProcedure);

                return resultado;
            }
        }
    }
}
