using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Services.Abstract
{
    public interface IEncargados_LegalesService
    {
        Task<IEnumerable<Entities.Tipos_Identificacion>> ObtenerTodosLosTiposIdentificacion();
        Task<IEnumerable<Entities.Parentescos>> ObtenerTodosLosParentescos();

        Task<(string, bool)> RegistrarEncargadoLegalAsync(Entities.Encargados_Legales encargado);
    }
}
