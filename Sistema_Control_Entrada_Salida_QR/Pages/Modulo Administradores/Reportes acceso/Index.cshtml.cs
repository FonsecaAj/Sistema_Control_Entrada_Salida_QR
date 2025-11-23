using CarnetDigital.Entities;
using CarnetDigital.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sistema_Control_Entrada_Salida_QR.Pages.Modulo_Administradores.Reportes_Accesos
{
    public class IndexModel : PageModel
    {
        private readonly IAccesosService _accesosService;

        public IndexModel(IAccesosService accesosService)
        {
            _accesosService = accesosService;
            Resultado = new ReporteAccesosResultado();
            ListaEstados = new List<SelectListItem>();
            ListaTiposUsuario = new List<SelectListItem>();
        }

        [BindProperty]
        public string Identificacion { get; set; }

        [BindProperty]
        public DateTime? FechaInicio { get; set; }

        [BindProperty]
        public DateTime? FechaFin { get; set; }

        [BindProperty]
        public string IdEstado { get; set; }

        [BindProperty]
        public string TipoUsuario { get; set; }

        public ReporteAccesosResultado Resultado { get; set; }

        public List<SelectListItem> ListaEstados { get; set; }
        public List<SelectListItem> ListaTiposUsuario { get; set; }

        public async Task OnGetAsync()
        {
            await CargarCombosAsync();
            Resultado = new ReporteAccesosResultado();
        }

        public async Task<IActionResult> OnPostGenerarReporteAsync()
        {
            await CargarCombosAsync();

            var identificacionFiltro = string.IsNullOrWhiteSpace(Identificacion)
                ? null
                : Identificacion.Trim();

            var estadoFiltro = string.IsNullOrWhiteSpace(IdEstado) || IdEstado == "TODOS"
                ? null
                : IdEstado;

            var tipoUsuarioFiltro = string.IsNullOrWhiteSpace(TipoUsuario) || TipoUsuario == "TODOS"
                ? null
                : TipoUsuario;

            Resultado = await _accesosService.ObtenerReporteAsync(identificacionFiltro, FechaInicio, FechaFin, estadoFiltro, tipoUsuarioFiltro);

            return Page();
        }

        private async Task CargarCombosAsync()
        {
            ListaEstados = new List<SelectListItem>
            {
                new SelectListItem("Todos los estados", "TODOS"),
                new SelectListItem("Autorizados", "A"),
                new SelectListItem("Rechazados", "R")
            };

            var tipos = await _accesosService.ObtenerTiposUsuarioAsync();

            ListaTiposUsuario = new List<SelectListItem>
            {
                new SelectListItem("Todos los tipos", "TODOS")
            };

            foreach (var t in tipos)
            {
                ListaTiposUsuario.Add(new SelectListItem(t.Nombre_Tipo, t.Id_Tipo));
            }
        }

    }
}
