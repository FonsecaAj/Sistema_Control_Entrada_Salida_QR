using CarnetDigital.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sistema_Control_Entrada_Salida_QR.Pages.Modulo_Usuarios.Generacion_QR
{
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

        private const string CedulaPrueba = "119700120";

        public async Task OnGet()
        {
            QRBase64 = await _qrService.GenerarYObtenerQRBase64Async(CedulaPrueba);
            ExpiraUTC = DateTime.UtcNow.AddSeconds(DuracionSegundos);
            TokenGenerado = true;
        }

        public async Task<IActionResult> OnPostGenerarAsync()
        {
            QRBase64 = await _qrService.GenerarYObtenerQRBase64Async(CedulaPrueba);
            ExpiraUTC = DateTime.UtcNow.AddSeconds(DuracionSegundos);
            TokenGenerado = true;

            return Page();
        }


        // Para inactivar el QR expirado
        
        public async Task<IActionResult> OnPostInactivarAsync()
        {
            
            await _qrService.InactivarAsync(CedulaPrueba);

            // Devuelve una respuesta 204 No Content para indicar éxito sin cuerpo de respuesta
            return new NoContentResult();
        }
    }
}