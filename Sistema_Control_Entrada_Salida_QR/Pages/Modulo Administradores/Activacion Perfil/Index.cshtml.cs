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

        public IEnumerable<Carreras_Programas> Carreras { get; set; }

        [BindProperty(SupportsGet = true)]
        public string NumeroIdentificacion { get; set; }

        [BindProperty(SupportsGet = true)]
        public string CorreoEstudiante { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FechaRegistro { get; set; }

        [BindProperty(SupportsGet = true)]
        public string IdCarreraFiltro { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Desde { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Hasta { get; set; }
        [TempData]
        public string Mensaje { get; set; }

        [TempData]
        public bool EsExito { get; set; }

        public async Task OnGetAsync()
        {
            Carreras = await _registrosService.ObtenerCarrerasAsync();

            var identificacionFiltro = string.IsNullOrWhiteSpace(NumeroIdentificacion)
                ? null
                : NumeroIdentificacion.Trim();

            var correoFiltro = string.IsNullOrWhiteSpace(CorreoEstudiante)
                ? null
                : CorreoEstudiante.Trim();

            RegistrosPendientes = await _registrosService.FiltrarAsync(
                identificacionFiltro,
                correoFiltro,
                FechaRegistro,
                IdCarreraFiltro,
                Desde,
                Hasta);
        }

        public async Task<IActionResult> OnPostLimpiarAsync()
        {
            NumeroIdentificacion = null;
            CorreoEstudiante = null;
            FechaRegistro = null;
            IdCarreraFiltro = null;
            Desde = null;
            Hasta = null;

            Carreras = await _registrosService.ObtenerCarrerasAsync();
            RegistrosPendientes = await _registrosService.GetAllAsync();

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostFiltrarAsync()
        {
            Carreras = await _registrosService.ObtenerCarrerasAsync();

            var identificacionFiltro = string.IsNullOrWhiteSpace(NumeroIdentificacion)
                ? null
                : NumeroIdentificacion.Trim();

            var correoFiltro = string.IsNullOrWhiteSpace(CorreoEstudiante)
                ? null
                : CorreoEstudiante.Trim();

            RegistrosPendientes = await _registrosService.FiltrarAsync(identificacionFiltro, correoFiltro, FechaRegistro, IdCarreraFiltro, Desde, Hasta);
            return Page();
        }
        public async Task<IActionResult> OnPostAprobarAsync(string identificacion, string correo)
        {
            Carreras = await _registrosService.ObtenerCarrerasAsync();

            var (mensaje, resultado) = await _registrosService.EjecutarDecisionAsync(identificacion, "A");
            Mensaje = mensaje;
            EsExito = (resultado == 1);

            if (resultado == 1 && !string.IsNullOrWhiteSpace(correo))
            {
                var asunto = "Activación de usuario CUC";

                var cuerpoHtml = $@"
<div style='font-family: Arial, sans-serif; padding: 20px; background-color: #f4f6f9;'>
    <div style='max-width: 650px; margin: auto; background: white; padding: 25px; border-radius: 12px; box-shadow: 0 3px 12px rgba(0,0,0,0.12);'>
        <h2 style='color: #002D74; text-align:center; margin-bottom:25px;'>
            Activación de usuario CUC
        </h2>

        <p style='font-size: 15px; color:#333;'>
            Hola,
        </p>

        <p style='font-size: 15px; color:#333;'>
            Su registro ha sido aprobado y su cuenta está activa.
        </p>

        <p style='font-size: 15px; color:#333;'>
            Puede ingresar al sistema con su usuario <strong>{correo}</strong> y respectiva contraseña registrada.
        </p>

        <p style='margin-top:20px; font-size: 15px; color:#444;'>
            Saludos cordiales,<br/>
            <strong>Sistema de Control de Entrada y Salida QR</strong>
        </p>

        <p style='margin-top:35px; font-size:12px; color:#777; text-align:center;'>
            Este es un mensaje automático. Por favor no responder a este correo.
        </p>
    </div>
</div>";


                await _registrosService.EnviarCorreoAsync(correo, asunto, cuerpoHtml);
            }

            return RedirectToPage(new
            {
                NumeroIdentificacion,
                CorreoEstudiante,
                FechaRegistro = FechaRegistro?.ToString("yyyy-MM-dd"),
                IdCarreraFiltro,
                Desde,
                Hasta
            });
        }

        public async Task<IActionResult> OnPostRechazarAsync(string identificacion)
        {
            Carreras = await _registrosService.ObtenerCarrerasAsync();

            var (mensaje, resultado) = await _registrosService.EjecutarDecisionAsync(identificacion, "R");
            Mensaje = mensaje;
            EsExito = (resultado == 1);

            return RedirectToPage(new
            {
                NumeroIdentificacion,
                CorreoEstudiante,
                FechaRegistro = FechaRegistro?.ToString("yyyy-MM-dd"),
                IdCarreraFiltro,
                Desde,
                Hasta
            });
        }
    }
}
