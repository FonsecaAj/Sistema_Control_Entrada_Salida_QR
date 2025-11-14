using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Entities
{
    public class Funcionarios
    {
        public string Identificacion { get; set; }
        public string Id_Tipo_Funcionario { get; set; }
        public string Id_Dependencia { get; set; }
        public DateTime Fecha_Registro { get; set; }
    }
}
