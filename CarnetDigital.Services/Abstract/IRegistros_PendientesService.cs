using System;
using CarnetDigital.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarnetDigital.Entities;

namespace CarnetDigital.Services.Abstract
{
    public interface IRegistros_PendientesService
    {
        Task<(string mensaje, int resultado)> EjecutarDecisionAsync(string identificacion, string decision);
        Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml);
        Task<IEnumerable<Registros_Pendientes>> GetAllAsync();

        Task<Registros_Pendientes> RegistrarUsuarioAsync(Registros_Pendientes registro);

    }
}
