using CarnetDigital.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Services.Abstract
{
    public interface ITipos_IdentificacionService
    {

        Task<IEnumerable<Tipos_Identificacion>> ObtenerTipos_IdentificacionAsync();

    }
}
