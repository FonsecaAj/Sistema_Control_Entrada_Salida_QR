using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Entities
{
    public class Persona
    {
        public string Identificacion { get; set; }
        public string ID_Tipo_Identificacion { get; set; }
        public string Nombre { get; set; }
        public string Primer_Apellido { get; set; }
        public string Segundo_Apellido { get; set; }
        public DateTime Fecha_Nacimiento { get; set; }
        public DateTime Fecha_Registro { get; set; }
        public DateTime? Fecha_Vencimiento { get; set; }
        public byte[] Foto { get; set; }
    }
}
