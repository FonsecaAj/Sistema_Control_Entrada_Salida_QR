using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Entities
{
    public class Estudiantes
    {
        public string Identificacion { get; set; }
        public string Id_Carrera { get; set; }
        public string ID_TipoEstudiante { get; set; }
        public DateTime Fecha_Registro { get; set; }
    }
}
