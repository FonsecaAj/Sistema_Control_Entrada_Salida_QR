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
    public class EncargadosTemporalesRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public EncargadosTemporalesRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        public async Task<(string, bool)> RegistrarEncargadoTemporalAsync(Encargados_Temporales encargado)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var parametros = new DynamicParameters();

                parametros.Add("p_Nombre", encargado.Nombre);
                parametros.Add("p_Primer_Apellido", encargado.Primer_Apellido);
                parametros.Add("p_Segundo_Apellido", encargado.Segundo_Apellido);
                parametros.Add("p_ID_Tipo_Identificacion", encargado.ID_Tipo_Identificacion);
                parametros.Add("p_Identificacion", encargado.Identificacion);
                parametros.Add("p_Telefono", encargado.Telefono);
                parametros.Add("p_Id_Parentesco", encargado.Id_Parentesco);
                parametros.Add("p_Identificacion_Estudiante", encargado.Identificacion_Estudiante);
                parametros.Add("p_Foto", encargado.Foto);

                // Parámetro de salida (mensaje del SP)
                parametros.Add("p_Mensaje", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "SP_InsertarTemporalLegal",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );
                var mensaje = parametros.Get<string>("p_Mensaje");
                bool exito = mensaje.Contains("Registro exitoso");
                // Recibir mensaje devuelto por el SP
                return (mensaje, exito);
            }
        }


        public async Task<IEnumerable<Tipos_Identificacion>> ObtenerTodosLosTiposIdentificacion()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var lista = await connection.QueryAsync<Tipos_Identificacion>(
               "SP_Obtener_Tipos_Identificacion",
               commandType: CommandType.StoredProcedure
           );
                return lista;
            }

        }
        public async Task<IEnumerable<Parentescos>> ObtenerTodosLosParentescos()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var lista = await connection.QueryAsync<Parentescos>(
               "SP_ObtenerTodosLosParentescos",
               commandType: CommandType.StoredProcedure
           );
                return lista;
            }

        }
    }
}
