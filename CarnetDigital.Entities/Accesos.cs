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
}
