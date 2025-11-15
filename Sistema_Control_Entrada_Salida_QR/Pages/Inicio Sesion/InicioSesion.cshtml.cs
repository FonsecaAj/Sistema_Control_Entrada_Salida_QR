using CarnetDigital.Entities;
using CarnetDigital.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sistema_Control_Entrada_Salida_QR.Pages.Inicio_Sesion
{
    public class InicioSesionModel : PageModel
    {

        private readonly IUsuarioService _usuarioService;

        public InicioSesionModel(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [BindProperty]
        public string Usuario { get; set; }

        [BindProperty]
        public string Contrasena { get; set; }

        public string Mensaje { get; set; }
        public string NombreCompleto { get; set; }

        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPostAsync()
        {
            
            // Llamamos al servicio de login
            Usuarios resultado = await _usuarioService.LoginAsync(Usuario, Contrasena);

            Mensaje = resultado.Mensaje;
            NombreCompleto = resultado.NombreCompleto;

            if (Mensaje == "Inicio de sesión exitoso")
            {
                
                return RedirectToPage("/Index");
            }

            
            return Page();
        }

    }
}
