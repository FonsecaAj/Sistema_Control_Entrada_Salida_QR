using CarnetDigital.Entities;
using CarnetDigital.Repository;
using Sistema_Control_Entrada_Salida_SQR.Services.Abstract;

namespace Sistema_Control_Entrada_Salida_SQR.Services
{
    public class TiposFuncionarioService : ITiposFuncionarioService
    {
        private readonly TiposFuncionarioRepository _repo;

        public TiposFuncionarioService(TiposFuncionarioRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Tipos_Funcionarios>> GetAllAsync()
        {
            return _repo.GetAllAsync();
        }
    }
}

