using CarnetDigital.Entities;
using CarnetDigital.Repository;
using CarnetDigital.Services.Abstract;
using QRCoder;

namespace CarnetDigital.Services
{
    public class CredencialesQRServices : ICredencialesQRServices
    {
        private readonly Credenciales_QRRepository _repository;

        private const int ExpirationSeconds = 60;
        private const string ActiveState = "A";

        public CredencialesQRServices(Credenciales_QRRepository repo)
        {
            _repository = repo;
        }

        public async Task<string> GenerarYObtenerQRBase64Async(string identificacion)
        {
            await _repository.InactivarQRExpiradosAsync();

            var existente = await _repository.GetByIdentificacionAsync(identificacion);

            string token = Guid.NewGuid().ToString("N");
            DateTime now = DateTime.UtcNow;

            var entidad = new Credenciales_QR
            {
                Identificacion = identificacion,
                Codigo_qr = token,
                Fecha_generacion = now,
                Fecha_expiracion = now.AddSeconds(ExpirationSeconds),
                ID_Estado = ActiveState
            };

            if (existente == null)
                await _repository.InsertAsync(entidad);
            else
                await _repository.UpdateAsync(entidad);

            return GenerateQRImageBase64(token);
        }

        public async Task<string> ObtenerOGenerarQRBase64Async(string identificacion)
        {
            await _repository.InactivarQRExpiradosAsync();

            var activo = await _repository.GetActiveAsync(identificacion);

            if (activo != null)
                return GenerateQRImageBase64(activo.Codigo_qr);

            return await GenerarYObtenerQRBase64Async(identificacion);
        }

        private string GenerateQRImageBase64(string data)
        {
            using var gen = new QRCodeGenerator();
            var qr = gen.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

            using var png = new PngByteQRCode(qr);
            var bytes = png.GetGraphic(20);

            return Convert.ToBase64String(bytes);
        }
    }
}
