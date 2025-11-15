using CarnetDigital.Entities;
using Dapper;
using System.Data;

namespace CarnetDigital.Repository
{
    public class Credenciales_QRRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

      
        public Credenciales_QRRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private IDbConnection CreateConnection() => _connectionFactory.CreateConnection();

        // ============================
        // Inactivar QR expirados
        // ============================
        public async Task InactivarQRExpiradosAsync()
        {
            const string sql = "EXEC sp_InactivarQRExpirados";

            using var conn = CreateConnection();
            await conn.ExecuteAsync(sql);
        }

        // ============================
        // Inactivar QR de usuario
        // ============================
        public async Task MarcarComoInactivoAsync(string identificacion)
        {
            const string sql = "EXEC sp_MarcarQRComoInactivo @Identificacion";

            using var connection = CreateConnection();
            await connection.ExecuteAsync(sql, new { Identificacion = identificacion });
        }

        // ============================
        // INSERT
        // ============================
        public async Task InsertAsync(Credenciales_QR credencial)
        {
            const string sql = "EXEC sp_InsertarCredencialQR @Identificacion, @Codigo_qr, @Fecha_generacion, @Fecha_expiracion, @ID_Estado";

            using var connection = CreateConnection();
            await connection.ExecuteAsync(sql, credencial);
        }

        // ============================
        // UPDATE
        // ============================
        public async Task UpdateAsync(Credenciales_QR credencial)
        {
            const string sql = "EXEC sp_ActualizarCredencialQR @Identificacion, @Codigo_qr, @Fecha_generacion, @Fecha_expiracion, @ID_Estado";

            using var connection = CreateConnection();
            await connection.ExecuteAsync(sql, credencial);
        }

        // ============================
        // Existe registro
        // ============================
        public async Task<bool> ExisteAsync(string identificacion)
        {
            const string sql = "EXEC sp_ExisteCredencialQR @Identificacion";

            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql, new { Identificacion = identificacion }) > 0;
        }


        // ============================
        // Obtener activo vigente
        // ============================
        public async Task<Credenciales_QR?> GetActiveAsync(string identificacion)
        {
            const string sql = "EXEC sp_ObtenerCredencialQRActiva @Identificacion";

            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Credenciales_QR>(sql, new
            {
                Identificacion = identificacion
            });
        }
    }
}
