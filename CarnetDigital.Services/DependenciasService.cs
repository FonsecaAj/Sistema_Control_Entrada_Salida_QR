using CarnetDigital.Entities;
using CarnetDigital.Repository;
using Sistema_Control_Entrada_Salida_QR.Services.Abstract;

namespace Sistema_Control_Entrada_Salida_QR.Services
{
    public class DependenciasService : IDependenciasService
    {
        private readonly DependenciasRepository _repo;

        public DependenciasService(DependenciasRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Dependencias>> GetAllAsync()
        {
            return _repo.GetAllAsync();
        }
    }
}

