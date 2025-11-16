using System;
using System.Threading.Tasks;
using CarnetDigital.Entities;

namespace CarnetDigital.Services.Abstract
{
    public interface IAccesosService
    {
        Task<IEnumerable<TipoUsuarioCombo>> ObtenerTiposUsuarioAsync();
        Task<ReporteAccesosResultado> ObtenerReporteAsync(string identificacion,DateTime? fechaInicio,DateTime? fechaFin,string idEstado,string tipoUsuario);
    }
}
