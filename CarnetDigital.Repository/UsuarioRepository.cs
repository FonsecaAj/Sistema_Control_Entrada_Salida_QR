using CarnetDigital.Entities;
using Dapper;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Repository
{
    public class UsuarioRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public UsuarioRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<Usuarios> LoginAsync(string correoInstitucional, string contrasena)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@p_CorreoInstitucional", correoInstitucional, DbType.String);

            // Parámetros de salida
            parameters.Add("@p_Mensaje", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);
            parameters.Add("@p_Nombre_Completo", dbType: DbType.String, size: 150, direction: ParameterDirection.Output);
            parameters.Add("@p_Rol", dbType: DbType.String, size: 3, direction: ParameterDirection.Output);
            parameters.Add("@p_Identificacion", dbType: DbType.String, size: 22, direction: ParameterDirection.Output);
            parameters.Add("@p_ContrasenaHash", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);
            parameters.Add("@p_Fecha_Vencimiento", dbType: DbType.Date, direction: ParameterDirection.Output);

            // Estudiante
            parameters.Add("@p_ID_Carrera", dbType: DbType.String, size: 3, direction: ParameterDirection.Output);
            parameters.Add("@p_ID_TipoEstudiante", dbType: DbType.String, size: 3, direction: ParameterDirection.Output);

            // Funcionario
            parameters.Add("@p_Id_Dependencia", dbType: DbType.String, size: 5, direction: ParameterDirection.Output);
            parameters.Add("@p_Id_Tipo_Funcionario", dbType: DbType.String, size: 5, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_Login_Usuario", parameters, commandType: CommandType.StoredProcedure);

            // Recibir datos desde el SP
            string mensaje = parameters.Get<string>("@p_Mensaje") ?? "";
            string nombreCompleto = parameters.Get<string>("@p_Nombre_Completo") ?? "";
            string rol = parameters.Get<string>("@p_Rol") ?? "";
            string identificacion = parameters.Get<string>("@p_Identificacion") ?? "";
            string contrasenaHash = parameters.Get<string>("@p_ContrasenaHash") ?? "";
            DateTime? fechaVenc = parameters.Get<DateTime?>("@p_Fecha_Vencimiento");

            // Datos estudiante
            string carrera = parameters.Get<string>("@p_ID_Carrera");
            string tipoEstudiante = parameters.Get<string>("@p_ID_TipoEstudiante");

            // Datos funcionario
            string dependencia = parameters.Get<string>("@p_Id_Dependencia");
            string tipoFunc = parameters.Get<string>("@p_Id_Tipo_Funcionario");


            if (mensaje != "Inicio de sesión exitoso")
            {
                return new Usuarios
                {
                    Mensaje = mensaje,
                    Correo_Institucional = correoInstitucional
                };
            }


            // Validar contraseña
            bool esValida = VerifyPassword(contrasena, contrasenaHash);

            if (!esValida)
            {
                return new Usuarios
                {
                    Mensaje = "Usuario o contraseña incorrecta",
                    Correo_Institucional = correoInstitucional
                };
            }

            return new Usuarios
            {
                Identificacion = identificacion,
                Correo_Institucional = correoInstitucional,
                NombreCompleto = nombreCompleto,
                Rol = rol,
                Mensaje = mensaje,
                FechaVencimiento = fechaVenc,

                // Estudiante
                ID_Carrera = carrera,
                ID_TipoEstudiante = tipoEstudiante,

                // Funcionario
                Id_Dependencia = dependencia,
                Id_Tipo_Funcionario = tipoFunc,

                Contrasena = contrasenaHash
            };
        }

        // Función para verificar contraseña contra hash+salt
        private bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash)) return false;

            var parts = storedHash.Split('$');
            if (parts.Length != 2) return false;

            var salt = parts[0];
            var hash = parts[1];

            using var sha = SHA256.Create();
            var combined = Encoding.UTF8.GetBytes(password + salt);
            var hashCheck = Convert.ToBase64String(sha.ComputeHash(combined));

            return hashCheck == hash;
        }

        public async Task<(bool Resultado, string Mensaje)> CambiarContrasenaAsync(string identificacion, string actual, string nueva)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@p_Identificacion", identificacion);
            parameters.Add("@p_ContrasenaActual", actual);
            parameters.Add("@p_ContrasenaNueva", nueva);

            parameters.Add("@p_Mensaje", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);
            parameters.Add("@p_Resultado", dbType: DbType.Boolean, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("sp_CambiarContrasena", parameters, commandType: CommandType.StoredProcedure);

            return (
                parameters.Get<bool>("@p_Resultado"),
                parameters.Get<string>("@p_Mensaje")
            );
        }
    }
}

