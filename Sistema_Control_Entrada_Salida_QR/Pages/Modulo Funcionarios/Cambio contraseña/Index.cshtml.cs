using CarnetDigital.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sistema_Control_Entrada_Salida_QR.Pages.Modulo_Funcionarios.Cambio_contraseña
{
    public class IndexModel : PageModel
    {
        private readonly IUsuarioService _usuarioService;

        public IndexModel(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [BindProperty] public string Actual { get; set; }
        [BindProperty] public string Nueva { get; set; }
        [BindProperty] public string Confirmar { get; set; }

        public string Mensaje { get; set; }
        public bool Resultado { get; set; }

        public void OnGet()
        {
            // Necesario para que TempData no se borre en GET
            TempData.Keep("Identificacion");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string identificacion = TempData["Identificacion"]?.ToString();

            if (identificacion == null)
            {
                Mensaje = "No se pudo validar el usuario. Inicie sesión nuevamente.";
                Resultado = false;
                TempData.Keep("Identificacion");
                return Page();
            }

            // VALIDACIONES
            if (string.IsNullOrWhiteSpace(Actual))
            {
                Mensaje = "Debe ingresar la contraseña actual.";
                Resultado = false;
                TempData.Keep("Identificacion");
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Nueva))
            {
                Mensaje = "El espacio de la nueva contraseña no puede quedar en blanco.";
                Resultado = false;
                TempData.Keep("Identificacion");
                return Page();
            }

            if (Nueva.Length < 8)
            {
                Mensaje = "Debe tener un mínimo de 8 caracteres entre letras, números y caracteres especiales.";
                Resultado = false;
                TempData.Keep("Identificacion");
                return Page();
            }

            if (Nueva == Actual)
            {
                Mensaje = "La contraseña ingresada no puede ser igual a la actual.";
                Resultado = false;
                TempData.Keep("Identificacion");
                return Page();
            }

            if (Nueva != Confirmar)
            {
                Mensaje = "La nueva contraseña no coincide con la confirmación.";
                Resultado = false;
                TempData.Keep("Identificacion");
                return Page();
            }

            // LLAMAR AL SERVICIO
            var resultado = await _usuarioService.CambiarContrasenaAsync(identificacion, Actual, Nueva);

            Mensaje = resultado.Mensaje;
            Resultado = resultado.Resultado;

            TempData.Keep("Identificacion");

            if (resultado.Resultado)
            {
                return Page();
            }

            return Page();
        }
    }
}
