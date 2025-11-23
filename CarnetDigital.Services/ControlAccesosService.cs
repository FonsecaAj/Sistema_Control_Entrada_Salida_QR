using CarnetDigital.Repository;
using CarnetDigital.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Services
{
    public class ControlAccesosService : IControlAccesosService
    {
        private readonly ControlAccesosRepository _controlAccesosRepository;
        public ControlAccesosService(ControlAccesosRepository controlAccesosRepository)
        {
            _controlAccesosRepository = controlAccesosRepository;
        }

        public async Task<IEnumerable<Entities.Accesos>> ObtenerAccesosById(string identificacion, DateOnly? filtroFechaInicio, DateOnly? filtroFechaFin, string? filtroEstado)
        {
            return await _controlAccesosRepository.ObtenerAccesosById(identificacion, filtroFechaInicio, filtroFechaFin, filtroEstado);
        }
        public async Task<IEnumerable<Entities.Estados>> ObtenerTodosEstados()
        {
            return await _controlAccesosRepository.ObtenerTodosEstados();
        }
    }
}
