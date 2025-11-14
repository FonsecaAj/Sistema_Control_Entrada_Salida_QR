using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Entities
{
    public class Encargados_Temporales
    {
        public int ID_EncargadoTemporal { get; set; }
        public string Nombre { get; set; }
        public string Primer_Apellido { get; set; }
        public string Segundo_Apellido { get; set; }
        public string ID_Tipo_Identificacion { get; set; }
        public string Identificacion { get; set; }
        public string Telefono { get; set; }
        public int Id_Parentesco { get; set; }
        public string Identificacion_Estudiante { get; set; }
        public byte[] Foto { get; set; }
    }
}
