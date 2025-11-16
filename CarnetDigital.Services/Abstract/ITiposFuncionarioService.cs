using CarnetDigital.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sistema_Control_Entrada_Salida_QR.Services.Abstract
{
    public interface ITiposFuncionarioService
    {
        Task<IEnumerable<Tipos_Funcionarios>> GetAllAsync();
    }
}

