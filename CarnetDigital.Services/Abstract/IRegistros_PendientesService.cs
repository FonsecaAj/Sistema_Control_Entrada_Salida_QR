using CarnetDigital.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Services.Abstract
{
    public interface IRegistros_PendientesService
    {

        Task<Registros_Pendientes> RegistrarUsuarioAsync(Registros_Pendientes registro);

    }
}
