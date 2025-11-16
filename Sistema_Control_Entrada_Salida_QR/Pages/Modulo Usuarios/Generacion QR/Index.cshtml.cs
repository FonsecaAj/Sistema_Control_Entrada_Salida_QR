using CarnetDigital.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sistema_Control_Entrada_Salida_QR.Pages.Modulo_Usuarios.Generacion_QR
{
    [Authorize]
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly ICredencialesQRServices _qrService;

        public IndexModel(ICredencialesQRServices qrService)
        {
            _qrService = qrService;
        }

        public string QRBase64 { get; set; } = "";
        public int DuracionSegundos { get; set; } = 60;
        public DateTime ExpiraUTC { get; set; }
        public bool TokenGenerado { get; set; } = false;

        private  string Identificacion { get; set; }

        public async Task OnGet()
        {
             Identificacion = User.FindFirst("Identificacion")?.Value;
            QRBase64 = await _qrService.GenerarYObtenerQRBase64Async(Identificacion);
            ExpiraUTC = DateTime.UtcNow.AddSeconds(DuracionSegundos);
            TokenGenerado = true;
        }

        public async Task<IActionResult> OnPostGenerarAsync()
        {
            Identificacion = User.FindFirst("Identificacion")?.Value;
            QRBase64 = await _qrService.GenerarYObtenerQRBase64Async(Identificacion);
            ExpiraUTC = DateTime.UtcNow.AddSeconds(DuracionSegundos);
            TokenGenerado = true;

            return Page();
        }


        // Para inactivar el QR expirado
        
        public async Task<IActionResult> OnPostInactivarAsync()
        {
            Identificacion = User.FindFirst("Identificacion")?.Value;
            await _qrService.InactivarAsync(Identificacion);

            // Devuelve una respuesta 204 No Content para indicar éxito sin cuerpo de respuesta
            return new NoContentResult();
        }
    }
}