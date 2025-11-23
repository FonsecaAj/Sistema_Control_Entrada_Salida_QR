using CarnetDigital.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sistema_Control_Entrada_Salida_QR.Services.Abstract
{
    public interface IDependenciasService
    {
        Task<IEnumerable<Dependencias>> GetAllAsync();
    }
}
