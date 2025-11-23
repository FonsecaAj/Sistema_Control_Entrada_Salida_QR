using CarnetDigital.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

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

        // Claims del login
        public string NombreCompleto { get; private set; }
        public string Identificacion { get; private set; }
        public string Carrera { get; private set; }
        public string Vigencia { get; private set; }
        public string Estado { get; private set; }
        public string Tipo { get; private set; }

        //Funcionarios
        public string ID_Dependencia { get; private set; }
        public string ID_Tipo_Funcionario { get; private set; }


        public async Task OnGet()
        {
            LeerClaims();
            QRBase64 = await _qrService.GenerarYObtenerQRBase64Async(Identificacion);
            ExpiraUTC = DateTime.UtcNow.AddSeconds(DuracionSegundos);
            TokenGenerado = true;
        }

        public async Task<IActionResult> OnPostGenerarAsync()
        {
            LeerClaims();
            QRBase64 = await _qrService.GenerarYObtenerQRBase64Async(Identificacion);
            ExpiraUTC = DateTime.UtcNow.AddSeconds(DuracionSegundos);
            TokenGenerado = true;

            return Page();
        }


        // Para inactivar el QR expirado
        
        public async Task<IActionResult> OnPostInactivarAsync()
        {
            LeerClaims();
            await _qrService.InactivarAsync(Identificacion);
            return new NoContentResult();
        }

        private void LeerClaims()
        {
            NombreCompleto = User.FindFirst(ClaimTypes.Name)?.Value;
            Identificacion = User.FindFirst("Identificacion")?.Value;
            Carrera = User.FindFirst("ID_Carrera")?.Value;
            Vigencia = User.FindFirst("FechaVencimiento")?.Value;
            Tipo = User.FindFirst("TipoEstudiante")?.Value;

            //Funcionarios
            ID_Dependencia = User.FindFirst("Id_Dependencia")?.Value;
            ID_Tipo_Funcionario = User.FindFirst("TipoFuncionario")?.Value;

            Estado = "Activo";


        }
    }
}