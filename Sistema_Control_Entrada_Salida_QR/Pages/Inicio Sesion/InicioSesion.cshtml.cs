using CarnetDigital.Entities;
using CarnetDigital.Services.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

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

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, resultado.NombreCompleto ?? Usuario),
                    new Claim("Identificacion", resultado.Identificacion ?? ""),
                    new Claim("Rol", resultado.Rol ?? ""),
                    new Claim("Correo", resultado.Correo_Institucional ?? ""),

                    // Estos son opcionales según lo que devuelva el SP
                    new Claim("FechaVencimiento", resultado.FechaVencimiento?.ToString("yyyy-MM-dd") ?? ""),
                    new Claim("ID_Carrera", resultado.ID_Carrera ?? ""),
                    new Claim("TipoEstudiante", resultado.ID_TipoEstudiante ?? ""),
                    new Claim("Id_Dependencia", resultado.Id_Dependencia ?? ""),
                    new Claim("TipoFuncionario", resultado.Id_Tipo_Funcionario ?? "")
                };

                // Crear la identidad de las cookies

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Autenticar al usuario y guardar la cookie

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));


                ViewData["Redirigir"] = true;

                return Page();

            }

            
            return Page();
        }

    }
}
