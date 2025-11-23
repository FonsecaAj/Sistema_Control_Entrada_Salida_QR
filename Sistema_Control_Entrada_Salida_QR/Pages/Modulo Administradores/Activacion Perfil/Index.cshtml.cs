using CarnetDigital.Entities;
using CarnetDigital.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sistema_Control_Entrada_Salida_QR.Pages.Modulo_Administradores.Activacion_Perfil
{
    public class IndexModel : PageModel
    {
        private readonly IRegistros_PendientesService _registrosService;

        public IndexModel(IRegistros_PendientesService registrosService)
        {
            _registrosService = registrosService;
            RegistrosPendientes = new List<Registros_Pendientes>();
        }

        public IEnumerable<Registros_Pendientes> RegistrosPendientes { get; set; }

        [TempData]
        public string Mensaje { get; set; }

        [TempData]
        public bool EsExito { get; set; }

        public async Task OnGetAsync()
        {
            RegistrosPendientes = await _registrosService.GetAllAsync();
        }

        public async Task<IActionResult> OnPostAprobarAsync(string identificacion, string correo)
        {
            var (mensaje, resultado) = await _registrosService.EjecutarDecisionAsync(identificacion, "A");
            Mensaje = mensaje;
            EsExito = (resultado == 1);

            if (resultado == 1 && !string.IsNullOrWhiteSpace(correo))
            {
                var asunto = "Activaci�n de usuario CUC";
                var cuerpoHtml = @"
<p>Hola,</p>
<p>Su registro ha sido aprobado y su cuenta est� activa.</p>
<p>Puede ingresar al sistema con sus credenciales registradas.</p>
<p>Saludos cordiales,<br/>
Sistema de Control de Entrada y Salida QR</p>";

                await _registrosService.EnviarCorreoAsync(correo, asunto, cuerpoHtml);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRechazarAsync(string identificacion)
        {
            var (mensaje, resultado) = await _registrosService.EjecutarDecisionAsync(identificacion, "R");
            Mensaje = mensaje;
            EsExito = (resultado == 1);

            return RedirectToPage();
        }
    }
}
