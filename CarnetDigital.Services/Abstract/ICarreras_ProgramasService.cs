using CarnetDigital.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Services.Abstract
{
    public interface ICarreras_ProgramasService
    {

        Task<IEnumerable<Carreras_Programas>> ObtenerCarreras_ProgramasAsync();

    }
}
