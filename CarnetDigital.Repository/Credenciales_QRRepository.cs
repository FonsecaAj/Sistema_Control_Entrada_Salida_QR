using CarnetDigital.Entities;
using Dapper;
using System.Data;

namespace CarnetDigital.Repository
{
    public class Credenciales_QRRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        
        private const string ActiveState = "A";

        public Credenciales_QRRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private IDbConnection CreateConnection() => _connectionFactory.CreateConnection();

        // ============================
        // 0. Inactivar QR expirados
        // ============================
        public async Task InactivarQRExpiradosAsync()
        {
            const string sql = @"
                UPDATE Credenciales_QR
                SET ID_Estado = 'I'
                WHERE Fecha_expiracion < @Now
                  AND ID_Estado = 'A';";

            using var conn = CreateConnection();
            await conn.ExecuteAsync(sql, new { Now = DateTime.UtcNow });
        }

        // ============================
        // 1. Inactivar QR de usuario
        // ============================
        public async Task MarcarComoInactivoAsync(string identificacion)
        {
            const string sql = @"
                UPDATE Credenciales_QR
                SET ID_Estado = 'I'
                WHERE Identificacion = @Identificacion
                  AND ID_Estado = 'A';";

            using var connection = CreateConnection();
            await connection.ExecuteAsync(sql, new { Identificacion = identificacion });
        }

        // ============================
        // 2. INSERT
        // ============================
        public async Task InsertAsync(Credenciales_QR credencial)
        {
            const string sql = @"
                INSERT INTO Credenciales_QR 
                (Identificacion, Codigo_qr, Fecha_generacion, Fecha_expiracion, ID_Estado)
                VALUES (@Identificacion, @Codigo_qr, @Fecha_generacion, @Fecha_expiracion, @ID_Estado);";

            using var connection = CreateConnection();
            await connection.ExecuteAsync(sql, credencial);
        }

        // ============================
        // 3. UPDATE
        // ============================
        public async Task UpdateAsync(Credenciales_QR credencial)
        {
            const string sql = @"
                UPDATE Credenciales_QR
                SET Codigo_qr = @Codigo_qr,
                    Fecha_generacion = @Fecha_generacion,
                    Fecha_expiracion = @Fecha_expiracion,
                    ID_Estado = @ID_Estado
                WHERE Identificacion = @Identificacion;";

            using var connection = CreateConnection();
            await connection.ExecuteAsync(sql, credencial);
        }

        // ============================
        // 4. Existe registro
        // ============================
        public async Task<bool> ExisteAsync(string identificacion)
        {
            const string sql = @"SELECT COUNT(*) 
                                 FROM Credenciales_QR 
                                 WHERE Identificacion = @Identificacion;";

            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql, new { Identificacion = identificacion }) > 0;
        }

        // ============================
        // 5. Obtener activo vigente
        // ============================
        public async Task<Credenciales_QR?> GetActiveAsync(string identificacion)
        {
            const string sql = @"
                SELECT TOP 1 *
                FROM Credenciales_QR
                WHERE Identificacion = @Identificacion
                  AND ID_Estado = @ActiveState
                  AND Fecha_expiracion > @Now
                ORDER BY Fecha_generacion DESC;";

            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Credenciales_QR>(sql, new
            {
                Identificacion = identificacion,
                ActiveState,
                Now = DateTime.UtcNow
            });
        }
    }
}
