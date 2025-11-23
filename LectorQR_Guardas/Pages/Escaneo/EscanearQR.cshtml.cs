using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace LectorQR_Guardas.Pages.Escaneo
{
    public class EscanearQRModel : PageModel
    {
        public IActionResult OnPostValidarQR([FromBody] JsonElement data)
        {
   
            string qr = data.GetProperty("qr").GetString() ?? string.Empty;

  
            TempData["CodigoQR"] = qr;

            return new JsonResult(new { redirectUrl = "/Escaneo/ResultadoQR" });
        }
    }
}
