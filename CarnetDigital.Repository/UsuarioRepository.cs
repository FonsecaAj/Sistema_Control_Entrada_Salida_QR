using CarnetDigital.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
            parameters.Add("@p_Mensaje", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);
            parameters.Add("@p_Nombre_Completo", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
            parameters.Add("@p_Rol", dbType: DbType.String, size: 3, direction: ParameterDirection.Output);
            parameters.Add("@p_Identificacion", dbType: DbType.String, size: 22, direction: ParameterDirection.Output);
            parameters.Add("@p_ContrasenaHash", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_Login_Usuario", parameters, commandType: CommandType.StoredProcedure);

            string mensaje = parameters.Get<string>("@p_Mensaje") ?? string.Empty;
            string nombreCompleto = parameters.Get<string>("@p_Nombre_Completo") ?? string.Empty;
            string rol = parameters.Get<string>("@p_Rol") ?? string.Empty;
            string identificacion = parameters.Get<string>("@p_Identificacion") ?? string.Empty;
            string contrasenaHash = parameters.Get<string>("@p_ContrasenaHash") ?? string.Empty;

            // Verificar contraseña usando SALT

            bool esValido = VerifyPassword(contrasena, contrasenaHash);

            if (!esValido)
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
                Mensaje = "Inicio de sesión exitoso",
                Rol = rol
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


    }
}
