using CarnetDigital.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Sistema_Control_Entrada_Salida_QR.Pages.Modulo_Usuarios.Generacion_QR
{
    [Authorize]
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly ICredencialesQRServices _qrService;

        public IndexModel(ICredencialesQRServices qrService)
        {
            _qrService = qrService;
        }

        public string QRBase64 { get; set; } = "";
        public int DuracionSegundos { get; set; } = 60;
        public DateTime ExpiraUTC { get; set; }
        public bool TokenGenerado { get; set; } = false;

        // Claims del login
        public string NombreCompleto { get; private set; }
        public string Identificacion { get; private set; }
        public string Carrera { get; private set; }
        public string Vigencia { get; private set; }
        public string Estado { get; private set; }
        public string Tipo { get; private set; }

        //Funcionarios
        public string ID_Dependencia { get; private set; }
        public string ID_Tipo_Funcionario { get; private set; }


        public async Task OnGet()
        {
            LeerClaims();
            QRBase64 = await _qrService.GenerarYObtenerQRBase64Async(Identificacion);
            ExpiraUTC = DateTime.UtcNow.AddSeconds(DuracionSegundos);
            TokenGenerado = true;
        }

        public async Task<IActionResult> OnPostGenerarAsync()
        {
            LeerClaims();
            QRBase64 = await _qrService.GenerarYObtenerQRBase64Async(Identificacion);
            ExpiraUTC = DateTime.UtcNow.AddSeconds(DuracionSegundos);
            TokenGenerado = true;

            return Page();
        }


        // Para inactivar el QR expirado
        
        public async Task<IActionResult> OnPostInactivarAsync()
        {
            LeerClaims();
            await _qrService.InactivarAsync(Identificacion);
            return new NoContentResult();
        }

        private void LeerClaims()
        {
            //Leemos el rol, importante para saber que estilo lleva el carnet jajaj

            string Rol = User.FindFirst("Rol")?.Value;

            NombreCompleto = User.FindFirst(ClaimTypes.Name)?.Value;
            Identificacion = User.FindFirst("Identificacion")?.Value;           
            Vigencia = User.FindFirst("FechaVencimiento")?.Value;


            //Funcionarios

            if (Rol == "FUN")
            {
                // --- FUNCIONARIO ---

                // Dependencia
                ID_Dependencia = User.FindFirst("Id_Dependencia")?.Value;

                ID_Dependencia = ID_Dependencia switch
                {
                    "DEP01" => "Recursos Humanos",
                    "DEP02" => "Finanzas",
                    "DEP03" => "Tecnología",
                    "DEP04" => "Dirección Académica",
                    "DEP05" => "Seguridad",
                    _ => "Otro"
                };

                // Tipo de funcionario
                ID_Tipo_Funcionario = User.FindFirst("TipoFuncionario")?.Value;

                ID_Tipo_Funcionario = ID_Tipo_Funcionario switch
                {
                    "TF001" => "Administrativo",
                    "TF002" => "Docente",
                    "TF003" => "Seguridad",
                    "TF004" => "Mantenimiento",
                    "TF005" => "Dirección",
                    _ => "Otro"
                };

                // No mostrar datos de estudiante
                Carrera = null;
                Vigencia = null;
                Tipo = "Funcionario";
            }
            else
            {
                // --- ESTUDIANTE ---
                Carrera = User.FindFirst("ID_Carrera")?.Value;

                Carrera = Carrera switch
                {
                    "ABC" => "Administración de Bases de Datos",
                    "ARK" => "Arquitectura de Computadoras",
                    "BD1" => "Bases de Datos I",
                    "BD2" => "Bases de Datos II",
                    "COM" => "Computación",
                    "INS" => "Ingeniería en Sistemas",
                    "PLL" => "Programación Lógica y Lenguajes",
                    "PRO" => "Programación",
                    "SOS" => "Soporte de Sistemas",
                    "SAC" => "Seguridad en Ambientes Computacionales",
                    _ => "Otro"
                };

                // No mostrar datos de funcionario
                ID_Dependencia = null;
                ID_Tipo_Funcionario = null;
            }

            Estado = "Activo";


        }
    }
}