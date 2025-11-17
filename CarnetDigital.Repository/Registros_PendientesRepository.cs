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
    public class Registros_PendientesRepository
    {

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public Registros_PendientesRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<Registros_Pendientes> RegistrarUsuarioAsync(Registros_Pendientes registro_p)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@Nombre", registro_p.Nombre);
            parameters.Add("@P_Apellido", registro_p.Primer_Apellido);
            parameters.Add("@S_Apellido", registro_p.Segundo_Apellido);
            parameters.Add("@Correo_Institucional", registro_p.Correo_Institucional);
            parameters.Add("@ID_Tipo_Identificacion", registro_p.ID_Tipo_Identificacion);
            parameters.Add("@Identificacion", registro_p.Identificacion);
            parameters.Add("@Fecha_Nacimiento", registro_p.Fecha_Nacimiento);
            parameters.Add("@ID_Carrera", registro_p.Id_Carrera);
            parameters.Add("@Contrasena", registro_p.Contrasena);
            parameters.Add("@Foto", registro_p.Foto, DbType.Binary);

            parameters.Add("@Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_MOD_Usuarios_Registro_Usuarios", parameters,commandType: CommandType.StoredProcedure);

            registro_p.Mensaje = parameters.Get<string>("@Mensaje");

            return registro_p;
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
