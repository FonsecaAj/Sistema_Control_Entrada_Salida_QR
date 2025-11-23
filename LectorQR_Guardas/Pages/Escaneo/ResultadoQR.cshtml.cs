using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LectorQR_Guardas.Pages.Escaneo
{
    public class ResultadoQRModel : PageModel
    {
        public string CodigoQR { get; set; } = string.Empty;

        public void OnGet()
        {
            if (TempData["CodigoQR"] != null)
            {
                CodigoQR = TempData["CodigoQR"]!.ToString() ?? string.Empty;
            }
            else
            {
                CodigoQR = "No se recibió ningún código QR.";
            }
        }
    }
}
