using CarnetDigital.Entities;
using CarnetDigital.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sistema_Control_Entrada_Salida_QR.Pages.Modulo_Usuarios.Panel_Encargados_temporales
{
    public class IndexModel : PageModel
    {
        private readonly IEncargadoTemporalService _encargados_TemporalesService;
        public IndexModel(IEncargadoTemporalService encargados_TemporalService)
        {
            _encargados_TemporalesService = encargados_TemporalService;
        }
        [BindProperty]
        public Encargados_Temporales Encargado_Temporal { get; set; } = new Encargados_Temporales();

        [BindProperty]
        public IFormFile? FotoFile { get; set; }


        #region Listas
        public IEnumerable<SelectListItem>? TiposIdentificacion { get; set; }
        public IEnumerable<SelectListItem>? Parentescos { get; set; }
        #endregion

        public async Task OnGet()
        {
            // Datos del usuario desde claims
            var identificacionUsuario = User.FindFirst("Identificacion")?.Value ?? "0000000000";

            Encargado_Temporal.Identificacion_Estudiante = identificacionUsuario;

            // Obtener lista para los tipos de identificacion
            await CargarListasAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CargarListasAsync(); // Recargar listas por si hay errores

            if (FotoFile != null)
            {
                using var memoryStream = new MemoryStream();
                await FotoFile.CopyToAsync(memoryStream);
                Encargado_Temporal.Foto = memoryStream.ToArray();
            }

            var (Mensaje, Exito) = await _encargados_TemporalesService.RegistrarEncargadoTemporalAsync(Encargado_Temporal);

            // Ajuste aquí para usar la propiedad Mensaje del modelo, igual que el ejemplo Legal
            Encargado_Temporal.Mensaje = Mensaje;

            // No es necesario RedirectToPage() para mostrar el mensaje en la misma página con Page()
            return Page();
        }


        private async Task CargarListasAsync()
        {
            // Obtener lista de tipos de identificación
            var tiposIdentificacion = await _encargados_TemporalesService.ObtenerTodosLosTiposIdentificacion();
            TiposIdentificacion = tiposIdentificacion.Select(ti => new SelectListItem
            {
                Value = ti.ID_Tipo_Identificacion,
                Text = ti.Nombre_Identificacion
            });

            // Obtener lista de parentescos
            var parentescos = await _encargados_TemporalesService.ObtenerTodosLosParentescos();
            Parentescos = parentescos.Select(p => new SelectListItem
            {
                Value = p.Id_Parentesco.ToString(),
                Text = p.Nombre_Parenresco
            });
        }
    }
}