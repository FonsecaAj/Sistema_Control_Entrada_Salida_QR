using CarnetDigital.Services.Abstract;
using Control_QR.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sistema_Control_Entrada_Salida_QR.Services.Abstract;
using Sistema_Control_Entrada_Salida_SQR.Services.Abstract;

namespace Sistema_Control_Entrada_Salida_QR.Pages.Modulo_Administradores.Registro_Funcionarios
{
    public class RegistrarModel : PageModel
    {
        private readonly IFuncionariosService _funcService;
        private readonly ITiposIdentificacionService _tiposIdService;
        private readonly ITiposFuncionarioService _tiposFuncService;
        private readonly IDependenciasService _depsService;

        public RegistrarModel(
            IFuncionariosService funcService,
            ITiposIdentificacionService tiposIdService,
            ITiposFuncionarioService tiposFuncService,
            IDependenciasService depsService)
        {
            _funcService = funcService;
            _tiposIdService = tiposIdService;
            _tiposFuncService = tiposFuncService;
            _depsService = depsService;
        }

        [BindProperty]
        public Funcionarios Funcionario { get; set; } = new();

        [BindProperty]
        public IFormFile? Foto { get; set; }

        public List<SelectListItem> TiposIdentificacion { get; set; } = new();
        public List<SelectListItem> TiposFuncionario { get; set; } = new();
        public List<SelectListItem> Dependencias { get; set; } = new();

        public string Mensaje { get; set; } = string.Empty;
        public bool Resultado { get; set; }

        public async Task OnGetAsync()
        {
            Funcionario = new Funcionarios
            {
                Fecha_Nacimiento = DateTime.Today
            };

            await CargarCombosAsync();
        }

        private async Task CargarCombosAsync()
        {
            var tiposId = await _tiposIdService.GetAllAsync();
            TiposIdentificacion = tiposId
                .Select(x => new SelectListItem
                {
                    Value = x.ID_Tipo_Identificacion,
                    Text = x.Nombre_Identificacion
                }).ToList();

            TiposIdentificacion.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "Seleccione un tipo de identificación"
            });

            var tiposFunc = await _tiposFuncService.GetAllAsync();
            TiposFuncionario = tiposFunc
                .Select(x => new SelectListItem
                {
                    Value = x.Id_Tipo_Funcionario,
                    Text = x.Nombre_Tipo
                }).ToList();

            TiposFuncionario.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "Seleccione un tipo de funcionario"
            });

            var deps = await _depsService.GetAllAsync();
            Dependencias = deps
                .Select(x => new SelectListItem
                {
                    Value = x.Id_Dependencia,
                    Text = x.Nombre_Dependencia
                }).ToList();

            Dependencias.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "Seleccione una dependencia"
            });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CargarCombosAsync();

            // FOTO A BASE64
            if (Foto != null)
            {
                using var ms = new MemoryStream();
                await Foto.CopyToAsync(ms);
                Funcionario.FotoBase64 = Convert.ToBase64String(ms.ToArray());
            }

            // REGISTRO
            var resultado = await _funcService.RegistrarAsync(Funcionario);

            Mensaje = resultado.Mensaje;
            Resultado = resultado.Resultado;

            if (resultado.Resultado)
            {
                Funcionario = new Funcionarios
                {
                    Fecha_Nacimiento = DateTime.Today  
                };

                Foto = null;
                ModelState.Clear();
                await CargarCombosAsync();
            }

            return Page();
        }
    }
}


