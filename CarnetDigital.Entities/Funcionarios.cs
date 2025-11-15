using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control_QR.Entities
{
    public class Funcionarios
    {
        public string Identificacion { get; set; }
        public string Id_Tipo_Identificacion { get; set; }
        public string Nombre { get; set; }
        public string Primer_Apellido { get; set; }
        public string Segundo_Apellido { get; set; }
        public DateTime Fecha_Nacimiento { get; set; }
        public string FotoBase64 { get; set; }
        public string CorreoInstitucional { get; set; }
        public string Contrasena { get; set; }
        public string Id_Tipo_Funcionario { get; set; }
        public string Id_Dependencia { get; set; }

        public bool? Resultado { get; set; }
        public string? Mensaje { get; set; }
    }
}
