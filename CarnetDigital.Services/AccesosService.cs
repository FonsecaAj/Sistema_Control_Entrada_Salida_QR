using System;
using System.Linq;
using System.Threading.Tasks;
using CarnetDigital.Entities;
using CarnetDigital.Repository;
using CarnetDigital.Services.Abstract;

namespace CarnetDigital.Services
{
    public class AccesosService : IAccesosService
    {
        private readonly AccesosRepository _accesosRepository;

        public AccesosService(AccesosRepository accesosRepository)
        {
            _accesosRepository = accesosRepository;
        }

        public async Task<ReporteAccesosResultado> ObtenerReporteAsync(string identificacion, DateTime? fechaInicio, DateTime? fechaFin, string idEstado, string tipoUsuario)
        {
            var lista = (await _accesosRepository.ObtenerReporteAsync(
                identificacion,
                fechaInicio,
                fechaFin,
                idEstado,
                tipoUsuario)).ToList();

            var resultado = new ReporteAccesosResultado
            {
                Accesos = lista,
                TotalAccesos = lista.Count,
                TotalAutorizados = lista.Count(a => a.ID_Estado == "Autorizado"),
                TotalRechazados = lista.Count(a => a.ID_Estado == "Rechazado"),
                UsuariosUnicos = lista
                    .Select(a => a.Identificacion_Usuario?.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()
                    .Count()
            };

            return resultado;
        }

        public async Task<IEnumerable<TipoUsuarioCombo>> ObtenerTiposUsuarioAsync()
        {
            return await _accesosRepository.ObtenerTiposUsuarioAsync();
        }
    }
}
