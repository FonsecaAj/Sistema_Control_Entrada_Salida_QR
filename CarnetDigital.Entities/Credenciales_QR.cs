using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Entities
{
    public class Credenciales_QR
    {

        public int ID_QR { get; set; }
        public string Identificacion { get; set; }
        public string Codigo_qr { get; set; }
        public DateTime Fecha_generacion { get; set; }
        public DateTime Fecha_expiracion { get; set; }
        public string ID_Estado { get; set; }


    }
}
