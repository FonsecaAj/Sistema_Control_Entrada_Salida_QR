using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Entities
{
    public class Accesos
    {
        public int Id_Acceso { get; set; }
        public string Identificacion_Usuario { get; set; }
        public DateTime Fecha_Acceso { get; set; }
        public string ID_Estado { get; set; }
        public string Tipo_Acceso { get; set; }
        public string Marca_Entrada_Salida { get; set; }



    }

    public class FiltrosAccesos
    {
        public string Identificacion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string IdEstado { get; set; }
        public string TipoUsuario { get; set; }
    }


    public class ReporteAccesosResultado
    {
        public List<Accesos> Accesos { get; set; } = new();
        public int TotalAccesos { get; set; }
        public int TotalAutorizados { get; set; }
        public int TotalRechazados { get; set; }
        public int UsuariosUnicos { get; set; }
    }


    public class TipoUsuarioCombo
    {
        public string Id_Tipo { get; set; }
        public string Nombre_Tipo { get; set; }
    }

    public class ReporteAccesosResultado
    {
        public List<Accesos> Accesos { get; set; } = new();
        public int TotalAccesos { get; set; }
        public int TotalAutorizados { get; set; }
        public int TotalRechazados { get; set; }
        public int UsuariosUnicos { get; set; }
    }


}
