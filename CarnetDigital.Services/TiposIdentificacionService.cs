using CarnetDigital.Entities;
using CarnetDigital.Repository;
using Sistema_Control_Entrada_Salida_QR.Services.Abstract;

namespace Sistema_Control_Entrada_Salida_QR.Services
{
    public class TiposIdentificacionService : ITiposIdentificacionService
    {
        private readonly TiposIdentificacionRepository _repo;

        public TiposIdentificacionService(TiposIdentificacionRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Tipos_Identificacion>> GetAllAsync()
        {
            return _repo.GetAllAsync();
        }
    }
}

