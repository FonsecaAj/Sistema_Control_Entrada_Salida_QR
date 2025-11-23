using CarnetDigital.Entities;
using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Repository
{
    public class ControlAccesosRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public ControlAccesosRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Accesos>> ObtenerAccesosById(string identificacion,DateOnly? filtroFechaInicio, DateOnly? filtroFechaFin,string? filtroEstado)
          {
                using (var connection = _dbConnectionFactory.CreateConnection())
                {
                    var parametros = new DynamicParameters();
                    parametros.Add("p_Identificacion_Usuario", identificacion);

                    // Convertir DateOnly a DateTime
                    parametros.Add("p_FechaInicio",
                        filtroFechaInicio.HasValue ? filtroFechaInicio.Value.ToDateTime(TimeOnly.MinValue) : null);

                    parametros.Add("p_FechaFin",
                        filtroFechaFin.HasValue ? filtroFechaFin.Value.ToDateTime(TimeOnly.MinValue) : null);

                    // Enviar NULL si el filtroEstado es vacío
                    parametros.Add("p_ID_Estado",
                        string.IsNullOrWhiteSpace(filtroEstado) ? null : filtroEstado);

                    var lista = await connection.QueryAsync<Accesos>(
                        "SP_ConsultarAccesosPorUsuario",
                        parametros,
                        commandType: CommandType.StoredProcedure
                    );

                    return lista;
                }
          }


        public async Task<IEnumerable<Estados>> ObtenerTodosEstados()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var lista = await connection.QueryAsync<Estados>(
                    "SP_ConsultarEstados",
                    commandType: CommandType.StoredProcedure
                );
                return lista;
            }
        }

    }
}
