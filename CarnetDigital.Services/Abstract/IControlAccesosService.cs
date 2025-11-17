using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Services.Abstract
{
    public interface IControlAccesosService
    {
     Task<IEnumerable<Entities.Accesos>> ObtenerAccesosById(string identificacion, DateOnly? filtroFechaInicio, DateOnly? filtroFechaFin, string? filtroEstado);
    Task<IEnumerable<Entities.Estados>> ObtenerTodosEstados();
    }
}
