using CarnetDigital.Entities;
using Control_QR.Entities;
using Dapper;
using System.Data;

namespace CarnetDigital.Repository
{
    public class FuncionariosRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public FuncionariosRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IDbConnection CreateConnection()
        {
            return _connectionFactory.CreateConnection();
        }

        public async Task<(bool Resultado, string Mensaje)> RegistrarAsync(Funcionarios func)
        {
            using var conn = CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@Identificacion", func.Identificacion);
            parameters.Add("@ID_Tipo_Identificacion", func.Id_Tipo_Identificacion);
            parameters.Add("@Nombre", func.Nombre);
            parameters.Add("@Primer_Apellido", func.Primer_Apellido);
            parameters.Add("@Segundo_Apellido", func.Segundo_Apellido);
            parameters.Add("@Fecha_Nacimiento", func.Fecha_Nacimiento);

            if (!string.IsNullOrWhiteSpace(func.FotoBase64))
                parameters.Add("@Foto", Convert.FromBase64String(func.FotoBase64), DbType.Binary);
            else
                parameters.Add("@Foto", null, DbType.Binary);

            parameters.Add("@Correo_Institucional", func.CorreoInstitucional);
            parameters.Add("@Contrasena", func.Contrasena);
            parameters.Add("@Id_Tipo_Funcionario", func.Id_Tipo_Funcionario);
            parameters.Add("@Id_Dependencia", func.Id_Dependencia);

            parameters.Add("@p_Resultado", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@p_Mensaje", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

            await conn.ExecuteAsync(
                "sp_RegistrarFuncionario",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var resultado = parameters.Get<bool>("@p_Resultado");
            var mensaje = parameters.Get<string>("@p_Mensaje");

            return (resultado, mensaje);
        }
    }
}

