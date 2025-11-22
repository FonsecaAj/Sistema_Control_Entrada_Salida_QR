using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sistema_Control_Entrada_Salida_QR.Pages.Inicio_Sesion
{
    [Authorize]
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // No permitir GET, solo POST
            return RedirectToPage("/Inicio Sesion/InicioSesion");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Cerrar sesión y limpiar cookies
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Limpiar sesión
            HttpContext.Session.Clear();

            // Redirigir al Login
            return RedirectToPage("/Inicio Sesion/InicioSesion");
        }
    }
}
