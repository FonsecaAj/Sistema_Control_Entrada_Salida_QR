using CarnetDigital.Entities;
using Control_QR.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Services.Abstract
{
    public interface IFuncionariosService
    {
        Task<(bool Resultado, string Mensaje)> RegistrarAsync(Funcionarios funcionario);
    }
}
