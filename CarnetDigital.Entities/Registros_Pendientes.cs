using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Entities
{
    public class Registros_Pendientes
    {
        public string Nombre { get; set; }
        public string Primer_Apellido { get; set; }
        public string Segundo_Apellido { get; set; }
        public string Correo_Institucional { get; set; }
        public string ID_Tipo_Identificacion { get; set; }
        public string Identificacion { get; set; }
        public DateTime Fecha_Nacimiento { get; set; }
        public string Id_Carrera { get; set; }
        public string Contrasena { get; set; }
        public byte[] Foto { get; set; }
        public DateTime Fecha_Registro { get; set; }
        public string ID_Estado { get; set; }

        // Agregué este Atributo para que devuelva los Mensajes de mi SP
        public string Mensaje { get; set; }

    }
}
