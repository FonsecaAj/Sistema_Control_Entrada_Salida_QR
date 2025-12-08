using CarnetDigital.Entities;
using CarnetDigital.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Sistema_Control_Entrada_Salida_QR.Pages.Modulo_Usuarios.Control_Accesos
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IControlAccesosService _controlAccesosService;

        public IndexModel(IControlAccesosService controlAccesosService)
        {
            _controlAccesosService = controlAccesosService;
        }

        #region Datos del Usuario
        public string? NombreCompleto { get; set; }
        public string? Identificacion { get; set; }
        public string? CarreraPrograma { get; set; }
        #endregion

        #region Listas
        public IEnumerable<Accesos>? ListaAccesos { get; set; }
        public IEnumerable<Estados>? ListaEstados { get; set; }

        #endregion
        #region Resumen de accesos
        public int TotalAccesos { get; set; }
        public int TotalAutorizados { get; set; }
        public int TotalRechazados { get; set; }


        #endregion

        #region Filtros
        [BindProperty(SupportsGet = true)]
        public DateOnly? FechaInicio { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateOnly? FechaFin { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FiltroEstado { get; set; }
        #endregion

        public async Task OnGet()
        {
            // Datos del usuario desde claims
            var NombreUsuario = User.Identity?.Name ?? "Usuario";
            var identificacionUsuario = User.FindFirst("Identificacion")?.Value ?? "0000000000";
            LeerClaims();
            NombreCompleto = NombreUsuario;
            Identificacion = identificacionUsuario;

            // Combobox de estados
            ListaEstados = new List<Estados>

            {
                new Estados { ID_Estado = "AU", Nombre_Estado = "Autorizado" },
                new Estados { ID_Estado = "R", Nombre_Estado = "Rechazado" }
            };

            // Obtener accesos sin filtros para resumen
            var accesos = await _controlAccesosService.ObtenerAccesosById(
                Identificacion, null, null, null
            );
            // Total accesos
            TotalAccesos = accesos.Count();

            // Contar autorizados y rechazados
            TotalAutorizados = accesos.Count(a => a.ID_Estado.Trim() == "AU");
            TotalRechazados = accesos.Count(a => a.ID_Estado.Trim() == "R");

            // Cargar lista filtrada
            ListaAccesos = await _controlAccesosService.ObtenerAccesosById(
                identificacionUsuario,
                FechaInicio,
                FechaFin,
                FiltroEstado
            );
        }
        private void LeerClaims()
        {
            string Rol = User.FindFirst("Rol")?.Value;

            // Nombre e identificación
            NombreCompleto = User.FindFirst(ClaimTypes.Name)?.Value;
            Identificacion = User.FindFirst("Identificacion")?.Value;

            // Carrera del estudiante
            var codigoCarrera = User.FindFirst("ID_Carrera")?.Value;
            //Funcionarios

            if (Rol == "FUN")
            {
                // --- FUNCIONARIO ---

                // Dependencia
                CarreraPrograma = User.FindFirst("Id_Dependencia")?.Value;

                CarreraPrograma = CarreraPrograma switch
                {
                    "DEP01" => "Recursos Humanos",
                    "DEP02" => "Finanzas",
                    "DEP03" => "Tecnología",
                    "DEP04" => "Dirección Académica",
                    "DEP05" => "Seguridad",
                    _ => "Otro"
                };

            }
            else
            {
                // --- ESTUDIANTE ---
                CarreraPrograma = User.FindFirst("ID_Carrera")?.Value;

                CarreraPrograma = CarreraPrograma switch
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

            }

        }



    }
}
