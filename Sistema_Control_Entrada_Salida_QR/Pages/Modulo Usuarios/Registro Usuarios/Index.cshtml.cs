using CarnetDigital.Entities;
using CarnetDigital.Services;
using CarnetDigital.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sistema_Control_Entrada_Salida_QR.Pages.Modulo_Usuarios.Registro_Usuarios
{
    public class IndexModel : PageModel
    {

        private readonly IRegistros_PendientesService _registroService;
        private readonly ITipos_IdentificacionService _tiposService;
        private readonly ICarreras_ProgramasService _carrerasService;


        [BindProperty]
        public Registros_Pendientes Registro { get; set; }

        [BindProperty]
        public IFormFile FotoFile { get; set; }

        public string Mensaje { get; set; } = string.Empty;

        // Listas para los selects
        public List<SelectListItem> TiposIdentificacion { get; set; }
        public List<SelectListItem> Carreras { get; set; }

        public IndexModel(IRegistros_PendientesService registroService, ITipos_IdentificacionService tiposService, ICarreras_ProgramasService carrerasService)
        {
            _registroService = registroService;
            _tiposService = tiposService;
            _carrerasService = carrerasService;
        }

        private async Task CargarListasSelect()
        {
            var tipos = await _tiposService.ObtenerTipos_IdentificacionAsync();
            var carreras = await _carrerasService.ObtenerCarreras_ProgramasAsync();

            TiposIdentificacion = tipos.Select(t => new SelectListItem
            {
                Value = t.ID_Tipo_Identificacion,
                Text = t.Nombre_Identificacion
            }).ToList();

            Carreras = carreras.Select(c => new SelectListItem
            {
                Value = c.Id_Carrera,
                Text = c.Nombre_Carrera
            }).ToList();
        }

        public async Task OnGet()
        {

            await CargarListasSelect();

        }

        public async Task<IActionResult> OnPostAsync()
        {


            await CargarListasSelect();

            // Validar foto
            if (FotoFile != null)
            {
                using var ms = new MemoryStream();
                await FotoFile.CopyToAsync(ms);
                Registro.Foto = ms.ToArray();
            }

            

            var resultado = await _registroService.RegistrarUsuarioAsync(Registro);

            if (!string.IsNullOrEmpty(resultado.Mensaje))
            {

                return Page();

            }

            return RedirectToPage("/Inicio Sesion/InicioSesion");


        }

       


    }
}
